using System.Collections.Generic;
using UnityEngine;

/* check the relative orientation of vector positionB to vector positionA within a margin of +-1 unit */
public class CheckOrientation : MonoBehaviour {

    private static int pathLength;
    private static int count = 0;
    private static int xPosition = 0;
    private static Vector3 tmpPosition;
    private static GameObject miscPiece;

    
    public static void CheckHorizontal(Vector3 pos01, Vector3 pos02, List<Vector3> path) {
        tmpPosition = new Vector3(0, 0, 0);
        // Debug.Log("Checking alignment");
        if (Triangulation.HorizontalAlignment(pos01, pos02)) {
            // Debug.Log("Checking horizontal alignment: " + Triangulation.HorizontalAlignment(pos01, pos02));
            if (Triangulation.IsItRight(pos01, pos02)) {
                pathLength = Triangulation.GetHorizontalDistance();
                // Debug.Log("Check if is right: " + Triangulation.IsItRight(pos01, pos02) + " Distance is: " + pathLength);
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x + count, pos01.y, pos01.z));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = Triangulation.GetHorizontalDistance();
                // Debug.Log("Check if is right: " + Triangulation.IsItRight(pos01, pos02) + " Distance is: " + pathLength);
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x - count, pos01.y, pos01.z));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
        }
        else {
            // Debug.Log("Skipped HorizAlign, creating a right angle position");
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
                if (Triangulation.HorizontalAlignment(tmpPosition, pos02)) {
                    // Debug.Log("Checking horizontal alignment: " + Triangulation.HorizontalAlignment(tmpPosition, pos02));
                    if (Triangulation.IsItRight(tmpPosition, pos02)) {
                        pathLength = Triangulation.GetHorizontalDistance();
                        // Debug.Log("Check if is right: " + Triangulation.IsItRight(tmpPosition, pos02) + " Distance is: " + pathLength);
                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(tmpPosition.x + count, tmpPosition.y, tmpPosition.z));
                            // Debug.Log("populating path array: " + path[count]);
                        }
                    }
                    else {
                        pathLength = Triangulation.GetHorizontalDistance();
                        // Debug.Log("Check if is right: " + Triangulation.IsItRight(tmpPosition, pos02) + " Distance is: " + pathLength);
                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(tmpPosition.x - count, tmpPosition.y, tmpPosition.z));
                            // Debug.Log("populating path array: " + path[count]);
                        }
                    }
                }
            }

            if (xPosition == 2) {
                if (Triangulation.HorizontalAlignment(pos01, tmpPosition)) {
                    // Debug.Log("Checking horizontal alignment: " + Triangulation.HorizontalAlignment(pos01, tmpPosition));
                    if (Triangulation.IsItRight(pos01, tmpPosition)) {
                        pathLength = Triangulation.GetHorizontalDistance();
                        // Debug.Log("Check if is right: " + Triangulation.IsItRight(pos01, tmpPosition) + " Distance is: " + pathLength);
                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(pos01.x + count, tmpPosition.y, tmpPosition.z));
                            // Debug.Log("populating path array: " + path[count]);
                        }
                    }
                    else {
                        pathLength = Triangulation.GetHorizontalDistance();
                        // Debug.Log("Check if is right: " + Triangulation.IsItRight(pos01, tmpPosition) + " Distance is: " + pathLength);
                        for (count = 0; count <= pathLength; count++) {
                            path.Add(new Vector3(pos01.x - count, tmpPosition.y, tmpPosition.z));
                            // Debug.Log("populating path array: " + path[count]);
                        }
                    }
                }
            }
        }
    }

    public static void CheckVertical(Vector3 pos01, Vector3 pos02, List<Vector3> path) {
        /* positions are within horizontal alignment margins */
        if (Triangulation.VerticalAlignment(pos01, pos02)) {
            // Debug.Log("Checking vertical alignment: " + Triangulation.VerticalAlignment(pos01, pos02));
            if (Triangulation.IsItForward(pos01, pos02)) {
                // Debug.Log("Check if is forward: " + Triangulation.IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                pathLength = Triangulation.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x, pos01.y, pos01.z + count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = Triangulation.GetVerticalDistance();
                // Debug.Log("Check if is forward: " + Triangulation.IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(pos01.x, pos01.y, pos01.z - count));
                    // Debug.Log("populating path array: " + path[count]);
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

        if (Triangulation.VerticalAlignment(pos01, tmpPosition)) {
            // Debug.Log("Checking vertical alignment: " + Triangulation.VerticalAlignment(pos01, pos02));
            if (Triangulation.IsItForward(pos01, tmpPosition)) {
                // Debug.Log("Check if is forward: " + Triangulation.IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                pathLength = Triangulation.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, pos01.z + count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = Triangulation.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, pos01.z - count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
        }
        
        if (Triangulation.VerticalAlignment(tmpPosition, pos02)) {
            // Debug.Log("Checking vertical alignment: " + Triangulation.VerticalAlignment(pos01, pos02));
            if (Triangulation.IsItForward(tmpPosition, pos02)) {
                // Debug.Log("Check if is forward: " + Triangulation.IsItForward(pos01, pos02) + " Distance is: " + pathLength);
                pathLength = Triangulation.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z + count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
            else {
                pathLength = Triangulation.GetVerticalDistance();
                for (count = 0; count <= pathLength; count++) {
                    path.Add(new Vector3(tmpPosition.x, tmpPosition.y, tmpPosition.z - count));
                    // Debug.Log("populating path array: " + path[count]);
                }
            }
        }
    }


    public static void NextTile(Vector3 pos01, Vector3 pos02, GameObject wallPanel) {
        // string north = "north";
        // string east = "east";
        // string south = "south";
        // string west = "west";
        string nextDirection = "";
        // bool firstTile = true;

        if (pos01.x < pos02.x && pos01.z == pos02.z) {
            nextDirection = "goEeast";
        } else if (pos01.x > pos02.x && pos01.z == pos02.z) {
            nextDirection = "goWest";
        } else if (pos01.x == pos02.x && pos01.z < pos02.z) {
            nextDirection = "goNorth";
        } else if (pos01.x == pos02.x && pos01.x > pos02.x) {
            nextDirection = "goSouth";
        }
        else {
            nextDirection = "";
        }

        switch (nextDirection) {
            case "goEeast":
                SpawnObject.Spawn(wallPanel, pos01, Quaternion.Euler(0, 0, 0));
                SpawnObject.Spawn(wallPanel, pos01, Quaternion.Euler(0, 180, 0));
                break;
            default:
                Debug.Log("exiting switch");
                break;
        }
    }

}