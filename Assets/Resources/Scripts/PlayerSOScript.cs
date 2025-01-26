using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_SO", menuName = "ScriptableObjects/Player Scriptable")]
public class Player : ScriptableObject {

    public string playerName = "";
    public float speed { get; set; }
    public float lookSpeed = 10f;
    public float moveSpeed;
    public Color currentColour = Color.white;
    public Color thisColour, thatColour, playerColour, previousColour, otherColour;

    private bool playerIsWhite /*, showHint, showMenu*/;
    private static string playerColourChangeOption;



    private void Awake() {

        if (playerName.Length < 1) {
            playerName = "Player01";
        }

        speed = 50;

    }

    private void OnTriggerEnter(Collider other) {
        // thatColour = gameObject.GetComponentInChildren<Renderer>().material.color;
        thisColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;



        if (other.gameObject.CompareTag("Waypoint")) {
            // Debug.Log("Player found a waypoint");
            if (playerIsWhite) {

                // blendedColour = other.gameObject.GetComponentInChildren<Renderer>().sharedMaterial.color;
                currentColour = thisColour;
                playerIsWhite = !playerIsWhite;
            }
            else {

                switch (playerColourChangeOption) {
                    case "switch":
                        Debug.Log("player colour is switching");
                        currentColour = GameManager.ChangeColours("switch", currentColour, thisColour);
                        break;
                    case "add":
                        if (GameManager._easyMode) {
                            Debug.Log("player colour is adding in _easyMode");
                            currentColour = GameManager.ChangeColours("add", thisColour, previousColour);
                            break;
                        }

                        Debug.Log("player colour is adding");
                        currentColour = GameManager.ChangeColours("add", currentColour, thisColour);
                        break;


                    case "blend":
                        if (GameManager._easyMode) {
                            Debug.Log("player colour is blending in _easyMode");
                            currentColour = GameManager.ChangeColours("blend", thisColour, previousColour);
                            break;
                        }

                        Debug.Log("player colour is blending");
                        currentColour = GameManager.ChangeColours("blend", currentColour, thisColour);
                        break;
                }
            }

            previousColour = thisColour;
            MazeUI.PaintPlayerBlip(currentColour);
        }

    }

}