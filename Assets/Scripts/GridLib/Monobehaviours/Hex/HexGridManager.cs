using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GridLib.Extensions;
using GridLib.Matrix;
using Athanor.EventHandling;

namespace GridLib.Hex
{
    public class HexGridManager<TCell> : MonoBehaviour where TCell : HexGridCell
    {
        #region Grid inventory

        protected Dictionary<HexCoords, TCell> gridContents = new Dictionary<HexCoords, TCell>();
        
        public IEnumerable<HexCoords> coords { get { return gridContents.Keys; } }

        public IEnumerable<TCell> cells { get { return gridContents.Values; } }

        public bool InBounds(HexCoords loc)
        {
            return coords.Contains(loc);
        }

        public bool OutOfBounds(HexCoords loc)
        {
            return !InBounds(loc);
        }

        public TCell CellAt(HexCoords loc)
        {
            if (InBounds(loc)) return gridContents[loc];
            else return null;
        }

        public TCell this[HexCoords loc] { get { return CellAt(loc); } }

        public void DeleteCell(HexCoords loc)
        {
            DeleteCell(CellAt(loc));
        }

        public virtual void DeleteCell(TCell cell)
        {
            if (cell != null)
            {
                if (cell.loc != null) gridContents.Remove(cell.loc);

                if (Application.isPlaying)
                    Destroy(cell.gameObject);
                else
                    DestroyImmediate(cell.gameObject);
            }
        }

        public virtual void InitCell(HexCoords loc, TCell newCell)
        {
            // If another cell exists at this loc, clear it
            if (InBounds(loc)) DeleteCell(loc);

            // Initialize cell
            newCell.name = loc.ToString();
            newCell.loc = loc;
            newCell.transform.SetParent(transform);
            newCell.transform.localPosition = GridToLocal(loc);

            // Try to set up events
            if (newCell.events != null) newCell.events.parent = events;

            // Store cell
            gridContents[loc] = newCell;
        }

        public IEnumerable<HexCoords> ValidNeighbors(HexCoords loc)
        {
            return loc.neighbors.Where(InBounds);
        }

        #endregion

        #region Initialization

        void Start()
        {
            if (Application.isPlaying)
                InitGrid();
        }

        public void ClearGrid()
        {
            GetComponentsInChildren<TCell>().ForEach(DeleteCell);
            UpdateMatrices();
        }

        public void InitGrid()
        {
            ClearGrid();

            // Call our grid initializers
            GetComponents<HexGridInitializer<TCell>>()
                .ForEach(x => x.InitGrid(this, Application.isPlaying));
        }

        #endregion

        #region World-space mapping

        public Vector3 gridX = new Vector3(1, 0, 0);
        public Vector3 gridY = new Vector3(0, 1, 0);

        private Matrix3 gridTransform = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
        private Matrix3 gridInverse = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public void UpdateMatrices()
        {
            gridTransform = new Matrix3(gridX, gridY);
            gridInverse = gridTransform.inverse;

            foreach (TCell cell in cells)
                InitCell(cell.loc, cell);
        }

        readonly private static Vector3 squareX = new Vector3(1, 0, 0);
        readonly private static Vector3 squareY = new Vector3(0, 1, 0);

        public void SetSquareDefault()
        {
            gridX = squareX;
            gridY = squareY;
            UpdateMatrices();
        }

        readonly private static Vector3 pointyX = new Vector3(1, 0, 0);
        readonly private static Vector3 pointyY = new Vector3(0.5f, Mathf.Sqrt(3) / 2, 0);

        public void SetPointyDefault()
        {
            gridX = pointyX;
            gridY = pointyY;
            UpdateMatrices();
        }

        readonly private static Vector3 flatX = new Vector3(Mathf.Sqrt(3) / 2, 0.5f, 0);
        readonly private static Vector3 flatY = new Vector3(0, 1, 0);

        public void SetFlatDefault()
        {
            gridX = flatX;
            gridY = flatY;
            UpdateMatrices();
        }

        public Vector3 GridToLocal(HexCoords loc)
        {
            return gridTransform * new Vector3(loc.x, loc.y, 0);
        }

        public Vector3 GridToWorld(HexCoords loc)
        {
            return transform.localToWorldMatrix.MultiplyPoint(GridToLocal(loc));
        }

        public HexCoords LocalToGrid(Vector3 pos)
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
            return HexCoords.Round(inverted);
        }

        public HexCoords WorldToGrid(Vector3 pos)
        {
            return LocalToGrid(transform.worldToLocalMatrix.MultiplyPoint(pos));
        }

        public HexCoords mousePosition { get { return WorldToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)); } }

        public TCell mouseCell { get { return InBounds(mousePosition) ? CellAt(mousePosition) : null; } }

        #endregion

        #region Event handling

        private PointerEventAggregator _events = null;
        public PointerEventAggregator events
        {
            get
            {
                if (_events == null) _events = GetComponent<PointerEventAggregator>();
                return _events;
            }
            set
            {
                _events = value;
            }
        }

        #endregion
    }
}
