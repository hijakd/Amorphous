using System.Collections.Generic;
using UnityEngine;

/* check the relative orientation of vector positionB to vector positionA within a margin of +-1 unit */
public class CheckOrientation : MonoBehaviour {

    static int pathLength;
    static int count, xPosition = 0;
    private static Vector3 tmpPosition;
    static GameObject miscPiece;

    // non debugging version of function signature
    public static void CheckHorizontal(Vector3 pos01, Vector3 pos02, List<Vector3> path) {
        tmpPosition = new Vector3(0, 0, 0);
        Debug.Log("Checking alignment");
        if (TriangulateVectors.HorizontalAlignment(pos01, pos02)) {
            Debug.Log("Checking horizontal alignment: " + TriangulateVectors.HorizontalAlignment(pos01, pos02));
            if (TriangulateVectors.IsItRight(pos01, pos02)) {
                pathLength = TriangulateVectors.GetHorizontalDistance();

                Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, pos02) + " Distance is: " +
                          pathLength);

                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x + count, pos01.y, pos01.z));

                    Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = TriangulateVectors.GetHorizontalDistance();

                Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, pos02) + " Distance is: " +
                          pathLength);
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x - count, pos01.y, pos01.z));

                    Debug.Log("populating path array: " + path[count]);
                }
            }
        }
        else {
            Debug.Log("Skipped HorizAlign, creating a right angle position");
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

            if (xPosition == 1) {
                if (TriangulateVectors.HorizontalAlignment(tmpPosition, pos02)) {
                    Debug.Log("Checking horizontal alignment: " +
                              TriangulateVectors.HorizontalAlignment(tmpPosition, pos02));
                    if (TriangulateVectors.IsItRight(tmpPosition, pos02)) {
                        pathLength = TriangulateVectors.GetHorizontalDistance();

                        Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(tmpPosition, pos02) +
                                  " Distance is: " + pathLength);

                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(tmpPosition.x + count, tmpPosition.y, tmpPosition.z));

                            Debug.Log("populating path array: " + path[count]);
                        }
                    }
                    else {
                        pathLength = TriangulateVectors.GetHorizontalDistance();

                        Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(tmpPosition, pos02) +
                                  " Distance is: " + pathLength);

                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(tmpPosition.x - count, tmpPosition.y, tmpPosition.z));

                            Debug.Log("populating path array: " + path[count]);
                        }
                    }
                }
            }

            if (xPosition == 2) {
                if (TriangulateVectors.HorizontalAlignment(pos01, tmpPosition)) {
                    Debug.Log("Checking horizontal alignment: " +
                              TriangulateVectors.HorizontalAlignment(pos01, tmpPosition));
                    if (TriangulateVectors.IsItRight(pos01, tmpPosition)) {
                        pathLength = TriangulateVectors.GetHorizontalDistance();

                        Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, tmpPosition) +
                                  " Distance is: " + pathLength);

                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(pos01.x + count, tmpPosition.y, tmpPosition.z));

                            Debug.Log("populating path array: " + path[count]);
                        }
                    }
                    else {
                        pathLength = TriangulateVectors.GetHorizontalDistance();

                        Debug.Log("Check if is right: " + TriangulateVectors.IsItRight(pos01, tmpPosition) +
                                  " Distance is: " + pathLength);

                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(pos01.x - count, tmpPosition.y, tmpPosition.z));

                            Debug.Log("populating path array: " + path[count]);
                        }
                    }
                }
            }
        }
    }

    public static void CheckVertical(Vector3 pos01, Vector3 pos02, List<Vector3> path) {
        /* positions are within horizontal alignment margins */
        if (TriangulateVectors.VerticalAlignment(pos01, pos02)) {
            Debug.Log("Checking vertical alignment: " + TriangulateVectors.VerticalAlignment(pos01, pos02));
            if (TriangulateVectors.IsItForward(pos01, pos02)) {
                Debug.Log("Check if is forward: " + TriangulateVectors.IsItForward(pos01, pos02) + " Distance is: " +
                          pathLength);
                pathLength = TriangulateVectors.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x, pos01.y, pos01.z + count));
                    Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = TriangulateVectors.GetVerticalDistance();
                Debug.Log("Check if is forward: " + TriangulateVectors.IsItForward(pos01, pos02) + " Distance is: " +
                          pathLength);
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x, pos01.y, pos01.z - count));
                    Debug.Log("populating path array: " + path[count]);
                }
            }
        }

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

        if (TriangulateVectors.VerticalAlignment(pos01, tmpPosition)) {
            Debug.Log("Checking vertical alignment: " + TriangulateVectors.VerticalAlignment(pos01, pos02));
            if (TriangulateVectors.IsItForward(pos01, tmpPosition)) {
                Debug.Log("Check if is forward: " + TriangulateVectors.IsItForward(pos01, pos02) + " Distance is: " +
                          pathLength);
                pathLength = TriangulateVectors.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, pos01.z + count));
                    Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = TriangulateVectors.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, pos01.z - count));
                    Debug.Log("populating path array: " + path[count]);
                }
            }
        }


        if (TriangulateVectors.VerticalAlignment(tmpPosition, pos02)) {
            Debug.Log("Checking vertical alignment: " + TriangulateVectors.VerticalAlignment(pos01, pos02));
            if (TriangulateVectors.IsItForward(tmpPosition, pos02)) {
                Debug.Log("Check if is forward: " + TriangulateVectors.IsItForward(pos01, pos02) + " Distance is: " +
                          pathLength);
                pathLength = TriangulateVectors.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z + count));
                    Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = TriangulateVectors.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z - count));
                    Debug.Log("populating path array: " + path[count]);
                }
            }
        }
    }


}