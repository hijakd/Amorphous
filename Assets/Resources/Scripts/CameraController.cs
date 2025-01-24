using UnityEngine;

public class CameraController : MonoBehaviour {
    
    public static PlayerSO playerSO;
    public GameObject player;
    public Vector3 offset;
    private Vector3 looking;
    private Vector2 look;
    private PlayerControls controls;

    
    void Awake() {
        offset = transform.position - player.transform.position;
        // playerControls.Player.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        // playerControls.Player.Look.canceled += ctx => look = Vector2.zero;

    }
    

    // Update is called once per frame
    void LateUpdate() {
        // looking = new Vector3(0f, look.x, 0f)/* * lookSpeed * Time.deltaTime*/;
        transform.position = player.transform.position + offset;
        // transform.RotateAround(player.transform.position, transform.up, looking.x);
        // RotateCamera();
    }
    
    void RotateCamera() {
        
    }
    
    private void OnEnable() {
        controls.Player.Enable();
    }
    
    private void OnDisable() {
        controls.Player.Disable();
    }
}