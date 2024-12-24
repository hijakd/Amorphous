using System;
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
    private Color currentPlayerColour = Color.white;
    private Color previousPlayerColour;
    private Color previousColour;
    private Color otherColour;

    private GameObject focalPoint;
    private PlayerControls controls;
    private Rigidbody playerRb;

    private bool showHint;
    private Vector2 move;
    private Vector2 look;

    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 forwardMovement;
    private Vector3 rightMovement;
    private Vector3 relativeMovement;
    private Vector3 playerPos;
    public static string playerColourChangeOption;


    private void Awake() {
        // rotationSpeed = lookSpeed;
        controls = new PlayerControls();
        playerRb = GetComponent<Rigidbody>();
        camera = GameObject.Find("Main Camera");
        focalPoint = GameObject.Find("Focal Point");
        playerIsWhite = true;
        playerColourChangeOption = "add";


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

        if (showHint) {
            MazeUI.PaintHintBlips(GameManager.hintColour01, GameManager.hintColour02);
        }
        else {
            MazeUI.PaintHintBlips(currentPlayerColour, GameManager.goalColour);
        }

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

    public void OnHint() {
        controls.Player.Hint.performed += context => showHint = true;
        controls.Player.Hint.canceled += context => showHint = false;
    }

    private void OnTriggerEnter(Collider other) {
        previousPlayerColour = gameObject.GetComponentInChildren<Renderer>().material.color;
        currentColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;

        

        if (other.gameObject.CompareTag("Waypoint")) {
            // Debug.Log("Player found a waypoint");
            if (playerIsWhite) {
                
                // blendedColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
                currentPlayerColour = currentColour;
                playerIsWhite = !playerIsWhite;
            }
            else {
                
                switch (playerColourChangeOption) {
                    case "switch":
                        Debug.Log("player colour is switching");
                        currentPlayerColour = GameUtils.ChangeColours("switch", currentPlayerColour, currentColour);
                        break;
                    case "add":
                        if (GameManager._easyMode) {
                            Debug.Log("player colour is adding in _easyMode");
                            currentPlayerColour = GameUtils.ChangeColours("add", currentColour, previousColour);
                            break;
                        }
                        Debug.Log("player colour is adding");
                        currentPlayerColour = GameUtils.ChangeColours("add", currentPlayerColour, currentColour);
                        break;


                    case "blend":
                        if (GameManager._easyMode) {
                            Debug.Log("player colour is blending in _easyMode");
                            currentPlayerColour = GameUtils.ChangeColours("blend", currentColour, previousColour);
                            break;
                        }

                        Debug.Log("player colour is blending");
                        currentPlayerColour = GameUtils.ChangeColours("blend", currentPlayerColour, currentColour);
                        break;
                }

                
            }
            previousColour = currentColour;
            MazeUI.PaintPlayerBlip(currentPlayerColour);
        }

        if (other.gameObject.CompareTag("ColourResetter")) {
            // Debug.Log("Player collided with the resetter");
            MazeUI.PaintPlayerBlipWhite();
        }

        // if (other.gameObject.CompareTag("White")) {
        //     // Debug.Log("Player found the white waypoint");
        //     playerIsWhite = true;
        //     MazeUI.PaintPlayerBlipWhite();
        // }

        // if (other.gameObject.CompareTag("Black")) {
        //     // Debug.Log("Player found the black waypoint");
        //     MazeUI.PaintPlayerBlipBlack();
        // }
        
        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Goal")) {
            // Debug.Log("Player found the goal");
            if (currentPlayerColour == other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) {
                // Debug.Log("Player found the goal unlocked");
                GameManager.goalFound = true;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Goal")) {
            Debug.Log("Player collided with the goal");
        }
    }

}