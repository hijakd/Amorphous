using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using AmorphousData;


public class GameManager : MonoBehaviour {

    public static GameData mazeData;
    public GameObject goalObject;
    public GameObject player;
    public float playerMovementSpeed = 50f;
    public GameObject groundPlane;
    public GameObject colourResetter;
    public GameObject[] floorTiles;
    public GameObject[] wallPanels; // _N_ever _E_at _S_oggy _W_eetbix
    public List<GameObject> waypoints;
    public TextMeshProUGUI winText, gameOverText;
    public Button restartButton;
    public GameObject titleScreen;
    public bool isGameActive;
    public bool easyMode = true;
    [Range(0.24f, 0.76f)] public float randomVariance = 0.42f;
    
    /* -- private constants -- */
    const int NORTH = 0;
    const int EAST = 1;
    const int SOUTH = 2;
    const int WEST = 3;

    /* -- private variables -- */

    bool horizAligned, vertAligned, isForward, isRight, isPaused;

    int count, horizDistance, vertDistance, xPosition, firstRowNumber, firstColumnNumber, lastRowNumber, lastColumnNumber;
    Material wallMaterial;
    Vector2 randVariance;
    Vector3 goalPosition, resetterPosition, pyramidPos02, spawnPosition, tmpPosition;

    /* -- private Lists -- */
    List<int> distances, lcms;
    List<Vector3> intersections, destinations, midPoints, drawnPath, sortedList, shortenedList;


    void Awake() {
        
        mazeData = ScriptableObject.CreateInstance<GameData>();
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

        // wallMaterial = Resources.Load<Material>("Materials/DryWall_Mat");
        wallMaterial = Resources.Load<Material>("Materials/Dungeon_Ground");

        if (easyMode) {
            mazeData.difficulty = 1;
        }

        mazeData.playerSpeed = playerMovementSpeed;
        mazeData.goalColour = Color.white;
        mazeData.playerColour = Color.white;
        mazeData.playerIsWhite = true;

        mazeData.goalColour = ChangeColours("add", waypoints);
        goalObject.GetComponentInChildren<Renderer>().sharedMaterial.color = mazeData.goalColour;


        /* END Awake() */
    }

