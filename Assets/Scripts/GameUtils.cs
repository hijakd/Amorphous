using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUtils : MonoBehaviour {
    
    
    
    /** find/return the value of the first row of the maze path */
    public static int FindFirstRow(List<Vector3> list) {
        // Debug.Log("Finding first row");
        int row = Mathf.RoundToInt(list[0].z);
        return row;
    }
    
    /** find/return the value of the first column of the maze path */
    public static int FindFirstColumn(List<Vector3> list) {
        int column = Mathf.RoundToInt(list[0].x);
        return column;
    }
    
    /** LCM's will be used to determine the number of intersections between two points **/
    public static int FindLcms(Vector3 midPoint, int distance) {
        
        Vector3 farthestCorner = FindFarCorner.Find(midPoint, GameManager.halfHeight, GameManager.halfWidth);
        int farCornerDistance = Mathf.RoundToInt(Vector3.Distance(midPoint, farthestCorner));
        int distance02 = distance + farCornerDistance;
        int lcm01 = ReduceLcm(LCM_GCD.Lcm(distance, farCornerDistance));
        int lcm02 = ReduceLcm(LCM_GCD.Lcm(distance, distance02));
        int lcm03 = ReduceLcm(LCM_GCD.Lcm(farCornerDistance, distance02));

        return lcm01 + lcm02 + lcm03;
    }
    
    /** reducing the value of LCM to ensure there is a reasonably usable number */
    private static int ReduceLcm(int lcm) {
        int tmpLcm = lcm;
        while (tmpLcm >= GameManager.halfWidth) {
            tmpLcm = tmpLcm / GameManager.halfWidth;
        }

        return tmpLcm;
    }
    
    /** remove duplicate Vector3's from a List<Vector3> */
    public static List<Vector3> ShortenList(List<Vector3> pathList) {
        HashSet<Vector3> tmpList = new HashSet<Vector3>(pathList);
        List<Vector3> shortList = tmpList.ToList();
        return shortList;
    }
    
    /** Blend/Mix the colours of two waypoints */
    public static Color BlendColours(List<GameObject> objects, bool addOrBlend) {
        Color outputColor;
        int choice = 0;

        if (addOrBlend) {
            choice = Random.Range(0, 6);
            switch (choice) {
                case 0:
                    outputColor =
                        ColourChanger.Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //0
                    break;
                case 1:
                    outputColor =
                        ColourChanger.Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //1
                    break;
                case 2:
                    outputColor =
                        ColourChanger.Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //2
                    break;
                case 3:
                    outputColor =
                        ColourChanger.Add(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //3
                    break;
                case 4:
                    outputColor =
                        ColourChanger.Add(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //4
                    break;
                case 5:
                    outputColor =
                        ColourChanger.Add(objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //5
                    break;
                default:
                    outputColor = Color.black;
                    break;
            }
        }
        else {
            choice = Random.Range(6, 12);
            switch (choice) {
                case 6:
                    outputColor =
                        ColourChanger.Blend(
                            objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //6
                    break;
                case 7:
                    outputColor =
                        ColourChanger.Blend(
                            objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //7
                    break;
                case 8:
                    outputColor =
                        ColourChanger.Blend(
                            objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //8
                    break;
                case 9:
                    outputColor =
                        ColourChanger.Blend(
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //9
                    break;
                case 10:
                    outputColor =
                        ColourChanger.Blend(
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //10
                    break;
                case 11:
                    outputColor =
                        ColourChanger.Blend(
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //11
                    break;
                default:
                    outputColor = Color.black;
                    break;
            }
        }
        // mixedColors.Add(tmp01); //0
        // mixedColors.Add(tmp02); //1
        // mixedColors.Add(tmp03); //2
        // mixedColors.Add(tmp04); //3
        // mixedColors.Add(tmp05); //4
        // mixedColors.Add(tmp06); //5
        // mixedColors.Add(tmp07); //6
        // mixedColors.Add(tmp08); //7
        // mixedColors.Add(tmp09); //8
        // mixedColors.Add(tmp10); //9
        // mixedColors.Add(tmp11); //10
        // mixedColors.Add(tmp12); //11
        
        return outputColor;
    }
}
