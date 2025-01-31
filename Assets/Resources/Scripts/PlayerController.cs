using System;
using UnityEngine;
using AmorphousData;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public new GameObject camera;
    public float speed = 50f;

    PlayerControls controls;
    Rigidbody playerRb;
    Vector2 move, look;
    Vector3 cameraForward, cameraRight, forwardMovement, rightMovement, relativeMovement, playerPos;
    bool playerIsWhite, showHint, easyMode;
    Color currentWaypoint, playerColour, previousColour;
    string playerColourChangeOption;


    void Awake() {
        controls = new PlayerControls();
        playerRb = GetComponent<Rigidbody>();
        playerColourChangeOption = "add";
    }

    void Start() {
        //     hint01 = GameManager.mazeData.hintColour01;
        //     hint02 = GameManager.mazeData.hintColour02;
        if (GameManager.mazeData.difficulty == 1) {
            easyMode = true;
        }
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
        playerRb.AddForce(relativeMovement * (speed * Time.deltaTime));


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

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Goal")) {
            Debug.Log("Player collided with the goal");
        }
    }

    /** TODO: disable the given waypoint temporarily to limit repeated collisions **/
    void OnTriggerEnter(Collider other) {
        currentWaypoint = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        playerColour = GameManager.mazeData.playerColour;

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
                        if (easyMode) {
                            Debug.Log("player colour is adding in _easyMode");
                            GameManager.mazeData.playerColour = GameManager.ChangeColours("add", previousColour, currentWaypoint);
                            break;
                        }

                        Debug.Log("player colour is adding");
                        GameManager.mazeData.playerColour = GameManager.ChangeColours("add", currentWaypoint, playerColour);
                        break;
                    case "blend":
                        if (easyMode) {
                            Debug.Log("player colour is adding in _easyMode");
                            GameManager.mazeData.playerColour = GameManager.ChangeColours("blend", previousColour, currentWaypoint);
                            break;
                        }

                        Debug.Log("player colour is blending");
                        GameManager.mazeData.playerColour = GameManager.ChangeColours("blend", currentWaypoint, playerColour);
                        break;
                }
            }

            MazeUI.PaintPlayer(GameManager.mazeData.playerColour);
            previousColour = currentWaypoint;
            
        }

        if (other.gameObject.CompareTag("ColourResetter")) {
            MazeUI.PaintPlayerWhite();
        }

        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Goal")) {
            // Debug.Log("Player found the goal");
            if (GameManager.mazeData.playerColour ==
                other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) {
                // Debug.Log("Player found the goal unlocked");
                GameManager.mazeData.goalFound = true;
            }
        }
    }

    /* END PlayerController() */

}