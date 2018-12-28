using GridLib.Hex;
using System.Collections.Generic;
using System.Linq;
using StrategyGame.Battle.Map;
using System.Collections;
using UnityEngine;
using Athanor.Tweening;

namespace StrategyGame.Battle.Game.Abilities
{
    class JumpAttackAbility : MonoBehaviour, IUnitAbility
    {
        #region Shorthands

        protected MapController map { get { return MapController.instance; } }

        public MapUnit unit { get { return GetComponentInParent<MapUnit>(); } }

        #endregion

        #region Ability button

        [SerializeField]
        private Sprite _icon = null;
        public Sprite icon { get { return _icon; } }

        #endregion

        #region Targeting

        private HexCoords target = null;
        private HexCoords landing = null;

        public int targetsMin { get { return 2; } }
        public int targetsMax { get { return 2; } }
        public int targetsHeld
        {
            get
            {
                return new[] { target, landing }.Count(x => x != null);
            }
        }

        public void SelectTarget(HexCoords target)
        {
            if (this.target == null) this.target = target;
            else if (landing == null) landing = target;
        }

        public void DeselectTarget()
        {
            if (landing != null) landing = null;
            else if (target != null) target = null;
        }

        public void ResetInternalState()
        {
            landing = target = null;
        }

        #endregion

        #region Attributes

        #region Range

        public IEnumerable<HexCoords> GetRange()
        {
            if (target == null)
            {
                return unit.loc.CompoundRing(1, (uint)unit.jump)
                    .Where(map.InBounds)
                    .Where(x => map[x].unitPresent != null)
                    .Where(x => map[x].unitPresent.team != unit.team);
            }
            else if (landing == null)
            {
                return target.Ring(1)
                    .Where(map.InBounds)
                    .Where(unit.CanEnter);
            }
            else
            {
                return new HexCoords[] { };
            }
        }

        public IEnumerable<HexCoords> GetCoveredArea()
        {
            if (target != null) yield return target;
            if (landing != null) yield return landing;
        }

        #endregion

        #region AoE

        public IEnumerable<HexCoords> GetAoE(HexCoords target)
        {
            return new[] { target };
        }

        #endregion

        #region Costs

        private IEnumerable<ModularCost> costs { get { return GetComponents<ModularCost>(); } }

        // Default cost is nothing and can always be paid
        public bool canPayCost
        {
            get
            {
                return costs.All(x => x.canPay);
            }
        }

        public void PayCost()
        {
            costs.ToList().ForEach(x => x.PayCost());
        }

        #endregion

        #endregion

        public IEnumerator Execute()
        {
            if ((targetsHeld < targetsMin) || (targetsHeld > targetsMax))
            {
                Debug.Log(name + " needed [" + targetsMin + ", " + targetsMax + "] targets, got " + targetsHeld);
                yield return null;
            }
            else
            {
                yield return unit.transform.ParabolicTween(
                    map.GridToWorld(target),
                    unit.loc.DistanceTo(target) / 2);

                // deal damage
                map[target].unitPresent.TakeDamage(1);

                yield return unit.transform.ParabolicTween(
                    map.GridToWorld(landing),
                    unit.loc.DistanceTo(landing) / 2);

                map.PlaceUnit(unit, landing);
            }
        }
    }
}
