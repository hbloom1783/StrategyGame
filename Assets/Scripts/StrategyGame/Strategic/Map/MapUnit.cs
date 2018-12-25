using Athanor.Pooling;
using GridLib.Hex;
using GridLib.Pathing;
using StrategyGame.Strategic.Persistence;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StrategyGame.Strategic.Map
{
    public class MapUnit : MonoBehaviour, IPoolable, ICanPath<HexCoords>
    {
        private HexCoords _loc = HexCoords.O;
        public HexCoords loc
        {
            get { return _loc; }
            set { _loc = value; }
        }

        public MapUnit()
        {
            persist = new StrategicUnitPersist();
        }

        #region Persistence

        public MapUnit(StrategicUnitPersist persist)
        {
            this.persist = persist;
        }

        public StrategicUnitPersist persist
        {
            get
            {
                StrategicUnitPersist result = new StrategicUnitPersist();

                return result;
            }

            set
            {
            }
        }

        #endregion

        #region Poolable

        public PooledObject sticker { get { return GetComponent<PooledObject>(); } }

        public void OnProvide()
        {
        }

        public void OnReturn()
        {
            persist = new StrategicUnitPersist();
        }

        #endregion

        #region Pathing

        private MapController map { get { return MapController.instance; } }

        public IEnumerable<HexCoords> ValidNeighbors(HexCoords loc)
        {
            return loc.neighbors.Where(map.InBounds);
        }

        public bool CanEnter(HexCoords loc)
        {
            return map[loc].isFilled;
        }

        public bool CanStay(HexCoords loc)
        {
            return map[loc].isFilled;
        }

        public bool CanLeave(HexCoords loc)
        {
            return map[loc].isFilled;
        }

        public int CostToEnter(HexCoords loc)
        {
            if (map[loc].type != CellType.empty)
                return 1;
            else
                return 10000;
        }

        public int Heuristic(HexCoords src, HexCoords dst)
        {
            return src.DistanceTo(dst);
        }

        #endregion
    }
}
