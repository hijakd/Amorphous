using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// ReSharper disable InvalidXmlDocComment
// ReSharper disable PossibleLossOfFraction

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local
// ReSharper disable Unity.RedundantEventFunction

public class GameManager : MonoBehaviour {

    public GameObject goalObject;
    public GameObject player;
    public GameObject pyramid;
    public GameObject floorTile01;
    public GameObject floorTile02;
    public GameObject[] wallPanels; // _N_ever _E_at _S_oggy _W_eetbix
    private GameObject groundPlane;
    public int gridHeight;
    public int gridWidth;
    public List<GameObject> waypoints;
    public TextMeshProUGUI winText, gameOverText;
    public bool isGameActive;
    public static bool goalFound;
    public Button restartButton;
    public GameObject titleScreen;
    [Range(0.24f, 0.76f)] public float randomVariance = 0.42f;
    public static Vector2Int mazeWidth, mazeHeight;
    public static List<Vector3> selectableCoords;
    public static Vector2 randVariance;

    private Color goalColour;
    private ColorBlock goalColorBlock;
    private float xVal, zVal;
    private int count, /*rowNumber,*/ columnNumber;
    [SerializeField] private int rowNumber;
    private Vector3 goalPosition, pyramidPos, pyramidPos02, spawnPosition;

    private List<Color> mixedColors;
    private List<GameObject> cardinals;
    private List<int> distances, lcms;

    [SerializeField] private List<Vector3> intersections,
        /*tmpIntersections,*/
        drawnPath,
        shortenedList,
        slicedPath,
        destinations,
        midPoints;

    public static Material obsMaterial; // added for testing
    private Material gypMaterial; // added for testing
    public static Material purpleMaterial; // added for testing
    public static Material pinkMaterial; // added for testing
    public static Material blueMaterial; // added for testing
    private Material greenMaterial; // added for testing
    private Material goalMaterial;

    public static bool shortListed = false;
    public static bool firstRowFound = false;
    public static bool firstColFound = false;
    public static int _north { get; private set; }
    public static int _east { get; private set; }
    public static int _south { get; private set; }
    public static int _west { get; private set; }
    // public static string telemetryData = "";
    


