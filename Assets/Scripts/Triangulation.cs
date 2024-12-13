using UnityEngine;
// ReSharper disable CompareOfFloatsByEqualityOperator

public class Triangulation : MonoBehaviour {

    static Vector3 position;
    private static bool horizAligned, vertAligned, isForward, isRight;
    // private static bool switchMinMax = false;
    static float backLeg, frontLeg;
    static int horizDistance, vertDistance;


    /* create an approximately "center" position between two points using */
    /* Slerp that is clamped within the maze boundaries, this is intended */
    /* to take a Random.Range() value to create a point close to center */
    /* within approx. 40/60 weighting */
    public static Vector3 Position(Vector3 position01, Vector3 position02,
        float centreMargin) {
        // position = Vector3.Slerp(position01, position02, Random.Range(0.42f, 0.58f));
        position = Vector3.Slerp(position01, position02, centreMargin);
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);

        /* check position is within the boundaries, if not clamp them in */
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

    /* find an intersecting vector position at right angles to two given positions */
    // public static Vector3 UsingMins(Vector3 position01, Vector3 position02) {
    //     // Debug.Log("UsingMins, switchMinMax: " + switchMinMax);
    //     position = Vector3.Min(position01, position02);
    //     position.x = Mathf.RoundToInt(position.x);
    //     position.y = 0f;
    //     position.z = Mathf.RoundToInt(position.z);
    //
    //     return position;
    // }

    /* find an intersecting vector position at right angles to two given positions */
    // public static Vector3 UsingMaxs(Vector3 position01, Vector3 position02) {
    //     // Debug.Log("UsingMaxs, switchMinMax: " + switchMinMax);
    //     position = Vector3.Max(position01, position02);
    //     position.x = Mathf.RoundToInt(position.x);
    //     position.y = 0f;
    //     position.z = Mathf.RoundToInt(position.z);
    //
    //     return position;
    // }

    /* are the 'X' values aligned within margin */
    public static bool VerticalAlignment(Vector3 position01, Vector3 position02) {
        if (position01.x == position02.x || position01.x == position02.x - 1 ||
            position01.x == position02.x + 1) {
            vertAligned = true;
            // Debug.Log("Triangulating vertAligned: " + vertAligned);
        }

        return vertAligned;
    }

    /* are the 'Z' values aligned within margin */
    public static bool HorizontalAlignment(Vector3 position01, Vector3 position02) {
        if (position01.z == position02.z || position01.z == position02.z - 1 ||
            position01.z == position02.z + 1) {
            horizAligned = true;
            // Debug.Log("Triangulating horizAligned: " + horizAligned);
        }

        return horizAligned;
    }

    /* check if positionB is ahead/north of positonA & by how far */
    public static bool IsItForward(Vector3 position01, Vector3 position02) {
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
    public static bool IsItRight(Vector3 position01, Vector3 position02) {
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

    public static int HorizontalDistance(Vector3 position01, Vector3 position02) {
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

    public static int VerticalDistance(Vector3 position01, Vector3 position02) {
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

    /* getter function for the distance between two vector positions */
    public static int GetVerticalDistance() {
        // Debug.Log("Triangulate, GetVerticalDistance: " + vertDistance);
        return vertDistance;
    }

    /* getter function for the distance between two vector positions */
    public static int GetHorizontalDistance() {
        // Debug.Log("Triangulate, GetHorizontalDistance: " + horizDistance);
        return horizDistance;
    }

    public static Vector3 Triangulate(int lcm, int rangeMax) {
        int count = 0;
        Vector3 lcmPos = new Vector3();
        while (count < lcm) {
            int select01 = Mathf.RoundToInt(Random.Range(0, rangeMax));
            int select02 = Mathf.RoundToInt(Random.Range(0, rangeMax));
            Vector3 coord01 = GameManager.selectableCoords[select01];
            Vector3 coord02 = GameManager.selectableCoords[select02];
            lcmPos = Position(coord01, coord02, Random.Range(GameManager.randVariance.x, GameManager.randVariance.y));
            count++;
        }
        return lcmPos;
    }
}