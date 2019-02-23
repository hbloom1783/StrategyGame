using StrategyGame.Game;
using StrategyGame.Battle.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridLib.Hex;
using GridLib.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StrategyGame.Battle.Map
{
    public class MapController : HexGridManager
    {
        public IEnumerable<MapCell> mapCells { get { return cells.Select(x => x.MapCell()); } }
        public MapCell MapCellAt(HexCoords loc)
        {
            return CellAt(loc).MapCell();
        }

        #region Singleton

        private static MapController _instance = null;
        public static MapController instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }

        void OnEnable()
        {
            // Either become the instance...
            if (instance == null) instance = this;
            // ... or self-destruct.
            else Destroy(gameObject);
        }

        void OnDestroy()
        {
            // If we were the instance, clear the instance.
            if (instance == this) instance = null;
        }

        #endregion

        #region Shorthands

        private GameController game { get { return GameController.instance; } }
        private BattlePersist persist { get { return game.persist.data.battle; } }

        #endregion

        #region Cell pooling

        public Vector3 elevationOffset = Vector3.up * 0.3f;

        public MapCell InitCell(HexCoords loc, BattleCellPersist persist)
        {
            MapCell newCell = game.pools.battleMapCellPool.Provide<MapCell>();
            InitCell(loc, newCell.gridCell);
            newCell.persist = persist;
            newCell.transform.position += elevationOffset * newCell.elevation;
            return newCell;
        }

        public override void DeleteCell(GridCell<HexCoords> cell)
        {
            MapCell mapCell = cell.GetComponent<MapCell>();
            if (cell != null)
            {
                if (cell.loc != null) gridContents.Remove(cell.loc);

                mapCell.sticker.Return();
            }
        }

        #endregion

        #region Unit management

        public IEnumerable<MapUnit> units
        {
            get
            {
                // may need to instrument/index this, it's a lot of ground to cover
                return mapCells
                    .Where(x => x.unitPresent != null)
                    .Select(x => x.unitPresent);
            }
        }

        public MapUnit UnitAt(HexCoords loc)
        {
            return MapCellAt(loc).unitPresent;
        }

        public MapCell UnitCell(MapUnit unit)
        {
            return MapCellAt(unit.loc);
        }

        public void UnplaceUnit(MapUnit unit)
        {
            if (InBounds(unit.loc)) MapCellAt(unit.loc).unitPresent = null;
            unit.loc = null;
            unit.transform.SetParent(null);
        }

        public void PlaceUnit(MapUnit unit, HexCoords newLoc)
        {
            if (OutOfBounds(newLoc)) InitCell(newLoc, new BattleCellPersist());

            if (UnitAt(newLoc) != null)
                throw new ArgumentException("Cell wasn't empty!");

            if (unit.loc != null)
                UnplaceUnit(unit);

            MapCellAt(newLoc).unitPresent = unit;
            unit.loc = newLoc;
            unit.transform.SetParent(MapCellAt(newLoc).unitFooting);
            unit.transform.localPosition = Vector3.zero;
        }

        #endregion

        #region Persistence

        public void LoadFromPersistence()
        {
            // Wipe grid
            foreach (MapCell cell in mapCells)
                cell.sticker.Return();

            // Copy persist to grid
            foreach (HexCoords loc in persist.mapContents.Keys)
                InitCell(loc, persist.mapContents[loc]);
        }

        public void SaveToPersistence()
        {
            // Wipe old persist
            persist.mapContents =
                new Dictionary<HexCoords, BattleCellPersist>();

            // Copy grid to persist
            foreach (var kv in gridContents)
                persist.mapContents[kv.Key] = kv.Value.GetComponent<MapCell>().persist;
        }

        #endregion
    }

    public static class HexGridCellExt
    {
        public static MapCell MapCell(this GridCell<HexCoords> cell)
        {
            if (cell == null) return null;
            else return cell.GetComponent<MapCell>();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapController))]
    class MapControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MapController myScript = (MapController)target;

            if (GUILayout.Button("Pointy Hex Defaults"))
            {
                myScript.SetPointyDefault();
            }
        }
    }
#endif
}