    private void OnDrawGizmos() {
        // visualize the corners of the grid in the Editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, gridHeight / 2),
            new Vector3(0.75f, 2f, 0.75f)); // NorthWest corner
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, gridHeight / 2),
            new Vector3(0.75f, 2f, 0.75f)); // NorthEast corner
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, -gridHeight / 2),
            new Vector3(0.75f, 2f, 0.75f)); // SouthEast corner
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, -gridHeight / 2),
            new Vector3(0.75f, 2f, 0.75f)); // SouthWest corner

        // visualize paths between 'intersections'
        // Gizmos.color = Color.blue;
        for (int i = 0; i < intersections.Count - 1; i++) {
            Gizmos.DrawLine(intersections[i], intersections[i + 1]);
        }

        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(spawnPosition, pyramidPos);
        // Gizmos.DrawLine(pyramidPos, destination01);
        // if (intersections.Count > 0) {
        //     Gizmos.DrawLine(intersections[0], intersections[1]);
        //     Gizmos.DrawLine(intersections[1], intersections[2]);
        //     Gizmos.DrawLine(intersections[2], intersections[3]);
        //     Gizmos.DrawLine(intersections[3], intersections[4]);
        //     Gizmos.DrawLine(intersections[4], intersections[5]);
        //     Gizmos.DrawLine(intersections[5], intersections[6]);
        //     Gizmos.DrawLine(intersections[6], intersections[7]);
        //     Gizmos.DrawLine(intersections[7], intersections[8]);
        //     Gizmos.DrawLine(intersections[8], intersections[9]);
        //     Gizmos.DrawLine(intersections[9], intersections[10]);
        // }
    }

    private void Awake() {
        groundPlane = GameObject.Find("GroundPlane");

        _north = gridHeight / 2;
        _east = gridWidth / 2;
        _south = -_north;
        _west = -_east;
       

        cardinals = new List<GameObject>(Resources.LoadAll<GameObject>("Cardinals"));
        
        destinations = new List<Vector3>();

        // selectableCoords = new List<Vector3>();
        intersections = new List<Vector3>();
        // tmpIntersections = new List<Vector3>(); // for testing TriangulateIntersection()
        drawnPath = new List<Vector3>();
        shortenedList = new List<Vector3>();
        slicedPath = new List<Vector3>();
        distances = new List<int>();
        lcms = new List<int>();
        midPoints = new List<Vector3>();
        mixedColors = new List<Color>();
        goalColour = new Color();

        randVariance.x = randomVariance;
        randVariance.y = 1f - randomVariance;

        gypMaterial = Resources.Load<Material>("Materials/DryWall_Mat"); // added for testing
        obsMaterial = Resources.Load<Material>("Materials/OBS_Mat"); // added for testing
        purpleMaterial = Resources.Load<Material>("Materials/Matt_Purple_Mat"); // added for testing
        pinkMaterial = Resources.Load<Material>("Materials/Matt_Pink_Mat"); // added for testing
        blueMaterial = Resources.Load<Material>("Materials/Plastic_Blue_Mat"); // added for testing
        // greenMaterial = Resources.Load<Material>("Materials/Matt_Green_Mat"); // added for testing

        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        GameUtils.Spawn(cardinals[0], new Vector3(_west, 0f, _north)); // NorthWest corner
        GameUtils.Spawn(cardinals[1], new Vector3(_east, 0f, _north)); // NorthEast corner
        GameUtils.Spawn(cardinals[2], new Vector3(_east, 0f, _south)); // SouthEast corner
        GameUtils.Spawn(cardinals[3], new Vector3(_west, 0f, _south)); // SouthWest corner


        goalObject.GetComponentInChildren<Renderer>().sharedMaterial.color = goalColour;

        /* END Awake() */
    }

    private void Start() {
        // Debug.Log("Trying to colour the goalBlip");

        MazeUI.PaintGoalBlip(goalColour);

        ResetCount();

        /* populate the destinations List with random positions for the player, waypoints & goal */
        while (count < waypoints.Count + 2) {
            destinations.Add(GameUtils.RandomPosition(_west, _east, _south, _north));
            count++;
        }

        ResetCount();

        /* populate distance & midpoint Lists */
        while (count < destinations.Count - 1) {
            distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[count], destinations[count + 1])));
            midPoints.Add(Vector3.Lerp(destinations[count], destinations[count + 1], 0.5f));
            count++;
        }

        goalColour = new Color(1, 1, 1);

        ResetCount();

        while (count < midPoints.Count) {
            lcms.Add(GameUtils.FindLcm(midPoints[count], distances[count], _north, _east));
            count++;
        }
        
        ResetCount();

        /* populate intersections list using destinations & lcms as seed values for triangulated positions */
        while (count < lcms.Count) {
            intersections.Add(destinations[count]);
            intersections.Add(GameUtils.TriangulateIntersection(destinations[count], destinations[count + 1],
                Random.Range(randVariance.x, randVariance.y), lcms[count]));
            count++;
        }
        
        /* then add the last destination which is for the goal */
        intersections.Add(destinations[count]);


        ResetCount();
        while (count < intersections.Count) {
            GameUtils.Spawn(floorTile01, intersections[count]);
            count++;
        }

        ResetCount();
        while (count < destinations.Count) {
            GameUtils.Spawn(floorTile02, destinations[count]);
            count++;
        }

        /* TODO: maybe "shorten" intersections list to remove possible duplicate entries */

        /* testing simpler path plotting function */
        // GameUtils.PlotHorizontalPath(intersections[count], intersections[count + 1], drawnPath);
        // GameUtils.PlotVerticalPath(intersections[count], intersections[count + 1], drawnPath);
        
        /* plot the paths between the intersections */
        ResetCount();
        while (count < intersections.Count) {
            if (count + 1 < intersections.Count) {
                GameUtils.PlotHorizontalPath(intersections[count], intersections[count + 1], drawnPath);
                GameUtils.PlotVerticalPath(intersections[count], intersections[count + 1], drawnPath);
            }
            else {
                GameUtils.PlotHorizontalPath(intersections[count], intersections[count], drawnPath);
                GameUtils.PlotVerticalPath(intersections[count], intersections[count], drawnPath);
            }
            count++;
        }
        
        // shortenedList = RemoveDuplicates(drawnPath);
        // GameUtils.WaitForListShortening();
        // drawnPath.Clear();
        // foreach (Vector3 step in shortenedList) {
        //     drawnPath.Add(step);
        // }
        //
        // shortenedList.Clear();

        if (slicedPath.Count > 0) {
            slicedPath.Clear();
        }

        rowNumber = Mathf.RoundToInt(drawnPath[0].z) < _north ? Mathf.RoundToInt(drawnPath[0].z) : _north;
        
        
        /* find the first row of the maze grid */
        slicedPath = GameUtils.SortAndSliceRows(drawnPath, _north);
        GameUtils.SpawnEastWestWalls(slicedPath, wallPanels, gypMaterial);
        
        /* find the value of the second row */
        // if (slicedPath.Count > 0) {
        //     rowNumber = Mathf.RoundToInt(slicedPath[0].z) - 1;
        // }
        
        /* spawn the east/west walls */
        // while (rowNumber >= _south) {
        //     // while (rowNumber >= _north) {
        //     slicedPath = GameUtils.SortAndSliceRows(drawnPath, rowNumber);
        //     GameUtils.SpawnEastWestWalls(slicedPath, wallPanels, gypMaterial);
        //     rowNumber--;
        // }
        
        /* delete the contents of slicedPath to eliminate junk data in the next step */
        // slicedPath.Clear();


        ResetCount();
        /* spawn the floor tiles for the maze path */
        while (count < drawnPath.Count) {
            GameUtils.Spawn(floorTile02, drawnPath[count]);
            count++;
        }
        
        // ResetCount();
        // while (count < slicedPath.Count) {
            // GameUtils.SpawnEastWestWalls(slicedPath, wallPanels, gypMaterial);
        // }
        
        /* reposition the player, spawn the waypoints and goal */
        player.transform.position = destinations[0];
        for (count = 1; count <= destinations.Count - 2; count++) {
            GameUtils.Spawn(waypoints[count - 1], destinations[count]);
        }
        // GameUtils.Spawn(pyramid, destinations[^1]);
        GameUtils.Spawn(goalObject, destinations[^1]);
        
        /* END Start() */
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        
        
        if (goalFound) {
            EndLevel();
        }

        /* END FixedUpdate() */
    }


    /** Utility functions for the GameManager **/
    private void ResetCount() {
        count = 0;
    }

    private static List<Vector3> RemoveDuplicates(List<Vector3> list) {
        var inputList = new HashSet<Vector3>(list);
        var shortened = inputList.ToList();
        shortListed = true;
        return shortened;
    }

    
    public void StartGame() {
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
    }

    /*public void StartGame(int difficulty) {
        modifier /= difficulty;
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
    }*/

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver() {
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
    }

    public void EndLevel() {
        Debug.Log("display winText");
        winText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }


}