using Athanor.Pooling;
using UnityEngine;

namespace StrategyGame.Game
{
    public class PoolContainer : MonoBehaviour
    {
        public ObjectPool strategicPieceButtonPool = null;
        public ObjectPool strategicMapCellPool = null;
        public ObjectPool strategicMapUnitPool = null;

        public ObjectPool battleMapCellPool = null;
        public ObjectPool battleMapUnitPool = null;
        public ObjectPool battleIconPool = null;
        public ObjectPool battleAbilityButtonPool = null;
    }
}
