using UnityEngine;

public class ShowGameData : MonoBehaviour {
    
    /* This script is intended only to display the GameData SO values in the GameManager Inspector */
    /* and for testing the differences between doing a Mathematical Addition of two given Colour */
    /* values, versus using a 50% Color.Lerp() */
    /* Color.Lerp() tends to produce a slightly darker colour than a Mathematical Addition */

    public int height, width, north, south, east, west;
    public bool initialised, playerIsWhite, shortListed;
    public float playerSpeed;

    public Color goalColour, hintColour01, hintColour02, playerColour, previousColourFound;


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
    }
}