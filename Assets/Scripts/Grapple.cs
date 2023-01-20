using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    public Transform cam;
    private Player player;
    PlayerInput playerInput;
    InputAction grappleAction;

    private Vector3 hookshotPosition;

    void Awake() {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        grappleAction = playerInput.actions["grapple"];
    }

    public void HandleHookShotStart()
    {
        var grappleInput = grappleAction.ReadValue<float>();

        if(grappleInput>0)
        {
            if(Physics.Raycast(cam.position, cam.forward, out RaycastHit hit))
            {
                player.state = Player.State.HookshotFlyingPlayer; 
                hookshotPosition = hit.point;
            }
        }
    }

    public void HandleHookMovement()
    {
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
        }
    }
}
