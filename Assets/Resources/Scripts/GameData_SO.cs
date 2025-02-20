using System.Collections.Generic;
using UnityEngine;

namespace AmorphousData {


    [CreateAssetMenu(fileName = "GameData_SO", menuName = "ScriptableObjects/GameData ScriptableObject")]
    public class GameData : ScriptableObject {

        /* playerColour => the current colour blend */
        /* previousColour01 => the colour of the last waypoint the player encountered */
        /* previousColour02 => the colour of the waypoint the player encountered before previousColour01 */

        public Color goalColour;
        public Color playerColour;
        public Color hintColour01;
        public Color hintColour02;
        public Color previousColour01;
        public Color previousColour02;
        [Range(1, 3)] public int difficulty;
        public float playerSpeed = 50f;
        public float zeroSpeed = 0f;
        public int gridHeight = 20;
        public int gridWidth = 20;
        public int northernEdge;
        public int easternEdge;
        public int southernEdge;
        public int westernEdge;

        public bool dataInitialized { get; set; }
        public bool firstRowFound { get; set; }
        public bool firstColFound { get; set; }
        public bool goalFound { get; set; }
        public bool playerIsWhite { get; set; }
        public bool shortListed { get; set; }
        public bool showMenu { get; set; }
        public bool isPaused { get; set; }

        public Waypoint goalWaypoint;
        public Color goalWaypointColour;
        
        public List<Waypoint> waypoints;
        
        public List<Vector3> mazePath;

        bool twentyFourHrTime;


        /* END vars */

        public void OnEnable() {
            northernEdge = gridHeight / 2;
            easternEdge = gridWidth / 2;
            southernEdge = -northernEdge;
            westernEdge = -easternEdge;
            waypoints ??= new List<Waypoint>();
            mazePath ??= new List<Vector3>();
            showMenu = false;
            shortListed = false;
            firstColFound = false;
            firstRowFound = false;
            dataInitialized = true;

        }

        public void SetTimeFormat(bool twentyFourHr) {
            twentyFourHrTime = twentyFourHr;
        }

        public string GetTimeFormat() {
            string time = twentyFourHrTime ? "HH:mm:ss" : "hh:mm:ss";
            return time;
        }

        public Waypoint GetWaypoint(int listPosition) {
            return waypoints[listPosition];
        }
        
        public List<Waypoint> GetWaypoints() {
            return waypoints;
        }


        public int GetWaypointCount() {
            return waypoints.Count;
        }

        /* END GameData*/
    }

    /* END NAMESPACE*/
}