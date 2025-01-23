using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming
// ReSharper disable InvalidXmlDocComment
// ReSharper disable NotAccessedField.Local
// ReSharper disable PossibleLossOfFraction
// ReSharper disable Unity.RedundantEventFunction

public class GameManager : MonoBehaviour {

    public GameData mazeData;
    public GameObject goalObject;
    public GameObject player;
    public GameObject colourResetter;
    public GameObject[] floorTiles;
    public GameObject[] wallPanels; // _N_ever _E_at _S_oggy _W_eetbix
    public int gridHeight;
    public int gridWidth;
    public Vector2Int gridSize;
    public List<GameObject> waypoints;
    public TextMeshProUGUI winText, gameOverText;
    public Button restartButton;
    public GameObject titleScreen;
    public bool isGameActive;
    public bool easyMode = true;
    [Range(0.24f, 0.76f)] public float randomVariance = 0.42f;
    
    public bool shortListed = false;
    public bool firstRowFound = false;
    public bool firstColFound = false;
    
    public static bool _easyMode, goalFound;
    // public static Color goalColour, hintColour01, hintColour02;
    
    /* -- private constants -- */
    private const int NORTH = 0;
    private const int EAST = 1;
    private const int SOUTH = 2;
    private const int WEST = 3;
    
    /* -- private variables -- */
    private GameObject groundPlane;
    private bool horizAligned, vertAligned, isForward, isRight, debugHoriz, debugVert, firstRow, firstColumn;
    private float xVal, zVal;
    private int count, columnNumber, rowNumber, lastRowNumber, horizDistance, vertDistance, xPosition, thisNorthernEdge;
    private int thisEasternEdge, thisSouthernEdge, thisWesternEdge, _firstRowNumber, _firstColumnNumber, _lastRowNumber, _lastColumnNumber;
    private Material wallMaterial;
    private Vector2 randVariance;
    private Vector3 goalPosition, resetterPosition, pyramidPos02, spawnPosition, tmpPosition;
    
    /* -- private Lists -- */
    private List<Color> mixedColors;
    private List<int> distances, lcms;
    private List<Vector3> intersections, destinations, midPoints, drawnPath, sortedList, shortenedList;
    [SerializeField] public static Color _playerColour;


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
        
        // goalColour = new Color();
        // hintColour01 = new Color();
        // hintColour02 = new Color();
        groundPlane = GameObject.Find("GroundPlane");
        mixedColors = new List<Color>();
        lcms = new List<int>();
        distances = new List<int>();
        destinations = new List<Vector3>();
        intersections = new List<Vector3>();
        drawnPath = new List<Vector3>();
        shortenedList = new List<Vector3>();
        midPoints = new List<Vector3>();
        sortedList = new List<Vector3>();

        randVariance.x = randomVariance;
        randVariance.y = 1f - randomVariance;
        _lastRowNumber = thisNorthernEdge = gridHeight / 2;
        _lastColumnNumber = thisEasternEdge = gridWidth / 2;
        _firstRowNumber = thisSouthernEdge = -thisNorthernEdge;
        _firstColumnNumber = thisWesternEdge = -thisEasternEdge;
        _easyMode = easyMode;

        if (gridSize.x < 1) {
            gridSize.x = gridWidth;
        }
        if (gridSize.y < 1) {
            gridSize.y = gridHeight;
        }

        mazeData.InitData(gridSize);
        
        wallMaterial = Resources.Load<Material>("Materials/DryWall_Mat");

        mazeData.goalColour = Color.white;
        // goalColour = ChangeColours("add", waypoints);
        mazeData.goalColour = ChangeColours("add", waypoints);
        goalObject.GetComponentInChildren<Renderer>().sharedMaterial.color = mazeData.goalColour;
        

