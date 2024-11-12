using UnityEngine;

public class ColourChanger : MonoBehaviour
{
    public static Color Add(GameObject waypoint01, GameObject waypoint02) {
        Color tmpColor = waypoint01.gameObject.GetComponentInChildren<Renderer>().material.color + waypoint02.gameObject.GetComponentInChildren<Renderer>().material.color;
        return tmpColor;
    }
    
    public static Color Add(Color waypoint01, Color waypoint02) {
        Color tmpColor = waypoint01 + waypoint02;
        return tmpColor;
    }

    public static Color Blend(GameObject waypoint01, GameObject waypoint02) {
        Color blendColor = Color.Lerp(waypoint01.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color, waypoint02.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color, 0.5f);
        return blendColor;
    }
    
    public static Color Blend(Color waypoint01, Color waypoint02) {
        Color blendColor = Color.Lerp(waypoint01, waypoint02, 0.5f);
        return blendColor;
    }
}
