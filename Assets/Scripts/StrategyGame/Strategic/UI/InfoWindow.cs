using StrategyGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyGame.Strategic.UI
{
    public class InfoWindow : UiElement
    {
        public UiElement closeButton;
        public UiElement moveButton;

        [SerializeField]
        private Text titleText = null;
        public string title
        {
            get { return titleText.text; }
            set { titleText.text = value; }
        }

        [SerializeField]
        private Text bodyText = null;
        public string body
        {
            get { return bodyText.text; }
            set { bodyText.text = value; }
        }
    }
}
