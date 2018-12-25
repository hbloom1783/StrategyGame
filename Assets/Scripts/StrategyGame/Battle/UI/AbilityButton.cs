using Athanor.Pooling;
using StrategyGame.Battle.Game.Abilities;
using StrategyGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyGame.Battle.UI
{
    class AbilityButton : UiElement, IPoolable
    {
        public PooledObject sticker { get { return GetComponent<PooledObject>(); } }

        [SerializeField]
        private Image icon = null;

        [SerializeField]
        private Button button = null;

        private IUnitAbility _ability = null;

        public bool interactable
        {
            get { return button.interactable; }
            set { button.interactable = value; }
        }

        public IUnitAbility ability
        {
            get { return _ability; }
            set
            {
                _ability = value;
                UpdateAppearance();
            }
        }

        private void UpdateAppearance()
        {
            if (ability == null)
                icon.sprite = null;
            else
                icon.sprite = ability.icon;
        }

        public void OnProvide()
        {
            ability = null;
        }

        public void OnReturn()
        {
        }
    }
}
