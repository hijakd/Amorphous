using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable InconsistentNaming

public class GUtils : MonoBehaviour
{
    static Vector3 rPos;
    private static int lcm_1;
    private static int lcm_2;
    private static int lcm_3;
    private static int thisDistance;
    
    private static int farthestDistance;
    private static Vector3 farCorner;

    public static Vector3 RandomPosition(int xMin, int xMax, int yMin, int yMax) {
        rPos = new Vector3(Mathf.Round(Random.Range(xMin, xMax)), 0, Mathf.Round(Random.Range(yMin, yMax)));
        return rPos;
    }

    public static int FindLCM(Vector3 midPoint, int distance) {
        int combined;

        farCorner = FindFarCorner.Find(midPoint, GManager.halfHeight, GManager.halfWidth);
        farthestDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farCorner));
        thisDistance = distance + farthestDistance;
        lcm_1 = ReduceLCM(LCM_GCD.Lcm(distance, farthestDistance));
        lcm_2 = ReduceLCM(LCM_GCD.Lcm(distance, thisDistance));
        lcm_3 = ReduceLCM(LCM_GCD.Lcm(farthestDistance, thisDistance));
        
        return combined = lcm_1 + lcm_2 + lcm_3;
    }

    private static int ReduceLCM(int lcm) {
        int tmplcm = lcm;
        while (tmplcm >= GManager.halfWidth) {
            // tmplcm = tmplcm / GManager.halfWidth;
            tmplcm /= GManager.halfWidth;
        }

        return tmplcm;
    }

    /* function to remove redundant/duplicate entries in a List<Vector3> */
    public static List<Vector3> ShortenList(List<Vector3> list) {
        HashSet<Vector3> tmpList = new HashSet<Vector3>(list);
        // List<Vector3> shortList = tmpList.ToList();
        // return shortList;
        return tmpList.ToList();
    }
}
