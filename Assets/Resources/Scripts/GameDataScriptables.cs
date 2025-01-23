using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject {

    public string dataName;
    public int gridHeight;
    public int gridWidth;
    public Color goalColour, hintColour01, hintColour02;
    [Range(1,3)]
    public int difficulty;
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
}
