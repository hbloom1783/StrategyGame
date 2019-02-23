using Athanor.Pooling;
using UnityEngine;
using StrategyGame.Game;
using StrategyGame.Battle.Persistence;
using System.Collections.Generic;
using GridLib.Hex;

namespace StrategyGame.Battle.Map
{
    public enum TerrainPersist
    {
        greenOpen,
        greenRoad,
        greenTree,
    }

    public enum HighlightState
    {
        none,
        steady,
        shimmer,
    }
    
    public class MapCell : MonoBehaviour, IPoolable
    {
        public HexGridCell gridCell { get { return GetComponent<HexGridCell>(); } }
        public HexCoords loc { get { return gridCell.loc; } }

        private MapUnit _unitPresent = null;
        public MapUnit unitPresent
        {
            get { return _unitPresent; }
            set { _unitPresent = value; }
        }

        public MapCell()
        {
            persist = new BattleCellPersist();
        }

        private MapController map { get { return MapController.instance; } }

        public IEnumerable<MapCell> neighbors;

        public Transform unitFooting;
        public SpriteRenderer cellSprite;

        #region Appearance

        #region Cell type

        [SerializeField]
        private BattleTerrain greenOpen = null;

        [SerializeField]
        private BattleTerrain greenRoad = null;

        [SerializeField]
        private BattleTerrain greenTree = null;

        private TerrainPersist _type = TerrainPersist.greenOpen;
        private BattleTerrain terrain = null;
        public TerrainPersist type
        {
            get { return _type; }
            set
            {
                _type = value;
                FixTerrain();
            }
        }

        public bool canWalkThru
        {
            get
            {
                return terrain.isWalkable;
            }
        }

        public bool canSeeThru
        {
            get
            {
                return terrain.canSeeThru;
            }
        }

        public void FixTerrain()
        {
            switch (_type)
            {
                case TerrainPersist.greenOpen: terrain = greenOpen; break;
                case TerrainPersist.greenRoad: terrain = greenRoad; break;
                case TerrainPersist.greenTree: terrain = greenTree; break;
            }

            if (unitFooting != null)
                unitFooting.localPosition = terrain.unitFooting;
        }

        #endregion

        #region Elevation

        public int elevation = 0;

        #endregion

        #region Highlighting

        private Color SwitchColor()
        {
            switch (_state)
            {
                case HighlightState.steady:
                    return highColor;

                case HighlightState.shimmer:
                    return Color.Lerp(shimColorA, shimColorB, lastShimmerT);

                default:
                case HighlightState.none:
                    return baseColor;
            }
        }

        private HighlightState _state = HighlightState.steady;
        public HighlightState state
        {
            get { return _state; }
            set
            {
                _state = value;
                color = SwitchColor();
            }
        }

        public Color baseColor = Color.white;
        public Color highColor = Color.white;
        public Color shimColorA = Color.white;
        public Color shimColorB = Color.white;

        public float lastShimmerT = 0.0f;
        public float shimmerT
        {
            set
            {
                if (state == HighlightState.shimmer)
                    color = Color.Lerp(shimColorA, shimColorB, value);
                lastShimmerT = value;
            }
        }

        #endregion

        private Color _color = Color.white;
        public Color color
        {
            get { return _color; }
            set
            {
                _color = value;
                if (cellSprite != null)
                    cellSprite.color = value;
            }
        }

        private void UpdateAppearance()
        {
            cellSprite.sprite = terrain.sprite;
        }

        #endregion

        #region Persistence

        public MapCell(BattleCellPersist persist)
        {
            this.persist = persist;
        }

        public BattleCellPersist persist
        {
            get
            {
                BattleCellPersist result = new BattleCellPersist(type, elevation);
                if (unitPresent != null)
                    result.unitPresent = unitPresent.persist;
                return result;
            }

            set
            {
                type = value.type;
                elevation = value.elevation;

                if (unitPresent != null) unitPresent.sticker.Return();
                if (value.unitPresent == null)
                {
                    unitPresent = null;
                }
                else
                {
                    map.PlaceUnit(GameController.instance.pools.battleMapUnitPool.Provide<MapUnit>(), gridCell.loc);
                    unitPresent.persist = value.unitPresent;
                }
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
            persist = new BattleCellPersist();
        }

        #endregion

        #region MonoBehaviour

        void Start()
        {
            UpdateAppearance();
        }

        #endregion
    }
}
