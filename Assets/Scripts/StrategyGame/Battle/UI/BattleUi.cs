using StrategyGame.UI;
using UnityEngine;

namespace StrategyGame.Battle.UI
{
    public class BattleUi : MonoBehaviour
    {
        #region Singleton

        private static BattleUi _instance = null;
        public static BattleUi instance
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

        public SmoothCam smoothCam = null;

        public ColorText marqueeText = null;

        public UiElement systemMenuButton = null;
        public UiElement endTurnButton = null;

        public ColorText instructions = null;
        public UiElement confirmButton = null;
        public ElementHolder abilityBar = null;

        public SystemMenu systemMenu = null;

        #endregion
    }
}
