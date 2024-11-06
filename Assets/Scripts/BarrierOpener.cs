using UnityEngine;

public class BarrierOpener : MonoBehaviour
{
    /* TODO: set a flag/value for "unlocking the barriers" & enable  win conditions on the goal for the player  */
    /* using the Particle effect on the waypoint to prove/test the player triggering an action */
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
            
        }
    }
    
}
