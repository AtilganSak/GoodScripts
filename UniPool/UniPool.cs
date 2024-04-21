using UnityEngine;
using UnityEngine.Pool;

namespace UnityPool
{
    public class MonoPoolItem : MonoBehaviour
    {
        public IObjectPool<MonoPoolItem> pool;

        protected void ReturnToPool()
        {
            pool.Release(this);
        }
        public virtual void OnCreated()
        {

        }
        public virtual void OnReturned()
        {

        }
        public virtual void OnTaken()
        {

        }
    }
    public enum PoolType
    {
        Stack,
        LinkedList
    }
    [System.Serializable]
    public class UniPool<T> where T : MonoBehaviour
    {
        private MonoPoolItem poolObject;
        private IObjectPool<MonoPoolItem> pool;
        private Transform parent;

        /// <param name="_poolObject">The pool object to be created.</param>
        /// <param name="_defSize">Specifies the number of objects that will be in the pool when it is initially created.</param>
        /// <param name="_maxSize">The maximum size of the pool. When the pool reaches the max size then any further instances returned to the pool will be ignored and can be garbage collected. This can be used to prevent the pool growing to a very large size.</param>
        /// <param name="_poolType">"PoolType refers to a specific strategy or behavior used to manage object pools.</param>
        /// <param name="_collectionChecks">Collection checks are performed when an instance is returned back to the pool. An exception will be thrown if the instance is already in the pool. Collection checks are only performed in the Editor.</param>
        /// <param name="_createParent">The parent of created objects.</param>
        public UniPool(MonoPoolItem _poolObject, int _defSize = 10, int _maxSize = 100, PoolType _poolType = PoolType.Stack, bool _collectionChecks = true, bool _createParent = true)
        {
            poolObject = _poolObject;

            if (_createParent && parent == null)
            {
                parent = new GameObject(_poolObject.name + " Pool").transform;
            }

            if (_poolType == PoolType.Stack)
            {
                //A repository is created for reuse of objects. Objects are stored in the pool and reused when needed. Usually a pool of the same type and fixed size is used. It is better in terms of performance.
                pool = new ObjectPool<MonoPoolItem>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, _collectionChecks, _defSize, _maxSize);
            }
            else
            {
                //Stores and manages objects using linked lists. These linked lists provide a structure in which objects in the repository are linked to each other and objects retrieved or returned are managed through these connections.
                //Addition and removal of objects are often done using dynamic memory allocation, which can increase the speed of insertions and removals but may use more memory for management.
                pool = new LinkedPool<MonoPoolItem>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, _collectionChecks, _maxSize);
            }
        }

        /// <summary>
        /// This method returns the MonoPoolItem class by casting it to the generic class.
        /// </summary>        
        public T Get()
        {
            return pool.Get() as T;
        }

        public MonoPoolItem GetMonoItem()
        {
            return pool.Get();
        }

        public void Release(MonoPoolItem poolObject)
        {
            pool.Release(poolObject);
        }

        public void ClearPool()
        {
            pool.Clear();
        }

        public int GetInactive() => pool.CountInactive;

        MonoPoolItem CreatePooledItem()
        {
            MonoPoolItem newItem = UnityEngine.Object.Instantiate(poolObject);
            newItem.pool = pool;
            if (parent != null)
            {
                newItem.transform.SetParent(parent);
            }
            newItem.OnCreated();
            return newItem;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(MonoPoolItem item)
        {
            item.transform.SetParent(parent);
            item.gameObject.SetActive(false);
            item.OnReturned();
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(MonoPoolItem item)
        {
            item.gameObject.SetActive(true);
            item.OnTaken();
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.-
        void OnDestroyPoolObject(MonoPoolItem item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}