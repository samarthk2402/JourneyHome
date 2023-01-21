using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    private const float NORM_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;

    public Transform cam;
    public GameObject hookshot;
    private Player player;
    PlayerInput playerInput;
    InputAction grappleAction;
    InputAction jumpAction;

    private Vector3 hookshotPosition;
    private float hookshotSize;

    private CameraFOV cameraFov;
    public ParticleSystem speedLines;

    void Awake() {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        cameraFov = GetComponentInChildren<CameraFOV>();
        grappleAction = playerInput.actions["grapple"];
        jumpAction = playerInput.actions["jump"];
        hookshot.SetActive(false);
        speedLines.Stop();
    }

    public void HandleHookShotStart()
    {
        var grappleInput = grappleAction.ReadValue<float>();

        if(grappleInput>0)
        {
            if(Physics.Raycast(cam.position, cam.forward, out RaycastHit hit))
            {
                hookshot.transform.localScale = Vector3.zero;
                hookshot.SetActive(true);
                player.state = Player.State.HookshotThrown;
                hookshotSize = 0f; 
                hookshotPosition = hit.point;
            }
        }
    }

    public void HandleHookShotThrow()
    {
        hookshot.transform.LookAt(hookshotPosition);

        float throwSpeed = 70f;
        hookshotSize += throwSpeed * Time.deltaTime;
        hookshot.transform.localScale = new Vector3(1, 1, hookshotSize);

        if(hookshotSize > Vector3.Distance(transform.position, hookshotPosition))
        {
            cameraFov.SetCameraFov(HOOKSHOT_FOV);
            speedLines.Play();
            player.state = Player.State.HookshotFlyingPlayer;
        }
    }

    public void HandleHookMovement()
    {
        hookshotSize = Vector3.Distance(transform.position, hookshotPosition);
        hookshot.transform.localScale = new Vector3(1, 1, hookshotSize);
        hookshot.transform.LookAt(hookshotPosition);
        var jumpInput = jumpAction.ReadValue<float>();
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;
        player.controller.Move(hookshotDir*hookshotSpeed*hookshotSpeedMultiplier*Time.deltaTime);

        float reachedPos = 1f;
        if (Vector3.Distance(transform.position, hookshotPosition) < reachedPos)
        {
            player.state = Player.State.Normal;
            hookshot.SetActive(false);
            cameraFov.SetCameraFov(NORM_FOV);
            speedLines.Stop();
        }

        
    }
    
}
