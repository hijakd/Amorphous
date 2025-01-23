using System;
using UnityEngine;

public class ColourResetter : MonoBehaviour {

    public GameObject resetter;

    public float rotationSpeed = 42f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        gameObject.transform.Rotate(new Vector3(0f, rotationSpeed, 0f) * Time.deltaTime);
    }

    // private void OnTriggerEnter(Collision other) {
    //     if (other.gameObject.CompareTag("Player")) {
    //         Debug.Log("Player collided with the resetter");
    //         MazeUI.PaintPlayerBlipWhite();
    //     }
    // }

}