    void Start() {
        MazeUI.PaintGoal(mazeData.goalColour);
        MazeUI.PaintPlayer(mazeData.playerColour);

        groundPlane.gameObject.SetActive(false);

        ResetCount();

        /* populate the destinations List with random positions for the player, waypoints & goal */
        while (count < waypoints.Count + 2) {
            destinations.Add(RandomPosition(mazeData.westernEdge, mazeData.easternEdge, mazeData.southernEdge,
                mazeData.northernEdge));
            count++;
        }

        /* TODO: convert this RemoveDuplicates(destinations) into a recursive function */
        /* remove duplicate values from the destinations list */
        shortenedList = RemoveDuplicates(destinations);

        if (shortenedList.Count < destinations.Count) {
            int missingDestinations = destinations.Count - shortenedList.Count;
            while (missingDestinations > 0) {
                shortenedList.Add(RandomPosition(mazeData.westernEdge, mazeData.easternEdge, mazeData.southernEdge,
                    mazeData.northernEdge));
                missingDestinations--;
            }

            destinations.Clear();

            /* swap the values back to destinations then erase shortenedList */
            foreach (Vector3 step in shortenedList) {
                destinations.Add(step);
            }
        }

        shortenedList.Clear();

        ResetCount();

        /* populate distance & midpoint Lists */
        while (count < destinations.Count - 1) {
            distances.Add(Mathf.RoundToInt(Vector3.Distance(destinations[count], destinations[count + 1])));
            midPoints.Add(Vector3.Lerp(destinations[count], destinations[count + 1], 0.5f));
            count++;
        }

        ResetCount();

        while (count < midPoints.Count) {
            lcms.Add(FindLcm(midPoints[count], distances[count], mazeData.northernEdge, mazeData.easternEdge));
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

        ResetCount();

        /* plot the paths between the intersections */
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
            Spawn(floorTiles[Mathf.RoundToInt(Random.Range(0, floorTiles.Length))], drawnPath[count]);
            count++;
        }

        /* find the value of the first and last rows of the maze grid */
        firstRowNumber = FindLargestValue(FindTheZs(drawnPath));
        lastRowNumber = FindSmallestValue(FindTheZs(drawnPath));

        /* spawn the east/west walls */
        while (firstRowNumber >= lastRowNumber) {
            sortedList = SortRows(RemoveDuplicates(SliceRow(drawnPath, firstRowNumber)));
            SpawnEastWestWalls(sortedList, wallPanels, wallMaterial);
            firstRowNumber--;
        }

        /* clearing the sortedList before parsing the North/South walls, to eliminate the chances of junk data */
        sortedList.Clear();

        /* find the value of the first and last columns of the maze grid */
        firstColumnNumber = FindLargestValue(FindTheXs(drawnPath));
        lastColumnNumber = FindSmallestValue(FindTheXs(drawnPath));

        /* spawn the north/south walls */
        while (firstColumnNumber >= lastColumnNumber) {
            sortedList = SortColumns(RemoveDuplicates(SliceColumn(drawnPath, firstColumnNumber)));
            SpawnNorthSouthWalls(sortedList, wallPanels, wallMaterial);
            firstColumnNumber--;
        }

        /* clearing the sortedList as it is no longer needed in memory */
        sortedList.Clear();

        /* reposition the player, spawn the waypoints and goal */
        player.transform.position = destinations[0];

        ResetCount();

        for (count = 1; count <= destinations.Count - 2; count++) {
            Spawn(waypoints[count - 1], destinations[count]);
        }

        Spawn(goalObject, destinations[^1]);

        SpawnColourResetter(colourResetter, resetterPosition);


        /* END Start() */
    }


    void FixedUpdate() {
        if (mazeData.goalFound) {
            EndLevel();
        }

        /* END FixedUpdate() */
    }


    /** Utility functions for the GameManager **/
    public void StartGame() {
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
    }

    /*public void StartGame(int difficulty) {
        modifier /= difficulty;
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
    }*/

    public static void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver() {
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
    }

    void EndLevel() {
        // Debug.Log("display winText");
        PauseGame();

        MazeUI.EnableWinScreen();
    }

    public static void PauseGame() {
        mazeData.isPaused = !mazeData.isPaused;
    }


    /** Object spawning functions **/
    /* spawn a given GameObject at a given position */
    void Spawn(GameObject gObject, Vector3 position) {
        gObject.transform.position = position;
        Instantiate(gObject);
    }

    /* overloading Spawn function to set rotation */
    void Spawn(GameObject gObject, Vector3 position, Quaternion rotation) {
        gObject.transform.position = position;
        gObject.transform.rotation = rotation;
        Instantiate(gObject);
    }

    /* overloading Spawn function to set material */
    void Spawn(GameObject gObject, Vector3 position, Material material) {
        gObject.transform.position = position;
        gObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;
        Instantiate(gObject);
    }

