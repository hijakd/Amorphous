// ReSharper disable RedundantUsingDirective
// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable UnusedMember.Global


using System.Collections.Generic;
using UnityEngine;

namespace AmorphousData {


    [CreateAssetMenu(fileName = "GameData_SO", menuName = "ScriptableObjects/GameData ScriptableObject")]
    public class GameData : ScriptableObject {


        public Color goalColour, playerColour, hintColour01, hintColour02, previousColourFound;
        [Range(1, 3)] public int difficulty;
        public int gridHeight { get; private set; }
        public int gridWidth { get; private set; }
        public int northernEdge { get; private set; }
        public int easternEdge { get; private set; }
        public int southernEdge { get; private set; }
        public int westernEdge { get; private set; }

        public bool dataInitialized { get; set; }
        public bool firstRowFound { get; set; }
        public bool firstColFound { get; set; }
        public bool goalFound { get; set; }
        public bool playerIsWhite { get; set; }
        public bool shortListed { get; set; }
        public bool showMenu { get; set; }
        
        
        public List<Vector3> mazePath;

        bool twentyFourHrTime;


        public void InitData(int mazeWidth, int mazeHeight) {
            gridWidth = mazeWidth;
            gridHeight = mazeHeight;
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