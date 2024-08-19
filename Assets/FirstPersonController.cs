using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController Instance;
    
    [SerializeField] public Image Crosshair;
    
    [SerializeField] public Sprite PickupCrosshairSprite;
    
    [SerializeField] public Sprite DefaultCrosshairSprite;
    
    [SerializeField] public Sprite DropCrosshairSprite;
    
    [SerializeField] public Sprite BusyCrosshairSprite;
    
    [SerializeField] public Sprite ScrollPlusClickCrosshairSprite;
    
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    
    private LayerMask layerMask;

    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        // Layer mask only including layer "Ground"
        layerMask = 1 << LayerMask.NameToLayer("Ground");
    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Handle movement
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveSide = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        Vector3 move = transform.right * moveSide + transform.forward * moveForward;
        move = new Vector3(move.x, 0, move.z); // Prevent player from moving up or down (y-axis)
        
        Vector3 newPosition = transform.position + move;
        
        if(GroundBelowNewPosition(newPosition))
        {
            transform.position += move;
        }

        // Handle mouse rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        
        rotationY += mouseX;
        transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
    
    private bool GroundBelowNewPosition(Vector3 newPosition)
    {
        RaycastHit hit;
        if(Physics.Raycast(newPosition, Vector3.down, out hit, 10f, layerMask))
        {
            return true;
        }
        return false;
    }
}