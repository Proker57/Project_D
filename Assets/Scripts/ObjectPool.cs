using System.Collections.Generic;
using BOYAREngine.Units;
using UnityEngine;

namespace BOYAREngine.Utils
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject _parent;
        [SerializeField] private GameObject _objectToPool;
        public int Amount;
        [SerializeField] private List<GameObject> _pooledObjects;

        private void Awake()
        {
            _pooledObjects = new List<GameObject>();
            for (var i = 0; i < Amount; i++)
            {
                var temp = Instantiate(_objectToPool);
                temp.GetComponent<BulletBase>().Ship = GetComponent<UnitBase>();
                temp.GetComponent<BulletBase>().IsAlly = GetComponent<UnitBase>().IsAlly;
                temp.transform.parent = _parent.transform;
                temp.SetActive(false);
                _pooledObjects.Add(temp);
            }
        }

        public GameObject GetPooledObject()
        {
            for (var i = 0; i < Amount; i++)
            {
                if (!_pooledObjects[i].activeInHierarchy)
                {
                    return _pooledObjects[i];
                }
            }

            return null;
        }
    }
}

