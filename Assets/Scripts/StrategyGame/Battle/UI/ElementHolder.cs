using Athanor.Pooling;
using StrategyGame.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StrategyGame.Battle.UI
{
    public class ElementHolder : UiElement
    {
        private List<UiElement> stored = new List<UiElement>();

        public void Clear()
        {
            foreach(UiElement element in stored.ToList())
            {
                if (element is IPoolable)
                {
                    (element as IPoolable).sticker.Return();
                }
                else
                {
                    if (Application.isPlaying)
                        Destroy(element.gameObject);
                    else
                        DestroyImmediate(element.gameObject);
                }

                stored.Remove(element);
            }
        }

        public void Store(UiElement element)
        {
            element.transform.SetParent(transform, false);
            stored.Add(element);
        }
    }
}
