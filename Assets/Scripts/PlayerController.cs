using System;
using UnityEngine;
using UnityEngine.InputSystem;

/* player movement relative to camera direction solved based on */
/* "Camera-Relative Movement in Unity 3D Explained" by iHeartGemDev */
/* https://www.youtube.com/watch?v=7kGCrq1cJew */

public class PlayerController : MonoBehaviour {

    PlayerControls controls;
    public GameObject camera;
    private GameObject focalPoint;
    private GameObject mazeCentre;
    private Rigidbody playerRb;
    private Vector2 move;
    private Vector2 look;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 forwardMovement;
    private Vector3 rightMovement;
    private Vector3 relativeMovement;
    private float moveX;
    private float moveY;
    private float movez;
    public float speed = 10;
    public float rotationSpeed = 10;
    public float lookSpeed = 100;
    
    void Awake() {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        mazeCentre = GameObject.Find("Maze Centre");
        controls = new PlayerControls();
        
        /* set focalPoint position to that of the players position for the camera to follow */
        focalPoint.transform.position = transform.position;

        // controls.Player.Look.performed += ctx => SendMessage(ctx.ReadValue<Vector2>());
    }
    
    void FixedUpdate()
    {
        /* get camera directional vectors & normalize them */
        cameraForward = camera.transform.forward;
        cameraRight = camera.transform.right;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;
        cameraRight.y = 0;
        cameraRight = cameraRight.normalized;
        
        /* convert then combine directional vectors to local space */
        forwardMovement = move.y * cameraForward;
        rightMovement = move.x * cameraRight;
        relativeMovement = forwardMovement + rightMovement;
        
        playerRb.AddForce(relativeMovement * speed * Time.deltaTime);
        focalPoint.transform.position = transform.position;

        RotateCamera();
    }

    private void OnEnable() {
        controls.Player.Enable();
    }

    private void OnDisable() {
        controls.Player.Look.Disable();
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Goal")) {
            Debug.Log("Found the Goal");
        }
        
    }
    
    

    private void SendMessage(Vector2 coordinates) {
        Debug.Log("Thumb-stick coordinates = " + coordinates);
    }

    public void OnMove(InputValue movementValue) {
        // Vector2 movementVector = movementValue.Get<Vector2>();
        // moveX = movementVector.x;
        // moveY = movementVector.y;
        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;
    }

    public void OnLook(InputValue lookValue) {
        // Vector2 lookVector = lookValue.Get<Vector2>();
        // lookX = lookVector.x;
        // lookY = lookVector.y;
        controls.Player.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => look = Vector2.zero;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
        }
        
        if (other.gameObject.CompareTag("Waypoint")) {
            Debug.Log("Found a Waypoint");
            // var otherParticle = other.gameObject.GetComponentInChildren<ParticleSystem>();
            // otherParticle.Play();
        }
        if (other.gameObject.CompareTag("Goal")) {
            Debug.Log("Found the Goal");
            GameManager.goalFound = true;

            // var otherParticle = other.gameObject.GetComponentInChildren<ParticleSystem>();
            // otherParticle.Play();
        }
    }

    void RotateCamera() {
        camera.transform.RotateAround(focalPoint.transform.position, Vector3.up, look.x);
    }

}
