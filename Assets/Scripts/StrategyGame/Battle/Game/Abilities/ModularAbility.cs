using GridLib.Hex;
using StrategyGame.Battle.Map;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StrategyGame.Battle.Game.Abilities
{
    public class ModularAbility : MonoBehaviour, IUnitAbility
    {
        #region Shorthands

        public MapUnit unit { get { return GetComponentInParent<MapUnit>(); } }

        #endregion

        #region Metadata

        [SerializeField]
        private Sprite _icon = null;
        public Sprite icon { get { return _icon; } }

        [SerializeField]
        private List<Descriptor> descriptorList = new List<Descriptor>();
        public IEnumerable<Descriptor> descriptors
        {
            get
            {
                return descriptorList;
            }
        }

        #endregion

        #region Targeting

        private HexCoords target = null;
        public int targetsHeld { get { return (target == null) ? 0 : 1; } }
        public int targetsMin { get { return 1; } }
        public int targetsMax { get { return 1; } }

        public void SelectTarget(HexCoords target)
        {
            if (GetRange().Contains(target))
                this.target = target;
        }

        public void DeselectTarget()
        {
            target = null;
        }

        public void ResetInternalState()
        {
            target = null;
        }

        public IEnumerable<HexCoords> GetCoveredArea()
        {
            if (target == null) return new HexCoords[] { };
            else return GetAoE(target);
        }

        #endregion

        #region Attributes

        #region Range

        private ModularRange range { get { return GetComponent<ModularRange>(); } }

        // Default range is self-only
        public IEnumerable<HexCoords> GetRange()
        {
            if (this.HasMaxTargets())
                return new HexCoords[0];
            else if (range != null)
                return range.rangeArea;
            else
                return new[] { unit.loc };
        }

        #endregion

        #region AoEs

        private IEnumerable<ModularAoE> aoes { get { return GetComponents<ModularAoE>(); } }

        // Default AoE is same-hex
        public IEnumerable<HexCoords> GetAoE(HexCoords target)
        {
            if (aoes.Any())
                return aoes.SelectMany(x => x.GetAoE(target));
            else
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

        #region Effect

        private ModularEffect effect { get { return GetComponent<ModularEffect>(); } }

        // Default effect does nothing
        private IEnumerator Proc()
        {
            if (effect != null) return effect.Proc(GetAoE(target));
            else return null;
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
                yield return Proc();
            }
        }
    }
}
