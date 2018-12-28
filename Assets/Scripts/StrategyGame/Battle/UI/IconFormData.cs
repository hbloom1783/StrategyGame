using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrategyGame.Battle.UI
{
    [CreateAssetMenu(fileName = "FormData", menuName = "Athanor/UI/IconFormData", order = 1)]
    public class IconFormData : ScriptableObject
    {
        public Sprite image = null;
        public Color color = Color.white;
    }
}
