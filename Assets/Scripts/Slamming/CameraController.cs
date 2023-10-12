using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController main;

    Camera cam;
    Plane plane;

    private void Awake()
    {
        if (Camera.main.gameObject == gameObject)
            main = this;

        cam = GetComponent<Camera>();
        plane = new Plane(Vector3.forward, Vector3.zero);
    }

    public Vector2 MouseToWorldPoint()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
            return ray.GetPoint(distance);
        return Vector2.zero;
    }
}
