using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    [SerializeField] float cooldown;
    [SerializeField] float maxRange;
    private float cooldownTimer = 0f;
    private const float NORM_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;

    public Transform cam;
    public GameObject hookshot;
    private Player player;
    PlayerInput playerInput;
    InputAction grappleAction;
    InputAction jumpAction;
    InputAction moveAction;

    private Vector3 hookshotPosition;
    private float hookshotSize;

    private CameraFOV cameraFov;
    public ParticleSystem speedLines;

    private bool canGrapple = true;


    void Awake() {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        cameraFov = GetComponentInChildren<CameraFOV>();
        grappleAction = playerInput.actions["grapple"];
        jumpAction = playerInput.actions["jump"];
        moveAction = playerInput.actions["move"];
        hookshot.SetActive(false);
        speedLines.Stop();
    }

    void Update()
    {
        if (cooldownTimer>0){
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void HandleHookShotStart()
    {
        var grappleInput = grappleAction.ReadValue<float>();

        if(grappleInput>0)
        {
            if(Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxRange))
            {
                if (cooldownTimer<=0 && canGrapple){
                    hookshot.transform.localScale = Vector3.zero;
                    hookshot.SetActive(true);
                    player.state = Player.State.HookshotThrown;
                    hookshotSize = 0f; 
                    hookshotPosition = hit.point;
                }
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
        var moveInput = moveAction.ReadValue<Vector2>();
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;
        Vector3 hookshotDirWithInput = hookshotDir+new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 hookshotDirWithCam = Vector3.Slerp(cam.forward, hookshotDirWithInput, 0.5f);

        if (Vector3.Distance(transform.position, hookshotPosition)>maxRange){
            hookshotDirWithCam = hookshotDir;
        }else{
            hookshotDirWithCam = Vector3.Slerp(cam.forward, hookshotDir, 0.1f);
        }

    

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;
        player.controller.Move(hookshotDirWithCam*hookshotSpeed*hookshotSpeedMultiplier*Time.deltaTime);

        float reachedPos = 1f;
        //Debug.Log( Vector3.Angle(cam.forward, hookshotDir));
        if (Vector3.Distance(transform.position, hookshotPosition) < reachedPos)
        {
            StopGrapple(Player.State.Normal);
        }

        if (Vector3.Angle(cam.forward, hookshotDir)>120){
            //player.ySpeed += player.jumpSpeed/20;
            player.momentum = hookshotDirWithCam*(hookshotSpeed/8);
            StopGrapple(Player.State.Normal);

        }

        
    }

    void StopGrapple(Player.State state){
        player.state = state;
        hookshot.SetActive(false);
        cameraFov.SetCameraFov(NORM_FOV);
        speedLines.Stop();
        cooldownTimer = cooldown;
        canGrapple = false;
        StartCoroutine(waitForKeyRelease());    
    }

    IEnumerator waitForKeyRelease()
    {
        yield return new WaitUntil(grappleKeyRelease);
        canGrapple = true;
    }

    bool grappleKeyRelease()
    {
        var grappleInput = grappleAction.ReadValue<float>();
        if(grappleInput>0)
        {
            return false;
        }else{
            return true;
        }
    }
    
}
