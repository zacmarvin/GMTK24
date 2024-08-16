using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField, Header("Height of the player used for ground detection")]
    private float _playerHeight = 2f;
    
    [SerializeField, Header("Speed at which the player moves")]
    private float _moveSpeed = 500f;
    
    [SerializeField, Header("Speed at which the player rotates")]
    private float _rotationSpeed = 500f;
    
    [SerializeField, Header("Force applied when jumping")]
    private float _jumpForce = 7f;
    
    [SerializeField, Header("Time to reach full jump force")]
    private float _jumpDuration = 0.5f;
    
    [SerializeField, Header("Time to accelerate laterally when grounded")]
    private float _acceleration = 75f;
    
    [SerializeField, Header("Time to decelerate laterally after stopping when grounded")]
    private float _deceleration = 225f;
    
    [SerializeField, Header("Speed at which gravity increases when falling")]
    private float _fallingGravityAcceleration = 2f;
    
    [SerializeField, Header("Time to decelerate laterally after leaving ground")]
    private float _floatingHorizontalDecelerationTime = 1f;
    
    [SerializeField, Header("Maximum fall speed")]
    private float _maxFallSpeed = 20f;
    
    [SerializeField]
    private Transform _cameraTransform;

    private Rigidbody _rb;

    [field: SerializeField]
    public bool IsGrounded { get; private set; }
    
    [field: SerializeField]
    public bool Jumping { get; private set; }

    private float _jumpTimer;
    private float _timeSinceGrounded;
    private float _mouseX;
    private Vector3 _moveDirection;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Capture inputs in Update
        _mouseX = Input.GetAxis("Mouse X");
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction based on input and camera direction
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            _moveDirection = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f) * direction;
        }
        else
        {
            _moveDirection = Vector3.zero;
        }

        // Check for jump input
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            Jumping = true;
            _jumpTimer = 0f;
        }

        // Check if player is grounded
        if (!IsGrounded)
        {
            _timeSinceGrounded += Time.deltaTime;
        }

        if (!Jumping && !IsGrounded)
        {
            // Check if ground is close and snap to it
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, _playerHeight / 2 + 0.2f))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + _playerHeight / 2, transform.position.z);
                // Remove vertical velocity to prevent player from bouncing
                _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            }
        }
    }

    void FixedUpdate()
    {
        // Apply physics-based movement and rotation in FixedUpdate
        MovePlayer();
        RotatePlayer();
        HandleJumping();
    }

    void MovePlayer()
    {
        if (_moveDirection != Vector3.zero && IsGrounded || _moveDirection != Vector3.zero && Jumping)
        {
            Debug.Log("Moving");
            
            Vector3 movement = _moveDirection * _moveSpeed * Time.fixedDeltaTime;
            
            // Apply acceleration to the player
            movement = Vector3.Lerp(_rb.velocity, movement, _acceleration * Time.fixedDeltaTime);

            _rb.velocity = new Vector3(movement.x, _rb.velocity.y, movement.z);
        }
        else if (_rb.velocity.magnitude > 3f && IsGrounded)
        {
            Debug.Log("Decelerating");
            
            // Apply deceleration to the player
            Vector3 decelerationVector = -_rb.velocity.normalized * _deceleration * Time.fixedDeltaTime;
            
            // If flipped direction, stop the player
            if (Vector3.Dot(_rb.velocity, decelerationVector) < 0)
            {
                _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            }
            else
            {
                _rb.velocity += new Vector3(decelerationVector.x, 0f, decelerationVector.z);
            }
            
        }
        else if (_rb.velocity.magnitude > 3f && !IsGrounded)
        {
            Debug.Log("Floating Decelerating");
            
            
            // Apply falling deceleration to the player
            float percentageToTimeSinceGrounded = _timeSinceGrounded / _floatingHorizontalDecelerationTime;
            if (percentageToTimeSinceGrounded > 1)
            {
                _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            }
            else
            {
                _rb.velocity += new Vector3(_rb.velocity.x * percentageToTimeSinceGrounded * Time.fixedDeltaTime, 0f,
                    _rb.velocity.z * percentageToTimeSinceGrounded * Time.fixedDeltaTime);
            }
        }

        // If falling
        if (!IsGrounded && _rb.velocity.y < 0)
        {
            Debug.Log("Falling");
            
            // Increase gravity over time but cap the fall speed to MaxFallSpeed
            float fallSpeed = _rb.velocity.y - _fallingGravityAcceleration * Time.fixedDeltaTime;
            if (fallSpeed < -_maxFallSpeed)
            {
                fallSpeed = -_maxFallSpeed;
            }
            _rb.velocity = new Vector3(_rb.velocity.x, fallSpeed, _rb.velocity.z);
        }
    }

    void RotatePlayer()
    {
        Vector3 rotation = new Vector3(0f, _mouseX, 0f) * _rotationSpeed * Time.fixedDeltaTime;
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotation));
    }

    void HandleJumping()
    {
        if (Jumping)
        {
            _jumpTimer += Time.fixedDeltaTime;
            if (_jumpTimer < _jumpDuration)
            {
                // Lerp jump force to 0 based on percentage of jump duration
                float lerpedJumpForce = Mathf.Lerp(_jumpForce, 0f, _jumpTimer / _jumpDuration);
                _rb.velocity = new Vector3(_rb.velocity.x, lerpedJumpForce, _rb.velocity.z);
            }
            else
            {
                Jumping = false;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Raycast to check if the player is grounded
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, _playerHeight / 2 + 0.1f))
            {
                IsGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
            _timeSinceGrounded = 0f;
        }
    }
}
