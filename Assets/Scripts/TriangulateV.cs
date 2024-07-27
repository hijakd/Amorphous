using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulateV : MonoBehaviour {

    static Vector3 position;
    static private float backLeg;
    static private float frontLeg;
    // private int distance;
    
    public static Vector3 Position(Vector3 position01, Vector3 position02, int distance, Vector2 xMinMax,
        Vector2 yMinMax) {
        int sectorSize = Mathf.RoundToInt(FindSectorSize.Sector(position01, position02));
        backLeg = distance * Random.Range(0.42f, 0.58f);
        frontLeg = distance - backLeg;
        
        position.x = Mathf.RoundToInt((position02.x - frontLeg) + (position01.x + backLeg));
        position.y = 0f;
        position.z = Mathf.RoundToInt((position02.z - frontLeg) + (position01.z + backLeg));

        /* check position is within the boundaries, if not clamp them in */
        if (position.x <= xMinMax.x) {
            position.x = xMinMax.x;
        }
        else if (position.x > xMinMax.y) {
            position.x = xMinMax.y;
        }

        if (position.z <= yMinMax.x) {
            position.z = yMinMax.x;
        }
        else if (position.z > yMinMax.y) {
            position.z = yMinMax.y;
        }

        return position;
    }
}