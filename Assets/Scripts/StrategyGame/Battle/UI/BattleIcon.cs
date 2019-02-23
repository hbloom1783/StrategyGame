using Athanor.Pooling;
using StrategyGame.UI;
using UnityEngine.UI;

namespace StrategyGame.Battle.UI
{
    public class BattleIcon : UiElement, IPoolable
    {
        public IconFormData heartForm;
        public IconFormData actionForm;

        private IconFormData SwitchForm(IconForm form)
        {
            switch(form)
            {
                case IconForm.heart:
                    return heartForm;

                case IconForm.action:
                    return actionForm;

                default:
                case IconForm.invalid:
                    return new IconFormData();
            }
        }

        private IconForm _form;
        public IconForm form
        {
            get { return _form; }
            set
            {
                _form = value;
                IconFormData data = SwitchForm(_form);
                GetComponent<Image>().sprite = data.image;
                GetComponent<Image>().color = data.color;
            }
        }

        #region Poolable

        public void OnProvide()
        {
        }

        public void OnReturn()
        {
        }

        public PooledObject sticker { get { return GetComponent<PooledObject>(); } }

        #endregion
    }

    public enum IconForm
    {
        invalid,
        heart,
        action,
    }

}
