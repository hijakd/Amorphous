using UnityEngine;

public class FindFarCorner : MonoBehaviour {

    static int xTemp;
    static int zTemp;

    public static Vector3 Find(Vector3 inputPos, int gridHeight, int gridWidth) {
        if (inputPos.x >= 0) {
            xTemp = -gridWidth;
        }
        else {
            xTemp = gridWidth;
        }

        if (inputPos.z >= 0) {
            zTemp = -gridHeight;
        }
        else {
            zTemp = gridWidth;
        }

        return new Vector3(xTemp, 0, zTemp);
    }

}