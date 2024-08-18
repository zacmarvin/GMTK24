using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public enum AbilityState
    {
        Shrink,
        Grow
    }
    
    public AbilityState CurrentAbilityState = AbilityState.Grow;

    [SerializeField, Header("Height of the player used for ground detection")]
    private float _playerHeight = 2f;
    
    [SerializeField, Header("Speed at which the player moves")]
    private float _moveSpeed = 500f;
    
    [SerializeField, Header("Speed at which the player sprints")]
    private float _sprintSpeed = 1000f;
    
    [SerializeField, Header("Speed at which the player rotates")]
    private float _rotationSpeed = 500f;
    
    [SerializeField, Header("Force applied when jumping")]
    private float _jumpForce = 7f;
    
    [SerializeField, Header("Time to reach full jump force")]
    private float _jumpDuration = 0.5f;
    
    [SerializeField, Header("Coyote time for jumping after leaving ground")]
    private float _coyoteTime = 0.2f;
    
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
    
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Material _growMaterial;
    
    [SerializeField] private Material _shrinkMaterial;
    
    private Rigidbody _rb;
    
    private MeshRenderer _meshRenderer;
    
    private int _lavaLayer = 0;
    private int _layerMask = 0;


    [field: SerializeField]
    public bool IsGrounded { get; private set; }
    
    [field: SerializeField]
    public bool Jumping { get; private set; }
    
    [field: SerializeField]
    public bool Sprinting { get; private set; }

    [field: SerializeField]
    public bool Dashing { get; private set; }
    
    [field: SerializeField]
    public bool Falling { get; private set; }
    
    private float _jumpTimer;
    private float _timeSinceGrounded;
    private float _mouseX;
    private Vector3 _moveDirection;
    
    private Coroutine _coyoteTimeCoroutine;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;


    private const float _threshold = 0.01f;

    private bool _hasAnimator;
    private Animator _animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        _lavaLayer = LayerMask.NameToLayer("Lava");
        _layerMask = ~(1 << _lavaLayer); // Create a layer mask that excludes the "Lava" layer
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = _growMaterial;
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();

    }

    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        // Capture inputs in Update
        _mouseX = Input.GetAxis("Mouse X");
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Determine if sprinting
        // Determine if dashing
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (IsGrounded)
            {
                Sprinting = true; }
        }
        else
        {
            Sprinting = false; }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!IsGrounded || Jumping)
            {
                Dashing = true;
            }
        }
        
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
            if(_coyoteTimeCoroutine != null)
            {
                StopCoroutine(_coyoteTimeCoroutine);
            }
            IsGrounded = false;
            Jumping = true;
            _jumpTimer = 0f;
        }

        // Check if player is grounded
        if (!IsGrounded)
        {
            _timeSinceGrounded += Time.deltaTime;
        }

        if (!Jumping)
        {
            // Check if ground is close and snap to it
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, _playerHeight / 2 + 0.2f, _layerMask))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + _playerHeight / 2, transform.position.z);
                // Remove vertical velocity to prevent player from bouncing
                _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCurrentAbilityState();
        }   
    }

    void FixedUpdate()
    {
        // Apply physics-based movement and rotation in FixedUpdate
        MovePlayer();
        RotatePlayer();
        HandleJumping();
    }

    private void LateUpdate()
    {
        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, IsGrounded);
            _animator.SetBool(_animIDFreeFall, Falling);
            _animator.SetBool(_animIDJump, Jumping);
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    
    void ToggleCurrentAbilityState()
    {
        if (CurrentAbilityState == AbilityState.Grow)
        {
            CurrentAbilityState = AbilityState.Shrink;
            _meshRenderer.material = _shrinkMaterial;
        }
        else
        {
            CurrentAbilityState = AbilityState.Grow;
            _meshRenderer.material = _growMaterial;
        }
    }

    void MovePlayer()
    {
        float currentSpeed = Sprinting || Dashing ? _sprintSpeed : _moveSpeed;

        // Adjust speed if in the air
        if (!IsGrounded && !Jumping)
        {
            currentSpeed *= 0.5f; // Reduce speed by 50% when in the air
        }

        if (_moveDirection != Vector3.zero && (IsGrounded || !IsGrounded))
        {
            Debug.Log("Moving");

            Vector3 movement = _moveDirection * currentSpeed * Time.fixedDeltaTime;

            // Apply acceleration to the player
            movement = Vector3.Lerp(_rb.velocity, movement, _acceleration * Time.fixedDeltaTime);

            _rb.velocity = new Vector3(movement.x, _rb.velocity.y, movement.z);
        }
        else if (_rb.velocity.magnitude > 1f && IsGrounded)
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
        else if (Dashing)
        {
            Debug.Log("Dashing");

            // Apply acceleration to the player
            Vector3 movement = _moveDirection * currentSpeed * Time.fixedDeltaTime;
            movement = Vector3.Lerp(_rb.velocity, movement, _acceleration * Time.fixedDeltaTime);
            _rb.velocity = new Vector3(movement.x, _rb.velocity.y, movement.z);
        }
        else if (_rb.velocity.magnitude > 1f && !IsGrounded)
        {
            Debug.Log("Floating Decelerating");

            // Apply floating deceleration to the player
            float percentageToTimeSinceGrounded = Mathf.Clamp01(_timeSinceGrounded / _floatingHorizontalDecelerationTime);

            // Ensure we scale down the deceleration correctly
            Vector3 decelerationVector = new Vector3(_rb.velocity.x * percentageToTimeSinceGrounded, 0f, _rb.velocity.z * percentageToTimeSinceGrounded) * Time.fixedDeltaTime;

            // Avoid over-deceleration
            if (decelerationVector.magnitude > _rb.velocity.magnitude)
            {
                _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            }
            else
            {
                _rb.velocity -= decelerationVector;
            }
        }

        // Apply stronger gravity slightly before reaching the apex of the jump
        float apexThreshold = 1.0f; // Adjust this value to fine-tune when the stronger gravity kicks in
        if (!IsGrounded && _rb.velocity.y > 0 && _rb.velocity.y <= apexThreshold)
        {
            Debug.Log("Approaching Apex");

            // Apply stronger gravity as the player approaches the apex
            float fallSpeed = _rb.velocity.y - (_fallingGravityAcceleration * 2f) * Time.fixedDeltaTime;
            _rb.velocity = new Vector3(_rb.velocity.x, fallSpeed, _rb.velocity.z);
        }
        else if (!IsGrounded && _rb.velocity.y <= 0)
        {
            
            Debug.Log("Falling");
            Falling = true;
            // Apply stronger gravity after reaching the apex
            float fallSpeed = _rb.velocity.y - (_fallingGravityAcceleration * 2f) * Time.fixedDeltaTime;
            if (fallSpeed < -_maxFallSpeed)
            {
                fallSpeed = -_maxFallSpeed;
            }
            _rb.velocity = new Vector3(_rb.velocity.x, fallSpeed, _rb.velocity.z);
        }
        else
        {
            Falling = false;
        }
        
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, currentSpeed);
            _animator.SetFloat(_animIDMotionSpeed, _moveDirection.magnitude);
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
            if (!IsGrounded && Sprinting)
            {
                Sprinting = false;
            }else if (!IsGrounded && Dashing)
            {
                Dashing = false;
            }
            
            // Raycast to check if the player is grounded
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, _playerHeight / 2 + 0.1f, _layerMask))
            {
                IsGrounded = true;
                if(_coyoteTimeCoroutine != null)
                {
                    StopCoroutine(_coyoteTimeCoroutine);
                }
            }
            else
            {
                // Check if the player is on the edge of a platform
                if (Vector3.Dot(_rb.velocity, hit.normal) < 0)
                {
                    IsGrounded = true;
                    if (_coyoteTimeCoroutine != null)
                    {
                        StopCoroutine(_coyoteTimeCoroutine);
                    }
                }


            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _coyoteTimeCoroutine = StartCoroutine(CoyoteTime());
            _timeSinceGrounded = 0f;
            Dashing = false;
        }
    }
    
    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(_coyoteTime);
        IsGrounded = false;
    }
    
    private void OnFootstep(AnimationEvent animationEvent)
    {
        // if (animationEvent.animatorClipInfo.weight > 0.5f)
        // {
        //     if (FootstepAudioClips.Length > 0)
        //     {
        //         var index = Random.Range(0, FootstepAudioClips.Length);
        //         AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
        //     }
        // }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        // if (animationEvent.animatorClipInfo.weight > 0.5f)
        // {
        //     AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        // }
    }
}
