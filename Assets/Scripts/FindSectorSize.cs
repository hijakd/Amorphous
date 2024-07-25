using UnityEngine;

public class FindSectorSize : MonoBehaviour
{
    // find the difference of 2 vectors, return the larger of horizontal or vertical values 
    public static float Sector(Vector3 position01, Vector3 position02) {
        float sectorSize;
        float x3 = Mathf.Abs(position01.x - position02.x);
        float z3 = Mathf.Abs(position01.z - position02.z);

        if (x3 > z3) {
            sectorSize = x3;
        }
        else {
            sectorSize = z3;
        }

        return sectorSize;
    }
}
