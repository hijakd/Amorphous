using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ReSharper disable InvalidXmlDocComment

// ReSharper disable InconsistentNaming

public class GameManager : MonoBehaviour {

    public GameObject player;
    public GameObject goal;
    public GameObject floorTile;
    public GameObject[] wallPanels;
    public List<GameObject> waypoints;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public GameObject titleScreen;

    public int gridWidth;
    public int gridHeight;
    [Range(0.24f, 0.76f)] public float randomVariance = 0.42f;

    public Vector2 rVariance;
    
    

    private bool isGameActive, foundFirstRow, foundFirstColumn, goalFound, horizAligned, vertAligned, isForward, isRight, shortListed;
    [SerializeField] private Color goalColour;
    private GameObject groundPlane;
    private int count, gridNorth, gridEast, gridWest, gridSouth, xPosition, pathLength, rowNumber, columnNumber, horizDistance, vertDistance;
    private Material gypMaterial; // added for testing
    private int north = 0;
    private int east = 1;
    private int south = 2;
    private int west = 3;

    private List<GameObject> cardinals;
    [SerializeField] private List<int> distances, lcms;
    [SerializeField] private List<Vector3> destinations, drawnPath, intersections, midPoints, shortList, slicedPath;


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
        count = 0;
        gridNorth = gridHeight / 2;
        gridEast = gridWidth / 2;
        gridSouth = -gridNorth;
        gridWest = -gridEast;
        rVariance.x = randomVariance;
        rVariance.y = 1f - randomVariance;

        groundPlane = GameObject.Find("GroundPlane");
        cardinals = new List<GameObject>(Resources.LoadAll<GameObject>("Cardinals"));
        distances = new List<int>();
        lcms = new List<int>();
        destinations = new List<Vector3>();
        drawnPath = new List<Vector3>();
        intersections = new List<Vector3>();
        midPoints = new List<Vector3>();
        shortList = new List<Vector3>();
        slicedPath = new List<Vector3>();
        gypMaterial = Resources.Load<Material>("Materials/DryWall_Mat"); // added for testing

        cardinals[0].transform.position = new Vector3(gridEast, 0f, gridNorth);
        cardinals[1].transform.position = new Vector3(gridWest, 0f, gridNorth);
        cardinals[2].transform.position = new Vector3(gridWest, 0f, gridSouth);
        cardinals[3].transform.position = new Vector3(gridEast, 0f, gridSouth);

        while (count < waypoints.Count + 2) {
            destinations.Add(RandomPosition(gridEast, gridWest, gridSouth, gridNorth));
            count++;
        }

        ResetCount();

