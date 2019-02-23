using Athanor.EventHandling;
using GridLib.Matrix;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridLib.Generic
{
    public abstract class GridManager<TCoords> : MonoBehaviour where TCoords : class
    {
        #region Inventory

        protected Dictionary<TCoords, GridCell<TCoords>> gridContents =
            new Dictionary<TCoords, GridCell<TCoords>>();

        public IEnumerable<TCoords> coords { get { return gridContents.Keys; } }
        public IEnumerable<GridCell<TCoords>> cells { get { return gridContents.Values; } }

        public GridCell<TCoords> CellAt(TCoords loc)
        {
            if (InBounds(loc)) return gridContents[loc];
            else return null;
        }

        public GridCell<TCoords> this[TCoords loc] { get { return CellAt(loc); } }

        #region Init/Delete

        public void DeleteCell(TCoords loc)
        {
            DeleteCell(CellAt(loc));
        }

        public virtual void DeleteCell(GridCell<TCoords> cell)
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

        public virtual void InitCell(TCoords loc, GridCell<TCoords> newCell)
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

        #endregion

        #region Predicates

        public bool InBounds(TCoords loc)
        {
            return coords.Contains(loc);
        }

        public bool OutOfBounds(TCoords loc)
        {
            return !InBounds(loc);
        }

        #endregion

        #endregion

        #region World-space mapping

        public Vector3 gridX = new Vector3(1, 0, 0);
        public Vector3 gridY = new Vector3(0, 1, 0);

        protected Matrix3 gridTransform = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
        protected Matrix3 gridInverse = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public void UpdateMatrices()
        {
            gridTransform = new Matrix3(gridX, gridY);
            gridInverse = gridTransform.inverse;

            foreach (GridCell<TCoords> cell in cells)
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

        public Vector3 GridToWorld(TCoords loc)
        {
            return transform.localToWorldMatrix.MultiplyPoint(GridToLocal(loc));
        }

        public TCoords WorldToGrid(Vector3 pos)
        {
            return LocalToGrid(transform.worldToLocalMatrix.MultiplyPoint(pos));
        }

        abstract public Vector3 GridToLocal(TCoords loc);

        abstract public TCoords LocalToGrid(Vector3 pos);

        public TCoords mousePosition { get { return WorldToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)); } }

        public GridCell<TCoords> mouseCell { get { return InBounds(mousePosition) ? CellAt(mousePosition) : null; } }

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
