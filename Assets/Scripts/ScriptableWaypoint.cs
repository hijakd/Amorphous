using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Waypoint_SO", menuName = "Scriptable Objects/Waypoint Scriptable")]
public class ScriptableWaypoint : ScriptableObject {

    public GameObject waypointBase;
    public ParticleSystem hintParticle;
    public Material baseMaterial;
    public Color primaryColor;
    public Color secondaryColor;
    public bool isGoal;
    private Gradient gradient = new Gradient();


    private void OnAwake() {
    }
    

    private void OnEnable() {

        // set gradient colour for the hint particle start & trail colour
        gradient.SetKeys(new[] { new GradientColorKey(primaryColor, 0.0f), new GradientColorKey(secondaryColor, 1.0f) }, new[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } );
        
        var mainColour = hintParticle.GetComponent<ParticleSystem>().main;
        mainColour.startColor = primaryColor;
        
        var col = hintParticle.GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        col.color = gradient;
        
        var trail = hintParticle.GetComponent<ParticleSystem>().trails;
        trail.enabled = true;
        trail.colorOverTrail = gradient;
        
        // set the material colour of the waypoint
        baseMaterial.color = primaryColor;
        // set the material of the waypoint
        waypointBase.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial = baseMaterial;
    }
    

}

