using System.Collections.Generic;
using UnityEngine;

/* check the relative orientation of vector positionB to vector positionA within a margin of +-1 unit */
public class CheckOrientation : MonoBehaviour {

    static int pathLength;
    static int count, xPosition = 0;
    // static List<Vector3> path;
    private static Vector3 tmpPosition;
    static GameObject miscPiece;

    // public static List<Vector3> Check(Vector3 pos01, Vector3 pos02, GameObject otherPiece) {
        // non debugging version of function signature
        public static List<Vector3> Check(Vector3 pos01, Vector3 pos02, List<Vector3> path) {
        /** -= begin debugging elements =- */
        // miscPiece = otherPiece;
        /** -= end debugging elements =- */
        tmpPosition = new Vector3(0, 0, 0);
        Debug.Log("Checking alignment");
        if (TriangulateVectors.HorizontalAlignment(pos01, pos02)) {
            Debug.Log("Checking horizontal alignment: " + TriangulateVectors.HorizontalAlignment(pos01, pos02));
            if (TriangulateVectors.IsItRight(pos01, pos02)) {
                pathLength = TriangulateVectors.GetHorizontalDistance();
                Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, pos02) + " Distance is: " +
                          pathLength);

                // path.Add(pos01);
                for (int i = 0; i < pathLength; i++) {
                    path.Add(new Vector3(pos01.x + i, pos01.y, pos01.z));
                    Debug.Log("populating path array: " + path[i]);
                }
            }
            else {
                pathLength = TriangulateVectors.GetHorizontalDistance();
                Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, pos02) + " Distance is: " +
                          pathLength);

                // path.Add(pos01);
                for (int i = 0; i < pathLength; i++) {
                    path.Add(new Vector3(pos01.x - i, pos01.y, pos01.z));
                    Debug.Log("populating path array: " + path[i]);
                }
            }
        }
        else {
            if (pos01.z < pos02.z) {
                tmpPosition.x = pos02.x;
                tmpPosition.z = pos01.z;
                xPosition = 2;
            }
            else {
                tmpPosition.x = pos01.x;
                tmpPosition.z = pos02.z;
                xPosition = 1;
            }

            // /** -= begin debugging elements =- */ 
            // miscPiece.transform.position = tmpPosition;
            // Instantiate(miscPiece);
            // Debug.Log("Adding a right angle position");
            // Debug.Log("Added position is at: " + tmpPosition);
            // /** -= end debugging elements =- */ 

            if (xPosition == 1) {
                if (tmpPosition.x < pos02.x) {
                    pathLength = Mathf.RoundToInt(pos02.x - tmpPosition.x);
                }
                else {
                    pathLength = Mathf.RoundToInt(tmpPosition.x - pos02.x);
                }
                
                Debug.Log("xPosition 1, pathLength check: " + pathLength);
                path.Add(new Vector3(pos02.x, tmpPosition.y, tmpPosition.z));
                for (int i = 0; i < pathLength; i++) {
                    path.Add(new Vector3(pos02.x + i, tmpPosition.y, tmpPosition.z));
                    Debug.Log("populating path array: " + path[i]);
                }
            }

            if (xPosition == 2) {
                if (tmpPosition.x < pos01.x) {
                    pathLength = Mathf.RoundToInt(pos01.x - tmpPosition.x);
                }
                else {
                    pathLength = Mathf.RoundToInt(tmpPosition.x - pos01.x);
                }
                
                Debug.Log("xPosition 2, pathLength check: " + pathLength);
                path.Add(new Vector3(pos01.x, tmpPosition.y, tmpPosition.z));
                for (int i = 0; i < pathLength; i++) {
                    path.Add(new Vector3(pos01.x + i, tmpPosition.y, tmpPosition.z));
                    Debug.Log("populating path array: " + path[i]);
                }
            }

            // if (TriangulateVectors.IsItRight(pos01, tmpPosition)) {
            //     pathLength = Mathf.RoundToInt(pos01.x - tmpPosition.x);
            //     Debug.Log("1st pathLength check: " + pathLength);
            //     Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, tmpPosition) + " Distance is: " + pathLength);
            //     Debug.Log("2nd pathLength check: " + pathLength);
            //     // path.Add(pos01);
            //     for (int i = 0; i < pathLength; i++) {
            //         path.Add(new Vector3(pos01.x + i, tmpPosition.y, tmpPosition.z));
            //         Debug.Log("populating path array: " + path[i]);
            //     }
            // }
            // else {
            //     pathLength = Mathf.RoundToInt(tmpPosition.x - pos01.x);
            //     Debug.Log("1st pathLength check: " + pathLength);
            //     Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, tmpPosition) + " Distance is: " + pathLength);
            //     Debug.Log("2nd pathLength check: " + pathLength);
            //     // path.Add(pos01);
            //     for (int i = 0; i < pathLength; i++) {
            //         path.Add(new Vector3(pos01.x - i, tmpPosition.y, tmpPosition.z));
            //         Debug.Log("populating path array: " + path[i]);
            //     }
            // }
        }

        return path;
    }


}