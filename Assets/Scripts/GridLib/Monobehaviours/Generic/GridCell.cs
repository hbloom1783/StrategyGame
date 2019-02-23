using Athanor.EventHandling;
using UnityEngine;

namespace GridLib.Generic
{
    public class GridCell<TCoords> : MonoBehaviour where TCoords : class
    {
        #region Serialization

        private TCoords _loc = null;
        public TCoords loc
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
