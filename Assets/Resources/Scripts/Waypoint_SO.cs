using UnityEngine;


[CreateAssetMenu(fileName = "Waypoint_SO", menuName = "ScriptableObjects/Waypoint ScriptableObject")]
public class Waypoint : ScriptableObject {

    public GameObject waypoint;
    public Color waypointColour;
    public bool isGoalWaypoint;

}
