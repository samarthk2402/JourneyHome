using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    private Camera cam;
    private float targetFov;
    private float fov;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetFov = cam.fieldOfView;
        fov = targetFov;
    }

    private void Update()
    {
        float fovSpeed = 4f;
        fov = Mathf.Lerp(fov, targetFov, fovSpeed*Time.deltaTime);
        cam.fieldOfView = fov; 
    }

    public void SetCameraFov(float targetFov)
    {
        this.targetFov = targetFov;
    }
}
