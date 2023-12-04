using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolController : MonoBehaviour
{

    [Serializable]
    public struct PooledPrefab
    {

        public GameObject prefab;

        public int count;

    }

    [SerializeField]
    private List<PooledPrefab> _pooledPrefabs;

    [SerializeField]
    private bool _showDebug = true;

    private readonly Dictionary<GameObject, ObjectPool<GameObject>> _pooledObjects = new();

    private void Start()
    {
        foreach (var pooledPrefab in _pooledPrefabs)
        {
            _pooledObjects.Add(pooledPrefab.prefab,
                new ObjectPool<GameObject>(() =>
                    {
                        var go = Instantiate(pooledPrefab.prefab);

                        go.name = $"{pooledPrefab.prefab.name} ({_pooledObjects[pooledPrefab.prefab].CountAll + 1})";

                        return go;
                    },
                    go => go.SetActive(true),
                    go => go.SetActive(false),
                    Destroy,
                    defaultCapacity : pooledPrefab.count));

            var prewarmObjects = new HashSet<GameObject>();

            for (var i = 0; i < pooledPrefab.count; i++)
            {
                prewarmObjects.Add(_pooledObjects[pooledPrefab.prefab].Get());
            }

            foreach (var prewarmObject in prewarmObjects)
            {
                _pooledObjects[pooledPrefab.prefab].Release(prewarmObject);
            }
        }
    }

    public GameObject Retrieve(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!_pooledObjects.ContainsKey(prefab))
        {
            Debug.LogWarning($"{prefab.name} is not part of the object pool.");

            return Instantiate(prefab, position, rotation);
        }

        var go = _pooledObjects[prefab].Get();

        go.transform.position = position;
        go.transform.rotation = rotation;

        return go;
    }

    public void Release(GameObject prefab, GameObject go)
    {
        _pooledObjects[prefab].Release(go);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {

        if (!_showDebug) return;

        GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 0, 10, 0) });

        GUILayout.Label($"Current FPS: {Math.Round(Time.frameCount / Time.time, 2)}");

        foreach (var pooledPrefab in _pooledPrefabs)
        {

            GUILayout.Label($"Pooled Prefab: {pooledPrefab.prefab.name}");

            if (_pooledObjects.ContainsKey(pooledPrefab.prefab))
            {
                GUILayout.Label($"↳ Total: {_pooledObjects[pooledPrefab.prefab].CountAll}" +
                                $" Active: {_pooledObjects[pooledPrefab.prefab].CountActive}" +
                                $" Inactive: {_pooledObjects[pooledPrefab.prefab].CountInactive}");
            }
        }

        GUILayout.EndVertical();

    }
#endif

}
