using UnityEngine;

public class PlayerController : MonoBehaviour {

    
    public Player playerSO;
    public new GameObject camera;
    
    public float moveSpeed;
    
    private GameObject focalPoint;
    private PlayerControls controls;
    private Rigidbody playerRb;
    private Vector2 move, look;
    private Vector3 cameraForward, cameraRight, forwardMovement, rightMovement, relativeMovement, playerPos;
    
    
    private void Awake() {
        controls = new PlayerControls();
        playerRb = GetComponent<Rigidbody>();
        camera = GameObject.Find("Main Camera");
        focalPoint = GameObject.Find("Focal Point");
        moveSpeed = playerSO.speed;
        
        /* set focalPoint to players position */
        focalPoint.transform.position = transform.position;

        /* END Awake() */
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        /* get camera's directional vectors & normalize them */
        cameraForward = camera.transform.forward;
        cameraRight = camera.transform.right;
        cameraForward.y = cameraRight.y = 0;
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;
        
        moveSpeed = playerSO.speed;

        /* convert then combine directional vectors to local space */
        forwardMovement = move.y * cameraForward;
        rightMovement = move.x * cameraRight;
        relativeMovement = forwardMovement + rightMovement;
        playerPos = transform.position; // for testing
        playerRb.AddForce(relativeMovement * (playerSO.speed * Time.deltaTime));
        focalPoint.transform.position = transform.position;
    }
    
    private void OnEnable() {
        controls.Player.Enable();
    }

    private void OnDisable() {
        controls.Player.Disable();
    }

    public void OnMove() {
        controls.Player.Move.performed += context => move = context.ReadValue<Vector2>();
        controls.Player.Move.canceled += context => move = Vector2.zero;
    }

    public void OnLook() {
        controls.Player.Look.performed += context => look = context.ReadValue<Vector2>();
        controls.Player.Look.canceled += context => look = Vector2.zero;
    }

    // public void OnHint() {
    //     controls.Player.Hint.performed += context => showHint = true;
    //     controls.Player.Hint.canceled += context => showHint = false;
    // }
    
    
}
