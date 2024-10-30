using System.Collections.Generic;
using UnityEngine;

public class SliceNSort : MonoBehaviour {

    /* return a sorted slice/portion of the path array at a given row of the maze grid */
    public static List<Vector3> SliceListRows(List<Vector3> path, int row) {
        int slicingCount = 0;
        List<Vector3> slice = new List<Vector3>();
        List<Vector3> sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing rows List");
        while (slicingCount < path.Count) {
            if (path[slicingCount].z == row) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListRows(slice, Mathf.RoundToInt(GameManager.xMinMax.x));
        return sortedSlice;
    }

    /* return a sorted slice/portion of the path array at a given column of the maze grid */
    public static List<Vector3> SliceListColumns(List<Vector3> path, int column) {
        int slicingCount = 0;
        List<Vector3> slice = new List<Vector3>();
        List<Vector3> sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing columns List");
        while (slicingCount < path.Count) {
            if (path[slicingCount].x == column) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListColumns(slice, Mathf.RoundToInt(GameManager.yMinMax.x));
        return sortedSlice;
    }

    /* for sorting an array slice based on X axis */
    public static List<Vector3> SortListRows(List<Vector3> list, int lowestValue) {
        List<Vector3> sorted = new List<Vector3>();
        int sortingCount = 0;
        int listSize = list.Count;

        // Debug.Log("Sorting Sliced List");
        while (sortingCount < listSize) {
            for (int i = 0; i < listSize; i++) {
                if (list[i].x == lowestValue) {
                    sorted.Add(list[i]);
                    sortingCount++;
                }
            }
            lowestValue++;
        }
        return sorted;
    }

    /* for sorting an array slice based on Z axis */
    private static List<Vector3> SortListColumns(List<Vector3> list, int lowestValue) {
        List<Vector3> sorted = new List<Vector3>();
        int sortingCount = 0;
        int listSize = list.Count;

        while (sortingCount < listSize) {
            for (int i = 0; i < listSize; i++) {
                if (list[i].z == lowestValue) {
                    sorted.Add(list[i]);
                    sortingCount++;
                }
            }
            lowestValue++;
        }
        return sorted;
    }
}