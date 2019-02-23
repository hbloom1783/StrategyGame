using Athanor.Pooling;
using GridLib.Hex;
using StrategyGame.Strategic.Persistence;
using UnityEngine;
using UnityEngine.UI;
using StrategyGame.Game;

namespace StrategyGame.Strategic.Map
{
    public enum CellType
    {
        empty,

        moonPlain,
        moonCrashSite,

        greenPlain,
        greenCity,
    }

    public enum Highlight
    {
        neutral,
        valid,
        invalid,
    }
    
    public class MapCell : MonoBehaviour, IPoolable
    {
        public HexGridCell gridCell { get { return GetComponent<HexGridCell>(); } }

        private MapUnit _unitPresent;
        public MapUnit unitPresent
        {
            get { return _unitPresent; }
            set { _unitPresent = value; }
        }

        public Sprite moonSprite;
        public Sprite greenSprite;

        public MapCell()
        {
            persist = new StrategicCellPersist();
        }

        private MapController map { get { return MapController.instance; } }

        #region Appearance

        #region Cell type

        private CellType _type = CellType.empty;
        private CellType _overlayType = CellType.empty;

        public CellType type
        {
            get { return _type; }
            set
            {
                _type = value;
                UpdateAppearance();
            }
        }

        public CellType overlayType
        {
            get { return _overlayType; }
            set
            {
                _overlayType = value;
                UpdateAppearance();
            }
        }

        public CellType displayType
        {
            get
            {
                if (_overlayType == CellType.empty) return _type;
                else return _overlayType;
            }
        }

        public bool isFilled { get { return type != CellType.empty; } }

        private Sprite SwitchSprite()
        {
            switch(displayType)
            {
                case CellType.moonPlain:
                case CellType.moonCrashSite:
                    return moonSprite;

                case CellType.greenPlain:
                case CellType.greenCity:
                    return greenSprite;

                default:
                case CellType.empty:
                    return null;
            }
        }

        #endregion

        #region Highlight

        private Highlight _highlight = Highlight.neutral;
        public Highlight highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                UpdateAppearance();
            }
        }

        private Color SwitchColor()
        {
            switch (highlight)
            {
                case Highlight.invalid:
                    return Color.red;

                case Highlight.valid:
                    return Color.green;

                default:
                case Highlight.neutral:
                    return Color.white;
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

        private Color color
        {
            get { return spriteRenderer.color; }
            set { spriteRenderer.color = value; }
        }

        public Text debugText;

        private void UpdateAppearance()
        {
            if (spriteRenderer != null)
            {
                sprite = SwitchSprite();
                color = SwitchColor();
            }

            if (debugText != null)
                debugText.text = gridCell.loc + "\n" + type;
        }

        #endregion

        #region Persistence

        public MapCell(StrategicCellPersist persist)
        {
            this.persist = persist;
        }

        public StrategicCellPersist persist
        {
            get
            {
                StrategicCellPersist result = new StrategicCellPersist();
                result.type = type;
                if (unitPresent != null)
                    result.unitPresent = unitPresent.persist;
                return result;
            }

            set
            {
                type = value.type;

                if (unitPresent != null)
                {
                    unitPresent.sticker.Return();
                    unitPresent = null;
                }

                if (value.unitPresent != null)
                {
                    map.PlaceUnit(GameController.instance.pools.strategicMapUnitPool.Provide<MapUnit>(), gridCell.loc);
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
            persist = new StrategicCellPersist();
        }

        #endregion

        #region MonoBehaviour

        void Start()
        {
#if UNITY_EDITOR
            debugText.enabled = true;
#endif
            UpdateAppearance();
        }

        #endregion
    }
}
