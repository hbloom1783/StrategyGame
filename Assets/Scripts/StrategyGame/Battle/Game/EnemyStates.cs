﻿using Athanor.Colors;
using System.Collections;
using UnityEngine;
using System.Linq;
using StrategyGame.Battle.Map;
using Athanor.Collections;
using StrategyGame.Battle.Game.Abilities;
using GridLib.Hex;
using System;

namespace StrategyGame.Battle.Game.Enemy
{
    class BeginTurn : BattleScript
    {
        private static readonly Color clearWhite = Color.white.withA(0.0f);
        private static readonly Color clearRed = Color.red.withA(0.0f);

        public override IEnumerator Script()
        {
            ui.marqueeText.shown = true;
            ui.marqueeText.text = "Enemy Turn";
            yield return ui.marqueeText.ColorTween(clearWhite, Color.red, 1.0f);
            yield return ui.marqueeText.ColorTween(Color.red, clearRed, 1.0f);
            ui.marqueeText.shown = false;

            yield return game.state.SteadyChange(new CheckVictory());
        }
    }

    class CheckVictory : BattleScript
    {
        public MapUnit unit = null;

        public override IEnumerator Script()
        {
            // Dump to persistence
            map.SaveToPersistence();

            // Check for victory
            if (!TeamUnits(Team.player).Where(x => x.hp > 0).Any())
                yield return game.state.SteadyChange(new Player.Defeated());
            else if (!TeamUnits(Team.enemy).Where(x => x.hp > 0).Any())
                yield return game.state.SteadyChange(new Defeated());
            // Identify next state
            else if ((unit != null) && (unit.ap > 0))
                yield return game.state.SteadyChange(new ChooseAbilityAndTargets { unit = unit });
            else if (TeamUnits(Team.enemy).Any(x => x.ap > 0))
                yield return game.state.SteadyChange(new ChooseUnit());
            else
                yield return game.state.SteadyChange(new EndTurn());
        }
    }

    class ChooseUnit : BattleScript
    {
        public override IEnumerator Script()
        {
            MapUnit unit = enemyUnits
                .Where(x => x.ap > 0)
                .First();

            yield return game.state.SteadyChange(new ChooseAbilityAndTargets { unit = unit });
        }
    }

    class ChooseAbilityAndTargets : BattleScript
    {
        public MapUnit unit = null;

        public override IEnumerator Script()
        {
            if (unit.ai == null)
            {
                IUnitAbility ability = unit.abilities
                    .FirstOrDefault(UnitAbilityExt.CanUse);

                if (ability == default(IUnitAbility))
                {
                    Debug.Log(unit.name + " has no viable abilities; wasting " + unit.ap + " AP.");
                    unit.ap = 0;

                    yield return game.state.SteadyChange(new ChooseUnit());
                }
                else
                {
                    while (ability.CanTarget() && !ability.HasMaxTargets())
                    {
                        HexCoords target = ability.GetRange()
                            .ToList()
                            .RandomPick();

                        ability.SelectTarget(target);
                    }

                    yield return game.state.SteadyChange(new ExecuteAbility { ability = ability });
                }
            }
            else
            {
                IUnitAbility ability = unit.ai.ChooseAbilityAndTargets();

                if (ability == null)
                    yield return game.state.SteadyChange(new ChooseUnit());
                else
                    yield return game.state.SteadyChange(new ExecuteAbility { ability = ability });
            }
        }
    }

    class ExecuteAbility : BattleScript
    {
        public IUnitAbility ability = null;

        public override IEnumerator Script()
        {
            ability.PayCost();

            yield return ability.Execute();
            ability.ResetInternalState();

            yield return game.state.SteadyChange(new CheckVictory { unit = ability.unit });
        }
    }

    class EndTurn : BattleScript
    {
        public override IEnumerator Script()
        {
            TeamUnits(Team.enemy)
                .ToList()
                .ForEach(x => x.ap = x.maxAp);

            // Dump to persistence
            map.SaveToPersistence();

            yield return game.state.SteadyChange(new NextTurn());
        }
    }

    class Defeated : BattleScript
    {
        private static readonly Color clearWhite = Color.white.withA(0.0f);
        private static readonly Color clearBlue = Color.blue.withA(0.0f);

        public override IEnumerator Script()
        {
            ui.marqueeText.shown = true;
            ui.marqueeText.text = "Enemy Defeated";
            yield return ui.marqueeText.ColorTween(clearWhite, Color.blue, 1.0f);
            yield return ui.marqueeText.ColorTween(Color.blue, clearBlue, 1.0f);
            ui.marqueeText.shown = false;

            game.persist.data.strategic.mapPieces.Add(Strategic.Map.MapPiece.Generate());
            yield return game.state.SteadyChange(new Strategic.Game.Boot());
        }
    }
}
