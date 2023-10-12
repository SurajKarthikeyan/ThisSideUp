using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [Tooltip("The transform to follow")]
    [SerializeField] Transform target;

    Vector3 offset;

    private void Start()
    {
        offset = transform.position - target.position;
    }

    private void Update()
    {
        transform.position = target.position + offset;
    }
}
