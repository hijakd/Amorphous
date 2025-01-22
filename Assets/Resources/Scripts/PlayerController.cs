using UnityEngine;

public class PlayerController : MonoBehaviour {


    public PlayerSO playerSO;
    public new GameObject camera;

    // public float moveSpeed;
    private GameData mazeData;
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
        playerSO = ScriptableObject.CreateInstance<PlayerSO>();

        /* set focalPoint to players position */
        focalPoint.transform.position = transform.position;

        /* END Awake() */
    }


    void FixedUpdate() {
        /* get camera's directional vectors & normalize them */
        cameraForward = camera.transform.forward;
        cameraRight = camera.transform.right;
        cameraForward.y = cameraRight.y = 0;
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        playerSO.moveSpeed = playerSO.speed;

        /* convert then combine directional vectors to local space */
        forwardMovement = move.y * cameraForward;
        rightMovement = move.x * cameraRight;
        relativeMovement = forwardMovement + rightMovement;
        playerPos = transform.position; // for testing
        playerRb.AddForce(relativeMovement * (playerSO.speed * Time.deltaTime));
        focalPoint.transform.position = transform.position;
        
        /* END FixedUpdate() */
    }

    private void OnEnable() {
        // playerSO.controls.Player.Enable();
        controls.Player.Enable();
    }

    private void OnDisable() {
        // playerSO.controls.Player.Disable();
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

    private void OnTriggerEnter(Collider other) {
        // thatColour = gameObject.GetComponentInChildren<Renderer>().material.color;
        playerSO.thisColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;


        if (other.gameObject.CompareTag("Waypoint")) {
            // Debug.Log("Player found a waypoint");
            if (playerSO.playerIsWhite) {
                // blendedColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
                playerSO.currentColour = playerSO.thisColour;
                playerSO.playerIsWhite = !playerSO.playerIsWhite;
            }
            else {
                switch (playerSO.playerColourChangeOption) {
                    case "switch":
                        Debug.Log("player colour is switching");
                        playerSO.currentColour =
                            GameManager.ChangeColours("switch", playerSO.currentColour, playerSO.thisColour);
                        playerSO.playerColour = playerSO.currentColour;
                        break;
                    case "add":
                        if (mazeData.easyMode) {
                            Debug.Log("player colour is adding in _easyMode");
                            playerSO.currentColour =
                                GameManager.ChangeColours("add", playerSO.thisColour, playerSO.previousColour);
                            playerSO.playerColour = playerSO.currentColour;
                            break;
                        }

                        Debug.Log("player colour is adding");
                        playerSO.currentColour =
                            GameManager.ChangeColours("add", playerSO.currentColour, playerSO.thisColour);
                        break;


                    case "blend":
                        if (mazeData.easyMode) {
                            Debug.Log("player colour is blending in _easyMode");
                            playerSO.currentColour =
                                GameManager.ChangeColours("blend", playerSO.thisColour, playerSO.previousColour);
                            playerSO.playerColour = playerSO.currentColour;
                            break;
                        }

                        Debug.Log("player colour is blending");
                        playerSO.currentColour =
                            GameManager.ChangeColours("blend", playerSO.currentColour, playerSO.thisColour);
                        break;
                }
            }

            playerSO.previousColour = playerSO.thisColour;

            // MazeUI.PaintPlayerBlip(playerSO.currentColour);
        }
    }

}