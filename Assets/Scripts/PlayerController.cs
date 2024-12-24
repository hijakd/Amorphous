using TMPro;
using UnityEngine;


// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

public class PlayerController : MonoBehaviour {
    
    public new GameObject camera;
    public float speed = 10f;
    public float lookSpeed = 10f;

    private static Color currentColour;
    // public static float rotationSpeed;

    private bool playerIsWhite;
    private Color blendColour = Color.white;
    private Color previousColour;
    private Color otherColour;
    
    private GameObject focalPoint;
    private PlayerControls controls;
    private Rigidbody playerRb;
    
    private Vector2 move;
    private Vector2 look;
    
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 forwardMovement;
    private Vector3 rightMovement;
    private Vector3 relativeMovement;
    private Vector3 playerPos;
    
    // public TextMeshProUGUI playerForwardText;
    // public TextMeshProUGUI playerRightText;
    // public TextMeshProUGUI relativeMovementText;
    // public TextMeshProUGUI playerPositionText;


    private void Awake() {
        // rotationSpeed = lookSpeed;
        controls = new PlayerControls();
        playerRb = GetComponent<Rigidbody>();
        camera = GameObject.Find("Main Camera");
        focalPoint = GameObject.Find("Focal Point");
        playerIsWhite = true;
        
        /* set focalPoint to players position */
        focalPoint.transform.position = transform.position;

        /* END Awake() */
    }

    // Update is called once per frame
    private void FixedUpdate() {
        /* get camera's directional vectors & normalize them */
        cameraForward = camera.transform.forward;
        cameraRight = camera.transform.right;
        cameraForward.y = cameraRight.y = 0;
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        /* convert then combine directional vectors to local space */
        forwardMovement = move.y * cameraForward;
        rightMovement = move.x * cameraRight;
        relativeMovement = forwardMovement + rightMovement;
        playerPos = transform.position; // for testing
        playerRb.AddForce(relativeMovement * speed * Time.deltaTime);
        focalPoint.transform.position = transform.position;

        
        // playerForwardText.text = "Forward: " + forwardMovement.ToString();
        // playerRightText.text = "Right: " + rightMovement.ToString();
        // relativeMovementText.text = "Relative: " + relativeMovement.ToString();
        // playerPositionText.text = "Position: " + playerPos.ToString();

        /* END FixedUpdate() */
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

    private void OnTriggerEnter(Collider other) {
        previousColour = currentColour;
        currentColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;

        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Waypoint")) {
            // Debug.Log("Player found a waypoint");
            if (playerIsWhite) {
                blendColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
            }
            else {
                // var tmpColour = blendColour;
                // blendColour = GameUtils.AddColoursTogether(gameObject.GetComponentInChildren<Renderer>().material.color, other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color);
                blendColour = GameUtils.ChangeColours("switch", gameObject.GetComponentInChildren<Renderer>().material.color, other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color);
                playerIsWhite = !playerIsWhite;
            }
            
            MazeUI.PaintPlayerBlip(blendColour);
        }

        if (other.gameObject.CompareTag("ColourResetter")) {
            // Debug.Log("Player collided with the resetter");
            MazeUI.PaintPlayerBlipWhite();
        }

        if (other.gameObject.CompareTag("White")) {
            // Debug.Log("Player found the white waypoint");
            playerIsWhite = true;
            MazeUI.PaintPlayerBlipWhite();
        }

        if (other.gameObject.CompareTag("Black")) {
            // Debug.Log("Player found the black waypoint");
            MazeUI.PaintPlayerBlipBlack();
        }
        if (other.gameObject.CompareTag("Goal")) {
            // Debug.Log("Player found the goal");
            if (blendColour == other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) {
                // Debug.Log("Player found the goal unlocked");
                GameManager.goalFound = true;
            }
            
            
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Goal")){
            Debug.Log("Player collided with the goal");
        }
    }

}
