using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    Camera mainCamera;

    Ray ray;
    RaycastHit hitInfo;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;

        Physics.Raycast(ray, out hitInfo);

        //Works like Doom 3 / Cyberpunk panel interaction
        transform.position = hitInfo.point;
    }
}
