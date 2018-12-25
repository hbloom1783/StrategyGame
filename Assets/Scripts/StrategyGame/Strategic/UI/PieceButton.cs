using Athanor.Collections;
using Athanor.Pooling;
using GridLib.Hex;
using StrategyGame.Strategic.Map;
using StrategyGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyGame.Strategic.UI
{
    class PieceButton : UiElement, IPoolable
    {
        private MapPiece _piece = null;
        public MapPiece piece
        {
            get { return _piece; }
            set
            {
                _piece = value;
                UpdateDisplay();
            }
        }

        public Sprite moonSprite = null;
        public Sprite greenSprite = null;

        #region Cell management

        [Serializable]
        public class DisplayCell
        {
            public Vector2 loc = Vector2.zero;
            public Image image = null;

            public KeyValuePair<HexCoords, Image> ToKeyValuePair()
            {
                return new KeyValuePair<HexCoords, Image>(
                    new HexCoords((int)loc.x, (int)loc.y),
                    image);
            }
        }

        public DisplayCell[] cellMap = new DisplayCell[0];

        private Dictionary<HexCoords, Image> _cellDict = null;
        public Dictionary<HexCoords, Image> cellDict
        {
            get
            {
                if (_cellDict == null) InitCellDict();
                return _cellDict;
            }
        }

        private void InitCellDict()
        {
            _cellDict = cellMap
                .Select(x => x.ToKeyValuePair())
                .FormDictionary();
        }

        #endregion

        private void UpdateDisplay()
        {
            foreach(Image cellImage in cellDict.Values)
                cellImage.enabled = false;

            foreach(var kv in _piece.content)
            {
                if (cellDict.Keys.Contains(kv.Key))
                {
                    switch (kv.Value)
                    {
                        case CellType.greenPlain:
                        case CellType.greenCity:
                            cellDict[kv.Key].enabled = true;
                            cellDict[kv.Key].sprite = greenSprite;
                            break;

                        case CellType.moonPlain:
                        case CellType.moonCrashSite:
                            cellDict[kv.Key].enabled = true;
                            cellDict[kv.Key].sprite = moonSprite;
                            break;

                        default:
                        case CellType.empty:
                            break;
                    }
                }
            }
        }

        #region Poolable

        public PooledObject sticker { get { return GetComponent<PooledObject>(); } }

        public void OnProvide()
        {
        }

        public void OnReturn()
        {
            foreach (Image cellImage in cellDict.Values)
                cellImage.enabled = false;
        }

        #endregion
    }
}
