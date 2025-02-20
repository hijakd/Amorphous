using UnityEngine;

public class ColourResetter : MonoBehaviour {

    public GameObject resetter;

    public float rotationSpeed = 42f;
    

    void FixedUpdate() {
        gameObject.transform.Rotate(new Vector3(0f, rotationSpeed, 0f) * Time.deltaTime);
    }
    
}
