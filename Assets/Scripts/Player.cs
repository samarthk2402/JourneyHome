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


    CharacterController controller;
    Vector3 velocity;
    float speed;
    float deltaSpeed;
    float currHeight;
    float heightTarget;
    bool isCrouching => normHeight-currHeight > .1f;
    bool isSprinting;
    float myAng = 0.0f;
    float slideTimer;
    bool sliding;
    int resetSlide;
    public bool canSlide = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        jumpAction = playerInput.actions["jump"];
        sprintAction = playerInput.actions["sprint"];
        crouchAction = playerInput.actions["crouch"];
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
        HandleLook();
        HandleMove();
        UpdateGravity();
        HandleJump();
        HandleSprint();
        HandleCrouch();
        HandleSpeed();

    }

    void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    void HandleCrouch()
    {
        var isTryingtoCrouch = crouchAction.ReadValue<float>() > 0;
        var heightTarget = isTryingtoCrouch && canSlide ? crouchHeight : normHeight;
        
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
        myAng = Vector3.Angle(Vector3.up, hit.normal); //Calc angle between normal and character
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
            velocity.y += jumpSpeed;
        }
    }

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
        input *= speed;

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        // Update player pos
        controller.Move(velocity*Time.deltaTime);
    }
}
