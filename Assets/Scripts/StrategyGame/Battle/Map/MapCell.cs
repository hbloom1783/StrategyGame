using Athanor.Pooling;
using GridLib.Hex;
using UnityEngine;
using StrategyGame.Game;
using StrategyGame.Battle.Persistence;
using System.Collections.Generic;

namespace StrategyGame.Battle.Map
{
    public enum CellType
    {
        walkable,
        notWalkable,
    }

    public enum HighlightState
    {
        none,
        steady,
        shimmer,
    }
    
    public class MapCell : HexGridCell, IPoolable
    {
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

        #region Appearance

        public Sprite walkableSprite = null;
        public Sprite notWalkableSprite = null;

        #region Cell type

        private CellType _type = CellType.walkable;
        public CellType type
        {
            get { return _type; }
            set
            {
                _type = value;
                UpdateAppearance();
            }
        }

        private Sprite SwitchSprite()
        {
            switch (type)
            {
                case CellType.walkable: return walkableSprite;
                case CellType.notWalkable: return notWalkableSprite;
                default: return null;
            }
        }

        public bool isWalkable
        {
            get
            {
                switch(type)
                {
                    case CellType.walkable:
                        return true;

                    default:
                    case CellType.notWalkable:
                        return false;
                }
            }
        }

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

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        private Sprite sprite
        {
            get { return spriteRenderer.sprite; }
            set { spriteRenderer.sprite = value; }
        }

        public Color color
        {
            get { return spriteRenderer.color; }
            set { spriteRenderer.color = value; }
        }

        private void UpdateAppearance()
        {
            if (spriteRenderer != null)
            {
                sprite = SwitchSprite();
            }
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
                BattleCellPersist result = new BattleCellPersist();
                result.type = type;
                if (unitPresent != null)
                    result.unitPresent = unitPresent.persist;
                return result;
            }

            set
            {
                type = value.type;

                if (unitPresent != null) unitPresent.sticker.Return();
                if (value.unitPresent == null)
                {
                    unitPresent = null;
                }
                else
                {
                    map.PlaceUnit(GameController.instance.pools.battleMapUnitPool.Provide<MapUnit>(), loc);
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
