using Athanor.EventHandling;
using UnityEngine;

namespace GridLib.Hex
{
    public class HexGridCell : MonoBehaviour
    {
        #region Serialization

        private HexCoords _loc = null;
        public HexCoords loc
        {
            get { return _loc; }
            set { _loc = value; }
        }

        #endregion

        #region Event handling

        private PointerEventHandler _events = null;
        public PointerEventHandler events
        {
            get
            {
                if (_events == null) _events = GetComponent<PointerEventHandler>();
                return _events;
            }
            set
            {
                _events = value;
            }
        }

        #endregion
    }
}
