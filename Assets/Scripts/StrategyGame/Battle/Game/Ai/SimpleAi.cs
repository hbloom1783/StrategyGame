using Athanor.Collections;
using GridLib.Pathing;
using GridLib.Hex;
using StrategyGame.Battle.Game.Abilities;
using System.Collections.Generic;
using System.Linq;

namespace StrategyGame.Battle.Game.Ai
{
    class SimpleAi : UnitAi
    {
        private IUnitAbility moveAbility;
        private List<IUnitAbility> attackAbilities;

        public int desiredRange = 1;

        void Start()
        {
            moveAbility = unit.abilities
                .First(x => x.descriptors.Contains(Descriptor.move));

            attackAbilities = unit.abilities
                .Where(x => x.descriptors.Contains(Descriptor.attack))
                .ToList();
        }

        private bool canAttack { get { return attackAbilities.Any(UnitAbilityExt.CanUse); } }
        private bool canMove { get { return (moveAbility != null) ? moveAbility.CanUse() : false; } }

        public override IUnitAbility ChooseAbilityAndTargets()
        {
            if (canAttack)
            {
                IUnitAbility ability = attackAbilities
                    .First(UnitAbilityExt.CanUse);
                
                while (ability.CanTarget() && !ability.HasMaxTargets())
                {
                    int minHp = ability.GetRange()
                        .Where(map.InBounds)
                        .Select(map.MapCellAt)
                        .Where(x => x.unitPresent != null)
                        .Where(x => x.unitPresent.team == Map.Team.player)
                        .Min(x => x.unitPresent.hp);

                    HexCoords target = ability.GetRange()
                         .Where(map.InBounds)
                         .Where(x => map.MapCellAt(x).unitPresent != null)
                         .Where(x => map.MapCellAt(x).unitPresent.team == Map.Team.player)
                         .Where(x => map.MapCellAt(x).unitPresent.hp == minHp)
                         .ToList()
                         .RandomPick();

                    ability.SelectTarget(target);
                }

                return ability;
            }
            else if (canMove)
            {
                int minDistance = map.units
                    .Where(x => x.team == Map.Team.player)
                    .Min(x => unit.loc.DistanceTo(x.loc));

                HexCoords destination = map.units
                    .Where(x => x.team == Map.Team.player)
                    .Select(x => x.loc)
                    .Where(x => unit.loc.DistanceTo(x) == minDistance)
                    .ToList()
                    .RandomPick();
                
                IEnumerable<HexCoords> path = unit.AStar(destination, int.MaxValue, desiredRange);
                
                if (path == null)
                    path = unit.loc.LineTo(destination).Skip(1).TakeWhile(unit.CanEnter);


                while (!moveAbility.HasMaxTargets())
                {
                    if (path.Any(moveAbility.GetRange().Contains))
                    {
                        HexCoords target = path
                            .Where(moveAbility.GetRange().Contains)
                            .Last();

                        moveAbility.SelectTarget(target);
                    }
                    else
                    {
                        WasteAp(unit.name + " has no viable move target; wasting " + unit.ap + " AP.");
                        break;
                    }
                }

                return moveAbility;
            }
            else
            {
                WasteAp(unit.name + " has no viable abilities; wasting " + unit.ap + " AP.");
                return null;
            }
        }
    }
}
