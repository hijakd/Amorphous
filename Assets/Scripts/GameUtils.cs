// #define DEBUGHORIZ
// #define DEBUGVERT

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable HeuristicUnreachableCode
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable PossibleLossOfFraction
// ReSharper disable InvalidXmlDocComment
// ReSharper disable InconsistentNaming

public class GameUtils : MonoBehaviour {

    private static Vector3 tmpPosition;
    private static int count, horizDistance, vertDistance, xPosition;
    private static bool horizAligned, vertAligned, isForward, isRight, debugHoriz, debugVert, firstRow, firstColumn;
    private const int north = 0;
    private const int east = 1;
    private const int south = 2;
    private const int west = 3;

    public static IEnumerator WaitForRowSlice() {
        yield return new WaitUntil(() => GameManager.firstRowFound == true);
    }

    public static IEnumerator WaitForColSlice() {
        yield return new WaitUntil(() => GameManager.firstColFound == true);
    }

    public static IEnumerator WaitForListShortening() {
        yield return new WaitUntil(() => GameManager.shortListed == true);
    }


    public static Vector3 RandomPosition(int minWidth, int maxWidth, int minHeight, int maxHeight) {
        // Debug.Log("Generating a random position");
        return new Vector3(Mathf.Round(Random.Range(minWidth, maxWidth)), 0,
            Mathf.Round(Random.Range(minHeight, maxHeight)));
    }

    /* spawn a given GameObject at a given position */
    public static void Spawn(GameObject gameObject, Vector3 position) {
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }

    /* overloading Spawn function to set rotation */
    public static void Spawn(GameObject gameObject, Vector3 position, Quaternion rotation) {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        Instantiate(gameObject);
    }

    /* overloading Spawn function to set material */
    public static void Spawn(GameObject gameObject, Vector3 position, Material material) {
        gameObject.transform.position = position;
        gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;
        Instantiate(gameObject);
    }

    /* Spawn the east & west walls across a given row of the maze path */
    public static void SpawnEastWestWalls(List<Vector3> path, GameObject[] walls, Material material) {
        // bool lastWallSpawned = false;
        // Debug.Log("Spawning East/West Walls");
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first east wall of the row */
                Spawn(walls[west], path[i], material);

                // Spawn(walls[west], path[i], GameManager.obsMaterial);
            }
            else if (path[i - 1].x < path[i].x - 1) {
                /* spawn east wall at end of a break in the row */
                Spawn(walls[west], path[i], material);
            }

            // else if (i == path.Count - 1) {
            //     /* spawn the last east wall of the row if the list ends before the boundary */
            //     Spawn(walls[east], path[i], GameManager.blueMaterial);
            //     lastWallSpawned = true;
            // }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last east wall of the row */
                Spawn(walls[east], path[j], material);

