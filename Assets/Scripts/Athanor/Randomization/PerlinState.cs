using UnityEngine;
using Random = UnityEngine.Random;

namespace Athanor.Randomization
{
    class PerlinState
    {
        public float xScale = 1.0f;
        public float yScale = 1.0f;

        private float xOffset = 0.0f;
        private float yOffset = 0.0f;

        public PerlinState()
        {
            xOffset = Random.Range(0.0f, 10000.0f);
            yOffset = Random.Range(0.0f, 10000.0f);
        }

        public PerlinState(float xScale, float yScale) : this()
        {
            this.xScale = xScale;
            this.yScale = yScale;
        }

        public float Sample(float x, float y)
        {
            return Mathf.PerlinNoise(
                (x * xScale) + xOffset,
                (y * yScale) + yOffset);
        }
    }
}
