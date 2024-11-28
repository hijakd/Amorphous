using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming

public class GManager : MonoBehaviour {
    
    public static GManager instance;
    
    
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
    // public static bool goalFound;
    public bool goalFound;
    public Button restartButton;
    public GameObject titleScreen;
    [Range(0.24f, 0.76f)] public float randomVariance = 0.42f;
    // public static Vector2 xMinMax;
    // public static Vector2 yMinMax;

    // public static List<Vector3> _selectableCoords;
    public List<Vector3> selectableCoords;
    // public static List<Vector3> _intersections;
    public List<Vector3> intersections;
    // public static List<Vector3> _shortList;
    public List<Vector3> shortList;
    // public static List<Vector3> _drawnPath;
    public List<Vector3> drawnPath;
    // public static List<Vector3> _slicedPath;
    public List<Vector3> slicedPath;
    // public static bool _foundFirstRow = false;
    public bool foundFirstRow = false;
    // public static bool _foundFirstColumn  = false;
    public bool foundFirstColumn  = false;
    private Color goalColour;
    private GameObject groundPlane;
    private List<GameObject> cardinals;
    private int count;
    // public static int halfHeight;
    public int halfHeight;
    // public static int halfWidth;
    public int halfWidth;
    private int rowNumber;
    private int columnNumber;
    public int xMin;
    public int yMin;
    public int selectablesIndex;
    public List<int> _distances;
    public List<Vector3> midPoints;
    public List<int> lcms;
    // public static Vector2 _randVariance;
    public Vector2 randVariance;
    private Vector3 center;
    
    private Material _gypMaterial; // added for testing

    public List<int> resetIndex;
    public List<int> resetCounter;
    
    
    
