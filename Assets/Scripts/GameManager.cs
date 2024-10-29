using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public GameObject goalObject;
    public GameObject player;
    public GameObject floorTile;
    public GameObject floorTile2;
    public GameObject mazeCell;
    public GameObject[] wallPanels; // _N_ever _E_at _S_oggy _W_eetbix
    public int gridHeight;
    public int gridWidth;
    public List<GameObject> waypoints;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI gameOverText;
    public bool isGameActive;
    public Button restartButton;
    public GameObject titleScreen;
    [UnityEngine.Range(0.24f, 0.76f)] public float randomVariance = 0.42f;

    [SerializeField] private List<GameObject> cardinals;
    [SerializeField] private List<Vector3> selectableCoords;
    [SerializeField] private List<Vector3> intersections;
    [SerializeField] private List<Vector3> drawnPath;
    [SerializeField] private List<Vector3> drawnWalls;
    [SerializeField] private List<Vector3> horizontalWalls;
    [SerializeField] private List<Vector3> horizontalWalls2;
    [SerializeField] private List<Vector3> verticalWalls;
    [SerializeField] private List<Vector3> shortenedList;
    [SerializeField] private List<Vector3> slicedPath;

    private int halfHeight;
    private int halfWidth;
    private int count;
    [SerializeField] private List<int> distances;
    private int distance02;
    private int farCornerDistance;
    private int lcm01;
    private int lcm02;
    private int lcm03;
    private int combinedLCM;
    int rowNumber;
    int columnNumber;
    float xVal;
    float zVal;
    [SerializeField] private List<int> lcms;
    private Vector3 spawnPosition;
    private Vector3 goalPosition;
    [SerializeField] private List<Vector3> destinations;
    [SerializeField] private List<Vector3> midPoints;
    private Vector3 pyramidPos;
    private Vector3 pyramidPos02;
    private Vector3 farthestCorner;
    public static Vector2 xMinMax;
    public static Vector2 yMinMax;
    private Vector2 randVariance;
    MazeCell[] maze;
    Material obsMaterial;
    Material gypMaterial;
    Material purpleMaterial;
    Material pinkMaterial;
    Material blueMaterial;
    Material greenMaterial;


    private void OnDrawGizmos() {
        // visualize the corners of the grid in the Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));

        // visualize paths between 'intersections'
        // Gizmos.color = Color.blue;
        // for (int i = 0; i < intersections.Count - 1; i++) {
        //     Gizmos.DrawLine(intersections[i], intersections[i + 1]);
        // }

        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(spawnPosition, pyramidPos);
        // Gizmos.DrawLine(pyramidPos, destination01);
    }

    private void Awake() {
        halfHeight = gridHeight / 2;
        halfWidth = gridWidth / 2;
        xMinMax.x = -halfWidth;
        xMinMax.y = halfWidth;
        yMinMax.x = -halfHeight;
        yMinMax.y = halfHeight;

        randVariance.x = randomVariance;
        randVariance.y = 1f - randomVariance;

        obsMaterial = Resources.Load<Material>("Materials/OBS_Mat");
        gypMaterial = Resources.Load<Material>("Materials/DryWall_Mat");
        purpleMaterial = Resources.Load<Material>("Materials/Purple_Mat");
        pinkMaterial = Resources.Load<Material>("Materials/Pink_Mat");
        blueMaterial = Resources.Load<Material>("Materials/Plastic_Blue_Mat");
        greenMaterial = Resources.Load<Material>("Materials/Green_Mat");

        // obsMaterial = GetComponent<Renderer>().material;


        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        SpawnObject.Spawn(cardinals[0], new Vector3(xMinMax.x, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[1], new Vector3(xMinMax.y, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[2], new Vector3(xMinMax.y, 0f, yMinMax.x));
        SpawnObject.Spawn(cardinals[3], new Vector3(xMinMax.x, 0f, yMinMax.x));

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
    }

    void Start() {
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
        intersections.Add(spawnPosition);
        Triangulate(lcms[0]);
        intersections.Add(destinations[0]);
        Triangulate(lcms[1]);
        intersections.Add(destinations[1]);
        Triangulate(lcms[2]);
        intersections.Add(destinations[2]);
        Triangulate(lcms[3]);
        intersections.Add(destinations[3]);
        Triangulate(lcms[4]);
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
            SpawnObject.Spawn(floorTile, intersections[count]);
            count++;
        }

        /*
        // pyramidPos = TriangulateV.Position(spawnPosition, destination01, xMinMax, yMinMax);
        // SpawnObject.Spawn(otherPiece, pyramidPos);


        // currently 'pyramids' are used for testing/visualising positions
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
                CheckOrientation.CheckVertical(drawnPath[drawnPath.Count - 1], intersections[count + 1],
                    drawnPath);
            }
            else {
                CheckOrientation.CheckHorizontal(intersections[count],
                    intersections[count], drawnPath);
                CheckOrientation.CheckVertical(drawnPath[drawnPath.Count - 1], intersections[count],
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

        /* ... */

        /* ... */

        /* spawn floor tiles */
        ResetCount();
        while (count < drawnPath.Count) {
            SpawnObject.Spawn(floorTile2, drawnPath[count]);

            // SpawnObject.Spawn(mazeCell, drawnPath[count]);
            count++;
        }

        // ResetCount();
        // while (count < drawnPath.Count) {
        //     maze = new MazeCell[Mathf.RoundToInt(drawnPath[count].x), Mathf.RoundToInt(drawnPath[count].z)];
        //     count++;
        // }


        /* currently this checks only the first row of the maze grid */
        /* will need to use this in a loop to process the entire grid */
        slicedPath = SliceListRows(drawnPath, halfHeight); // height gives the rows

        rowNumber = FindFirstRow(slicedPath);
        
        // Debug.Log("rowNumber == " + rowNumber + 
        //           "\ncolumnNumber == " + columnNumber);

        /* Spawn the "north" walls of the first row */
        /* currently this will only spawn for the first grid row */
        for (int i = 0; i < slicedPath.Count; i++) {
            SpawnObject.Spawn(wallPanels[0], slicedPath[i], gypMaterial);
        }

        SpawnEastWestWalls(slicedPath);

        rowNumber--;


        while (rowNumber >= -halfHeight) {
            Debug.Log("looping rowNumber == " + rowNumber);

            slicedPath = SliceListRows(drawnPath, rowNumber);
            SpawnEastWestWalls(slicedPath);
            slicedPath.Clear();
            rowNumber--;
        }
        
        slicedPath = SliceListColumns(drawnPath, halfWidth);
        columnNumber = FindFirstColumn(slicedPath);
        
        while (columnNumber >= -halfWidth) {
            Debug.Log("looping columnNumber == " + columnNumber);

            slicedPath = SliceListColumns(drawnPath, columnNumber);
            SpawnNorthSouthWalls(slicedPath);
            slicedPath.Clear();
            columnNumber--;
        }
        
    }

    // Update is called once per frame
    void Update() {
    }

    /* find/return the value of the first row of the maze path */
    private int FindFirstRow(List<Vector3> list) {
        // Debug.Log("Finding first row");
        int rowNumber = Mathf.RoundToInt(list[0].z);

        // Debug.Log("FFR rowNumber == " + rowNumber);
        return rowNumber;
    }

    /* find/return the value of the first column of the maze path */
    private int FindFirstColumn(List<Vector3> list) {
        int columnNumber = Mathf.RoundToInt(list[0].x);
        return columnNumber;
    }

    /* Spawn the east & west walls across a given row of the maze path */
    private void SpawnEastWestWalls(List<Vector3> path) {
        Debug.Log("Spawning East/West Walls");

        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first west wall of the row */
                SpawnObject.Spawn(wallPanels[3], path[i], gypMaterial);
            }
            else if (path[i - 1].x < path[i].x - 1) {
                /* spawn west wall at end of a break in the row */
                SpawnObject.Spawn(wallPanels[3], path[i], blueMaterial);
            }
            else if (i == path.Count - 1) {
                /* spawn the last east wall of the row if the list ends before the boundary */
                SpawnObject.Spawn(wallPanels[1], path[i], gypMaterial);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last east wall of the row */
                SpawnObject.Spawn(wallPanels[1], path[j], obsMaterial);
            }
            else if (path[j - 1].x > path[j].x + 1) {
                /* spawn east wall at end of a break in the row */
                SpawnObject.Spawn(wallPanels[1], path[j], greenMaterial);
            }
        }
    }

    private void SpawnNorthSouthWalls(List<Vector3> path) {
        Debug.Log("Spawning North/South Walls");
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first north wall of the row */
                SpawnObject.Spawn(wallPanels[0], path[i], gypMaterial);
            }
            else if (path[i - 1].z < path[i].z - 1) {
                /* spawn north wall at end of a break in the row */
                SpawnObject.Spawn(wallPanels[0], path[i], blueMaterial);
            }
            else if (i == path.Count - 1) {
                /* spawn the last south wall of the row if the list ends before the boundary */
                SpawnObject.Spawn(wallPanels[2], path[i], gypMaterial);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last south wall of the row */
                SpawnObject.Spawn(wallPanels[2], path[j], obsMaterial);
            }
            else if (path[j - 1].z > path[j].z + 1) {
                /* spawn south wall at end of a break in the row */
                SpawnObject.Spawn(wallPanels[2], path[j], greenMaterial);
            }
        }
    }

    private List<Vector3> SliceListRows(List<Vector3> path, int row) {
        int slicingCount = 0;
        List<Vector3> slice = new List<Vector3>();
        List<Vector3> sortedSlice = new List<Vector3>();

        Debug.Log("Slicing rows List");
        while (slicingCount < path.Count) {
            if (path[slicingCount].z == row) {
                slice.Add(path[slicingCount]);
            }

            slicingCount++;
        }

        sortedSlice = SortList(slice, Mathf.RoundToInt(xMinMax.x));
        return sortedSlice;
    }

    private List<Vector3> SliceListColumns(List<Vector3> path, int column) {
        int slicingCount = 0;
        List<Vector3> slice = new List<Vector3>();
        List<Vector3> sortedSlice = new List<Vector3>();

        Debug.Log("Slicing columns List");
        while (slicingCount < path.Count) {
            if (path[slicingCount].x == column) {
                slice.Add(path[slicingCount]);
            }

            slicingCount++;
        }

        sortedSlice = SortList(slice, Mathf.RoundToInt(yMinMax.x));

        return sortedSlice;
    }

    private List<Vector3> SortList(List<Vector3> list, int lowest) {
        List<Vector3> sorted = new List<Vector3>();
        // int lowest = Mathf.RoundToInt(xMinMax.x);
        int sortingCount = 0;
        int listSize = list.Count;

        Debug.Log("Sorting Sliced List");
        while (sortingCount < listSize) {
            for (int i = 0; i < listSize; i++) {
                if (list[i].x == lowest) {
                    sorted.Add(list[i]);
                    sortingCount++;
                }
            }

            lowest++;
        }

        return sorted;
    }

    private void ResetCount() {
        count = 0;
    }

    private void Triangulate(int lcm) {
        while (count < lcm) {
            int select01 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            int select02 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            Vector3 coord01 = selectableCoords[select01];
            Vector3 coord02 = selectableCoords[select02];
            intersections.Add(Triangulation.Position(coord01, coord02, Random.Range(randVariance.x, randVariance.y)));
            count++;
        }
    }

    /** LCM's will be used to determine the number of intersections between two points **/
    private int FindLcms(Vector3 midPoint, int distance) {
        farthestCorner = FindFarCorner.Find(midPoint, halfHeight, halfWidth);
        farCornerDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farthestCorner));
        distance02 = distance + farCornerDistance;
        lcm01 = ReduceLcm(LCM_GCD.Lcm(distance, farCornerDistance));
        lcm02 = ReduceLcm(LCM_GCD.Lcm(distance, distance02));
        lcm03 = ReduceLcm(LCM_GCD.Lcm(farCornerDistance, distance02));

        return combinedLCM = lcm01 + lcm02 + lcm03;
    }

    /* reducing the value of LCM to ensure there is a reasonably usable number */
    private int ReduceLcm(int lcm) {
        int tmpLcm = lcm;
        while (tmpLcm >= halfWidth) {
            tmpLcm = tmpLcm / halfWidth;
        }

        return tmpLcm;
    }

    /* remove duplicate Vector3's from a List<Vector3> */
    private List<Vector3> ShortenList(List<Vector3> pathList) {
        HashSet<Vector3> tmpList = new HashSet<Vector3>(pathList);
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

    public List<Vector3> scanForHorizontalBoundaries(List<Vector3> boundaries) {
        int xValueTemp = Mathf.RoundToInt(boundaries[0].x);
        int xValueCounter = 0;
        int i = 0;
        HashSet<Vector3> boundariesTemp = new HashSet<Vector3>();

        boundariesTemp.Add(boundaries[0]);

        while (xValueCounter < boundaries.Count) {
            while (boundaries[i].x == xValueTemp) {
                xValueCounter++;
                i++;
            }

            boundariesTemp.Add(boundaries[xValueCounter]);
            xValueTemp++;
        }

        List<Vector3> boundariesList = boundariesTemp.ToList();

        return boundariesList;
    }

}