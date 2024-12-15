using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
// ReSharper disable PossibleLossOfFraction
// ReSharper disable InvalidXmlDocComment

// ReSharper disable InconsistentNaming

public class GameUtils : MonoBehaviour {

    public static IEnumerator WaitForRowSlice() {
        yield return new WaitUntil(() => GameManager.firstRowFound == true);
    }

    public static IEnumerator WaitForColSlice() {
        yield return new WaitUntil(() => GameManager.firstColFound == true);
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

    /* overloading Spawn function to set rotation */
    public static void Spawn(GameObject gameObject, Vector3 position, Material material) {
        gameObject.transform.position = position;
        gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;
        Instantiate(gameObject);
    }

    /* Spawn the east & west walls across a given row of the maze path */
    public static void SpawnEastWestWalls(List<Vector3> path, GameObject[] walls, Material material) {
        // Debug.Log("Spawning East/West Walls");

        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first west wall of the row */
                Spawn(walls[3], path[i], material);
            }
            else if (path[i - 1].x < path[i].x - 1) {
                /* spawn west wall at end of a break in the row */
                Spawn(walls[3], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last east wall of the row if the list ends before the boundary */
                Spawn(walls[1], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last east wall of the row */
                Spawn(walls[1], path[j], material);
            }
            else if (path[j - 1].x > path[j].x + 1) {
                /* spawn east wall at end of a break in the row */
                Spawn(walls[1], path[j], material);
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
        var farCorner = FarthestCorner(midPoint, mazeHeight, mazeWidth);
        var farthestDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farCorner));
        var otherDistance = distance + farthestDistance;
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
        var position = Vector3.Slerp(origin, destination, centreMargin);

        // normalizing the position
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);

        return ClampWithinBoundaries(position);
    }

    public static Vector3 TriangulateIntersection(Vector3 origin, Vector3 destination, float centreMargin, int modifier) {
        var position = Vector3.Slerp(origin, destination, centreMargin);
        var modifierX = 0f;
        var modifierY = 0f;
        var decimalMultiplier = 10;

        if (modifier > Mathf.Abs(1000) && modifier < Mathf.Abs(10000)) {
            decimalMultiplier = 1000;
        } else if (modifier > Mathf.Abs(100) && modifier < Mathf.Abs(1000)) {
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
        
        if (position.x <= GameManager.mazeWidth.x) {
            position.x = GameManager.mazeWidth.x;
        }
        else if (position.x > GameManager.mazeWidth.y) {
            position.x = GameManager.mazeWidth.y;
        }

        if (position.z <= GameManager.mazeHeight.x) {
            position.z = GameManager.mazeHeight.x;
        }
        else if (position.z > GameManager.mazeHeight.y) {
            position.z = GameManager.mazeHeight.y;
        }

        return position;
    }

}