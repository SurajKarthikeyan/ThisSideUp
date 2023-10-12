using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [Tooltip("How long until this object is destroyed")]
    public float lifetime;

    private void Start()
    {
        StartCoroutine(nameof(DelayedDestroy), lifetime);
    }

    private IEnumerator DelayedDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
