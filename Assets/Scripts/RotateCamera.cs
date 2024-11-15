using UnityEngine;

public class RotateCamera : MonoBehaviour {
    PlayerControls controls;
    public GameObject player;
    public Vector3 offset;
    public float rotationSpeed;
    private Vector2 look;
    
    
    void Awake() {
        controls.Player.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => look = Vector2.zero;
    }

    // Update is called once per frame
    void Update() {
        transform.position = player.transform.position + offset;
        transform.Rotate(Vector3.up, look.x * rotationSpeed * Time.deltaTime);
        // transform.Rotate(transform.up, looking.y);
    }
    
    private void OnEnable() {
        controls.Player.Enable();
    }
    
    private void OnDisable() {
        controls.Player.Disable();
    }
}