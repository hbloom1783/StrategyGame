using StrategyGame.UI;
using UnityEngine;

namespace StrategyGame.MainMenu.UI
{
    public class MainMenuUi : MonoBehaviour
    {
        #region Singleton

        private static MainMenuUi _instance = null;
        public static MainMenuUi instance
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

        public UiElement newGameButton;
        public UiElement loadGameButton;
        public UiElement settingsButton;
        public UiElement quitButton;

        #endregion
    }
}
