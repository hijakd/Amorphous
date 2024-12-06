using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Colouring : MonoBehaviour
{
    public static Color Add(GameObject object01, GameObject object02) {
        Color tmpColor = object01.gameObject.GetComponentInChildren<Renderer>().material.color + object02.gameObject.GetComponentInChildren<Renderer>().material.color;
        return tmpColor;
    }
    
    public static Color Add(Color colour01, Color colour02) {
        Color tmpColor = colour01 + colour02;
        return tmpColor;
    }

    public static Color Blend(GameObject object01, GameObject object02) {
        Color blendColor = Color.Lerp(object01.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color, object02.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color, 0.5f);
        return blendColor;
    }
    
    public static Color Blend(Color colour01, Color colour02) {
        Color blendColor = Color.Lerp(colour01, colour02, 0.5f);
        return blendColor;
    }
    
    /** Blend/Mix the colours of two random waypoints */
    public static Color BlendColours(List<GameObject> objects, bool addOrBlend) {
        Color outputColor;
        int choice = 0;

        if (addOrBlend) {
            /* add a "random" pair of waypoints colours together */
            choice = Random.Range(0, 6);
            switch (choice) {
                case 0:
                    outputColor =
                        Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //0
                    break;
                case 1:
                    outputColor =
                        Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //1
                    break;
                case 2:
                    outputColor =
                        Add(objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //2
                    break;
                case 3:
                    outputColor =
                        Add(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //3
                    break;
                case 4:
                    outputColor =
                        Add(objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //4
                    break;
                case 5:
                    outputColor =
                        Add(objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //5
                    break;
                default:
                    outputColor = Color.black;
                    break;
            }
        }
        else {
            /* blend a "random" pair of waypoints colours together */
            choice = Random.Range(6, 12);
            switch (choice) {
                case 6:
                    outputColor =
                        Blend(
                            objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //6
                    break;
                case 7:
                    outputColor =
                        Blend(
                            objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //7
                    break;
                case 8:
                    outputColor =
                        Blend(
                            objects[0].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //8
                    break;
                case 9:
                    outputColor =
                        Blend(
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //9
                    break;
                case 10:
                    outputColor =
                        Blend(
                            objects[1].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //10
                    break;
                case 11:
                    outputColor =
                        Blend(
                            objects[2].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color,
                            objects[3].gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color); //11
                    break;
                default:
                    outputColor = Color.black;
                    break;
            }
        }
        return outputColor;
    }

    /* combine the colours of two given GameObjects/waypoints */
    public static Color BlendColours(GameObject waypoint01, GameObject waypoint02, bool addOrBlend) {
        Color output;
        if (addOrBlend) {
            output = Add(waypoint01.GetComponentInChildren<Renderer>().sharedMaterial.color,
                waypoint02.GetComponentInChildren<Renderer>().sharedMaterial.color);
        }
        else {
            output = Blend(waypoint01.GetComponentInChildren<Renderer>().sharedMaterial.color, waypoint02.GetComponentInChildren<Renderer>().sharedMaterial.color);
        }

        return output;
    }
}
