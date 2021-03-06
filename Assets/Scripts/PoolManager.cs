﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Innovision.Helpers
{
    public class PoolManager<T> where T : IPoolItem
    {
        #region Properties
        private List<T> _pool;
        private List<T> _active;
        private Transform _container;
        private bool _autoReuse;
        #endregion

        #region Controls
        public PoolManager(Transform container, bool autoReuse = false)
        {
            _autoReuse = autoReuse;
            _pool = new List<T>();
            _active = new List<T>();

            if (container == null)
                Debug.LogError("Pool container is null");

            _container = container;
            T[] children = _container.GetComponentsInChildren<T>(true);
            foreach (T child in children)
            {
                child.Init();
                _pool.Add(child);
            }
        }

        public virtual T GetItemFromPool()
        {
            if (_pool.Count > 0)
            {
                T item = _pool[0];
                _pool.RemoveAt(0);
                _active.Add(item);
                return item;
            }
            else if (_autoReuse && _active.Count > 0)
            {
                ReturnItemToPool(_active[0]);
                return GetItemFromPool();
            }
            else
            {
                Debug.LogError("Not enough items in the pool");
                return default(T);
            }
        }

        public virtual T[] GetItemFromPool(int number)
        {
            List<T> items = new List<T>();
            for (int i = 0; i < number; i++)
            {
                T item = GetItemFromPool();
                if (item != null)
                    items.Add(item);
            }

            return items.ToArray();
        }

        public virtual void ReturnItemToPool(T item)
        {
            _active.Remove(item);
            item.Deactivate();
            _pool.Add(item);
        }

        public virtual void ReturnItemToPoolAtIndex(int index)
        {
            T item = _active[index];
            _active.RemoveAt(index);
            item.Deactivate();
            _pool.Add(item);
        }

        public void ReturnAllItemsToPool()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                ReturnItemToPoolAtIndex(i);
        }
        #endregion

        #region Gettter
        public List<T> Active
        {
            get { return _active; }
        }
        #endregion
    }
}