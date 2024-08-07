using System.Collections.Generic;
using UnityEngine;

/* check the relative orientation of vector positionB to vector positionA within a margin of +-1 unit */
public class CheckOrientation : MonoBehaviour {

    static int pathLength;
    static int count = 0;
    static List<Vector3> path;
    private static Vector3 tmpPosition;

    public static List<Vector3> Check(Vector3 pos01, Vector3 pos02) {
        Debug.Log("Checking alignment");
        if (TriangulateVectors.HorizontalAlignment(pos01, pos02)) {
            Debug.Log("Checking horizontal alignment: " + TriangulateVectors.HorizontalAlignment(pos01, pos02));
            if (TriangulateVectors.IsItRight(pos01, pos02)) {
                pathLength = TriangulateVectors.GetHorizontalDistance();
                Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, pos02) + " Distance is: " + pathLength);
                // path.Add(pos01);
                for (int i = 0; i < pathLength; i++) {
                    path.Add(new Vector3(pos01.x + i, pos01.y, pos01.z));
                }
            }
            else {
                pathLength = TriangulateVectors.GetHorizontalDistance();
                Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, pos02) + " Distance is: " + pathLength);
                // path.Add(pos01);
                for (int i = 0; i < pathLength; i++) {
                    path.Add(new Vector3(pos01.x - i, pos01.y, pos01.z));
                }
            }
        }
        else {
            tmpPosition = TriangulateVectors.UsingMins(pos01, pos02);
            Debug.Log("Adding a right angle position");
            Debug.Log("Added position is at: " + tmpPosition);
            if (TriangulateVectors.HorizontalAlignment(pos01, tmpPosition)) {
                Debug.Log("Checking horizontal alignment: " + TriangulateVectors.HorizontalAlignment(pos01, tmpPosition));
                if (TriangulateVectors.IsItRight(pos01, tmpPosition)) {
                    pathLength = TriangulateVectors.GetHorizontalDistance();
                    Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, tmpPosition) + " Distance is: " + pathLength);
                    // path.Add(pos01);
                    for (int i = 0; i < pathLength; i++) {
                        path.Add(new Vector3(pos01.x + i, pos01.y, pos01.z));
                    }
                }
                else {
                    pathLength = TriangulateVectors.GetHorizontalDistance();
                    Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, tmpPosition) + " Distance is: " + pathLength);
                    // path.Add(pos01);
                    for (int i = 0; i < pathLength; i++) {
                        path.Add(new Vector3(pos01.x - i, pos01.y, pos01.z));
                        
                    }
                }
            }
        }

        return path;
    }


}