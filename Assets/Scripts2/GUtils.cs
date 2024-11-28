using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable InconsistentNaming

public class GUtils : MonoBehaviour
{
    // static Vector3 rPos;
    private static int lcm_1;
    private static int lcm_2;
    private static int lcm_3;
    private static int thisDistance;
    private static int farthestDistance;
    private static Vector3 farCorner;

    public static Vector3 RandomPosition(int xMin, int xMax, int yMin, int yMax) {
        // rPos = new Vector3(Mathf.Round(Random.Range(xMin, xMax)), 0, Mathf.Round(Random.Range(yMin, yMax)));
        // return rPos;
        return new Vector3(Mathf.Round(Random.Range(xMin, xMax)), 0, Mathf.Round(Random.Range(yMin, yMax)));
    }

    public static int CalculateDistance(Vector3 position01, Vector3 position02) {
        return Mathf.RoundToInt(Vector3.Distance(position01, position02));
    }

    public static int FindLCM(Vector3 midPoint, int distance) {
        // int combined;

        // farCorner = FindFarCorner.Find(midPoint, GManager.halfHeight, GManager.halfWidth);
        farCorner = FindFarCorner.Find(midPoint, GManager.instance.halfHeight, GManager.instance.halfWidth);
        farthestDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farCorner));
        thisDistance = distance + farthestDistance;
        lcm_1 = ReduceLCM(LCM_GCD.Lcm(distance, farthestDistance));
        lcm_2 = ReduceLCM(LCM_GCD.Lcm(distance, thisDistance));
        lcm_3 = ReduceLCM(LCM_GCD.Lcm(farthestDistance, thisDistance));
        
        // return combined = lcm_1 + lcm_2 + lcm_3;
        return lcm_1 + lcm_2 + lcm_3;
    }

    private static int ReduceLCM(int lcm) {
        int tmplcm = lcm;
        // while (tmplcm >= GManager.halfWidth) {
        while (tmplcm >= GManager.instance.halfWidth) {
            // tmplcm = tmplcm / GManager.halfWidth;
            // tmplcm /= GManager.halfWidth;
            tmplcm /= GManager.instance.halfWidth;
        }

        return tmplcm;
    }

    /* function to remove redundant/duplicate entries in a List<Vector3> */
    public static List<Vector3> ShortenList(List<Vector3> list) {
        var tmpList = new HashSet<Vector3>(list);
        var shortList = tmpList.ToList();
        return shortList;
        // return tmpList.ToList();
    }

    public static IEnumerator WaitForRowSlice() {
        yield return new WaitUntil(() => GManager.instance.foundFirstRow == true);
    }

    public static IEnumerator WaitForColumnSlice() {
        yield return new WaitUntil(() => GManager.instance.foundFirstColumn == true);
    }

    /* find/return the value of the first row of the maze path */
    public static int FindFirstRow(List<Vector3> list) {
        Debug.Log("Finding first row \n 1st row is: " + list[0].z);
        int row = Mathf.RoundToInt(list[0].z);
        return row;
        // return Mathf.RoundToInt(list[0].z);
    }

    /* find/return the value of the first column of the maze path */
    public static int FindFirstColumn(List<Vector3> list) {
        int column = Mathf.RoundToInt(list[0].x);
        return column;
        // return Mathf.RoundToInt(list[0].x);
    }
    
    
}
