using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming

public class GManager : MonoBehaviour {
    
    public GameObject player;
    public GameObject goal;
    public GameObject floorTile;
    public GameObject[] wallPanels;
    public int gridHeight;
    public int gridWidth;
    public List<GameObject> waypoints;  
    public TextMeshProUGUI winText;
    public TextMeshProUGUI gameOverText;
    public bool isGameActive;
    public static bool goalFound;
    public Button restartButton;
    public GameObject titleScreen;
    [Range(0.24f, 0.76f)] public float randomVariance = 0.42f;
    // public static Vector2 xMinMax;
    // public static Vector2 yMinMax;

    public static List<Vector3> _selectableCoords;
    public static List<Vector3> _intersections;
    public static List<Vector3> _shortList;
    private Color goalColour;
    private GameObject groundPlane;
    private List<GameObject> cardinals;
    private int count;
    public static int halfHeight;
    public static int halfWidth;
    private int distance02;
    private int farCornerDistance;
    private int lcm01;
    private int lcm02;
    private int lcm03;
    private int combinedLCM;
    private int rowNumber;
    private int columnNumber;
    private int xMin;
    private int yMin;
    private int selectablesIndex;
    private List<int> distances;
    private List<Vector3> midPoints;
    private List<int> lcms;
    public static Vector2 randVariance;
    private Vector3 center;
    
    
    private void OnDrawGizmos() {
        // visualize the corners of the grid in the Editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
    }
    
    
    private void Awake() {
        
        groundPlane = GameObject.Find("GroundPlane");
        halfHeight = gridHeight / 2;
        halfWidth = gridWidth / 2;
        xMin = -halfWidth;
        yMin = -halfHeight;
        center = new Vector3(0, 0, 0);
        ResetSIndex();
        
        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        cardinals = new List<GameObject>(Resources.LoadAll<GameObject>("Cardinals"));
        // Spawn.Object(cardinals[0], new Vector3(xMin, 0f, halfHeight)); // Top Left corner
        // Spawn.Object(cardinals[1], new Vector3(halfWidth, 0f, halfHeight)); // Top Right corner
        // Spawn.Object(cardinals[2], new Vector3(halfWidth, 0f, yMin)); // Bottom Right corner
        // Spawn.Object(cardinals[3], new Vector3(xMin, 0f, yMin)); // Bottom Left corner
        
        _selectableCoords = new List<Vector3>();
        _intersections = new List<Vector3>();
        
        /* populate the list of coordinates for the GameObjects that will be used for generating the maze path */
        /* these GameObjects include the maze center, corners, player Spawn position, goal position and waypoints */
        _selectableCoords.Add(new Vector3(0, 0, 0)); // grid/maze center
        _selectableCoords.Add(new Vector3(xMin, 0f, halfHeight)); // Top Left cardinal/corner
        _selectableCoords.Add(new Vector3(halfWidth, 0f, halfHeight)); // Top Right cardinal/corner
        _selectableCoords.Add(new Vector3(halfWidth, 0f, yMin)); // Bottom Right cardinal/corner
        _selectableCoords.Add(new Vector3(xMin, 0f, yMin)); // Bottom Left cardinal/corner
        _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // player position
        for (int i = 0; i < waypoints.Count; i++) {
            _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight));
        }
        _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // goal position

        /* populate the lists containing the distances & midpoints between the spawn positions & waypoints */
        for (int x = 0; x < _selectableCoords.Count; x += 2) {
            distances.Add(Mathf.RoundToInt(Vector3.Distance(_selectableCoords[selectablesIndex], _selectableCoords[selectablesIndex++])));
            midPoints.Add(Vector3.Lerp(_selectableCoords[selectablesIndex],
                _selectableCoords[selectablesIndex++], 0.5f));
            selectablesIndex++;
        }

        goalColour = new Color(1, 1, 1);
        goal.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.white;
        goalColour = ColourChanger.BlendColours(waypoints, true);


        /* reset selectablesIndex to starting value before Start to assist with spawning the Player & other
         objects while ignoring the corner/cardinal GameObjects */
        ResetSIndex();
        /** END AWAKE **/
    }
    
    void Start() {

        MazeUI.PaintGoalBlip(goalColour);
        groundPlane.gameObject.SetActive(false);

        /* Spawn the player */
        player.transform.position = _selectableCoords[selectablesIndex];
        selectablesIndex++;
        /* Spawn the waypoints */
        int index = 0;
        while (selectablesIndex < _selectableCoords.Count - 1) {
            Spawn.Object(waypoints[index], _selectableCoords[selectablesIndex]);
            selectablesIndex++;
            index++;
        }
        /* Spawn the goal */
        Spawn.Object(goal, _selectableCoords[selectablesIndex]);

        for (int i = 0; i < midPoints.Count; i++) {
            lcms.Add(GUtils.FindLCM(midPoints[i], distances[i]));
        }

        ResetCounters();
        int rangeMax = Mathf.RoundToInt(_selectableCoords.Count);
        while (selectablesIndex < _selectableCoords.Count - 1) {
            _intersections.Add(_selectableCoords[selectablesIndex]);
            _intersections.Add(Triangulation.Triangulate(lcms[count], rangeMax));
            selectablesIndex++;
            count++;
        }
        _intersections.Add(_selectableCoords[selectablesIndex]);
        
        /* remove duplicate values from intersections */
        _shortList = new List<Vector3>(GUtils.ShortenList(_intersections));
        _intersections.Clear();
        for (int i = 0; i < _shortList.Count; i++) {
            _intersections.Add(_shortList[i]);
        }
        /* delete contents of shortenedList to save unnecessary use of memory */
        _shortList.Clear();
        
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void ResetCount() {
        count = 0;
    }

    private void ResetSIndex() {
        selectablesIndex = 5;
    }

    private void ResetCounters() {
        ResetCount();
        ResetSIndex();
    }
}
