using UnityEngine;

public class TriangulateVectors : MonoBehaviour {

    static Vector3 position;
    private static bool horizAligned, vertAligned, isForward, isRight;
    private static bool switchMinMax = false;
    static float backLeg, frontLeg;
    static int horizDistance, vertDistance;
    // private int distance;
    
    /*public static Vector3 Position(Vector3 position01, Vector3 position02, int distance, Vector2 horizontalBounds,
        Vector2 verticalBounds) {
        int sectorSize = Mathf.RoundToInt(FindSectorSize.Sector(position01, position02));
        backLeg = distance * Random.Range(0.42f, 0.58f);
        frontLeg = distance - backLeg;
        
        position.x = Mathf.RoundToInt((position02.x - frontLeg) + (position01.x + backLeg));
        position.y = 0f;
        position.z = Mathf.RoundToInt((position02.z - frontLeg) + (position01.z + backLeg));
        
        /* check position is within the boundaries, if not clamp them in #1#
        if (position.x <= horizontalBounds.x) {
            position.x = horizontalBounds.x;
        }
        else if (position.x > horizontalBounds.y) {
            position.x = horizontalBounds.y;
        }

        if (position.z <= verticalBounds.x) {
            position.z = verticalBounds.x;
        }
        else if (position.z > verticalBounds.y) {
            position.z = verticalBounds.y;
        }

        return position;
    }*/

    // public static Vector3 Slerping(Vector3 position01, Vector3 position02, Vector2 horizontalBounds, Vector2 verticalBounds) {
    public static Vector3 Position(Vector3 position01, Vector3 position02, Vector2 horizontalBounds, Vector2 verticalBounds) {
        position = Vector3.Slerp(position01, position02, Random.Range(0.42f, 0.58f));
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);
        
        /* check position is within the boundaries, if not clamp them in */
        if (position.x <= horizontalBounds.x) {
            position.x = horizontalBounds.x;
        }
        else if (position.x > horizontalBounds.y) {
            position.x = horizontalBounds.y;
        }

        if (position.z <= verticalBounds.x) {
            position.z = verticalBounds.x;
        }
        else if (position.z > verticalBounds.y) {
            position.z = verticalBounds.y;
        }
        
        return position;
    }

    /* find an intersecting vector position at right angles to two given positions */
    public static Vector3 UsingMins(Vector3 position01, Vector3 position02) {
        Debug.Log("UsingMins, switchMinMax: " + switchMinMax);
        position = Vector3.Min(position01, position02);
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);
        // if (!switchMinMax) {
        //     if (position.Equals(position01)) {
        //         Debug.Log("Switching to UsingMaxs");
        //         UsingMaxs(position01, position02);
        //         switchMinMax = true;
        //     } else if (position.Equals(position02)) {
        //         Debug.Log("Switching to UsingMaxs");
        //         UsingMaxs(position01, position02);
        //         switchMinMax = true;
        //     }
        // }

        return position;
    }
    
    /* find an intersecting vector position at right angles to two given positions */
    public static Vector3 UsingMaxs(Vector3 position01, Vector3 position02) {
        Debug.Log("UsingMaxs, switchMinMax: " + switchMinMax);
        position = Vector3.Max(position01, position02);
        position.x = Mathf.RoundToInt(position.x);
        position.y = 0f;
        position.z = Mathf.RoundToInt(position.z);
        // if (!switchMinMax) {
        //     if (position.Equals(position01)) {
        //         Debug.Log("Switching to UsingMins");
        //         UsingMins(position01, position02);
        //         switchMinMax = true;
        //     } else if (position.Equals(position02)) {
        //         Debug.Log("Switching to UsingMins");
        //         UsingMins(position01, position02);
        //         switchMinMax = true;
        //     }
        // }
        return position;
    }
    
    /* are the 'X' values aligned within margin */
    public static bool VerticalAlignment(Vector3 position01, Vector3 position02) {
        if (position01.x == position02.x || position01.x == position02.x - 1 || position01.x == position02.x + 1) {
            vertAligned = true;
            Debug.Log("Triangulating vertAligned: " + vertAligned);
        }

        return vertAligned;
    }

    /* are the 'Z' values aligned within margin */
    public static bool HorizontalAlignment(Vector3 position01, Vector3 position02) {
        if (position01.z == position02.z || position01.z == position02.z - 1 || position01.z == position02.z + 1) {
            horizAligned = true;
            Debug.Log("Triangulating horizAligned: " + horizAligned);
        }

        return horizAligned;
    }

    /* check if positionB is ahead/north of positonA & by how far */
    public static bool IsItForward(Vector3 position01, Vector3 position02) {
        if (vertAligned == true) {
            if (position01.z < position02.z) {
                vertDistance = Mathf.RoundToInt(position02.z - position01.z);
                Debug.Log("Triangulating vertDistance: " + vertDistance);
                isForward = true;
                Debug.Log("Triangulating forward: " + isForward);
            }
            else if (position01.z > position02.z) {
                vertDistance = Mathf.RoundToInt(position01.z - position02.z);
                Debug.Log("Triangulating vertDistance: " + vertDistance);
                isForward = false;
                Debug.Log("Triangulating forward: " + isForward);
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
                Debug.Log("Triangulating horizDistance: " + horizDistance);
                isRight = true;
                Debug.Log("Triangulating right: " + isRight);
            } else if (position01.x > position02.x) {
                horizDistance = Mathf.RoundToInt(position01.x - position02.x);
                Debug.Log("Triangulating horizDistance: " + horizDistance);
                isRight = false;
                Debug.Log("Triangulating right: " + isRight);
            }
            else {
                horizDistance = 0;
            }
        }

        return isRight;
    }

    /* getter function for the distance between two vector positions */
    public static int GetVerticalDistance() {
        return vertDistance;
    }

    /* getter function for the distance between two vector positions */
    public static int GetHorizontalDistance() {
        return horizDistance;
    }
}