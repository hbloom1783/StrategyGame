using StrategyGame.Battle.Map;
using StrategyGame.Game;
using UnityEngine;

namespace StrategyGame.Battle.Game.Abilities
{
    public abstract class ModularAttribute : MonoBehaviour
    {
        protected GameController game { get { return GameController.instance; } }
        protected MapController map { get { return MapController.instance; } }

        protected IUnitAbility ability { get { return GetComponent<IUnitAbility>(); } }
        protected MapUnit unit { get { return ability.unit; } }
    }
}
