using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulateV : MonoBehaviour {

    static Vector3 position;
    static private float backLeg;
    static private float frontLeg;
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
}