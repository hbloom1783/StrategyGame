using System;
using System.Collections.Generic;
using UnityEngine;

namespace Athanor.Pooling
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;

        private Stack<GameObject> contents = new Stack<GameObject>();
        private int count = 0;

        public GameObject Provide()
        {
            GameObject obj;

            if (contents.Count > 0)
            {
                obj = contents.Pop();
                obj.SetActive(true);
            }
            else
            {
                obj = Instantiate(prefab);

                PooledObject sticker = obj.AddComponent<PooledObject>();
                sticker.parent = this;
                sticker.serialNumber = count++;

                obj.name = prefab.name + " [" + sticker.serialNumber + "]";
            }
            
            obj.transform.SetParent(null, false);

            return obj;
        }

        public T Provide<T>() where T : MonoBehaviour
        {
            if (prefab.GetComponent<T>() == null)
                throw new ArgumentException(name + " asked to provide wrong MonoBehaviour!");
            return Provide().GetComponent<T>();
        }

        public void Return(GameObject obj)
        {
            PooledObject sticker = obj.GetComponent<PooledObject>();

            if (sticker == null)
            {
                // No sticker - Destroy
                Destroy(obj);
            }
            else if (sticker.parent != this)
            {
                // Wrong parent - send home
                sticker.Return();
            }
            else if (contents.Contains(obj))
            {
                // Object already home - ignore
            }
            else
            {
                sticker.OnReturn();

                obj.transform.SetParent(transform, false);
                obj.transform.position = Vector3.zero;
                obj.name = prefab.name + " [" + sticker.serialNumber + "]";

                contents.Push(obj);

                obj.SetActive(false);
            }
        }

        public void Return<T>(T comp) where T : MonoBehaviour
        {
            Return(comp.gameObject);
        }
    }

    public interface IPoolable
    {
        void OnProvide();
        void OnReturn();
        PooledObject sticker { get; }
    }

    public class PooledObject : MonoBehaviour
    {
        public ObjectPool parent;
        public int serialNumber;

        public void Return()
        {
            parent.Return(this);
        }

        public void OnProvide()
        {
            foreach (IPoolable poolable in GetComponents<IPoolable>())
                poolable.OnProvide();
        }

        public void OnReturn()
        {
            foreach (IPoolable poolable in GetComponents<IPoolable>())
                poolable.OnReturn();
        }
    }
}
