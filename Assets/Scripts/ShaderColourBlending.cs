using UnityEngine;

public class ShaderColourBlending : MonoBehaviour {

    private static Material materialInstance;
    
    private void Start() {
        materialInstance = gameObject.GetComponent<Renderer>().material;
    }

    public static void ResetWhite() {
        materialInstance.SetColor("_Color", Color.white);
    }

    public static void ResetBlack() {
        materialInstance.SetColor("_Color", Color.black);
    }

    public static void BlendColour(Color otherColor) {
        Color blendedColour = Color.Lerp(materialInstance.color, otherColor, 0.5f);
        Debug.Log("Blending colours, " + otherColor + "\nblended colour is: " + blendedColour);
        materialInstance.SetColor("_Color", blendedColour);

    }

}