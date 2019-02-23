using UnityEngine;

namespace Athanor.Clamping
{
    public static class ClampExt
    {
        public static float Clamp(this float f, float min, float max)
        {
            if (f < min) return min;
            else if (f > max) return max;
            else return f;
        }

        public static Vector3 Clamp(this Vector3 v, Rect bounds)
        {
            float x = v.x;
            float y = v.y;

            if (x < bounds.xMin) x = bounds.xMin;
            else if (x > bounds.xMax) x = bounds.xMax;

            if (y < bounds.yMin) y = bounds.yMin;
            else if (y > bounds.yMax) y = bounds.yMax;

            return new Vector3(x, y, v.z);
        }
    }
}
