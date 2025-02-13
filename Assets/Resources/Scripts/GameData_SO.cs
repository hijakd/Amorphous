// ReSharper disable RedundantUsingDirective
// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable UnusedMember.Global


using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;

namespace AmorphousData {


    [CreateAssetMenu(fileName = "GameData_SO", menuName = "ScriptableObjects/GameData ScriptableObject")]
    public class GameData : ScriptableObject {

        // static GameData instance;
        // public static GameData Instance {
        //     get {
        //         if (!instance) {
        //             instance = <GameData>();
        //         }
        //         if (!instance) {
        //             instance = CreateInstance<GameData>();
        //         }
        //         return instance;
        //     }
        // }

        /* playerColour => the current colour blend */
        /* previousColour01 => the colour of the last waypoint the player encountered */
        /* previousColour02 => the colour of the waypoint the player encountered before previousColour01 */
        

        public Color goalColour, playerColour, hintColour01, hintColour02, previousColour01, previousColour02;
        [Range(1, 3)] public int difficulty;
        public float playerSpeed;
        public float zeroSpeed = 0f;
        public int gridHeight = 20;
        public int gridWidth = 20;
        // public int northernEdge { get; private set; }
        public int northernEdge;
        // public int easternEdge { get; private set; }
        public int easternEdge;
        // public int southernEdge { get; private set; }
        public int southernEdge;
        // public int westernEdge { get; private set; }
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
        // public GameObject player;
        
        public List<Waypoint> waypoints;
        
        
        public List<Vector3> mazePath;

        bool twentyFourHrTime;


        /* END vars */

        // public void InitData(/*int mazeWidth, int mazeHeight*/) {
        public void OnEnable() {
            // gridWidth = mazeWidth;
            // gridHeight = mazeHeight;
            northernEdge = gridHeight / 2;
            easternEdge = gridWidth / 2;
            southernEdge = -northernEdge;
            westernEdge = -easternEdge;
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


        /* END GameData*/
    }

    /* END NAMESPACE*/
}