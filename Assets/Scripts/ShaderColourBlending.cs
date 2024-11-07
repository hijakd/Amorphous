using UnityEngine;

public class ShaderColourBlending : MonoBehaviour
{
    private Material material;

    public void ResetWhite() {
        material.SetColor("_Color", Color.white);
    }
    
    public void ResetBlack() {
        material.SetColor("_Color", Color.black);
    }

    public void BlendColour(Material otherMaterial) {
        Color colour = Color.Lerp(material.color, otherMaterial.color, 0.5f); ;
        
        material.SetColor("_Colour", colour);
    }

    
}
