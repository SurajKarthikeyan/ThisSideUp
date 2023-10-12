using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageShaderValues : MonoBehaviour
{

    SpriteRenderer render;
    Rigidbody2D rigid;

    float delayedRotation;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (render.material.HasProperty("_DelayedRotation"))
        {
            delayedRotation = Mathf.LerpAngle(delayedRotation, transform.eulerAngles.z, 0.05f);
            render.material.SetFloat("_DelayedRotation", -delayedRotation);
        }

        if (render.material.HasProperty("_Rotation"))
            render.material.SetFloat("_Rotation", -transform.eulerAngles.z);
    }
}