    /* Spawn the east & west walls across a given row of the maze path */
    void SpawnEastWestWalls(List<Vector3> path, GameObject[] walls, Material material) {
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
    void SpawnNorthSouthWalls(List<Vector3> path, GameObject[] walls, Material material) {
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

    void SpawnColourResetter(GameObject gObject, Vector3 position) {
        // position.y = 0.275f;
        position.y = 0.05f;
        gObject.transform.position = position;
        Instantiate(gObject);
    }


    /** maze position related functions **/
    Vector3 RandomPosition(int mazeWesternEdge, int mazeEasternEdge, int mazeSouthernEdge, int mazeNorthernEdge) {
        return new Vector3(Mathf.Round(Random.Range(mazeWesternEdge, mazeEasternEdge)), 0,
            Mathf.Round(Random.Range(mazeSouthernEdge, mazeNorthernEdge)));
    }

    Vector3 ResetterPosition(List<Vector3> possiblePositions, List<Vector3> invalidPositions) {
        List<Vector3> allowedPositions =
            (from t in invalidPositions from pos in possiblePositions where pos != t select pos).ToList();
        return allowedPositions[Mathf.RoundToInt(Random.Range(0, allowedPositions.Count))];
    }

    void PlotVerticalPath(Vector3 origin, Vector3 destination, List<Vector3> pathToDraw) {
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

    void PlotHorizontalPath(Vector3 origin, Vector3 destination, List<Vector3> pathToDraw) {
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

    /* create an approximately "center" position between two points using */
    /* Slerp that is clamped within the maze boundaries, this is intended */
    /* to take a Random.Range() value to create a point close to center */
    /* within approx. 40/60 weighting */
    Vector3 TriangulateIntersection(Vector3 origin, Vector3 destination, float centreMargin) {
        Vector3 position = Vector3.Slerp(origin, destination, centreMargin);

        // normalizing the position
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);

        return ClampWithinBoundaries(position);
    }

    /* overloading the function to use an LCM value as a position modifier */
    Vector3 TriangulateIntersection(Vector3 origin, Vector3 destination, float centreMargin, int modifier) {
        Vector3 position = Vector3.Slerp(origin, destination, centreMargin);
        int decimalMultiplier = 10;

        if (modifier > Mathf.Abs(1000) && modifier < Mathf.Abs(10000)) {
            decimalMultiplier = 1000;
        }
        else if (modifier > Mathf.Abs(100) && modifier < Mathf.Abs(1000)) {
            decimalMultiplier = 100;
        }

        /** TODO: possibly change this to randomly switch proportioning **/
        /* split the modifier by the random variance to be used proportionally across the X & Z axis' */
        float modifierX = (modifier / decimalMultiplier) * centreMargin;
        float modifierY = (modifier / decimalMultiplier) * (1 - centreMargin);

        /* normalizing the position */
        position.x = Mathf.RoundToInt(position.x * (modifierX));
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z * (modifierY));

        return ClampWithinBoundaries(position);
    }

    bool IsOriginWest(Vector3 position01, Vector3 position02) {
        bool isWest = position01.x < position02.x;
        return isWest;
    }

    bool IsOriginSouth(Vector3 position01, Vector3 position02) {
        bool isSouth = position01.z < position02.z;
        return isSouth;
    }

    /* ensure that a given grid position is within the grid boundaries */
    Vector3 ClampWithinBoundaries(Vector3 position) {
        if (position.x <= mazeData.westernEdge) {
            position.x = mazeData.westernEdge;
        }
        else if (position.x > mazeData.easternEdge) {
            position.x = mazeData.easternEdge;
        }

        if (position.z <= mazeData.southernEdge) {
            position.z = mazeData.southernEdge;
        }
        else if (position.z > mazeData.northernEdge) {
            position.z = mazeData.northernEdge;
        }

        return position;
    }


    /** Math type functions **/
    Vector3 FarthestCorner(Vector3 inputPosition, int mazeNorthernEdge, int mazeEasternEdge) {
        int xTmp, zTmp;
        if (inputPosition.x >= 0) {
            xTmp = -mazeEasternEdge;
        }
        else {
            xTmp = mazeEasternEdge;
        }

        if (inputPosition.z >= 0) {
            zTmp = -mazeNorthernEdge;
        }
        else {
            zTmp = mazeNorthernEdge;
        }

        return new Vector3(xTmp, 0, zTmp);
    }

    int MeasureHorizontal(Vector3 position01, Vector3 position02) {
        return Mathf.Abs(Mathf.RoundToInt(position01.x - position02.x));
    }

    int MeasureVertical(Vector3 position01, Vector3 position02) {
        return Mathf.Abs(Mathf.RoundToInt(position01.z - position02.z));
    }

    int FindLargestValue(List<int> values) {
        int tmpValue = mazeData.westernEdge < mazeData.southernEdge ? mazeData.southernEdge : mazeData.westernEdge;
        return values.Prepend(tmpValue).Max();
    }

    int FindSmallestValue(List<int> values) {
        int tmpValue = mazeData.northernEdge < mazeData.easternEdge ? mazeData.easternEdge : mazeData.northernEdge;
        return values.Prepend(tmpValue).Min();
    }

    int FindLcm(Vector3 midPoint, int distance, int mazeNorthernEdge, int mazeEasternEdge) {
        Vector3 farCorner = FarthestCorner(midPoint, mazeNorthernEdge, mazeEasternEdge);
        int farthestDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farCorner));
        int otherDistance = distance + farthestDistance;
        int lcm01 = ReduceLCM(LCM(distance, farthestDistance), mazeNorthernEdge, mazeEasternEdge);
        int lcm02 = ReduceLCM(LCM(distance, otherDistance), mazeNorthernEdge, mazeEasternEdge);
        int lcm03 = ReduceLCM(LCM(farthestDistance, otherDistance), mazeNorthernEdge, mazeEasternEdge);
        return lcm01 + lcm02 + lcm03;
    }

