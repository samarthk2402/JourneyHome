using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleShot : MonoBehaviour
{
    //public GameObject grappleHook;
    public Player playerScript;
    public Transform cam;
    public Transform gunTip;
    public LineRenderer lr;
    [SerializeField] float grappleSpeed = 20f;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float cdTimer;
    [SerializeField] float cd;
    [SerializeField] float grappleDelayTime;
    [SerializeField] float overshootYAxis;

    PlayerInput playerInput;
    InputAction gearAction;

    private bool isGrappling = false;
    private Vector3 grapplePoint;

    void Awake()
    {
        playerScript = GetComponentInParent<Player>();
        lr = GetComponent<LineRenderer>();
        playerInput = GetComponentInParent<PlayerInput>();
        gearAction = playerInput.actions["gear"];
    }

    void LateUpdate()
    {
        if (isGrappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    void Update()
    {
        var gearInput = gearAction.ReadValue<float>();

        if (gearInput>0 && !isGrappling) StartGrapple();

        if (cdTimer>0)
        {
            cdTimer -= Time.deltaTime;
        }

        // if (isGrappling)
        // {
        //     player.transform.position = Vector3.MoveTowards(player.transform.position, grapplePoint, grappleSpeed * Time.deltaTime);
        // }

        // if (gearInput<=0)
        // {
        //     isGrappling = false;
        //     //grappleHook.SetActive(false);
        // }
    }

    void StartGrapple()
    {
        playerScript.freeze = true;
        if(cdTimer>0) return;

        isGrappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }else{
            grapplePoint = cam.position + cam.forward*maxDistance;
            StopGrapple();
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    void ExecuteGrapple()
    {
        playerScript.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        playerScript.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        isGrappling = false;
        cdTimer = cd;
        lr.enabled = false;
        playerScript.freeze = false;
    }
}