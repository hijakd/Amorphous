using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Waypoint_SO", menuName = "ScriptableObjects/Waypoint ScriptableObject")]
public class Waypoint : ScriptableObject {

    public GameObject waypoint;
    public Color waypointColour;
    public bool isGoalWaypoint;
    public string ID = Guid.NewGuid().ToString().ToUpper();
    public string name;

    void OnEnable() {
        name = waypoint.name;
    }

}
