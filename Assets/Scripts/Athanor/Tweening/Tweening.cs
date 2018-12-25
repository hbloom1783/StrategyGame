using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athanor.Tweening
{
    public static class TweeningExt
    {
        #region Helper functions

        public static IEnumerable<float> LinearTimeTween(float duration)
        {
            for (float timeElapsed = 0.0f; timeElapsed < duration; timeElapsed += Time.deltaTime)
                yield return timeElapsed / duration;

            // Just to be sure
            yield return 1.0f;
        }

        public static IEnumerable<float> ReverseLinearTimeTween(float duration)
        {
            for (float timeElapsed = duration; timeElapsed > 0.0f; timeElapsed -= Time.deltaTime)
                yield return timeElapsed / duration;

            // Just to be sure
            yield return 0.0f;
        }

        // Assumes a parabola crossing (0,0) (0.5,1) (1,0)
        // y = 4x - 4x^2 = 4x(1 - x);
        private static float ParabolicHeight(float x)
        {
            return (4 * x) * (1 - x);
        }

        #endregion

        #region Transform position

        public static IEnumerator LinearTween(
            this Transform transform,
            Vector3 src,
            Vector3 dst,
            float duration = 1.0f)
        {
            foreach (float t in LinearTimeTween(duration))
            {
                transform.position = Vector3.Lerp(src, dst, t);
                yield return null;
            }
        }

        public static IEnumerator LinearTween(
            this Transform transform,
            Vector3 dst,
            float duration = 1.0f)
        {
            return LinearTween(transform, transform.position, dst, duration);
        }

        public static IEnumerator ParabolicTween(
            this Transform transform,
            Vector3 src,
            Vector3 dst,
            float height = 1.0f,
            float duration = 1.0f)
        {
            foreach (float t in LinearTimeTween(duration))
            {
                Vector3 newPos = Vector3.Lerp(src, dst, t);
                newPos.y += height * ParabolicHeight(t);
                transform.position = newPos;
                yield return null;
            }
        }

        public static IEnumerator ParabolicTween(
            this Transform transform,
            Vector3 dst,
            float height = 1.0f,
            float duration = 1.0f)
        {
            return ParabolicTween(transform, transform.position, dst, height, duration);
        }

        #endregion

        #region Spriterenderer Color

        public static IEnumerator ColorTween(
            this SpriteRenderer sprite,
            Color src,
            Color dst,
            float duration = 1.0f)
        {
            foreach (float t in LinearTimeTween(duration))
            {
                sprite.color = Color.Lerp(src, dst, t);
                yield return null;
            }
        }

        public static IEnumerator ColorTween(
            this SpriteRenderer sprite,
            Color dst,
            float duration = 1.0f)
        {
            return ColorTween(sprite, sprite.color, dst, duration);
        }

        #endregion
    }
}
