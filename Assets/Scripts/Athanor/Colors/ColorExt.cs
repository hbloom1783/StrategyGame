using UnityEngine;

namespace Athanor.Colors
{
    public static class ColorExt
    {
        public static Color withR(this Color c, float newR)
        {
            return new Color(newR, c.g, c.b, c.a);
        }

        public static Color withG(this Color c, float newG)
        {
            return new Color(c.r, newG, c.b, c.a);
        }

        public static Color withB(this Color c, float newB)
        {
            return new Color(c.r, c.g, newB, c.a);
        }

        public static Color withA(this Color c, float newA)
        {
            return new Color(c.r, c.g, c.b, newA);
        }
    }
}
