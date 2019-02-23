using GridLib.Generic;
using UnityEngine;

namespace GridLib.Square
{
    public class SquareGridManager : GridManager<SquareCoords>
    {
        void Start()
        {
            UpdateMatrices();
        }

        #region World-space mapping

        public override Vector3 GridToLocal(SquareCoords loc)
        {
            return gridTransform * new Vector3(loc.x, loc.y, 0);
        }

        public override SquareCoords LocalToGrid(Vector3 pos)
        {
            // Normal is crossproduct of axes
            Vector3 n = Vector3.Cross(gridX, gridY).normalized;

            // Project world coordinates onto in-world plane

            // This only works if the camera is looking straight into the plane
            //Vector3 projected = Vector3.ProjectOnPlane(pos, n);

            // This only works if the camera is looking straight up the Z axis
            float newZ = (-n.x * pos.x - n.y * pos.y) / n.z;
            Vector3 projected = new Vector3(pos.x, pos.y, newZ);

            // Change world-plane coordinates back into cube-plane coordinates
            Vector3 inverted = gridInverse * projected;
            inverted.z = -inverted.x - inverted.y;

            // Identify nearest cube-plane integer point
            return SquareCoords.Round(inverted);
        }

        #endregion
    }
}
