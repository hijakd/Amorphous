using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierOpener : MonoBehaviour
{
    
    // GameObject playerObject;
    
    // private void OnCollisionEnter(Collision other) {
    //     if (other.gameObject.CompareTag("Player")) {
    //         // other.GetComponent<Renderer>().material.color = gameObject.GetComponent<Material>().color;
    //         gameObject.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(true);
    //         
    //     }
    // }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // other.GetComponent<Renderer>().material.color = gameObject.GetComponent<Material>().color;
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
            
        }
    }
    
}
