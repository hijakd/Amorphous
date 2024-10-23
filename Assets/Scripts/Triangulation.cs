using UnityEngine;

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
        if (position.x <= GameManager.xMinMax.x) {
            position.x = GameManager.xMinMax.x;
        }
        else if (position.x > GameManager.xMinMax.y) {
            position.x = GameManager.xMinMax.y;
        }

        if (position.z <= GameManager.yMinMax.x) {
            position.z = GameManager.yMinMax.x;
        }
        else if (position.z > GameManager.yMinMax.y) {
            position.z = GameManager.yMinMax.y;
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
        if (vertAligned == true) {
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
        if (horizAligned == true) {
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
        if (horizAligned == true) {
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
        if (vertAligned == true) {
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

}