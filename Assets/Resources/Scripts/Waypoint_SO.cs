using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Waypoint_SO", menuName = "ScriptableObjects/Waypoint ScriptableObject")]
public class Waypoint : ScriptableObject {

    public GameObject waypoint;
    public Color waypointColour;
    public bool isGoalWaypoint;
    public string ID = Guid.NewGuid().ToString().ToUpper();
    public string waypointName;

    void OnEnable() {
        waypointName = waypoint.name;
        waypointColour = waypoint.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
    }

    public Color GetColour() {
        return waypoint.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
    }

    public void SetColour(Color inputColour) {
        waypointColour = inputColour;
    }

    public void SetWaypointColour(Color inputColour) {
        waypoint.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color = inputColour;
    }

    public void SetWaypoint(GameObject input) {
        waypoint = input;
        SetColour(waypoint.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color);
        waypointName = waypoint.name;
    }
}
