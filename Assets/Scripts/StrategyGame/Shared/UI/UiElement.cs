using StrategyGame.Game;
using UnityEngine;

namespace StrategyGame.UI
{
    public class UiElement : MonoBehaviour
    {
        #region Showing and hiding

        public bool shownOnAwake = true;

        public bool shown
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
                Canvas.ForceUpdateCanvases();
            }
        }

        public void ToggleShown()
        {
            shown = !shown;
        }

        void Awake()
        {
            shown = shownOnAwake;
        }

        #endregion

        #region Events

        protected GameController game { get { return GameController.instance; } }

        public void SendUiSignal()
        {
            game.input.ReceiveUISignal(this);
        }

        #endregion
    }
}
