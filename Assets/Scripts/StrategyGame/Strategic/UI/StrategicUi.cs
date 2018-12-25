using StrategyGame.UI;
using UnityEngine;

namespace StrategyGame.Strategic.UI
{
    public class StrategicUi : MonoBehaviour
    {
        #region Singleton

        private static StrategicUi _instance = null;
        public static StrategicUi instance
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

        #region UI Elements

        public UiElement battleButton = null;

        public UiElement systemMenuButton = null;
        public SystemMenu systemMenu = null;

        public UiElement pieceMenuButton = null;
        public PieceMenu pieceMenu = null;

        public InfoWindow infoWindow = null;

        #endregion
    }
}
