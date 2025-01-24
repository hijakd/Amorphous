using System;
// using Unity.VisualScripting;
using UnityEngine;

public class Waypoints : MonoBehaviour {

    public WaypointsSO thisWaypoint;
    public Vector3 waypointPos;


    private void Awake() {
        waypointPos = thisWaypoint.waypoint.gameObject.transform.position;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        
    }

    // public void ChangePosition(GameObject gObject) {
    //     if (Mathf.Approximately(gObject.gameObject.transform.position.x, thisWaypoint.wpPos.x) &&
    //         Mathf.Approximately(gObject.gameObject.transform.position.z, thisWaypoint.wpPos.z)) {
    //         gObject.gameObject.transform.position = thisWaypoint.wpPos;
    //     }
    // }
    
    public void SetGoalColour(Color inputColour) {
        // set the material colour of the waypoint
        thisWaypoint.baseMaterial.color = inputColour;
        // set the material of the waypoint
        thisWaypoint.waypoint.gameObject.GetComponent<MeshRenderer>().sharedMaterial = thisWaypoint.baseMaterial;
    }
}
