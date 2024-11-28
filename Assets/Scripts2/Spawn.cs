using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    static int north = 0;
    static int east = 1;
    static int south = 2;
    static int west = 3;

    public static void Object(GameObject gameObject, Vector3 position) {
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }

    public static void Object(GameObject gameObject, Vector3 position, Quaternion rotation) {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        Instantiate(gameObject);
    }

    public static void Object(GameObject gameObject, Vector3 position, Material material) {
        gameObject.transform.position = position;
        gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;
        Instantiate(gameObject);
    }

    /* Spawn the east & west walls across a given row of the maze path */
    public static void EastWestWalls(List<Vector3> path, GameObject[] walls, Material material) {
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first west wall of the row */
                Object(walls[west], path[i], material);
            }
            else if (path[i - 1].x < path[i].x - 1) {
                /* spawn west wall at end of a break in the row */
                Object(walls[west], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last east wall of the row if the list ends before the boundary */
                Object(walls[east], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last east wall of the row */
                Object(walls[east], path[j], material);
            }
            else if (path[j - 1].x > path[j].x + 1) {
                /* spawn east wall at end of a break in the row */
                Object(walls[east], path[j], material);
            }
        }
    }


    /* Spawn the north & south walls along a given column of the maze path */
    /* this function parses the maze grid from bottom to the top */
    public static void NorthSouthWalls(List<Vector3> path, GameObject[] walls, Material material) {
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first south wall of the row */
                Object(walls[south], path[i], material);
            }
            else if (path[i - 1].z < path[i].z - 1) {
                /* spawn south wall at end of a break in the row */
                Object(walls[south], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last north wall of the row if the list ends before the boundary */
                Object(walls[north], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last north wall of the row */
                Object(walls[north], path[j], material);
            }
            else if (path[j - 1].z > path[j].z + 1) {
                /* spawn north wall at end of a break in the row */
                Object(walls[north], path[j], material);
            }
        }
    }

}