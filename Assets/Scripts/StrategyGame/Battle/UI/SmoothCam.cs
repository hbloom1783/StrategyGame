using Athanor.Clamping;
using StrategyGame.Game;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyGame.Battle.UI
{
    public class SmoothCam : MonoBehaviour
    {
        #region Shorthands

        private GameController game { get { return GameController.instance; } }

        #endregion

        #region Camera position

        private Rect _posBounds = Rect.zero;
        public Rect posBounds
        {
            get { return _posBounds; }
            set
            {
                _posBounds = value;
                if (!posBounds.Contains(pos))
                    posDst = posDst.Clamp(posBounds);
            }
        }

        private Vector3 _posDst;
        public Vector3 posDst
        {
            get { return _posDst; }
            set
            {
                _posDst = new Vector3(value.x, value.y, transform.position.z).Clamp(posBounds);
            }
        }
        public float posDstX
        {
            get { return posDst.x; }
            set
            {
                posDst = new Vector3(value, posDst.y, posDst.z);
            }
        }
        public float posDstY
        {
            get { return posDst.y; }
            set
            {
                posDst = new Vector3(posDst.x, value, posDst.z);
            }
        }

        public Vector3 pos
        {
            get { return transform.position; }
            set
            {
                posDst = value;
                transform.position = posDst;
            }
        }

        #endregion

        #region Camera size

        public float minSize = 2.5f;
        public float maxSize = 10.0f;

        private float _sizeDst = 5.0f;
        public float sizeDst
        {
            get { return _sizeDst; }
            set { _sizeDst = value.Clamp(minSize, maxSize); }
        }

        public float size
        {
            get { return myCamera.orthographicSize; }
            set
            {
                sizeDst = value;
                myCamera.orthographicSize = sizeDst;
            }
        }

        #endregion

        #region Camera lock functionality

        private Stack<Transform> lockStack = new Stack<Transform>();

        public void PushLock(Transform t)
        {
            lockStack.Push(t);
        }

        public void PopLock()
        {
            lockStack.Pop();
        }

        public Transform lockTarget
        {
            get
            {
                if (lockStack.Count > 0) return lockStack.Peek();
                else return null;
            }
        }

        #endregion

        #region Monobehaviour

        private Camera myCamera { get { return GetComponent<Camera>(); } }

        public float t = 0.5f;

        void Start()
        {
            posDst = transform.position;
        }

        void Update()
        {
            if (lockTarget != null)
            {
                pos = lockTarget.position;
                size = 5.0f;
                /*posDst = lockTarget.position;
                sizeDst = 5.0f;
                transform.position =
                    Vector3.Lerp(transform.position, posDst, t);
                myCamera.orthographicSize =
                    Mathf.Lerp(myCamera.orthographicSize, sizeDst, t);*/
            }
            else
            {
                transform.position =
                    Vector3.Lerp(transform.position, posDst, t);
                myCamera.orthographicSize =
                    Mathf.Lerp(myCamera.orthographicSize, sizeDst, t);
            }
        }

        #endregion
    }
}
