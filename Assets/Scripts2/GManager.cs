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
    private GameObject groundPlane;
    private List<GameObject> cardinals;
    private int count;
    private int halfHeight;
    private int halfWidth;
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
    private List<int> distances;
    private List<int> lcms;
    public static Vector2 randVariance;
    private Vector3 center;
    
    private void Awake() {
        
        groundPlane = GameObject.Find("GroundPlane");
        halfHeight = gridHeight / 2;
        halfWidth = gridWidth / 2;
        xMin = -halfWidth;
        yMin = -halfHeight;
        center = new Vector3(0, 0, 0);
        
        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        cardinals = new List<GameObject>(Resources.LoadAll<GameObject>("Cardinals"));
        // Spawn.Object(cardinals[0], new Vector3(xMin, 0f, halfHeight)); // Top Left corner
        // Spawn.Object(cardinals[1], new Vector3(halfWidth, 0f, halfHeight)); // Top Right corner
        // Spawn.Object(cardinals[2], new Vector3(halfWidth, 0f, yMin)); // Bottom Right corner
        // Spawn.Object(cardinals[3], new Vector3(xMin, 0f, yMin)); // Bottom Left corner
        
        _selectableCoords = new List<Vector3>();
        _intersections = new List<Vector3>();
        
        
        _selectableCoords.Add(new Vector3(0, 0, 0)); // grid/maze center
        _selectableCoords.Add(new Vector3(xMin, 0f, halfHeight)); // Top Left cardinal/corner
        _selectableCoords.Add(new Vector3(halfWidth, 0f, halfHeight)); // Top Right cardinal/corner
        _selectableCoords.Add(new Vector3(halfWidth, 0f, yMin)); // Bottom Right cardinal/corner
        _selectableCoords.Add(new Vector3(xMin, 0f, yMin)); // Bottom Left cardinal/corner
        _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // player position
        _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight)); // goal position
        for (int i = 0; i < waypoints.Count; i++) {
            _selectableCoords.Add(GUtils.RandomPosition(xMin, halfWidth, yMin, halfHeight));
        }
        
        
        /** END AWAKE **/
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
