using GridLib.Hex;
using StrategyGame.Game;
using StrategyGame.Battle.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StrategyGame.Battle.Map
{
    public class MapController : HexGridManager<MapCell>
    {
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

        public MapCell InitCell(HexCoords loc)
        {
            MapCell newCell = game.pools.battleMapCellPool.Provide<MapCell>();
            InitCell(loc, newCell);
            return newCell;
        }

        public override void DeleteCell(MapCell cell)
        {
            if (cell != null)
            {
                if (cell.loc != null) gridContents.Remove(cell.loc);

                cell.sticker.Return();
            }
        }

        #endregion

        #region Unit management

        public IEnumerable<MapUnit> units
        {
            get
            {
                // may need to instrument/index this, it's a lot of ground to cover
                return cells
                    .Where(x => x.unitPresent != null)
                    .Select(x => x.unitPresent);
            }
        }

        public MapUnit UnitAt(HexCoords loc)
        {
            return gridContents[loc].unitPresent;
        }

        public MapCell UnitCell(MapUnit unit)
        {
            return gridContents[unit.loc];
        }

        public void UnplaceUnit(MapUnit unit)
        {
            if (InBounds(unit.loc)) gridContents[unit.loc].unitPresent = null;
            unit.loc = null;
            unit.transform.SetParent(null);
        }

        public void PlaceUnit(MapUnit unit, HexCoords newLoc)
        {
            if (OutOfBounds(newLoc)) InitCell(newLoc);

            if (UnitAt(newLoc) != null)
                throw new ArgumentException("Cell wasn't empty!");

            if (unit.loc != null)
                UnplaceUnit(unit);

            gridContents[newLoc].unitPresent = unit;
            unit.loc = newLoc;
            unit.transform.SetParent(CellAt(newLoc).transform);
            unit.transform.localPosition = Vector3.zero;
        }

        #endregion

        #region Persistence

        public void LoadFromPersistence()
        {
            // Wipe grid
            foreach(MapCell cell in gridContents.Values)
                cell.sticker.Return();

            // Copy persist to grid
            foreach(HexCoords loc in persist.mapContents.Keys)
                InitCell(loc).persist = persist.mapContents[loc];
        }

        public void SaveToPersistence()
        {
            // Wipe old persist
            persist.mapContents =
                new Dictionary<HexCoords, BattleCellPersist>();

            // Copy grid to persist
            foreach (var kv in gridContents)
                persist.mapContents[kv.Key] = kv.Value.persist;
        }

        #endregion
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
