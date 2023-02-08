using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerLocomotion : MonoBehaviour
{
    [Header("Options")]
    [SerializeField, Tooltip("Allow cursor locking?")] bool useCursorLock = true;
    [SerializeField, Tooltip("Lock rotation on cursor visibility?")] bool allowCursorLock = true;
    [SerializeField] KeyCode cursorLockKeyCode = KeyCode.Escape;

    [Header("Camera References")]
    //Head is marked as internal since we are referencing it from PlayerDissociation.cs
    [SerializeField, Tooltip("This should be the Parent Transform of the Camera")] internal Transform head;
    [SerializeField, Tooltip("Local Player Camera Transform")] Transform cam;

    [Header("Movement Settings")]
    [SerializeField, Tooltip("Rotation Speed based on Mouse Input for the Camera")] Vector2 camRotationSpeed = new Vector2(1, 1);
    [SerializeField, Range(250, 1000)] float moveSpeed = 500.0f;

    Rigidbody myBody;
    Vector2 moveDirection;
    bool allowMovement = true;

    #region Mono Methods
    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckInput();
        UpdateCameraAngle();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }
    #endregion

    internal void ToggleMovement(bool toggle)
    {
        allowMovement = toggle;
    }

    void ToggleCursor()
    {
        if (useCursorLock)
        {
            //Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    void CheckInput()
    {
        
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        
        if (Input.GetKeyDown(cursorLockKeyCode))
        {
            ToggleCursor();
        }

        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) &! Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void UpdateCameraAngle()
    {
        if(Cursor.lockState == CursorLockMode.None && allowCursorLock)
        {
            return;
        }


        float horizontalRotation = Input.GetAxis("Mouse X") * camRotationSpeed.x;
        float verticalRotation = Input.GetAxis("Mouse Y") * camRotationSpeed.y;
        head.transform.Rotate(0, horizontalRotation, 0);
        cam.transform.Rotate(-verticalRotation, 0, 0);
    }

    void UpdateMovement()
    {
        if (moveDirection.magnitude > .1f && allowMovement)
        {
            //Sample target angle based on the combination of WASD input combined with current forward facing
            //This will ensure WASD movement is always relative to where the player is facing
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg + head.eulerAngles.y;
            Vector3 movement = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            //Directly setting velocity here, could change to simple transform updating as well
            myBody.velocity = moveSpeed * Time.fixedDeltaTime * movement;
        }
    }
}
