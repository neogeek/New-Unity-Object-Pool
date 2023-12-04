using System.Collections;
using UnityEngine;

public class SpawnCubes : MonoBehaviour
{

    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private ObjectPoolController _objectPoolController;

    [SerializeField]
    private int _groupCount = 10;

    [SerializeField]
    private float _delayBetweenSpawns = 0.05f;

    [SerializeField]
    private float _delayBetweenGroups = 1;

    private IEnumerator Start()
    {

        while (true)
        {

            yield return SpawnObjects();

            yield return new WaitForSeconds(_delayBetweenGroups);

        }

    }

    private IEnumerator SpawnObjects()
    {

        var spawned = 0;

        while (spawned < _groupCount)
        {

            yield return new WaitForSeconds(_delayBetweenSpawns);

            var cubeController = _objectPoolController.Retrieve(_prefab, Vector3.zero, Quaternion.identity)
                .GetComponent<CubeController>();

            cubeController.Cleanup += go => _objectPoolController.Release(_prefab, go);

            spawned += 1;

        }

    }

}
