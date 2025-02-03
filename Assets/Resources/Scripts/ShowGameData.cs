using System;
using UnityEngine;

public class ShowGameData : MonoBehaviour {
    
    /* This script is intended only to display the GameData SO values in the GameManager Inspector */
    /* and for testing the differences between doing a Mathematical Addition of two given Colour */
    /* values, versus using a 50% Color.Lerp() */
    /* Color.Lerp() tends to produce a slightly darker colour than a Mathematical Addition */

    public int height, width, north, south, east, west;
    public bool initialised, playerIsWhite, shortListed;
    public float playerSpeed;

    public Color goalColour, hintColour01, hintColour02, playerColour, previousColourFound/*, goalAddedColour, goalBlendedColour*/;

    // public Vector3 goalAddColourV3, goalBlendColourV3, playerColourV3;

    void Start() {
        height = GameManager.mazeData.gridHeight;
        width = GameManager.mazeData.gridWidth;
        north = GameManager.mazeData.northernEdge;
        south = GameManager.mazeData.southernEdge;
        east = GameManager.mazeData.easternEdge;
        west = GameManager.mazeData.westernEdge;
        goalColour = GameManager.mazeData.goalColour;
        hintColour01 = GameManager.mazeData.hintColour01;
        hintColour02 = GameManager.mazeData.hintColour02;
        

    }

    void FixedUpdate() {
        initialised = GameManager.mazeData.dataInitialized;
        playerIsWhite = GameManager.mazeData.playerIsWhite;
        shortListed = GameManager.mazeData.shortListed;
        playerColour = GameManager.mazeData.playerColour;
        previousColourFound = GameManager.mazeData.previousColour01;
        playerSpeed = GameManager.mazeData.playerSpeed;
        
        // goalAddedColour = AddColours(hintColour01, hintColour02);
        // goalBlendedColour = BlendColours(hintColour01, hintColour02);
        
        // goalAddColourV3 = new Vector3(goalAddedColour.r, goalAddedColour.g, goalAddedColour.b);
        // goalBlendColourV3 = new Vector3(goalBlendedColour.r, goalBlendedColour.g, goalBlendedColour.b);
        // playerColourV3 = new Vector3(playerColour.r, playerColour.g, playerColour.b);
    }

    // Color AddColours(Color colour01, Color colour02) {
    //     return colour01 + colour02;
    // }
    //
    // Color BlendColours(Color playersColour, Color waypointColor) {
    //     Color blendedColour = Color.Lerp(playersColour, waypointColor, 0.5f);
    //     return blendedColour;
    // }

}