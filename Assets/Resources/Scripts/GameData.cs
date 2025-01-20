using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData_SO", menuName = "ScriptableObjects/GameData Scriptable")]
public class GameData : ScriptableObject {

    public string dataName;
    public ScriptableObject player;
    public ScriptableObject goalObject;
    public ScriptableObject[] waypoints;
    public GameObject[] walls;
    public GameObject[] floorTiles;
    public int gridHeight, gridWidth;
    public static Color hintColour01, hintColour02;

    [Range(1,3)]
    public int difficulty;
    
    public static int count, horizDistance, vertDistance, xPosition;
    public static bool easyMode, goalFound, horizAligned, vertAligned, isForward, isRight, debugHoriz, debugVert, firstRow, firstColumn;
    public static bool showMenu = false;
    public static bool shortListed = false;
    public static bool firstRowFound = false;
    public static bool firstColFound = false;
    public const int north = 0;
    public const int east = 1;
    public const int south = 2;
    public const int west = 3;
    public static int northernEdge;
    public static int easternEdge;
    public static int southernEdge;
    public static int westernEdge;


    private void Awake() {
        northernEdge = gridHeight / 2;
        easternEdge = gridWidth / 2;
        southernEdge = -northernEdge;
        westernEdge = -easternEdge;
    }

}
