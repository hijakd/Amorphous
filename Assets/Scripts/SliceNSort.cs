using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable RedundantAssignment

public class SliceNSort : MonoBehaviour {
    
    /* return a sorted slice/portion of the path array at a given row of the maze grid */
    public static List<Vector3> SliceListRows(List<Vector3> path, int row) {
        int slicingCount = 0;
        var slice = new List<Vector3>();
        var sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing rows List");
        while (slicingCount < path.Count) {
            if (path[slicingCount].z == row) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListRows(slice, Mathf.RoundToInt(GameManager.xMinMax.x));
        GameManager.firstRowFound = true;
        
        return sortedSlice;
    }
    
    public static List<Vector3> SliceRows(List<Vector3> path, int row, int xMin) {
        int slicingCount = 0;
        var slice = new List<Vector3>();
        var sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing rows List (GM)");
        while (slicingCount < path.Count) {
            if (path[slicingCount].z == row) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListRows(slice, xMin);
        GManager.instance.foundFirstRow = true;
        
        return sortedSlice;
    }
    
    

    /* return a sorted slice/portion of the path array at a given column of the maze grid */
    public static List<Vector3> SliceListColumns(List<Vector3> path, int column) {
        int slicingCount = 0;
        var slice = new List<Vector3>();
        var sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing columns List");
        while (slicingCount < path.Count) {
            if (path[slicingCount].x == column) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListColumns(slice, Mathf.RoundToInt(GameManager.yMinMax.x));
        GameManager.firstColFound = true;
        
        return sortedSlice;
    }
    
    public static List<Vector3> SliceColumns(List<Vector3> path, int column, int yMin) {
        int slicingCount = 0;
        var slice = new List<Vector3>();
        var sortedSlice = new List<Vector3>();

        // Debug.Log("Slicing columns List (GM)");
        while (slicingCount < path.Count) {
            if (path[slicingCount].x == column) {
                slice.Add(path[slicingCount]);
            }
            slicingCount++;
        }
        sortedSlice = SortListColumns(slice, yMin);
        GManager.instance.foundFirstColumn = true;
        
        return sortedSlice;
    }

    /* for sorting an array slice based on X axis */
    private static List<Vector3> SortListRows(List<Vector3> list, int lowestValue) {
        var sorted = new List<Vector3>();
        int sortingCount = 0;
        int listSize = list.Count;

        // Debug.Log("Sorting Sliced List (R)");
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
        var sorted = new List<Vector3>();
        int sortingCount = 0;
        int listSize = list.Count;

        // Debug.Log("Sorting Sliced List (C)");
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