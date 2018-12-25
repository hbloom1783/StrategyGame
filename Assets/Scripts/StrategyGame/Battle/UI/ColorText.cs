using StrategyGame.UI;
using Athanor.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace StrategyGame.Battle.UI
{
    public class ColorText : UiElement
    {
        public Text textScript = null;
        public string text
        {
            get { return textScript.text; }
            set { textScript.text = value; }
        }

        public IEnumerator ColorTween(Color src, Color dst, float duration)
        {
            Text text = GetComponent<Text>();

            foreach (float t in TweeningExt.LinearTimeTween(duration))
            {
                text.color = Color.Lerp(src, dst, t);
                yield return null;
            }
        }
    }
}