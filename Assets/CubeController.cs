using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CubeController : MonoBehaviour
{

    public event UnityAction<GameObject> Cleanup;

    private IEnumerator SpawnStart()
    {

        yield return new WaitForSeconds(1);

        Cleanup?.Invoke(gameObject);

        Cleanup = null;

    }

    private void OnEnable()
    {

        StartCoroutine(SpawnStart());

    }

}