    private void OnDrawGizmos() {
        // visualize the corners of the grid in the Editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
    }
    
    
    private void Awake() {
        
        if (instance != null && instance != this) {
            Destroy(this);
        }
        else {
            instance = this;
        }
        
        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        cardinals = new List<GameObject>(Resources.LoadAll<GameObject>("Cardinals"));
        _distances = new List<int>();
        lcms = new List<int>();
        // _drawnPath = new List<Vector3>();
        // _selectableCoords = new List<Vector3>();
        // _intersections = new List<Vector3>();
        midPoints = new List<Vector3>();
        // _slicedPath = new List<Vector3>();
        slicedPath = new List<Vector3>();
        center = new Vector3(0, 0, 0);
        
        drawnPath = new List<Vector3>();
        selectableCoords = new List<Vector3>();
        intersections = new List<Vector3>();
        resetIndex = new List<int>(); // for testing
        resetCounter = new List<int>(); // for testing
        
        _gypMaterial = Resources.Load<Material>("Materials/DryWall_Mat"); // added for testing
        groundPlane = GameObject.Find("GroundPlane");
        
        halfHeight = gridHeight / 2;
        halfWidth = gridWidth / 2;
        xMin = -halfWidth;
        yMin = -halfHeight;
        
        randVariance.x = randomVariance;
        randVariance.y = 1f - randomVariance;
        
        ResetSIndex();
        
        /* populate the list of coordinates for the GameObjects that will be used for generating the maze path */
        /* these GameObjects include the maze center, corners, player Spawn position, goal position and waypoints */
        // _selectableCoords.Add(new Vector3(0, 0, 0)); // grid/maze center
        selectableCoords.Add(new Vector3(0, 0, 0)); // grid/maze center
        // _selectableCoords.Add(new Vector3(xMin, 0f, halfHeight)); // Top Left cardinal/corner
        selectableCoords.Add(new Vector3(xMin, 0f, halfHeight)); // Top Left cardinal/corner
        // _selectableCoords.Add(new Vector3(halfWidth, 0f, halfHeight)); // Top Right cardinal/corner
        selectableCoords.Add(new Vector3(halfWidth, 0f, halfHeight)); // Top Right cardinal/corner
        // _selectableCoords.Add(new Vector3(halfWidth, 0f, yMin)); // Bottom Right cardinal/corner
        selectableCoords.Add(new Vector3(halfWidth, 0f, yMin)); // Bottom Right cardinal/corner
        // _selectableCoords.Add(new Vector3(xMin, 0f, yMin)); // Bottom Left cardinal/corner
        selectableCoords.Add(new Vector3(xMin, 0f, yMin)); // Bottom Left cardinal/corner
        // _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // player position
        selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // player position
        for (int i = 0; i < waypoints.Count; i++) {
            // _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight));
            selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight));
        }
        // _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // goal position
        selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // goal position

        /* spawn 'cardinals' before calculating distances & midpoints */
        // Spawn.Object(cardinals[0], _selectableCoords[1]); // Top Left corner
        // Spawn.Object(cardinals[1], _selectableCoords[2]); // Top Right corner
        // Spawn.Object(cardinals[2], _selectableCoords[3]); // Bottom Right corner
        // Spawn.Object(cardinals[3], _selectableCoords[4]); // Bottom Left corner
        
        /* populate the lists containing the distances & midpoints between the spawn positions & waypoints */
        // for (int x = selectablesIndex; x < _selectableCoords.Count - 1; x++) {
        for (int x = selectablesIndex; x < selectableCoords.Count - 1; x++) {
            Debug.Log("int x: " + x + 
                "\n_selectablesIndex: " + selectablesIndex);
            // _distances.Add(GUtils.CalculateDistance(_selectableCoords[selectablesIndex], _selectableCoords[selectablesIndex + 1]));
            _distances.Add(GUtils.CalculateDistance(selectableCoords[selectablesIndex], selectableCoords[selectablesIndex + 1]));
            // midPoints.Add(Vector3.Lerp(_selectableCoords[selectablesIndex],
            midPoints.Add(Vector3.Lerp(selectableCoords[selectablesIndex],
                selectableCoords[selectablesIndex + 1], 0.5f));
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
        // player.transform.position = _selectableCoords[selectablesIndex];
        player.transform.position = selectableCoords[selectablesIndex];
        Debug.Log("selectableIndex: " + selectablesIndex);
        selectablesIndex++;
        Debug.Log("selectableIndex: " + selectablesIndex);
        /* Spawn the waypoints */
        count = 0;
        // while (selectablesIndex < _selectableCoords.Count - 1) {
        while (selectablesIndex < selectableCoords.Count - 1) {
            Debug.Log("selectableIndex: " + selectablesIndex);
            // Spawn.Object(waypoints[count], _selectableCoords[selectablesIndex]);
            Spawn.Object(waypoints[count], selectableCoords[selectablesIndex]);
            selectablesIndex++;
            count++;
        }
        /* Spawn the goal */
        // Spawn.Object(goal, _selectableCoords[selectablesIndex]);
        Spawn.Object(goal, selectableCoords[selectablesIndex]);

        for (int i = 0; i < midPoints.Count; i++) {
            lcms.Add(GUtils.FindLCM(midPoints[i], _distances[i]));
        }

        /* reset counter/index to zero/starting values */
        ResetCounters();
        /* populate the list of maze 'intersections' */
        // int rangeMax = Mathf.RoundToInt(_selectableCoords.Count);
        int rangeMax = Mathf.RoundToInt(selectableCoords.Count);
        // while (selectablesIndex < _selectableCoords.Count - 1) {
        while (selectablesIndex < selectableCoords.Count - 1) {
            // _intersections.Add(_selectableCoords[selectablesIndex]);
            intersections.Add(selectableCoords[selectablesIndex]);
            // _intersections.Add(Triangulation.Triangulate(lcms[count], rangeMax, _randVariance, _selectableCoords));
            intersections.Add(Triangulation.Triangulate(lcms[count], rangeMax, randVariance, selectableCoords));
            selectablesIndex++;
            count++;
        }
        // _intersections.Add(_selectableCoords[selectablesIndex]);
        intersections.Add(selectableCoords[selectablesIndex]);
        
        /* remove any duplicate values from the intersections list */
        // _shortList = new List<Vector3>(GUtils.ShortenList(_intersections));
        shortList = new List<Vector3>(GUtils.ShortenList(intersections));
        // _intersections.Clear();
        intersections.Clear();
        for (int i = 0; i < shortList.Count; i++) {
            // _intersections.Add(_shortList[i]);
            intersections.Add(shortList[i]);
        }
        /* delete contents of shortenedList to save unnecessary use of memory */
        shortList.Clear();
        
        ResetSIndex();

        // while (count < _intersections.Count) {
        while (count < intersections.Count) {
            // if (count + 1 < _intersections.Count) {
            if (count + 1 < intersections.Count) {
                // CheckOrientation.CheckHorizontal(_intersections[count], _intersections[count + 1], _drawnPath);
                CheckOrientation.CheckHorizontal(intersections[count], intersections[count + 1], drawnPath);
                // CheckOrientation.CheckVertical(_drawnPath[_drawnPath.Count - 1], _intersections[count + 1], _drawnPath);
                CheckOrientation.CheckVertical(drawnPath[drawnPath.Count - 1], intersections[count + 1], drawnPath);
            }
            else {
                // CheckOrientation.CheckHorizontal(_intersections[count], _intersections[count - 1], _drawnPath);
                CheckOrientation.CheckHorizontal(intersections[count], intersections[count - 1], drawnPath);
                // CheckOrientation.CheckVertical(_drawnPath[_drawnPath.Count - 1], _intersections[count], _drawnPath);
                CheckOrientation.CheckVertical(drawnPath[drawnPath.Count - 1], intersections[count], drawnPath);
            }

            count++;
        }
        
        /* remove duplicate values from drawnPath */
        // _shortList = GUtils.ShortenList(_drawnPath);
        shortList = GUtils.ShortenList(drawnPath);
        // _drawnPath.Clear();
        drawnPath.Clear();
        for (int i = 0; i < shortList.Count; i++) {
            // _drawnPath.Add(_shortList[i]);
            drawnPath.Add(shortList[i]);
        }
        
        /* delete contents of shortenedList to save unnecessary use of memory */
        shortList.Clear();
        
        ResetCount();
        /* spawn floor tiles */
        // while (count < _drawnPath.Count) {
        while (count < drawnPath.Count) {
            // Spawn.Object(floorTile, _drawnPath[count]);
            Spawn.Object(floorTile, drawnPath[count]);
            count++;
        }
        
        /* find the first row of the maze grid */
        // _slicedPath = SliceNSort.SliceRows(_drawnPath, halfHeight, xMin); // height gives the number of rows
        slicedPath = SliceNSort.SliceRows(drawnPath, halfHeight, xMin); // height gives the number of rows
        /* find the value of the first row */
        GUtils.WaitForRowSlice();
        if (slicedPath.Count > 0) {
            rowNumber = GUtils.FindFirstRow(slicedPath);
        }
        else {
            rowNumber = halfHeight;
        }
        // rowNumber = Mathf.RoundToInt(_slicedPath[0].z);
        
        
        /* Spawn the east/west walls */
        while (rowNumber >= -halfHeight) {
            // _slicedPath = SliceNSort.SliceRows(_drawnPath, rowNumber, xMin);
            slicedPath = SliceNSort.SliceRows(drawnPath, rowNumber, xMin);
            Spawn.EastWestWalls(slicedPath, wallPanels, _gypMaterial);
            /* clear _slicedPath before parsing the next row */
            slicedPath.Clear();
            rowNumber--;
        }
        
        /* delete contents of slicedPath to eliminate junk data in the next step */
        slicedPath.Clear();
        
        /* find the first column of the maze grid */
        // _slicedPath = SliceNSort.SliceColumns(_drawnPath, halfWidth, yMin); // width gives the number of columns
        slicedPath = SliceNSort.SliceColumns(drawnPath, halfWidth, yMin); // width gives the number of columns
        /* find the value of the first column */
        GUtils.WaitForColumnSlice();
        if (slicedPath.Count > 0) {
            columnNumber = GUtils.FindFirstColumn(slicedPath);
        }
        else {
            columnNumber = halfWidth;
        }
        // columnNumber = Mathf.RoundToInt(_slicedPath[0].x);
        
        /* Spawn the north/south walls */
        while (columnNumber >= -halfWidth) {
            // _slicedPath = SliceNSort.SliceColumns(_drawnPath, columnNumber, yMin);
            slicedPath = SliceNSort.SliceColumns(drawnPath, columnNumber, yMin);
            Spawn.NorthSouthWalls(slicedPath, wallPanels, _gypMaterial);
            /* clear _slicedPath before parsing the next column */
            slicedPath.Clear();
            columnNumber--;
        }
        
        
        
        /** END Start() **/
    }

    
    void FixedUpdate() {

        if (goalFound) {
            EndLevel();
        }
        
        /** END FixedUpdate() **/
    }
    
    
    
    /** Utility functions for the GameManager **/
    private void ResetCount() {
        resetCounter.Add(count);
        count = 0;
    }

    private void ResetSIndex() {
        // Debug.Log("Resetting SIndex");
        resetIndex.Add(selectablesIndex);
        selectablesIndex = 5;
        
    }

    private void ResetCounters() {
        ResetCount();
        ResetSIndex();
    }

    public void EndLevel() {
        if (PlayerController.currentColour == goalColour) {
            Debug.Log("display winText");
            winText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
        }
    }
    
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame() {
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
    }
}
