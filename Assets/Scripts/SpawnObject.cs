using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {

    // spawn a given GameObject at a given position
    public static void Spawn(GameObject gameObject, Vector3 position) {
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }

    // overloading Spawn function to set rotation
    public static void Spawn(GameObject gameObject, Vector3 position, Quaternion rotation) {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        Instantiate(gameObject);
    }

    public static void Spawn(GameObject gameObject, Vector3 position, Material material) {
        gameObject.transform.position = position;
        gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = material;

        // gameObject.
        Instantiate(gameObject);
    }

    
    /* Spawn the east & west walls across a given row of the maze path */
    public static void SpawnEastWestWalls(List<Vector3> path, GameObject[] walls, Material material) {
        Debug.Log("Spawning East/West Walls");

        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first west wall of the row */
                Spawn(walls[3], path[i], material);
            }
            else if (path[i - 1].x < path[i].x - 1) {
                /* spawn west wall at end of a break in the row */
                Spawn(walls[3], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last east wall of the row if the list ends before the boundary */
                Spawn(walls[1], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last east wall of the row */
                Spawn(walls[1], path[j], material);
            }
            else if (path[j - 1].x > path[j].x + 1) {
                /* spawn east wall at end of a break in the row */
                Spawn(walls[1], path[j], material);
            }
        }
    }

    /* Spawn the north & south walls along a given column of the maze path */
    public static void SpawnNorthSouthWalls(List<Vector3> path, GameObject[] walls, Material material) {
        Debug.Log("Spawning North/South Walls");
        for (int i = 0; i < path.Count; i++) {
            if (i == 0) {
                /* spawn the first north wall of the row */
                Spawn(walls[2], path[i], material);
            }
            else if (path[i - 1].z < path[i].z - 1) {
                /* spawn north wall at end of a break in the row */
                Spawn(walls[2], path[i], material);
            }
            else if (i == path.Count - 1) {
                /* spawn the last south wall of the row if the list ends before the boundary */
                Spawn(walls[0], path[i], material);
            }
        }

        /* reserving the List to spawn the east walls */
        path.Reverse();

        for (int j = 0; j < path.Count; j++) {
            if (j == 0) {
                /* spawn the last south wall of the row */
                Spawn(walls[0], path[j], material);
            }
            else if (path[j - 1].z > path[j].z + 1) {
                /* spawn south wall at end of a break in the row */
                Spawn(walls[0], path[j], material);
            }
        }
    }
}