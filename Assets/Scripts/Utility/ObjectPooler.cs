using System.Collections.Generic;
using UnityEngine;

namespace JoyKit.Utility
{
    public class ObjectPooler<T>  where T : Component
    {
        private readonly T prefab;
        private readonly Queue<T> objects = new Queue<T>();

        public ObjectPooler(T prefab)
        {
            this.prefab = prefab;
        }

        public T Get()
        {
            if (objects.Count == 0)
            {
                return Object.Instantiate(prefab);
            }
            else
            {
                var obj = objects.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }

        public void Clear()
        {
            while (objects.Count > 0)
            {
                Object.Destroy(objects.Dequeue().gameObject);
            }
        }
    }
}