    /* Find the Greatest Common Denominator/Divisor */
    int GCD(int int01, int int02) {
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
    int LCM(int distance01, int distance02) {
        int lcmTmp = distance01 / GCD(distance01, distance02);
        return distance02 * lcmTmp;
    }

    int ReduceLCM(int lcm, int mazeNorthernEdge, int mazeEasternEdge) {
        int tmp = lcm;
        if (mazeNorthernEdge == mazeEasternEdge || mazeNorthernEdge > mazeEasternEdge) {
            while (tmp >= mazeEasternEdge) {
                tmp /= mazeEasternEdge;
            }
        }
        else {
            while (tmp >= mazeNorthernEdge) {
                tmp /= mazeNorthernEdge;
            }
        }

        return tmp;
    }


    /** maze grid segment selection & sorting functions **/
    List<Vector3> SortRows(List<Vector3> pathList) {
        /* https://stackoverflow.com/questions/36070425/simplest-way-to-sort-a-list-of-vector3s */
        return pathList.OrderBy(v => v.x).ToList();
    }

    List<Vector3> SortColumns(List<Vector3> pathList) {
        /* https://stackoverflow.com/questions/36070425/simplest-way-to-sort-a-list-of-vector3s */
        return pathList.OrderBy(v => v.z).ToList();
    }

    /* 'Slice' the list down to the specified row/Z value */
    List<Vector3> SliceRow(List<Vector3> sorted, int rowToSlice) {
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
    List<Vector3> SliceColumn(List<Vector3> sorted, int columnToSlice) {
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

    List<Vector3> SortAndSliceRows(List<Vector3> pathList, int rowToSlice) {
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

    List<int> FindTheZs(List<Vector3> path) {
        List<int> foundZs = path.Select(slice => Mathf.RoundToInt(slice.z)).ToList();
        HashSet<int> shortened = new(foundZs);

        return shortened.ToList();
    }

    List<int> FindTheXs(List<Vector3> path) {
        List<int> foundXs = path.Select(slice => Mathf.RoundToInt(slice.x)).ToList();
        HashSet<int> shortened = new(foundXs);

        return shortened.ToList();
    }


    /** Colour changing/blending functions **/
    public static Color ChangeColours(string addOrBlend, List<GameObject> waypointsList) {
        Color returningColour = new();

        switch (addOrBlend) {
            case "add":
                returningColour = AddColoursTogether(waypointsList);
                break;
            case "blend":
                returningColour = BlendColoursTogether(waypointsList);
                break;
        }

        return returningColour;
    }

    public static Color ChangeColours(string addOrBlend, List<Waypoint> waypointsList) {
        Color returningColour = new();

        switch (addOrBlend) {
            case "add":
                returningColour = AddColoursTogether(waypointsList);
                break;
            case "blend":
                returningColour = BlendColoursTogether(waypointsList);
                break;
        }

        return returningColour;
    }

    public static Color ChangeColours(string switchAddOrBlend, Color playersColor, Color waypointColor) {
        Color returningColour = new();
        switch (switchAddOrBlend) {
            case "switch":
                returningColour = waypointColor;
                break;
            case "add":
                returningColour = AddColoursTogether(playersColor, waypointColor);
                break;
            case "blend":
                returningColour = BlendColoursTogether(playersColor, waypointColor);
                break;
        }

        return returningColour;
    }

    static Color AddColoursTogether(Color playersColour, Color waypointColor) {
        Color mixedColour;
        if (waypointColor == Color.white || waypointColor == Color.black) {
            mixedColour = BlendColoursTogether(playersColour, waypointColor);
        }
        else if (playersColour == Color.white) {
            mixedColour = playersColour - waypointColor;
        }
        else {
            mixedColour = playersColour + waypointColor;
        }

        return mixedColour;
    }

    static Color AddColoursTogether(List<GameObject> waypointsList) {
        int waypoint01 = Random.Range(0, waypointsList.Count);
        int waypoint02 = Random.Range(0, waypointsList.Count);

        mazeData.hintColour01 =
            waypointsList[waypoint01].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        mazeData.hintColour02 =
            waypointsList[waypoint02].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;

        Color tmpColour = mazeData.hintColour01 + mazeData.hintColour02;
        return tmpColour;
    }

    static Color AddColoursTogether(List<Waypoint> waypointsList) {
        int waypoint01 = Random.Range(0, waypointsList.Count);
        int waypoint02 = Random.Range(0, waypointsList.Count);
        
        mazeData.hintColour01 = waypointsList[waypoint01].GetColour();
        mazeData.hintColour02 = waypointsList[waypoint02].GetColour();

        Color tmpColour = mazeData.hintColour01 + mazeData.hintColour02;
        return tmpColour;
    }

    static Color BlendColoursTogether(Color playersColour, Color waypointColor) {
        Color blendedColour = Color.Lerp(playersColour, waypointColor, 0.5f);
        return blendedColour;
    }

    static Color BlendColoursTogether(List<GameObject> waypointsList) {
        int index01 = Random.Range(0, waypointsList.Count);
        int index02 = Random.Range(0, waypointsList.Count);

        mazeData.hintColour01 =
            waypointsList[index01].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
        mazeData.hintColour02 =
            waypointsList[index02].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;

        Color blendedColour = Color.Lerp(mazeData.hintColour01, mazeData.hintColour02, 0.5f);
        return blendedColour;
    }

    static Color BlendColoursTogether(List<Waypoint> waypointsList) {
        int index01 = Random.Range(0, waypointsList.Count);
        int index02 = Random.Range(0, waypointsList.Count);
        
        mazeData.hintColour01 = waypointsList[index01].GetColour();
        mazeData.hintColour02 = waypointsList[index02].GetColour();

        Color blendedColour = Color.Lerp(mazeData.hintColour01, mazeData.hintColour02, 0.5f);
        return blendedColour;
    }

    /** misc. functions **/
    void ResetCount() {
        count = 0;
    }

    /* by parsing a List through a HashSet duplicates are eliminated, as HashSets only contain unique values */
    List<Vector3> RemoveDuplicates(List<Vector3> pathList) {
        HashSet<Vector3> inputList = new(pathList);
        List<Vector3> shortened = inputList.ToList();

        mazeData.shortListed = true;
        return shortened.ToList();
    }


/* END GameManager() */

}