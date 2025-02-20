using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public new GameObject camera;
    public static float speed = 50f;

    PlayerControls controls;
    static Rigidbody playerRb;
    Vector2 move, look;
    Vector3 cameraForward, cameraRight, forwardMovement, rightMovement, relativeMovement, playerPos;
    bool playerIsWhite, showHint;
    Color currentWaypoint, playerColour, previousColour, theColourBefore;
    string playerColourChangeOption;


    void Awake() {
        controls = new PlayerControls();
        playerRb = GetComponent<Rigidbody>();
        playerColourChangeOption = "add";
    }

    void Start() {
        speed = GameManager.mazeData.playerSpeed;
    }

    void FixedUpdate() {
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
        PlayerMovement();


        if (showHint) {
            MazeUI.PaintHints(GameManager.mazeData.hintColour01, GameManager.mazeData.hintColour02);
        }
        else {
            MazeUI.PaintHints(Color.clear, Color.clear);
        }
    }

    void OnEnable() {
        controls.Player.Enable();
    }

    void OnDisable() {
        controls.Player.Disable();
    }

    public void OnMove() {
        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;
    }

    public void OnLook() {
        controls.Player.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => look = Vector2.zero;
    }

    public void OnHint() {
        controls.Player.Hint.performed += ctx => showHint = true;
        controls.Player.Hint.canceled += ctx => showHint = false;
    }

    void PlayerMovement() {
        if (GameManager.mazeData.isPaused) {
            relativeMovement = Vector3.zero;
        }
        playerRb.AddForce(relativeMovement * (GameManager.mazeData.playerSpeed * Time.deltaTime));
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Goal")) {
            Debug.Log("Player collided with the goal");
        }
    }

    /** TODO: disable the given waypoint temporarily to limit repeated collisions **/
    void OnTriggerEnter(Collider other) {
        currentWaypoint = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        previousColour = GameManager.mazeData.previousColour01;
        theColourBefore = GameManager.mazeData.previousColour02;

        // /* playerColour == GameManager.mazeData.playerColour; */

        if (other.gameObject.CompareTag("Waypoint")) {
            if (GameManager.mazeData.playerIsWhite) {
                GameManager.mazeData.playerColour = currentWaypoint;
                GameManager.mazeData.playerIsWhite = false;
            }
            else {
                switch (playerColourChangeOption) {
                    case "switch":
                        Debug.Log("player colour is switching");
                        GameManager.mazeData.playerColour = GameManager.ChangeColours("switch",
                            GameManager.mazeData.playerColour, currentWaypoint);
                        break;
                    case "add":
                        /* easyMode */
                        if (GameManager.mazeData.difficulty == 1) {
                            /* combine the current and previous colours */
                            Debug.Log("player colour is adding in easyMode: " + other.gameObject.name);
                            GameManager.mazeData.playerColour =
                                GameManager.ChangeColours("add", previousColour, currentWaypoint);
                            break;
                        }

                        /* normalMode */
                        if (GameManager.mazeData.difficulty == 2) {
                            /* combine the previous two colours then combine with the current colour */
                            Debug.Log("player colour is adding in normalMode: " + other.gameObject.name);
                            Color previousBlend = GameManager.ChangeColours("add", theColourBefore, previousColour);
                            GameManager.mazeData.playerColour =
                                GameManager.ChangeColours("add", previousBlend, currentWaypoint);
                            GameManager.mazeData.previousColour02 = previousColour;
                            break;
                        }

                        /* hardMode */
                        if (GameManager.mazeData.difficulty == 3) {
                            /* combine the current colour with the previous resulting combination */
                            Debug.Log("player colour is adding in hardMode: " + other.gameObject.name);
                            GameManager.mazeData.playerColour =
                                GameManager.ChangeColours("add", previousColour, currentWaypoint);
                            currentWaypoint = GameManager.mazeData.playerColour;
                        }

                        break;
                    case "blend":
                        /* easyMode */
                        if (GameManager.mazeData.difficulty == 1) {
                            Debug.Log("player colour is blending in easyMode: " + other.gameObject.name);
                            GameManager.mazeData.playerColour =
                                GameManager.ChangeColours("blend", previousColour, currentWaypoint);
                            break;
                        }

                        /* normalMode */
                        if (GameManager.mazeData.difficulty == 2) {
                            /* combine the previous two colours then combine with the current colour */
                            Debug.Log("player colour is adding in normalMode: " + other.gameObject.name);
                            Color previousBlend = GameManager.ChangeColours("add", theColourBefore, previousColour);
                            GameManager.mazeData.playerColour =
                                GameManager.ChangeColours("blend", previousBlend, currentWaypoint);
                            GameManager.mazeData.previousColour02 = previousColour;
                            break;
                        }

                        /* hardMode */
                        if (GameManager.mazeData.difficulty == 3) {
                            /* combine the current colour with the previous resulting combination */
                            Debug.Log("player colour is adding in hardMode: " + other.gameObject.name);
                            GameManager.mazeData.playerColour =
                                GameManager.ChangeColours("blend", previousColour, currentWaypoint);
                            currentWaypoint = GameManager.mazeData.playerColour;
                        }

                        break;
                }
            }

            GameManager.mazeData.previousColour01 = currentWaypoint;

            MazeUI.PaintPlayer(GameManager.mazeData.playerColour);
        }

        if (other.gameObject.CompareTag("ColourResetter")) {
            MazeUI.PaintPlayerWhite();
        }

        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Goal")) {
            if (GameManager.mazeData.playerColour ==
                other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) {
                GameManager.mazeData.goalFound = true;
            }
        }
    }

    /* END PlayerController() */

}