        while (count < destinations.Count - 1) {
            distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[count], destinations[count + 1])));
            midPoints.Add(Vector3.Lerp(destinations[count], destinations[count + 1], 0.5f));
            count++;
        }
        


        /* END Awake() */
    }

    void Start() {
        MazeUI.PaintGoalBlip(goalColour);

        // groundPlane.gameObject.SetActive(false);

        /* ensure the counter is zero */
        ResetCount();

        /* Set the player's position */
        player.transform.position = destinations[count];

        /* Spawn the waypoints */
        while (count < waypoints.Count) {
            Spawn(waypoints[count], destinations[count + 1]);
            count++;
        }

        /* Spawn the goal */
        Spawn(goal, destinations[++count]);
        Debug.Log("Goal spawned at: " + goal.transform.position);

        /* populate the list of LCMS for triangulating the path intersections */
        // for (int z = 0; z < midPoints.Count; z++) {
        //     Debug.Log("populate LCMS");
        //     lcms.Add(FindLcm(midPoints[z], distances[z]));
        // }

        ResetCount();

        while (count < midPoints.Count) {
            Debug.Log("populating LCMS");
            lcms.Add(FindLcm(midPoints[count], distances[count]));
            count++;
        }
        
        ResetCount();

        /* populate the list of maze 'intersections' */
        int rangeMax = Mathf.RoundToInt(destinations.Count);
        Debug.Log("count: " + count + "\nrangeMax: " + rangeMax);
        // while (count < destinations.Count - 1) {
        //     intersections.Add(destinations[count]);
        //     intersections.Add(Triangulate(lcms[count], rangeMax, rVariance, destinations));
        //     count++;
        // }

        // Debug.Log("LCMS count: " + lcms.Count);
        // for (int i = 0; i < destinations.Count - 1; i++) {
        //     intersections.Add(destinations[i]);
        //     intersections.Add(Triangulate(lcms[i], rangeMax, rVariance, destinations));
        // }

        /* add the goal to the intersections list */
        // intersections.Add(destinations[count]);

        // ResetCount();

        // while (count < intersections.Count) {
        //     if (count + 1 < intersections.Count) {
        //         CheckHorizontal(intersections[count], intersections[count + 1], drawnPath);
        //         CheckVertical(drawnPath[^1], intersections[count], drawnPath);
        //     }
        //     else {
        //         CheckHorizontal(intersections[count], intersections[count - 1], drawnPath);
        //         CheckVertical(drawnPath[^1], intersections[count], drawnPath);
        //     }
        //     count++;
        // }
        
        /* remove duplicate values from the drawnPath list */
        // shortList = ShortenList(drawnPath);
        // WaitForShortList();
        // drawnPath.Clear();
        // foreach (Vector3 path in shortList) {
        //     drawnPath.Add(path);
        // }

        /* delete contents of shortList to save unnecessary use of memory */
        // shortList.Clear();
        
        // ResetCount();

        /* Spawn the floor tiles */
        // while (count < drawnPath.Count) {
        //     Spawn(floorTile, drawnPath[count]);
        //     count++;
        // }
        
        /* Find the first row of the maze grid */
        // slicedPath = SliceRows(drawnPath, gridNorth, gridEast);
        /* find the value of the first row */
        // WaitForRowSlice();
        // rowNumber = slicedPath.Count > 0 ? FindFirstRow(slicedPath): gridNorth;
        
        /* Spawn the east/west walls */
        // while (rowNumber >= gridEast) {
        //     slicedPath = SliceRows(drawnPath, rowNumber, gridEast);
        //     SpawnEastWestWall(slicedPath, wallPanels, gypMaterial);
        //     /* clear slicedPath before parsing the next row */
        //     slicedPath.Clear();
        //     rowNumber--;
        // }
        /* delete contents of slicedPath to eliminate junk data in the next step */
        // slicedPath.Clear();

        /* find the first column of the maze grid */
        // slicedPath = SliceColumns(drawnPath, gridEast, gridSouth);
        /* find the value of the first column */
        // WaitForColumnSlice();
        // columnNumber = slicedPath.Count > 0 ? FindFirstColumn(slicedPath) : gridWest;
        
        /* Spawn the north/south walls */
        // while (columnNumber >= gridWest) {
            // slicedPath = SliceColumns(drawnPath, gridEast, gridSouth);
            // NorthSouthWalls(slicedPath, wallPanels, gypMaterial);
            // /* clear slicedPath before parsing the next column */
            // slicedPath.Clear();
            // columnNumber--;
        // }


        /* END Start() */
    }

    


    void FixedUpdate() {
        if (goalFound) {
            EndLevel();
        }
        /* END FixedUpdate() */
    }


    /** Utility functions **/
    private void ResetCount() {
        count = 0;
    }

    private Vector3 RandomPosition(int minWidth, int maxWidth, int minHeight, int maxHeight) {
        return new Vector3(Mathf.Round(Random.Range(minWidth, maxWidth)), 0,
            Mathf.Round(Random.Range(minHeight, maxHeight)));
    }

    /* Spawn a GameObject at a given position on the maze grid */
    private void Spawn(GameObject thing, Vector3 position) {
        thing.transform.position = position;
        Instantiate(thing);
    }

    private void Spawn(GameObject thing, Vector3 position, Quaternion rotation) {
        thing.transform.position = position;
        thing.transform.rotation = rotation;
        Instantiate(thing);
    }

    private void Spawn(GameObject thing, Vector3 position, Material material) {
        thing.transform.position = position;
        thing.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;
        Instantiate(thing);
    }

    /* find the corner farthest from any given point on the grid */
    private Vector3 FarthestCorner(Vector3 inputPos) {
        int xTmp, zTmp;
        if (inputPos.x >= 0) {
            xTmp = gridEast;
        }
        else {
            xTmp = gridWest;
        }

        if (inputPos.z >= 0) {
            zTmp = gridSouth;
        }
        else {
            zTmp = gridNorth;
        }

        return new Vector3(xTmp, 0, zTmp);
    }

    /* find the corner farthest from any given point on the grid */
    /*
    private Vector3 FarthestCorner(Vector3 inputPos, int mazeGridHeigth, int mazeGridWidth) {
        int xTmp, zTmp;

        if (inputPos.x >= 0) {
            xTmp = -mazeGridWidth;
        }
        else {
            xTmp = mazeGridWidth;
        }

        if (inputPos.z >= 0) {
            zTmp = -mazeGridHeigth;
        }
        else {
            zTmp = mazeGridWidth;
        }

        return new Vector3(xTmp, 0, zTmp);
    }
    */

    /* Calculate the Greatest Common Divisor/Denominator */
    private int GCD(int int01, int int02) {
        int tempA = int01;
        int tempB = int02;

        while (tempB > 0) {
            int temp = tempB;
            tempB = tempA % tempB;
            tempA = temp;
        }

        return tempA;
    }

    /* find the Lowest Common Multiple */
    private int LCM(int distance01, int distance02) {
        Debug.Log("start LCM");
        Debug.Log("LCM; int a: " + distance01 + " int b: " + distance02);
        int lcmTemp = distance01 / GCD(distance01, distance02);

        // int result = dist02 * lcmTemp;
        // return result;
        return distance02 * lcmTemp;
    }

    /* This function is used to pick values that will be used to generate points in the maze grid */
    private int FindLcm(Vector3 midpoint, int distance) {
        Vector3 farCorner = FarthestCorner(midpoint);
        int farthestDistance = Mathf.RoundToInt(Vector3.Distance(midpoint, farCorner));
        int thisDistance = distance + farthestDistance;
        int lcm01 = ReduceLCM(LCM(distance, farthestDistance));
        int lcm02 = ReduceLCM(LCM(distance, thisDistance));
        int lcm03 = ReduceLCM(LCM(farthestDistance, thisDistance));

        return lcm01 + lcm02 + lcm03;
    }

    /* ensures that an LCM value is valid for the maze grid */
    private int ReduceLCM(int lcm) {
        int tmplcm = lcm;
        while (tmplcm >= gridWest) {
            // tmplcm = tmplcm / GameManager.instance.halfWidth;
            tmplcm /= gridWest;
        }

        return tmplcm;
    }

    private Vector3 Triangulate(int lcm, int destinationsCount, Vector2 variance, List<Vector3> selectable) {
        int counter = 0;
        Vector3 lcmPos = new Vector3();
        while (counter < lcm) {
            Vector3 coord01 = selectable[Mathf.RoundToInt(Random.Range(0, destinationsCount))];
            Vector3 coord02 = selectable[Mathf.RoundToInt(Random.Range(0, destinationsCount))];
            lcmPos = ApproximateCenter(coord01, coord02, Random.Range(variance.x, variance.y));
            counter++;
        }

        return lcmPos;
    }

    /* create an approximately "center" position between two points using */
    /* Slerp that is clamped within the maze boundaries, this is intended */
    /* to take a Random.Range() value to create a point close to center */
    /* within approx. 40/60 weighting */
    private Vector3 ApproximateCenter(Vector3 position01, Vector3 position02,
        float centreMargin) {
        Vector3 position = Vector3.Slerp(position01, position02, centreMargin);
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);

        /* check position is within the boundaries, if not clamp it to the boundaries */
        if (position.x <= gridEast) {
            position.x = gridEast;
        }
        else if (position.x > gridWest) {
            position.x = gridWest;
        }

        if (position.z <= gridSouth) {
            position.z = gridSouth;
        }
        else if (position.z > gridNorth) {
            position.z = gridNorth;
        }

        return position;
    }


    private void CheckHorizontal(Vector3 position01, Vector3 position02, List<Vector3> pathToDraw) {
        // int pathLength = 0;
        // int xPosition;
        Vector3 tmpPosition = new Vector3(0, 0, 0);
        if (HorizontalAlignment(position01, position02)) {
            if (IsItRight(position01, position02)) {
                pathLength = GetHorizontalDistance(position01, position02);
                for (int i = 0; i <= pathLength; i++) {
                    pathToDraw.Add(new Vector3(position01.x + i, position01.y, position01.z));
                }
            }
            else {
                pathLength = GetHorizontalDistance(position01, position02);
                for (int y = 0; y <= pathLength; y++) {
                    pathToDraw.Add(new Vector3(position01.x - y, position01.y, position01.z));
                }
            }
        }
        else {
            if (position01.z < position02.z) {
                tmpPosition.x = position02.x;
                tmpPosition.z = position01.z;
                xPosition = 2;
            }
            else {
                tmpPosition.x = position01.x;
                tmpPosition.z = position02.z;
                xPosition = 1;
            }

            if (xPosition == 1) {
                if (HorizontalAlignment(tmpPosition, position02)) {
                    if (IsItRight(tmpPosition, position02)) {
                        pathLength = GetHorizontalDistance(tmpPosition, position02);
                        for (int j = 0; j <= pathLength; j++) {
                            pathToDraw.Add(new Vector3(tmpPosition.x + j, tmpPosition.y, tmpPosition.z));
                        }
                    }
                    else {
                        pathLength = GetHorizontalDistance(tmpPosition, position02);
                        for (int k = 0; k <= pathLength; k++) {
                            pathToDraw.Add(new Vector3(tmpPosition.x - k, tmpPosition.y, tmpPosition.z));
                        }
                    }
                }
            }

            if (xPosition == 2) {
                if (HorizontalAlignment(position01, tmpPosition)) {
                    if (IsItRight(position01, tmpPosition)) {
                        pathLength = GetHorizontalDistance(position01, tmpPosition);
                        for (int t = 0; t <= pathLength; t++) {
                            pathToDraw.Add(new Vector3(position01.x + t, tmpPosition.y, tmpPosition.z));
                        }
                    }
                    else {
                        pathLength = GetHorizontalDistance(position01, tmpPosition);
                        for (int w = 0; w <= pathLength; w++) {
                            pathToDraw.Add(new Vector3(position01.x - w, tmpPosition.y, tmpPosition.z));
                        }
                    }
                }
            }
        }
    }
    
    private void CheckVertical(Vector3 position01, Vector3 position02, List<Vector3> pathToDraw) {
        Vector3 tmpPosition = new Vector3(0, 0, 0);
        /* positions are within horizontal alignment margins */
        if (VerticalAlignment(position01, position02)) {
            // Debug.Log("Checking vertical alignment: " + VerticalAlignment(pos01, pos02));
            if (IsItForward(position01, position02)) {
                // Debug.Log("Check if is forward: " + IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                pathLength = GetVerticalDistance(position01, position02);
                for (count = 0; count <= pathLength; count++) {
                    pathToDraw.Add(new Vector3(position01.x, position01.y, position02.z + count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = GetVerticalDistance(position01, position02);
                // Debug.Log("Check if is forward: " + Triangulation.IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                for (count = 0; count <= pathLength; count++) {
                    pathToDraw.Add(new Vector3(position01.x, position01.y, position02.z - count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
        }

        if (position01.z < position02.z) {
            tmpPosition.x = position02.x;
            tmpPosition.z = position01.z;
            xPosition = 2;
        }
        else {
            tmpPosition.x = position01.x;
            tmpPosition.z = position02.z;
            xPosition = 1;
        }

        if (VerticalAlignment(position01, tmpPosition)) {
            // Debug.Log("Checking vertical alignment: " + Triangulation.VerticalAlignment(pos01, pos02));
            if (IsItForward(position01, tmpPosition)) {
                // Debug.Log("Check if is forward: " + Triangulation.IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                pathLength = GetVerticalDistance(position01, tmpPosition);
                for (count = 0; count <= pathLength; count++) {
                    pathToDraw.Add(new Vector3(tmpPosition.x, tmpPosition.y, position01.z + count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = GetVerticalDistance(position01, tmpPosition);
                for (count = 0; count <= pathLength; count++) {
                    pathToDraw.Add(new Vector3(tmpPosition.x, tmpPosition.y, position01.z - count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
        }
        
        if (VerticalAlignment(tmpPosition, position02)) {
            // Debug.Log("Checking vertical alignment: " + Triangulation.VerticalAlignment(pos01, pos02));
            if (IsItForward(tmpPosition, position02)) {
                // Debug.Log("Check if is forward: " + Triangulation.IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                pathLength = GetVerticalDistance(tmpPosition, position02);
                for (count = 0; count <= pathLength; count++) {
                    pathToDraw.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z + count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = GetVerticalDistance(tmpPosition, position02);
                for (count = 0; count <= pathLength; count++) {
                    pathToDraw.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z - count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
        }
    }
    
    /* are the 'Z' values aligned within margin */
    private bool HorizontalAlignment(Vector3 hPosition01, Vector3 hPosition02) {
        if (hPosition01.z == hPosition02.z || hPosition01.z == hPosition02.z - 1 ||
            hPosition01.z == hPosition02.z + 1) {
            horizAligned = true;
            // Debug.Log("Triangulating horizAligned: " + horizAligned);
        }

        return horizAligned;
    }

    /* are the 'X' values aligned within margin */
    private bool VerticalAlignment(Vector3 vPosition01, Vector3 vPosition02) {
        if (vPosition01.x == vPosition02.x || vPosition01.x == vPosition02.x - 1 ||
            vPosition01.x == vPosition02.x + 1) {
            vertAligned = true;
            // Debug.Log("Triangulating vertAligned: " + vertAligned);
        }

        return vertAligned;
    }
    
    private int GetHorizontalDistance(Vector3 position01, Vector3 position02) {
        if (horizAligned) {
            if (position01.x < position02.x) {
                horizDistance = Mathf.RoundToInt(position02.x - position01.x);
                // Debug.Log("Triangulating horizDistance: " + horizDistance);
                isRight = true;
                // Debug.Log("Triangulating right: " + isRight);
            }
            else if (position01.x > position02.x) {
                horizDistance = Mathf.RoundToInt(position01.x - position02.x);
                // Debug.Log("Triangulating horizDistance: " + horizDistance);
                isRight = false;
                // Debug.Log("Triangulating right: " + isRight);
            }
            else {
                horizDistance = 0;
            }
        }

        return horizDistance;
    }

    private int GetVerticalDistance(Vector3 position01, Vector3 position02) {
        if (vertAligned) {
            if (position01.z < position02.z) {
                vertDistance = Mathf.RoundToInt(position02.z - position01.z);
                // Debug.Log("Triangulating vertDistance: " + vertDistance);
                isForward = true;
                // Debug.Log("Triangulating forward: " + isForward);
            }
            else if (position01.z > position02.z) {
                vertDistance = Mathf.RoundToInt(position01.z - position02.z);
                // Debug.Log("Triangulating vertDistance: " + vertDistance);
                isForward = false;
                // Debug.Log("Triangulating forward: " + isForward);
            }
            else {
                vertDistance = 0;
            }
        }

        return vertDistance;
    }
    
    /* check if positionB is right/east of positonA & by how far */
    private bool IsItRight(Vector3 position01, Vector3 position02) {
        if (horizAligned) {
            if (position01.x < position02.x) {
                horizDistance = Mathf.RoundToInt(position02.x - position01.x);
                // Debug.Log("Triangulating horizDistance: " + horizDistance);
                isRight = true;
                // Debug.Log("Triangulating right: " + isRight);
            }
            else if (position01.x > position02.x) {
                horizDistance = Mathf.RoundToInt(position01.x - position02.x);
                // Debug.Log("Triangulating horizDistance: " + horizDistance);
                isRight = false;
                // Debug.Log("Triangulating right: " + isRight);
            }
            else {
                horizDistance = 0;
            }
        }

        return isRight;
    }
    
    /* check if positionB is ahead/north of positonA & by how far */
    private bool IsItForward(Vector3 position01, Vector3 position02) {
        if (vertAligned) {
            if (position01.z < position02.z) {
                vertDistance = Mathf.RoundToInt(position02.z - position01.z);
                // Debug.Log("Triangulating vertDistance: " + vertDistance);
                isForward = true;
                // Debug.Log("Triangulating forward: " + isForward);
            }
            else if (position01.z > position02.z) {
                vertDistance = Mathf.RoundToInt(position01.z - position02.z);
                // Debug.Log("Triangulating vertDistance: " + vertDistance);
                isForward = false;
                // Debug.Log("Triangulating forward: " + isForward);
            }
            else {
                vertDistance = 0;
            }
        }

        return isForward;
    }

    private List<Vector3> ShortenList(List<Vector3> list) {
        var tmpList = new HashSet<Vector3>(list);
        var shortened = tmpList.ToList();
        shortListed = true;
        return shortened;
    }

    private IEnumerator WaitForShortList() {
        yield return new WaitUntil(() => shortListed == true);
    }
    
    private IEnumerator WaitForRowSlice() {
        yield return new WaitUntil(() => foundFirstRow == true);
    }

    private IEnumerator WaitForColumnSlice() {
        yield return new WaitUntil(() => foundFirstColumn == true);
    }
    
    /* return a sorted slice/portion of the path array at a given row of the maze grid */
    private List<Vector3> SliceRows(List<Vector3> path, int rowToSlice, int minWidth) {
        int slicingCount = 0;
        var slice = new List<Vector3>();
        var sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing rows List (GM)");
        while (slicingCount < path.Count) {
            if (path[slicingCount].z == rowToSlice) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListRows(slice, minWidth);
        foundFirstRow = true;
        
        return sortedSlice;
    }
    
    /* return a sorted slice/portion of the path array at a given column of the maze grid */
    private List<Vector3> SliceColumns(List<Vector3> path, int columnToSlice, int minHeight) {
        int slicingCount = 0;
        var slice = new List<Vector3>();
        var sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing columns List (GM)");
        while (slicingCount < path.Count) {
            if (path[slicingCount].x == columnToSlice) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListColumns(slice, minHeight);
        foundFirstColumn = true;
        
        return sortedSlice;
    }
    
    /* for sorting an array slice based on X axis */
    private List<Vector3> SortListRows(List<Vector3> list, int lowestValue) {
        var sorted = new List<Vector3>();
        int sortingCount = 0;
        int listSize = list.Count;

        // Debug.Log("Sorting Sliced List (R)");
        while (sortingCount < listSize) {
            for (int i = 0; i < listSize; i++) {
                if (list[i].x == lowestValue) {
                    sorted.Add(list[i]);
                    sortingCount++;
                }
            }
            lowestValue++;
        }
        return sorted;
    }

    /* for sorting an array slice based on Z axis */
    private List<Vector3> SortListColumns(List<Vector3> list, int lowestValue) {
        var sorted = new List<Vector3>();
        int sortingCount = 0;
        int listSize = list.Count;

        // Debug.Log("Sorting Sliced List (C)");
        while (sortingCount < listSize) {
            for (int i = 0; i < listSize; i++) {
                if (list[i].z == lowestValue) {
                    sorted.Add(list[i]);
                    sortingCount++;
                }
            }
            lowestValue++;
        }
        return sorted;
    }
    
    /* find/return the value of the first row of the maze path */
    private int FindFirstRow(List<Vector3> list) {
        List<int> tmp = new List<int>();
        
        foreach (Vector3 listZ in list) {
            tmp.Add(Mathf.RoundToInt(listZ.z));
        }
        // Debug.Log("Find1stR early: " + tmp[0]);
        tmp.Sort();
        // Debug.Log("Find1stR late: " + tmp[0]);
        return tmp[0];
    }

    /* find/return the value of the first column of the maze path */
    private int FindFirstColumn(List<Vector3> list) {
        List<int> tmp = new List<int>();
        
        foreach (Vector3 listX in list) {
            tmp.Add(Mathf.RoundToInt(listX.x));
        }
        // Debug.Log("Find1stC early: " + tmp[0]);
        tmp.Sort();
        // Debug.Log("Find1stC late: " + tmp[0]);
        return tmp[0];
    }
    
    /* Spawn the east & west walls across a given row of the maze path */
    private void SpawnEastWestWall(List<Vector3> path, GameObject[] walls, Material material) {
        for (int q = 0; q < path.Count; q++) {
            if (q == 0) {
                /* Spawn the first west wall of the row */
                Spawn(walls[west], path[q], material);
            }
            else if (path[q - 1].x < path[q].x - 1) {
                /* Spawn a west wall at the end of a break in a row */
                Spawn(walls[west], path[q], material);
            }
            else if (q == path.Count - 1) {
                /* Spawn the last east wall of the row if the list ends before the boundary */
                Spawn(walls[east], path[q], material);
            }
        }
        
        /* reversing the list to spawn the east walls */
        path.Reverse();

        for (int g = 0; g < path.Count; g++) {
            if (g == 0) {
                /* spawn the last east wall of the row */
                Spawn(walls[east], path[g], material);
            } else if (path[g - 1].x > path[g].x + 1) {
                /* spawn an east wall at the end of a break in a row */
                Spawn(walls[east], path[g], material);
            }
        }
    }
    
    /* Spawn the north & south walls along a given column of the maze path */
    /* this function parses the maze grid from bottom to the top */
    private void NorthSouthWalls(List<Vector3> path, GameObject[] walls, Material material) {
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first south wall of the row */
                Spawn(walls[south], path[i], material);
            }
            else if (path[i - 1].z < path[i].z - 1) {
                /* spawn south wall at end of a break in the row */
                Spawn(walls[south], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last north wall of the row if the list ends before the boundary */
                Spawn(walls[north], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last north wall of the row */
                Spawn(walls[north], path[j], material);
            }
            else if (path[j - 1].z > path[j].z + 1) {
                /* spawn north wall at end of a break in the row */
                Spawn(walls[north], path[j], material);
            }
        }
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
 }