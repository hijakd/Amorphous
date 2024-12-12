using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local
// ReSharper disable Unity.RedundantEventFunction

public class GameManager : MonoBehaviour {

    public GameObject goalObject;
    public GameObject player;
    public GameObject floorTile01;
    public GameObject floorTile02;
    public GameObject[] wallPanels; // _N_ever _E_at _S_oggy _W_eetbix
    private GameObject groundPlane;
    public int gridHeight;
    public int gridWidth;
    public List<GameObject> waypoints;
    public TextMeshProUGUI winText, gameOverText, clockText, goalColourText;
    public Button goalColourButton;
    public Image goalColourPatch;
    public Texture boxTexture;
    public bool isGameActive;
    public static bool goalFound;
    public Button restartButton;
    public GameObject titleScreen;
    [Range(0.24f, 0.76f)] public float randomVariance = 0.42f;
    public static Vector2Int xMinMax, yMinMax;
    public static List<Vector3> selectableCoords;
    public static Vector2 randVariance;
    
    private Color goalColor;
    private ColorBlock goalColorBlock;
    private float xVal, zVal;
    private int count, rowNumber, columnNumber, gridNorth, gridEast, gridSouth, gridWest;
    private Vector3 goalPosition, pyramidPos, pyramidPos02, spawnPosition;
    
    private List<Color> mixedColors;
    private List<GameObject> cardinals;
    private List<int> distances, lcms;
    private List<Vector3> intersections, drawnPath, shortenedList, slicedPath, destinations, midPoints;
    
    private Material obsMaterial; // added for testing
    private Material gypMaterial; // added for testing
    private Material purpleMaterial; // added for testing
    private Material pinkMaterial; // added for testing
    private Material blueMaterial; // added for testing
    private Material greenMaterial; // added for testing
    private Material goalMaterial;
    private MazeCell[] maze;

    public static bool firstRowFound = false;
    public static bool firstColFound = false;


