using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable InconsistentNaming

[CreateAssetMenu(fileName = "GameData_SO", menuName = "ScriptableObjects/GameData Scriptable")]
public class GameData : ScriptableObject {

    public string dataName;
    // public Player player;
    // public static Waypoints goalObject;
    // public static List<Waypoints> waypoints;
    // public GameObject[] walls;
    // public GameObject[] floorTiles;
    public int gridHeight;
    public int gridWidth;
    public Color goalColour, hintColour01, hintColour02;

    [Range(1,3)]
    public int difficulty;
    
    // public int count, horizDistance, vertDistance, xPosition;
    public bool easyMode, goalFound, dataInitialized;
    public bool showMenu = false;
    public bool shortListed = false;
    public bool firstRowFound = false;
    public bool firstColFound = false;
    public int northernEdge;
    public int easternEdge;
    public int southernEdge;
    public int westernEdge;
    
    public List<Vector3> mazePath;

    private void Awake() {
        // Debug.Log("grid height/width = " +gridHeight + " / " + gridWidth);
        // northernEdge = gridHeight / 2;
        // easternEdge = gridWidth / 2;
        // southernEdge = -northernEdge;
        // westernEdge = -easternEdge;
        // mazePath ??= new List<Vector3>();
        
    }

    // public List<GameObject> GetWaypoints() {
    //     return (from waypoint in waypoints where waypoint != null select Waypoints.waypointBase).ToList();
    // }

    // public void SetGoalColour(Color inputColour) {
    //     // goalObject = (Waypoints)ScriptableObject.
    //     Waypoints.SetColour(inputColour);
    // }

    public void InitData(Vector2Int gridDimensions) {
        gridWidth = gridDimensions.x;
        gridHeight = gridDimensions.y;
        northernEdge = gridHeight / 2;
        easternEdge = gridWidth / 2;
        southernEdge = -northernEdge;
        westernEdge = -easternEdge;
        mazePath ??= new List<Vector3>();
        dataInitialized = true;
    }
    
    public void InitPath() {
        mazePath = new List<Vector3>();
    }

}