                // if (!lastWallSpawned) {
                // Spawn(walls[east], path[j], GameManager.pinkMaterial);
                // }
            }
            else if (path[j - 1].x > path[j].x + 1) {
                /* spawn east wall at end of a break in the row */
                Spawn(walls[east], path[j], material);

                // Spawn(walls[east], path[j], GameManager.purpleMaterial);
            }
        }
    }

    /* Spawn the north & south walls along a given column of the maze path */
    public static void SpawnNorthSouthWalls(List<Vector3> path, GameObject[] walls, Material material) {
        // Debug.Log("Spawning North/South Walls");
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first north wall of the row */
                Spawn(walls[2], path[i], material);
            }
            else if (path[i - 1].z < path[i].z - 1) {
                /* spawn north wall at end of a break in the row */
                Spawn(walls[2], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last south wall of the row if the list ends before the boundary */
                Spawn(walls[0], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last south wall of the row */
                Spawn(walls[0], path[j], material);
            }
            else if (path[j - 1].z > path[j].z + 1) {
                /* spawn south wall at end of a break in the row */
                Spawn(walls[0], path[j], material);
            }
        }
    }

    private static Vector3 FarthestCorner(Vector3 inputPosition, int mazeHeight, int mazeWidth) {
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
    private static int GCD(int int01, int int02) {
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
    private static int LCM(int distance01, int distance02) {
        int lcmTmp = distance01 / GCD(distance01, distance02);

        // int result = distance02 * lcmTmp;
        // return result;
        return distance02 * lcmTmp;
    }

    private static int ReduceLCM(int lcm, int mazeHeight, int mazeWidth) {
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

    public static int FindLcm(Vector3 midPoint, int distance, int mazeHeight, int mazeWidth) {
        Vector3 farCorner = FarthestCorner(midPoint, mazeHeight, mazeWidth);
        int farthestDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farCorner));
        int otherDistance = distance + farthestDistance;
        int lcm01 = ReduceLCM(LCM(distance, farthestDistance), mazeHeight, mazeWidth);
        int lcm02 = ReduceLCM(LCM(distance, otherDistance), mazeHeight, mazeWidth);
        int lcm03 = ReduceLCM(LCM(farthestDistance, otherDistance), mazeHeight, mazeWidth);
        return lcm01 + lcm02 + lcm03;
    }

    /* create an approximately "center" position between two points using */
    /* Slerp that is clamped within the maze boundaries, this is intended */
    /* to take a Random.Range() value to create a point close to center */
    /* within approx. 40/60 weighting */
    public static Vector3 TriangulateIntersection(Vector3 origin, Vector3 destination, float centreMargin) {
        Vector3 position = Vector3.Slerp(origin, destination, centreMargin);

        // normalizing the position
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);

        return ClampWithinBoundaries(position);
    }

    /* overloading the function to use an LCM value as a position modifier */
    public static Vector3
        TriangulateIntersection(Vector3 origin, Vector3 destination, float centreMargin, int modifier) {
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

        // normalizing the position
        position.x = Mathf.RoundToInt(position.x * (modifierX));
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z * (modifierY));

        return ClampWithinBoundaries(position);
    }

    /* check position is within the boundaries, if not clamp them in */
    private static Vector3 ClampWithinBoundaries(Vector3 position) {
        if (position.x <= GameManager._west) {
            position.x = GameManager._west;
        }
        else if (position.x > GameManager._east) {
            position.x = GameManager._east;
        }

        if (position.z <= GameManager._south) {
            position.z = GameManager._south;
        }
        else if (position.z > GameManager._north) {
            position.z = GameManager._north;
        }

        return position;
    }


    public static void PlotHorizontalPath(Vector3 origin, Vector3 destination, List<Vector3> path) {
        bool pDebug = false;
        int hDistance = MeasureHorizontal(origin, destination);
        if (IsOriginWest(origin, destination)) {
            if (pDebug) {
                Debug.Log("origin is to the west, the destination is: " + hDistance + " steps away");
            }

            for (count = 0; count <= hDistance; count++) {
                path.Add(new Vector3(origin.x + count, origin.y, origin.z));
            }
        }
        else {
            if (pDebug) {
                Debug.Log("origin is to the east, the destination is: " + hDistance + " steps away");
            }

            for (count = hDistance; count >= 0; count--) {
                path.Add(new Vector3(origin.x - count, origin.y, origin.z));
            }
        }
    }

    public static void PlotVerticalPath(Vector3 origin, Vector3 destination, List<Vector3> path) {
        bool pDebug = false;
        int vDistance = MeasureVertical(origin, destination);
        if (IsOriginSouth(origin, destination)) {
            if (pDebug) {
                Debug.Log("origin is to the north, the destination is: " + vDistance + " steps away");
            }

            for (count = vDistance; count >= 0; count--) {
                path.Add(new Vector3(destination.x, destination.y, destination.z - count));
            }
        }
        else {
            if (pDebug) {
                Debug.Log("origin is to the south, the destination is: " + vDistance + " steps away");
            }

            for (count = 0; count <= vDistance; count++) {
                path.Add(new Vector3(destination.x, destination.y, destination.z + count));
            }
        }
    }

    public static int MeasureHorizontal(Vector3 position01, Vector3 position02) {
        // int distance = Mathf.Abs(Mathf.RoundToInt(position01.x - position02.x));
        // return distance;
        return Mathf.Abs(Mathf.RoundToInt(position01.x - position02.x));
    }

    public static int MeasureVertical(Vector3 position01, Vector3 position02) {
        // int distance;
        // if (position01.z < position02.z) {
        //     distance = Mathf.RoundToInt(position01.z - position02.z);
        // }
        // else if (position01.z > position02.z) {
        //     distance = Mathf.RoundToInt(position02.z - position01.z);
        // }
        // else {
        //     distance = 0;
        // }
        // return distance;
        return Mathf.Abs(Mathf.RoundToInt(position01.z - position02.z));
    }

    private static bool HorizontalAlignment(Vector3 position01, Vector3 position02) {
        if (Mathf.Approximately(position01.z, position02.z) || Mathf.Approximately(position01.z, position02.z - 1) ||
            Mathf.Approximately(position01.z, position02.z + 1)) {
            horizAligned = true;

            // Debug.Log("Triangulating horizAligned: " + horizAligned);
        }

        return horizAligned;
    }

    private static bool VerticalAlignment(Vector3 position01, Vector3 position02) {
        if (Mathf.Approximately(position01.x, position02.x) || Mathf.Approximately(position01.x, position02.x - 1) ||
            Mathf.Approximately(position01.x, position02.x + 1)) {
            vertAligned = true;

            // Debug.Log("Triangulating vertAligned: " + vertAligned);
        }

        return vertAligned;
    }

    /* check if positionB is ahead/north of positonA & by how far */
    private static bool IsItForward(Vector3 position01, Vector3 position02) {
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

    /* check if positionB is right/east of positonA & by how far */
    private static bool IsItRight(Vector3 position01, Vector3 position02) {
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

    private static bool IsOriginWest(Vector3 position01, Vector3 position02) {
        bool isWest = position01.x < position02.x;
        return isWest;
    }

    private static bool IsOriginSouth(Vector3 position01, Vector3 position02) {
        bool isSouth = position01.z < position02.z;
        return isSouth;
    }

    public static void CheckHorizontal(Vector3 position01, Vector3 position02, List<Vector3> path) {
        tmpPosition = new Vector3(0, 0, 0);

// #if DEBUGHORIZ
//         Debug.Log("Checking alignment");
// #endif
        if (debugHoriz) Debug.Log("Checking alignment");
        if (HorizontalAlignment(position01, position02)) {
            if (debugHoriz) Debug.Log("Checking horizontal alignment: " + HorizontalAlignment(position01, position02));
            if (IsItRight(position01, position02)) {
                if (debugHoriz)
                    Debug.Log("Check if is right: " + IsItRight(position01, position02) + " Distance is: " +
                              horizDistance);
                for (count = 0; count <= horizDistance; count++) {
                    path.Add(new Vector3(position01.x + count, position01.y, position01.z));
                    if (debugHoriz) Debug.Log("populating horizontal path array: " + path[count]);
                }
            }
            else {
                if (debugHoriz)
                    Debug.Log("Check if is right: " + IsItRight(position01, position02) + " Distance is: " +
                              horizDistance);
                for (count = 0; count <= horizDistance; count++) {
                    path.Add(new Vector3(position01.x - count, position01.y, position01.z));
                    if (debugHoriz) Debug.Log("populating horizontal path array: " + path[count]);
                }
            }
        }
        else {
            if (debugHoriz) Debug.Log("Skipped HorizAlign, creating a right angle position");
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
                    if (debugHoriz)
                        Debug.Log("Checking horizontal alignment: " + HorizontalAlignment(tmpPosition, position02));
                    if (IsItRight(tmpPosition, position02)) {
                        if (debugHoriz)
                            Debug.Log("Check if is right: " + IsItRight(tmpPosition, position02) + " Distance is: " +
                                      horizDistance);
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(tmpPosition.x + count, tmpPosition.y, tmpPosition.z));
                            if (debugHoriz) Debug.Log("populating horizontal path array: " + path[count]);
                        }
                    }
                    else {
                        if (debugHoriz)
                            Debug.Log("Check if is right: " + IsItRight(tmpPosition, position02) + " Distance is: " +
                                      horizDistance);
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(tmpPosition.x - count, tmpPosition.y, tmpPosition.z));
                            if (debugHoriz) Debug.Log("populating horizontal path array: " + path[count]);
                        }
                    }
                }
            }

            if (xPosition == 2) {
                if (HorizontalAlignment(position01, tmpPosition)) {
                    if (debugHoriz)
                        Debug.Log("Checking horizontal alignment: " + HorizontalAlignment(position01, tmpPosition));
                    if (IsItRight(position01, tmpPosition)) {
                        if (debugHoriz)
                            Debug.Log("Check if is right: " + IsItRight(position01, tmpPosition) + " Distance is: " +
                                      horizDistance);
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(position01.x + count, tmpPosition.y, tmpPosition.z));
                            if (debugHoriz) Debug.Log("populating horizontal path array: " + path[count]);
                        }
                    }
                    else {
                        if (debugHoriz)
                            Debug.Log("Check if is right: " + IsItRight(position01, tmpPosition) + " Distance is: " +
                                      horizDistance);
                        for (count = 0; count <= horizDistance; count++) {
                            path.Add(new Vector3(position01.x - count, tmpPosition.y, tmpPosition.z));
                            if (debugHoriz) Debug.Log("populating horizontal path array: " + path[count]);
                        }
                    }
                }
            }
        }
    }


    public static void CheckVertical(Vector3 position01, Vector3 position02, List<Vector3> path) {
        /* positions are within horizontal alignment margins */
        if (VerticalAlignment(position01, position02)) {
            if (debugVert) Debug.Log("Checking vertical alignment: " + VerticalAlignment(position01, position02));
            if (IsItForward(position01, position02)) {
                if (debugVert)
                    Debug.Log("Check if is forward: " + IsItForward(position01, position02) + " Distance is: " +
                              vertDistance);
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(position01.x, position01.y, position01.z + count));
                    if (debugVert) Debug.Log("populating vertical path array: " + path[count]);
                }
            }
            else {
                if (debugVert)
                    Debug.Log("Check if is forward: " + IsItForward(position01, position02) + " Distance is: " +
                              vertDistance);
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(position01.x, position01.y, position01.z - count));
                    if (debugVert) Debug.Log("populating vertical path array: " + path[count]);
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
            if (debugVert) Debug.Log("Checking vertical alignment: " + VerticalAlignment(position01, position02));
            if (IsItForward(position01, tmpPosition)) {
                if (debugVert)
                    Debug.Log("Check if is forward: " + IsItForward(position01, position02) + " Distance is: " +
                              vertDistance);
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, position01.z + count));
                    if (debugVert) Debug.Log("populating vertical path array: " + path[count]);
                }
            }
            else {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, position01.z - count));
                    if (debugVert) Debug.Log("populating vertical path array: " + path[count]);
                }
            }
        }

        if (VerticalAlignment(tmpPosition, position02)) {
            if (debugVert) Debug.Log("Checking vertical alignment: " + VerticalAlignment(position01, position02));
            if (IsItForward(tmpPosition, position02)) {
                if (debugVert)
                    Debug.Log("Check if is forward: " + IsItForward(position01, position02) + " Distance is: " +
                              vertDistance);
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z + count));
                    if (debugVert) Debug.Log("populating vertical path array: " + path[count]);
                }
            }
            else {
                for (count = 0; count <= vertDistance; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z - count));
                    if (debugVert) Debug.Log("populating vertical path array: " + path[count]);
                }
            }
        }
    }

    /* by parsing a List through a HashSet are eliminated, as HashSets only contain unique values */
    public static List<Vector3> RemoveDuplicates(List<Vector3> pathList) {
        HashSet<Vector3> inputList = new(pathList);
        List<Vector3> shortened = inputList.ToList();

        // GameManager.shortListed = true;
        return shortened;
    }

    /* begin parsing the pathList by sorting the Z values in descending order  */
    public static List<Vector3> SortRows(List<Vector3> pathList, int rowHeight) {
        int counter = 0;
        int searchValue = rowHeight;
        List<Vector3> sorted = new();

        // Debug.Log("EARLY SortAndSliceRows");
        // Debug.Log("Sorting Sliced List");
        while (counter < pathList.Count) {
            for (int i = 0; i < pathList.Count; i++) {
                if (searchValue == Mathf.RoundToInt(pathList[i].z)) {
                    sorted.Add(pathList[i]);
                    counter++;
                }
            }

            searchValue--;
        }

        // GameManager._rowNumber = searchValue;
        // Debug.Log("LATE SortAndSliceRows");

        return sorted.ToList();
    }

    /* 'Slice' the list down to the specified row/Z value */
    public static List<Vector3> SliceRow(List<Vector3> sorted, int rowToSlice) {
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
    

    public static List<Vector3> SortAndSliceRows(List<Vector3> pathList, int rowToSlice) {
        int counter = 0;
        int searchValue = rowToSlice;
        List<Vector3> sorted = new();

        // Debug.Log("EARLY SortAndSliceRows");
        // Debug.Log("Sorting Sliced List");
        while (counter < pathList.Count) {
            for (int i = 0; i < pathList.Count; i++) {
                if (searchValue == Mathf.RoundToInt(pathList[i].z)) {
                    sorted.Add(pathList[i]);
                    counter++;
                }
            }

            searchValue--;
        }

        // GameManager._rowNumber = searchValue;
        // Debug.Log("LATE SortAndSliceRows");

        return SliceRow(sorted, rowToSlice).ToList();
    }


    // /* 'Slice' the list down to the specified row/Z value, then calling RemoveDuplicates to return unique values only */
    // private static List<Vector3> SliceRow(List<Vector3> sorted, int rowToSlice) {
    //     int counter = 0;
    //     int rowValue = rowToSlice > Mathf.RoundToInt(sorted[0].z) ? Mathf.RoundToInt(sorted[0].z) : rowToSlice;
    //     List<Vector3> sliced = new();
    //     // List<Vector3> sortedSlice = new();
    //     
    //     while (counter < sorted.Count) {
    //         if (Mathf.RoundToInt(sorted[counter].z) == rowValue) {
    //             sliced.Add(sorted[counter]);
    //         }
    //         counter++;
    //     }
    //
    //     List<Vector3> shortenedSlice = RemoveDuplicates(sliced);
    //     return SortV3ListOnX(shortenedSlice);
    // }


    private static int ContainsRowValue(List<Vector3> list, int rowToFind) {
        int rowIndex = 0;
        bool rowFound = false;

        while (!rowFound && rowIndex < list.Count) {
            Debug.Log("searching for the first row at " + rowIndex);
            if (rowToFind == Mathf.RoundToInt(list[rowIndex].z)) {
                rowFound = true;

                // return rowIndex;
            }

            rowIndex++;
        }

        Debug.Log("found the first row at " + rowIndex);

        return rowIndex;
    }

    // static List<Vector3> SortListV3OnX(List<Vector3> data)
    // {
    //     if (data.Count < 2) return data.ToList();  //Or throw error, or return null
    //
    //     var min = data.OrderBy(x => x.x).First();
    //     var max = data.OrderByDescending(x => x.x).First();
    //
    //     data.Remove(min);
    //     data.Remove(max);
    //
    //     data.Insert(0, min);
    //     data.Add(max);
    //     return data.ToList();
    // }

    static List<Vector3> SortV3ListOnX(List<Vector3> list) {
        if (list.Count < 2) return list.ToList();
        int count = 0;
        List<Vector3> sorted = new();
        int minimumXvalue = GameManager._west;

        // foreach (Vector3 t in list) {
        //     Debug.Log("early sorting: " + t);
        // }

        while (count < list.Count) {
            for (int i = 0; i < list.Count; i++) {
                if (minimumXvalue == Mathf.RoundToInt(list[i].x)) {
                    sorted.Add(list[i]);
                    count++;
                }
            }

            minimumXvalue++;
        }

        // foreach (Vector3 t in sorted) {
        //     Debug.Log("late sorting: " + t);
        // }

        // if (!firstRow) {
        //     GameManager._rowNumber = Mathf.RoundToInt(sorted[^1].x);
        //     firstRow = true;
        // }

        return sorted;
    }

}