    private void OnDrawGizmos() {
        // visualize the corners of the grid in the Editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f)); // NorthWest corner
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f)); // NorthEast corner
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f)); // SouthEast corner
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f)); // SouthWest corner

        // visualize paths between 'intersections'
        // Gizmos.color = Color.blue;
        // for (int i = 0; i < intersections.Count - 1; i++) {
        //     Gizmos.DrawLine(intersections[i], intersections[i + 1]);
        // }

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
        
        yMinMax.y = gridNorth = gridHeight / 2;
        xMinMax.y = gridEast = gridWidth / 2;
        yMinMax.x = gridSouth = -gridNorth;
        xMinMax.x = gridWest = -gridEast;

        cardinals = new List<GameObject>(Resources.LoadAll<GameObject>("Cardinals"));
        selectableCoords = new List<Vector3>();
        intersections = new List<Vector3>();
        drawnPath = new List<Vector3>();
        shortenedList = new List<Vector3>();
        slicedPath = new List<Vector3>();
        distances = new List<int>();
        lcms = new List<int>();
        destinations = new List<Vector3>();
        midPoints = new List<Vector3>();
        mixedColors = new List<Color>();

        randVariance.x = randomVariance;
        randVariance.y = 1f - randomVariance;

        obsMaterial = Resources.Load<Material>("Materials/OBS_Mat"); // added for testing
        gypMaterial = Resources.Load<Material>("Materials/DryWall_Mat"); // added for testing
        purpleMaterial = Resources.Load<Material>("Materials/Matt_Purple_Mat"); // added for testing
        pinkMaterial = Resources.Load<Material>("Materials/Matt_Pink_Mat"); // added for testing
        blueMaterial = Resources.Load<Material>("Materials/Plastic_Blue_Mat"); // added for testing
        greenMaterial = Resources.Load<Material>("Materials/Matt_Green_Mat"); // added for testing

        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        SpawnObject.Spawn(cardinals[0], new Vector3(gridWest, 0f, gridNorth)); // NorthWest corner
        SpawnObject.Spawn(cardinals[1], new Vector3(gridEast, 0f, gridNorth)); // NorthEast corner
        SpawnObject.Spawn(cardinals[2], new Vector3(gridEast, 0f, gridSouth)); // SouthEast corner
        SpawnObject.Spawn(cardinals[3], new Vector3(gridWest, 0f, gridSouth)); // SouthWest corner

        goalPosition = RandomPosition.Position(xMinMax, yMinMax);
        spawnPosition = RandomPosition.Position(xMinMax, yMinMax);
        for (int i = 0; i < waypoints.Count; i++) {
            destinations.Add(RandomPosition.Position(xMinMax, yMinMax));
        }

        /* TODO: refactor into a function to accomodate a variable number "endpoints/destinations" based on the [].Count of destinations[] */
        /* this is used for 'random' vector triangulation */
        selectableCoords.Add(new Vector3(0, 0, 0));
        selectableCoords.Add(cardinals[0].transform.position);
        selectableCoords.Add(cardinals[1].transform.position);
        selectableCoords.Add(cardinals[2].transform.position);
        selectableCoords.Add(cardinals[3].transform.position);
        selectableCoords.Add(spawnPosition);
        selectableCoords.Add(goalPosition);
        selectableCoords.Add(destinations[0]);
        selectableCoords.Add(destinations[1]);
        selectableCoords.Add(destinations[2]);
        selectableCoords.Add(destinations[3]);

        /* TODO: refactor into a function that uses appropriate selectableCoords[] elements */
        distances.Add(Mathf.RoundToInt(Vector3.Distance(spawnPosition, destinations[0])));
        distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[0], destinations[1])));
        distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[1], destinations[2])));
        distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[2], destinations[3])));
        distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[3], goalPosition)));
        /* TODO: refactor into a function that uses appropriate selectableCoords[] elements */
        midPoints.Add(Vector3.Lerp(spawnPosition, destinations[0], 0.5f));
        midPoints.Add(Vector3.Lerp(destinations[0], destinations[1], 0.5f));
        midPoints.Add(Vector3.Lerp(destinations[1], destinations[2], 0.5f));
        midPoints.Add(Vector3.Lerp(destinations[2], destinations[3], 0.5f));
        midPoints.Add(Vector3.Lerp(destinations[3], goalPosition, 0.5f));

        // goalColor = new Color(0, 0, 0);
        goalColor = new Color(1, 1, 1);
        
        /** TODO: create a function/method that can 'select' a colour from a combination of two or more objects colour,
         * this could be a single-pass or multi-pass 'action' of Add or Blend, or both in the case of multi-pass.
         * A single-pass action for either Add or Blend has 6 possible results each
         * 0-1,0-2,0-3,1-2,1-3,2-3
         * A two-pass action for either Add or Blend adds another 20 possible results each, totalling 26 possibilities
         * 0-1,0-2,0-3,0-4,0-5,0-6,
         * 1-2,1-3,1-4,1-5,1-6,
         * 2-3,2-4,2-5,2-6,
         * 3-4,3-5,3-6,
         * 4-5,4-6,
         * 5-6
         */

        
        goalColor = BlendColours(waypoints, true);

        // goalColor = (waypoints[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color + waypoints[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color);
        // goalColor = Color.Lerp(tmp03, tmp04, 0.5f);
        // goalColor = Color.Lerp(goalColor, waypoints[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color, 0.5f);
        // goalColor = Color.Lerp(goalColor, waypoints[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color, 0.5f);
        // goalColor = mixedColors[0];

        goalObject.GetComponentInChildren<Renderer>().sharedMaterial.color = goalColor;
        
    }

    void Start() {
        
        MazeUI.PaintBlip(goalColor);
        
        groundPlane.gameObject.SetActive(false);

        /** TODO: possibly add an array of alike materials to randomly select from for use on the walls **/

        // SpawnObject.Spawn(player, spawnPosition);
        player.transform.position = spawnPosition;
        SpawnObject.Spawn(goalObject, goalPosition);
        SpawnObject.Spawn(waypoints[0], destinations[0]);
        SpawnObject.Spawn(waypoints[1], destinations[1]);
        SpawnObject.Spawn(waypoints[2], destinations[2]);
        SpawnObject.Spawn(waypoints[3], destinations[3]);


        for (int i = 0; i < midPoints.Count; i++) {
            lcms.Add(FindLcms(midPoints[i], distances[i]));
        }

        ResetCount();
        int maxRange = Mathf.RoundToInt(selectableCoords.Count);
        intersections.Add(spawnPosition);
        Triangulate(lcms[0], maxRange);
        intersections.Add(destinations[0]);
        Triangulate(lcms[1], maxRange);
        intersections.Add(destinations[1]);
        Triangulate(lcms[2], maxRange);
        intersections.Add(destinations[2]);
        Triangulate(lcms[3], maxRange);
        intersections.Add(destinations[3]);
        Triangulate(lcms[4], maxRange);
        intersections.Add(goalPosition);

        /* remove duplicate values from intersections */
        shortenedList = new List<Vector3>(ShortenList(intersections));
        intersections.Clear();
        for (int i = 0; i < shortenedList.Count; i++) {
            intersections.Add(shortenedList[i]);
        }

        /* delete contents of shortenedList to save unnecessary use of memory */
        shortenedList.Clear();

        ResetCount();
        while (count < intersections.Count) {
            SpawnObject.Spawn(floorTile01, intersections[count]);
            count++;
        }

        /*
        // pyramidPos = TriangulateV.Position(spawnPosition, destination01, xMinMax, yMinMax);
        // SpawnObject.Spawn(otherPiece, pyramidPos);


        // currently 'pyramids' are used for testing/visualizing positions
        // pyramidPos = Vector3.Min(shortenedLegOneIntersections[0], shortenedLegOneIntersections[1]);
        // pyramidPos02 = Vector3.Max(shortenedLegOneIntersections[0], shortenedLegOneIntersections[1]);
        // Debug.Log("printing 'Min' of pos1 & pos2: " + pyramidPos);
        // Debug.Log("printing 'Max' of pos1 & pos2: " + pyramidPos02);

        // SpawnObject.Spawn(otherPiece, pyramidPos);
        // SpawnObject.Spawn(otherPiece, pyramidPos02);
        */


        /* "decide" which way the path needs to be drawn from one intersection to the next, saving data in drawnPath[] */
        ResetCount();
        while (count < intersections.Count) {
            // Debug.Log("\nCount equals: " + count + "\n" + "Array length is: " + shortenedLegOneIntersections.Count);
            // Debug.Log("\nArray index a: " + count + "\n" + "Array index b: " + (count + 1));
            if ((count + 1) < intersections.Count) {
                CheckOrientation.CheckHorizontal(intersections[count],
                    intersections[count + 1], drawnPath);
                CheckOrientation.CheckVertical(drawnPath[^1], intersections[count + 1],
                    drawnPath);
            }
            else {
                CheckOrientation.CheckHorizontal(intersections[count],
                    intersections[count], drawnPath);
                CheckOrientation.CheckVertical(drawnPath[^1], intersections[count],
                    drawnPath);
            }

            count++;
        }

        /* remove duplicate values from drawnPath */
        shortenedList = new List<Vector3>(ShortenList(drawnPath));
        drawnPath.Clear();
        for (int i = 0; i < shortenedList.Count; i++) {
            drawnPath.Add(shortenedList[i]);
        }

        /* delete contents of shortenedList to save unnecessary use of memory */
        shortenedList.Clear();

        /* spawn floor tiles */
        ResetCount();
        while (count < drawnPath.Count) {
            SpawnObject.Spawn(floorTile02, drawnPath[count]);
            count++;
        }

        /* find the first row of the maze grid */
        slicedPath = SliceNSort.SliceRows(drawnPath, gridNorth, gridWest); // height gives the number of rows
        /* find the value of the first row */
        GameUtils.WaitForRowSlice();
        // rowNumber = FindFirstRow(slicedPath);
        rowNumber = Mathf.RoundToInt(slicedPath[0].z);
        
        /* Spawn the east/west walls */
        while (rowNumber >= gridSouth) {
            // Debug.Log("looping rowNumber == " + rowNumber);
            slicedPath = SliceNSort.SliceRows(drawnPath, rowNumber, gridWest);

            // SpawnEastWestWalls(slicedPath);
            SpawnObject.SpawnEastWestWalls(slicedPath, wallPanels, gypMaterial);
            slicedPath.Clear();
            rowNumber--;
        }

        /* delete contents of slicedPath to eliminate junk data in the next step */
        slicedPath.Clear();

        /* find the first column of the maze grid */
        slicedPath = SliceNSort.SliceColumns(drawnPath, gridEast, gridSouth); // width gives the number of columns
        /* find the value of the first column */
        GameUtils.WaitForColSlice();
        // columnNumber = FindFirstColumn(slicedPath);
        columnNumber = Mathf.RoundToInt(slicedPath[0].x);

        /* Spawn the north/south walls */
        while (columnNumber >= gridWest) {
            // Debug.Log("looping columnNumber == " + columnNumber);
            slicedPath = SliceNSort.SliceColumns(drawnPath, columnNumber, gridSouth);

            // SpawnNorthSouthWalls(slicedPath);
            SpawnObject.SpawnNorthSouthWalls(slicedPath, wallPanels, gypMaterial);
            slicedPath.Clear();
            columnNumber--;
        }

        // ShaderColourBlending.ResetWhite();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (goalFound) {
            EndLevel();
        }
        


        // clockText.text = DateTime.Now.ToString("HH:mm:ss");
        // goalColor = mixedColors[0];
        // goalColourText.text = goalColor.ToString();
        // goalColourText.gameObject.GetComponent<TMP_Text>().color = goalColor;
        // goalColourButton.gameObject.GetComponent<Renderer>().material.color = goalColor;
        // goalColorBlock = goalColourButton.colors;
        // goalColorBlock.normalColor = goalColor;
        // goalColourButton.colors = goalColorBlock;
    }


    /** Utility functions for the GameManager **/
    /* find/return the value of the first row of the maze path */
    // private int FindFirstRow(List<Vector3> list) {
    //     // Debug.Log("Finding first row");
    //     // int row = Mathf.RoundToInt(list[0].z);
    //     // return row;
    //     
    //     var tmp = new List<int>();
    //     
    //     foreach (Vector3 listZ in list) {
    //         tmp.Add(Mathf.RoundToInt(listZ.z));
    //     }
    //     // Debug.Log("Find1stR early: " + tmp[0]);
    //     tmp.Sort();
    //     // Debug.Log("Find1stR late: " + tmp[0]);
    //     return tmp[0];
    // }

    /* find/return the value of the first column of the maze path */
    // private int FindFirstColumn(List<Vector3> list) {
    //     // int column = Mathf.RoundToInt(list[0].x);
    //     // return column;
    //     
    //     var tmp = new List<int>();
    //     
    //     foreach (Vector3 listX in list) {
    //         tmp.Add(Mathf.RoundToInt(listX.x));
    //     }
    //     // Debug.Log("Find1stC early: " + tmp[0]);
    //     tmp.Sort();
    //     // Debug.Log("Find1stC late: " + tmp[0]);
    //     return tmp[0];
    // }

    private Color BlendColours(List<GameObject> objects, bool addOrBlend) {
        Color outputColor;
        int choice;

        if (addOrBlend) {
            choice = Random.Range(0, 6);
            outputColor = choice switch {
                0 => ColourChanger.Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //0
                ,
                1 => ColourChanger.Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //1
                ,
                2 => ColourChanger.Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //2
                ,
                3 => ColourChanger.Add(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //3
                ,
                4 => ColourChanger.Add(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //4
                ,
                5 => ColourChanger.Add(objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //5
                ,
                _ => Color.black
            };
        }
        else {
            choice = Random.Range(6, 12);
            outputColor = choice switch {
                6 => ColourChanger.Blend(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //6
                ,
                7 => ColourChanger.Blend(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //7
                ,
                8 => ColourChanger.Blend(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //8
                ,
                9 => ColourChanger.Blend(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //9
                ,
                10 => ColourChanger.Blend(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //10
                ,
                11 => ColourChanger.Blend(objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                    objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color) //11
                ,
                _ => Color.black
            };
        }
        return outputColor;
    }


    private void ResetCount() {
            count = 0;
        }

    /* Find an approximate centre between two points */
    private void Triangulate(int lcm, int rangeMax) {
        Debug.Log("Triangulate, rangeMax is: " + rangeMax);
        while (count < lcm) {
            int select01 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            // int select01 = Mathf.RoundToInt(Random.Range(0, rangeMax));
            int select02 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            // int select02 = Mathf.RoundToInt(Random.Range(0, rangeMax));
            Vector3 coord01 = selectableCoords[select01];
            Vector3 coord02 = selectableCoords[select02];
            intersections.Add(
                Triangulation.Position(coord01, coord02, Random.Range(randVariance.x, randVariance.y)));
            count++;
        }
    }

    /** LCM's will be used to determine the number of intersections between two points **/
    private int FindLcms(Vector3 midPoint, int distance) {
        var farthestCorner = FindFarCorner.Find(midPoint, gridNorth, gridEast);
        int farCornerDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farthestCorner));
        int dist2 = distance + farCornerDistance;
        int lcm01 = ReduceLcm(LCM_GCD.Lcm(distance, farCornerDistance));
        int lcm02 = ReduceLcm(LCM_GCD.Lcm(distance, dist2));
        int lcm03 = ReduceLcm(LCM_GCD.Lcm(farCornerDistance, dist2));

        // return combinedLCM = lcm01 + lcm02 + lcm03;
        return lcm01 + lcm02 + lcm03;
    }

    /* reducing the value of LCM to ensure there is a reasonably usable number */
    private int ReduceLcm(int lcm) {
        int tmpLcm = lcm;
        while (tmpLcm >= gridEast) {
            tmpLcm = tmpLcm / gridEast;
        }

        return tmpLcm;
    }

    /* remove duplicate Vector3's from a List<Vector3> */
    private List<Vector3> ShortenList(List<Vector3> pathList) {
        var tmpList = new HashSet<Vector3>(pathList);
        List<Vector3> shortList = tmpList.ToList();
        return shortList;
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

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    // private void OnGUI() {
    //     GUI.Box(new Rect(252, 42, 42, 42), boxTexture);
    // }
}