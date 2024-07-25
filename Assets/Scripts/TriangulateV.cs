using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulateV : MonoBehaviour {

    static Vector3 position;
    static private float backLeg;
    static private float frontLeg;
    // private int distance;
    
    public static Vector3 Position(Vector3 position01, Vector3 position02, int distance) {
        int sectorSize = Mathf.RoundToInt(FindSectorSize.Sector(position01, position02));
        backLeg = distance * Random.Range(0.42f, 0.58f);
        frontLeg = distance - backLeg;

        position.x = (position02.x - frontLeg) + (position01.x + backLeg);
        position.y = 0f;
        position.z = (position02.z - frontLeg) + (position01.z + backLeg);
        
        return position;
    }


}
