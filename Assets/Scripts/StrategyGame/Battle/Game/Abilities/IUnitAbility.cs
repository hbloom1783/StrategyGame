using GridLib.Hex;
using StrategyGame.Battle.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrategyGame.Battle.Game.Abilities
{
    public interface IUnitAbility
    {
        MapUnit unit { get; }

        Sprite icon { get; }

        int targetsHeld { get; }
        int targetsMin { get; }
        int targetsMax { get; }

        IEnumerable<HexCoords> GetAoE(HexCoords target);

        IEnumerable<HexCoords> GetRange();
        void SelectTarget(HexCoords target);
        void DeselectTarget();
        IEnumerable<HexCoords> GetCoveredArea();

        void ResetInternalState();

        bool canPayCost { get; }
        void PayCost();

        IEnumerator Execute();
    }

    public static class UnitAbilityExt
    {
        public static bool HasMinTargets(this IUnitAbility ability)
        {
            return ability.targetsHeld >= ability.targetsMin;
        }

        public static bool HasMaxTargets(this IUnitAbility ability)
        {
            return ability.targetsHeld == ability.targetsMax;
        }

        public static bool CanTarget(this IUnitAbility ability)
        {
            return ability.GetRange().Any();
        }
    }
}
