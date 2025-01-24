using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_SO", menuName = "ScriptableObjects/Player Scriptable")]
public class PlayerSO : ScriptableObject {

    // public GameObject player;
    public string playerName = "";
    public float speed;
    public float lookSpeed = 15f;
    public float moveSpeed;
    public Color currentColour = Color.white;
    public Color thisColour, thatColour;
    public Color playerColour;
    public Color previousColour, otherColour;

    public bool playerIsWhite /*, showHint, showMenu*/;
    public string playerColourChangeOption;


    private void Awake() {
        
        if (playerName.Length < 1) {
            playerName = "Player01";
        }

        speed = 50;
        moveSpeed = speed;

    }

    

}