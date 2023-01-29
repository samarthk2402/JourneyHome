using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 look;
    // Make reference to camera
    [SerializeField] Transform cameraTransform;
    Vector3 initCamPos;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float yAxisClamp;
    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float sprintSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float slideSpeed;
    [SerializeField] float slopeBoost;
    [SerializeField] float slideLength;
    [SerializeField] float slideCoolDown;
    [SerializeField] float mass;
    [SerializeField] float jumpSpeed;
    [SerializeField] float crouchTime;
    [SerializeField] float crouchHeight;
    [SerializeField] float normHeight;
    
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;


    public CharacterController controller;
    public Vector3 velocity;
    float ySpeed;
    float speed;
    float deltaSpeed;
    float currHeight;
    float heightTarget;
    bool isCrouching => normHeight-currHeight > .1f;
    bool isSprinting;
    float slideTimer;
    bool sliding;
    int resetSlide;
    public bool canSlide = true;
    public State state;
    private Grapple grapple;

    public enum State{
        Normal,
        HookshotFlyingPlayer,
        HookshotThrown
    }

    void Awake()
    {
        grapple = GetComponent<Grapple>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        jumpAction = playerInput.actions["jump"];
        sprintAction = playerInput.actions["sprint"];
        crouchAction = playerInput.actions["crouch"];
        state = State.Normal;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.Locked;
        speed = moveSpeed;
        normHeight = currHeight = controller.height;
        initCamPos = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state) {
        default:
        case State.Normal:
            HandleLook();
            HandleMove();
            UpdateGravity();
            HandleJump();
            HandleSprint();
            HandleCrouch();
            HandleSpeed();
            grapple.HandleHookShotStart();
            break;
        case State.HookshotThrown:
            grapple.HandleHookShotThrow();
            HandleLook();
            HandleMove();
            break;
        case State.HookshotFlyingPlayer:
            grapple.HandleHookMovement();
            HandleLook();
            break;
        }


    }

    void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        ySpeed = controller.isGrounded ? -1f : ySpeed + gravity.y;
    }

    void HandleCrouch()
    {
        var isTryingtoCrouch = crouchAction.ReadValue<float>() > 0;
        float heightTarget = 0;

        if(isTryingtoCrouch&&canSlide){
            heightTarget = crouchHeight;
        }else{
            heightTarget = normHeight;
        }
        
        var crouchDelta = Time.deltaTime * crouchTime;
        currHeight = Mathf.Lerp(currHeight, heightTarget, crouchDelta);

        var halfHeightDiff = new Vector3(0, (normHeight-currHeight) / 2, 0);
        var newCamPos = initCamPos - halfHeightDiff;

        cameraTransform.localPosition = newCamPos;
        controller.height = currHeight;
    }

    void HandleSprint()
    {
        var sprintInput = sprintAction.ReadValue<float>();
        if (sprintInput>0 && controller.isGrounded)
        {
            isSprinting = true;
        }else{
            isSprinting = false;
        }
    }

    void HandleSlide()
    {
        speed = slideSpeed;
        slideTimer -= Time.deltaTime;
        sliding = true;

        if(slideTimer <= 0)
        {
            speed = moveSpeed;
            var crouchDelta = Time.deltaTime * crouchTime;
            currHeight = Mathf.Lerp(currHeight, normHeight, crouchDelta);
            sliding = false;
            canSlide = false;
            StartCoroutine(waitForKeyRelease());

        }
    }

    public IEnumerator waitForKeyRelease()
    {
      yield return new WaitUntil(CrouchKeyReleased);

      canSlide = true;
      
    }

    bool CrouchKeyReleased()
    {
        if (crouchAction.ReadValue<float>() <= 0){
            return true;
        }else{
            return false;
        }
    }

    void HandleSpeed()
    {
        if(isCrouching && isSprinting && canSlide){
            if(!sliding){
                slideTimer = slideLength;
            }
            HandleSlide();
            
        }else if(isCrouching && canSlide){
            speed = crouchSpeed;
        }else if(isSprinting && canSlide){
            speed = sprintSpeed;
        }else{
            speed = moveSpeed;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        // Debug.DrawLine(hit.point, hit.point + hit.normal, Color.black, 3f);
        var angle = Vector3.Angle(Vector3.up, hit.normal);
        if(angle > controller.slopeLimit)
        {
            var normal = hit.normal;
            var yInverse = 1f - normal.y;
            velocity.x += yInverse * normal.x;
            velocity.z += yInverse * normal.z; 
        }
    }


    void HandleJump()
    {
        var jumpInput = jumpAction.ReadValue<float>();
        if (jumpInput>0  && controller.isGrounded)
        {
            ySpeed += jumpSpeed;
        }
    }

    // bool OnSlope(){
    //         float frontPosY = 0;
    //         float backPosY = 0;
    //         if(Physics.Raycast(transform.position+(transform.forward*0.2f), Vector3.down, out RaycastHit frontHit, Mathf.Infinity)){
    //             Debug.DrawLine(frontHit.point, frontHit.point + frontHit.normal, Color.black, 3f);
    //             frontPosY = frontHit.point.y;
    //         }

    //         if(Physics.Raycast(transform.position+(transform.forward*-0.2f), Vector3.down, out RaycastHit backHit, Mathf.Infinity)){
    //             Debug.DrawLine(backHit.point, backHit.point + backHit.normal, Color.black, 3f);
    //             backPosY = backHit.point.y;
    //         }

    //         if(backPosY-frontPosY>0.1){
    //             // Vector3 reverse = new Vector3(-1, 0, -1);
    //             // adjustedVelocity = reverse;
    //             return true;
    //         }else{
    //             return false;
    //         }


    // }

    void HandleLook()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        // Updates look vector2 when mouse moves
        look.x += lookInput.x*mouseSensitivity;
        look.y += lookInput.y*mouseSensitivity;

        // Restrict camera movement
        look.y = Mathf.Clamp(look.y, -yAxisClamp, yAxisClamp);
        
        // Rotates player according to mouse x movement
        transform.localRotation = Quaternion.Euler(0, look.x, 0);

        // Rotates camera according to mouse y movement
        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
    }

    void HandleMove()
    {
        // Get X and Y values from Input
        var moveInput = moveAction.ReadValue<Vector2>();

        // Make new Vector3 for input
        var input = new Vector3();

        // update vector3 according to player input
        input += transform.forward*moveInput.y;
        input += transform.right*moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        velocity.y = ySpeed;

        controller.Move(velocity*speed*Time.deltaTime);
    }
}