        /* END Awake() */
    }

    private void Start() {
        
        groundPlane.gameObject.SetActive(false);

        MazeUI.PaintGoalBlip(mazeData.goalColour);
        MazeUI.PaintPlayerBlipWhite();
        // MazeUI.PaintHintBlips(hintColour01, hintColour02);
        
        ResetCount();

        /* populate the destinations List with random positions for the player, waypoints & goal */
        while (count < waypoints.Count + 2) {
            destinations.Add(RandomPosition(thisWesternEdge, thisEasternEdge, thisSouthernEdge, thisNorthernEdge));
            count++;
        }

        ResetCount();

        /* populate distance & midpoint Lists */
        while (count < destinations.Count - 1) {
            distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[count], destinations[count + 1])));
            midPoints.Add(Vector3.Lerp(destinations[count], destinations[count + 1], 0.5f));
            count++;
        }

        ResetCount();

        while (count < midPoints.Count) {
            lcms.Add(FindLcm(midPoints[count], distances[count], thisNorthernEdge, thisEasternEdge));
            count++;
        }

        ResetCount();

        /* populate intersections list using destinations & lcms as seed values for triangulated positions */
        while (count < lcms.Count) {
            intersections.Add(destinations[count]);
            intersections.Add(TriangulateIntersection(destinations[count], destinations[count + 1],
                Random.Range(randVariance.x, randVariance.y), lcms[count]));
            count++;
        }

        /* then add the last destination which is for the goal */
        intersections.Add(destinations[count]);
        

        /* plot the paths between the intersections */
        ResetCount();
        while (count < intersections.Count) {
            if (count + 1 < intersections.Count) {
                PlotHorizontalPath(intersections[count], intersections[count + 1], drawnPath);
                PlotVerticalPath(intersections[count], intersections[count + 1], drawnPath);
            }
            else {
                PlotHorizontalPath(intersections[count], intersections[count], drawnPath);
                PlotVerticalPath(intersections[count], intersections[count], drawnPath);
            }

            count++;
        }

        /* remove duplicate values from the drawnPath list */
        shortenedList = RemoveDuplicates(drawnPath);
        drawnPath.Clear();
        
        /* swap the values back to drawnPath then erase shortenedList */
        foreach (var step in shortenedList) {
            drawnPath.Add(step);
        }
        
        /* clearing Lists as they are no longer needed in memory */
        shortenedList.Clear();
        midPoints.Clear();

        resetterPosition = ResetterPosition(drawnPath, destinations);
        
        
        ResetCount();
        
        /* spawn the floor tiles for the maze path */
        while (count < drawnPath.Count) {
            // Spawn(floorTiles[1], drawnPath[count]);
            Spawn(floorTiles[Mathf.RoundToInt(Random.Range(0, floorTiles.Length))], drawnPath[count]);
            count++;
        }
        
        /* find the value of the first and last rows of the maze grid */
        _firstRowNumber = FindLargestValue(FindTheZs(drawnPath));
        _lastRowNumber = FindSmallestValue(FindTheZs(drawnPath));
        
        /* spawn the east/west walls */
        while (_firstRowNumber >= _lastRowNumber) {
            sortedList = SortRows(RemoveDuplicates(SliceRow(drawnPath, _firstRowNumber)));
            SpawnEastWestWalls(sortedList, wallPanels, wallMaterial);
            _firstRowNumber--;
        }
        
        /* clearing the sortedList before parsing the North/South walls, to eliminate the chances of junk data */
        sortedList.Clear();
        
        /* find the value of the first and last columns of the maze grid */
        _firstColumnNumber = FindLargestValue(FindTheXs(drawnPath));
        _lastColumnNumber = FindSmallestValue(FindTheXs(drawnPath));

        /* spawn the north/south walls */
        while (_firstColumnNumber >= _lastColumnNumber) {
            sortedList =
                SortColumns(RemoveDuplicates(SliceColumn(drawnPath, _firstColumnNumber)));
            SpawnNorthSouthWalls(sortedList, wallPanels, wallMaterial);
            _firstColumnNumber--;
        }
        
        /* clearing the sortedList as it is no longer needed in memory */
        sortedList.Clear();
        
        /* reposition the player, spawn the waypoints and goal */
        player.transform.position = destinations[0];
        for (count = 1; count <= destinations.Count - 2; count++) {
            Spawn(waypoints[count - 1], destinations[count]);
        }
        
        Spawn(goalObject, destinations[^1]);
        
        SpawnColourResetter(colourResetter, resetterPosition);
        

        /* END Start() */
    }

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

    private void EndLevel() {
        Debug.Log("display winText");
        winText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }
    
    
    /** Object spawning functions **/
    /* spawn a given GameObject at a given position */
    public void Spawn(GameObject gameObject, Vector3 position) {
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }

    /* overloading Spawn function to set rotation */
    public void Spawn(GameObject gameObject, Vector3 position, Quaternion rotation) {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        Instantiate(gameObject);
    }

    /* overloading Spawn function to set material */
    public void Spawn(GameObject gameObject, Vector3 position, Material material) {
        gameObject.transform.position = position;
        gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;
        Instantiate(gameObject);
    }

    /* Spawn the east & west walls across a given row of the maze path */
    private void SpawnEastWestWalls(List<Vector3> path, GameObject[] walls, Material material) {
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first east wall of the row */
                Spawn(walls[WEST], path[i], material);
            }
            else if (path[i - 1].x < path[i].x - 1) {
                /* spawn east wall at end of a break in the row */
                Spawn(walls[WEST], path[i], material);
            }

            else if (i == path.Count - 1) {
                /* spawn the last WEST wall of the row if the list ends before the boundary */
                Spawn(walls[EAST], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last east wall of the row */
                Spawn(walls[EAST], path[j], material);
            }
            else if (path[j - 1].x > path[j].x + 1) {
                /* spawn east wall at end of a break in the row */
                Spawn(walls[EAST], path[j], material);
            }
        }
    }

    /* Spawn the north & south walls along a given column of the maze path */
    private void SpawnNorthSouthWalls(List<Vector3> path, GameObject[] walls, Material material) {
        // Debug.Log("Spawning North/South Walls");
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first south wall of the row */
                Spawn(walls[SOUTH], path[i], material);
            }
            else if (path[i - 1].z < path[i].z - 1) {
                /* spawn south wall at end of a break in the row */
                Spawn(walls[SOUTH], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last north wall of the row if the list ends before the boundary */
                Spawn(walls[NORTH], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last north wall of the row */
                Spawn(walls[NORTH], path[j], material);
            }
            else if (path[j - 1].z > path[j].z + 1) {
                /* spawn north wall at end of a break in the row */
                Spawn(walls[NORTH], path[j], material);
            }
        }
    }

    public void SpawnColourResetter(GameObject gameObject, Vector3 position) {
        // position.y = 0.275f;
        position.y = 0.05f;
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }

    /** Math type functions **/
    private Vector3 FarthestCorner(Vector3 inputPosition, int mazeHeight, int mazeWidth) {
        int xTmp, zTmp;
        if (inputPosition.x >= 0) {
            xTmp = -mazeWidth;
        }
        else {
            xTmp = mazeWidth;
        }

        if (inputPosition.z >= 0) {
            zTmp = -mazeHeight;
        }
        else {
            zTmp = mazeHeight;
        }

        return new Vector3(xTmp, 0, zTmp);
    }

    /* Find the Greatest Common Denominator/Divisor */
    private int GCD(int int01, int int02) {
        int tmpA = int01;
        int tmpB = int02;

        while (tmpB > 0) {
            int temp = tmpB;
            tmpB = tmpA % tmpB;
            tmpA = temp;
        }

        return tmpA;
    }

    /* find the Lowest Common Multiple */
    private int LCM(int distance01, int distance02) {
        int lcmTmp = distance01 / GCD(distance01, distance02);
        return distance02 * lcmTmp;
    }

    private int ReduceLCM(int lcm, int mazeHeight, int mazeWidth) {
        int tmp = lcm;
        if (mazeHeight == mazeWidth || mazeHeight > mazeWidth) {
            while (tmp >= mazeWidth) {
                tmp /= mazeWidth;
            }
        }
        else {
            while (tmp >= mazeHeight) {
                tmp /= mazeHeight;
            }
        }

        return tmp;
    }

    private int FindLcm(Vector3 midPoint, int distance, int mazeHeight, int mazeWidth) {
        Vector3 farCorner = FarthestCorner(midPoint, mazeHeight, mazeWidth);
        int farthestDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farCorner));
        int otherDistance = distance + farthestDistance;
        int lcm01 = ReduceLCM(LCM(distance, farthestDistance), mazeHeight, mazeWidth);
        int lcm02 = ReduceLCM(LCM(distance, otherDistance), mazeHeight, mazeWidth);
        int lcm03 = ReduceLCM(LCM(farthestDistance, otherDistance), mazeHeight, mazeWidth);
        return lcm01 + lcm02 + lcm03;
    }

    private int MeasureHorizontal(Vector3 position01, Vector3 position02) {
        return Mathf.Abs(Mathf.RoundToInt(position01.x - position02.x));
    }

    private int MeasureVertical(Vector3 position01, Vector3 position02) {
        return Mathf.Abs(Mathf.RoundToInt(position01.z - position02.z));
    }

    private int FindLargestValue(List<int> values) {
        int tmpValue = thisWesternEdge < thisSouthernEdge ? thisSouthernEdge : thisWesternEdge;
        return values.Prepend(tmpValue).Max();
    }

    private int FindSmallestValue(List<int> values) {
        int tmpValue = thisNorthernEdge < thisEasternEdge ? thisEasternEdge : thisNorthernEdge;
        return values.Prepend(tmpValue).Min();
    }


    /** maze position related functions **/
    private Vector3 RandomPosition(int minWidth, int maxWidth, int minHeight, int maxHeight) {
        return new Vector3(Mathf.Round(Random.Range(minWidth, maxWidth)), 0,
            Mathf.Round(Random.Range(minHeight, maxHeight)));
    }

    private Vector3 ResetterPosition(List<Vector3> possiblePositions, List<Vector3> invalidPositions) {
        List<Vector3> allowedPositions = (from t in invalidPositions from pos in possiblePositions where pos != t select pos).ToList();
        return allowedPositions[Mathf.RoundToInt(Random.Range(0, allowedPositions.Count))];
    }

    /* create an approximately "center" position between two points using */
    /* Slerp that is clamped within the maze boundaries, this is intended */
    /* to take a Random.Range() value to create a point close to center */
    /* within approx. 40/60 weighting */
    public Vector3 TriangulateIntersection(Vector3 origin, Vector3 destination, float centreMargin) {
        Vector3 position = Vector3.Slerp(origin, destination, centreMargin);

        // normalizing the position
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);

        return ClampWithinBoundaries(position);
    }

    /* overloading the function to use an LCM value as a position modifier */
    private Vector3 TriangulateIntersection(Vector3 origin, Vector3 destination, float centreMargin, int modifier) {
        Vector3 position = Vector3.Slerp(origin, destination, centreMargin);
        float modifierX = 0f;
        float modifierY = 0f;
        int decimalMultiplier = 10;

        if (modifier > Mathf.Abs(1000) && modifier < Mathf.Abs(10000)) {
            decimalMultiplier = 1000;
        }
        else if (modifier > Mathf.Abs(100) && modifier < Mathf.Abs(1000)) {
            decimalMultiplier = 100;
        }

        /** TODO: possibly change this to randomly switch proportioning **/
        /* split the modifier by the random variance to be used proportionally across the X & Z axis' */
        modifierX = (modifier / decimalMultiplier) * centreMargin;
        modifierY = (modifier / decimalMultiplier) * (1 - centreMargin);

        /* normalizing the position */
        position.x = Mathf.RoundToInt(position.x * (modifierX));
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z * (modifierY));

        return ClampWithinBoundaries(position);
    }

    private void PlotHorizontalPath(Vector3 origin, Vector3 destination, List<Vector3> pathToDraw) {
        int thisCount;
        int hDistance = MeasureHorizontal(origin, destination);
        if (IsOriginWest(origin, destination)) {
            for (thisCount = 0; thisCount <= hDistance; thisCount++) {
                pathToDraw.Add(new Vector3(origin.x + thisCount, origin.y, origin.z));
            }
        }
        else {
            for (thisCount = hDistance; thisCount >= 0; thisCount--) {
                pathToDraw.Add(new Vector3(origin.x - thisCount, origin.y, origin.z));
            }
        }
    }

    private void PlotVerticalPath(Vector3 origin, Vector3 destination, List<Vector3> pathToDraw) {
        int thisCount;
        int vDistance = MeasureVertical(origin, destination);
        if (IsOriginSouth(origin, destination)) {
            for (thisCount = vDistance; thisCount >= 0; thisCount--) {
                pathToDraw.Add(new Vector3(destination.x, destination.y, destination.z - thisCount));
            }
        }
        else {
            for (thisCount = 0; thisCount <= vDistance; thisCount++) {
                pathToDraw.Add(new Vector3(destination.x, destination.y, destination.z + thisCount));
            }
        }
    }

    private bool HorizontalAlignment(Vector3 position01, Vector3 position02) {
        if (Mathf.Approximately(position01.z, position02.z) || Mathf.Approximately(position01.z, position02.z - 1) ||
            Mathf.Approximately(position01.z, position02.z + 1)) {
            horizAligned = true;
        }
        return horizAligned;
    }

    private bool VerticalAlignment(Vector3 position01, Vector3 position02) {
        if (Mathf.Approximately(position01.x, position02.x) || Mathf.Approximately(position01.x, position02.x - 1) ||
            Mathf.Approximately(position01.x, position02.x + 1)) {
            vertAligned = true;
        }
        return vertAligned;
    }

    /* check if positionB is ahead/north of positionA & by how far */
    private bool IsItForward(Vector3 position01, Vector3 position02) {
        if (vertAligned) {
            if (position01.z < position02.z) {
                vertDistance = Mathf.RoundToInt(position02.z - position01.z);
                isForward = true;
            }
            else if (position01.z > position02.z) {
                vertDistance = Mathf.RoundToInt(position01.z - position02.z);
                isForward = false;
            }
            else {
                vertDistance = 0;
            }
        }

        return isForward;
    }

    /* check if positionB is right/east of positionA & by how far */
    private bool IsItRight(Vector3 position01, Vector3 position02) {
        if (horizAligned) {
            if (position01.x < position02.x) {
                horizDistance = Mathf.RoundToInt(position02.x - position01.x);
                isRight = true;
            }
            else if (position01.x > position02.x) {
                horizDistance = Mathf.RoundToInt(position01.x - position02.x);
                isRight = false;
            }
            else {
                horizDistance = 0;
            }
        }

        return isRight;
    }

    private bool IsOriginWest(Vector3 position01, Vector3 position02) {
        bool isWest = position01.x < position02.x;
        return isWest;
    }

    private bool IsOriginSouth(Vector3 position01, Vector3 position02) {
        bool isSouth = position01.z < position02.z;
        return isSouth;
    }

    public void CheckHorizontal(Vector3 position01, Vector3 position02, List<Vector3> path) {
        tmpPosition = new Vector3(0, 0, 0);
        if (HorizontalAlignment(position01, position02)) {
            if (IsItRight(position01, position02)) {
                for (count = 0; count <= horizDistance; count++) {
                    path.Add(new Vector3(position01.x + count, position01.y, position01.z));
                }
            }
            else {
                for (count = 0; count <= horizDistance; count++) {
                    path.Add(new Vector3(position01.x - count, position01.y, position01.z));
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
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(tmpPosition.x + count, tmpPosition.y, tmpPosition.z));
                        }
                    }
                    else {
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(tmpPosition.x - count, tmpPosition.y, tmpPosition.z));
                        }
                    }
                }
            }

            if (xPosition == 2) {
                if (HorizontalAlignment(position01, tmpPosition)) {
                    if (IsItRight(position01, tmpPosition)) {
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(position01.x + count, tmpPosition.y, tmpPosition.z));
                        }
                    }
                    else {
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(position01.x - count, tmpPosition.y, tmpPosition.z));
                        }
                    }
                }
            }
        }
    }

    public void CheckVertical(Vector3 position01, Vector3 position02, List<Vector3> path) {
        /* positions are within horizontal alignment margins */
        if (VerticalAlignment(position01, position02)) {
            if (IsItForward(position01, position02)) {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(position01.x, position01.y, position01.z + count));
                }
            }
            else {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(position01.x, position01.y, position01.z - count));
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
            if (IsItForward(position01, tmpPosition)) {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, position01.z + count));
                }
            }
            else {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, position01.z - count));
                }
            }
        }

        if (VerticalAlignment(tmpPosition, position02)) {
            if (IsItForward(tmpPosition, position02)) {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z + count));
                }
            }
            else {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z - count));
                }
            }
        }
    }

    /* check position is within the boundaries, if not clamp them in */
    private Vector3 ClampWithinBoundaries(Vector3 position) {
        if (position.x <= thisWesternEdge) {
            position.x = thisWesternEdge;
        }
        else if (position.x > thisEasternEdge) {
            position.x = thisEasternEdge;
        }

        if (position.z <= thisSouthernEdge) {
            position.z = thisSouthernEdge;
        }
        else if (position.z > thisNorthernEdge) {
            position.z = thisNorthernEdge;
        }

        return position;
    }


    /** maze grid segment selection & sorting functions **/
    private List<Vector3> SortRows(List<Vector3> pathList) {
        /* https://stackoverflow.com/questions/36070425/simplest-way-to-sort-a-list-of-vector3s */
        return pathList.OrderBy(v => v.x).ToList();
    }

    private List<Vector3> SortColumns(List<Vector3> pathList) {
        /* https://stackoverflow.com/questions/36070425/simplest-way-to-sort-a-list-of-vector3s */
        return pathList.OrderBy(v => v.z).ToList();
    }

    /* 'Slice' the list down to the specified row/Z value */
    private List<Vector3> SliceRow(List<Vector3> sorted, int rowToSlice) {
        int counter = 0;
        List<Vector3> sliced = new();

        while (counter < sorted.Count) {
            if (Mathf.RoundToInt(sorted[counter].z) == rowToSlice) {
                sliced.Add(sorted[counter]);
            }

            counter++;
        }

        return sliced.ToList();
    }

    /* 'Slice' the list down to the specified column/X value */
    private List<Vector3> SliceColumn(List<Vector3> sorted, int columnToSlice) {
        int counter = 0;
        List<Vector3> sliced = new();

        while (counter < sorted.Count) {
            if (Mathf.RoundToInt(sorted[counter].x) == columnToSlice) {
                sliced.Add(sorted[counter]);
            }

            counter++;
        }

        return sliced.ToList();
    }

    public List<Vector3> SortAndSliceRows(List<Vector3> pathList, int rowToSlice) {
        int counter = 0;
        int searchValue = rowToSlice;
        List<Vector3> sorted = new();

        while (counter < pathList.Count) {
            for (int i = 0; i < pathList.Count; i++) {
                if (searchValue == Mathf.RoundToInt(pathList[i].z)) {
                    sorted.Add(pathList[i]);
                    counter++;
                }
            }

            searchValue--;
        }
        return SliceRow(sorted, rowToSlice).ToList();
    }

    private List<int> FindTheZs(List<Vector3> path) {
        List<int> foundZs = path.Select(slice => Mathf.RoundToInt(slice.z)).ToList();
        HashSet<int> shortened = new(foundZs);

        return shortened.ToList();
    }

    private List<int> FindTheXs(List<Vector3> path) {
        List<int> foundXs = path.Select(slice => Mathf.RoundToInt(slice.x)).ToList();
        HashSet<int> shortened = new(foundXs);

        return shortened.ToList();
    }


    /** Colour changing/blending functions **/
    
    /** TODO: modify Add & Blend func() so they can use just the current and last waypoint colours, while "discarding" the 2nd last waypoint colour **/
    private Color ChangeColours(string AddOrBlend, List<GameObject> waypoints) {
        Color returningColour = new();
        switch (AddOrBlend) {
            case "add" :
                returningColour = AddColoursTogether(waypoints);
                break;
            case "blend" :
                returningColour = BlendColoursTogether(waypoints);
                break;
            
        }

        return returningColour;
    }
    
    public static Color ChangeColours(string switchAddOrBlend, Color playersColor, Color waypointColor) {
        Color returningColour = new();
        switch (switchAddOrBlend) {
            case "switch" :
                returningColour = waypointColor;
                break;
            case "add" :
                returningColour = AddColoursTogether(playersColor, waypointColor);
                break;
            case "blend" :
                returningColour = BlendColoursTogether(playersColor, waypointColor);
                break;
            
        }

        return returningColour;
    }
    
    public Color ChangeColours(string switchAddOrBlend, Color playersColor, Color currentWaypointColor, Color previousWaypointColour) {
        Color returningColour = new();
        switch (switchAddOrBlend) {
            case "switch" :
                returningColour = currentWaypointColor;
                break;
            case "add" :
                returningColour = AddColoursTogether(playersColor, currentWaypointColor, previousWaypointColour);
                break;
            case "blend" :
                returningColour = BlendColoursTogether(playersColor, currentWaypointColor, previousWaypointColour);
                break;
            
        }

        return returningColour;
    }

    private Color AddColoursTogether(List<GameObject> waypoints) {
        var waypoint01 = Random.Range(0, waypoints.Count);
        var waypoint02 = Random.Range(0, waypoints.Count);
        // hintColour01 = waypoints[waypoint01].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        mazeData.hintColour01 = waypoints[waypoint01].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        // hintColour02 = waypoints[waypoint02].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        mazeData.hintColour02 = waypoints[waypoint02].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        var tmpColour = mazeData.hintColour01 + mazeData.hintColour02;
        return tmpColour;
    }

    private static Color AddColoursTogether(Color playersColour, Color waypointColor) {
        Color mixedColour;
        if (waypointColor == Color.white || waypointColor == Color.black) {
            mixedColour = BlendColoursTogether(playersColour, waypointColor);
        }
        else {
            mixedColour = playersColour + waypointColor;
        }
        return mixedColour;
    }

    private Color AddColoursTogether(Color playersColour, Color currentColour, Color previousColour) {
        Color mixedColour = playersColour + currentColour - previousColour;
        return mixedColour;
    }

    private Color BlendColoursTogether(List<GameObject> waypoints) {
        int index01 = Random.Range(0, waypoints.Count);
        int index02 = Random.Range(0, waypoints.Count);
        
        mazeData.hintColour01 = waypoints[index01].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        mazeData.hintColour02 = waypoints[index02].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;

        // Color blendedColour =
        //     Color.Lerp(waypoints[index01].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
        //         waypoints[index02].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color, 0.5f);
        Color blendedColour = Color.Lerp(mazeData.hintColour01, mazeData.hintColour02, 0.5f);
        return blendedColour;
    }

    
    private static Color BlendColoursTogether(Color playersColour, Color waypointColor) {
        Color blendedColour = Color.Lerp(playersColour, waypointColor, 0.5f);
        return blendedColour;
    }
    
    private Color BlendColoursTogether(Color playersColour, Color currentColor, Color previousColour) {
        Color blendedColour = Color.Lerp(playersColour, currentColor, 0.5f) - previousColour;
        return blendedColour;
    }

    /** misc. functions **/
    /* by parsing a List through a HashSet duplicates are eliminated, as HashSets only contain unique values */
    private List<Vector3> RemoveDuplicates(List<Vector3> pathList) {
        HashSet<Vector3> inputList = new(pathList);
        List<Vector3> shortened = inputList.ToList();

        shortListed = true;
        return shortened.ToList();
    }

    // public IEnumerator WaitForRowSlice() {
    //     yield return new WaitUntil(() => firstRowFound == true);
    // }
    //
    // public IEnumerator WaitForColSlice() {
    //     yield return new WaitUntil(() => firstColFound == true);
    // }
    //
    // public IEnumerator WaitForListShortening() {
    //     yield return new WaitUntil(() => shortListed == true);
    // }



}