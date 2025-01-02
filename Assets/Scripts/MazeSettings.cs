using System;
using UnityEngine;
using UnityEngine.UI;

public class MazeSettings : MonoBehaviour {

    static GameObject goalBlip { get; set; }
    static GameObject playerBlip { get; set; }
    static GameObject hintBlip01 { get; set; }
    static GameObject hintBlip02 { get; set; }
    
    
    public static Image goalColourBlip;
    private static Color goalBlipColour { get; set; }
    private static Color playerBlipColour { get; set; }
    public static float playerSpeed { get; set; }
    public static Toggle enableHints { get; set; }
    public static ToggleGroup difficulty { get; set; }


    private void Awake() {
        // goalColourBlip = gameObject.CompareTag("GoalColourBlip");
        goalBlip = GameObject.Find("GoalColour");
        // playerBlip = GameObject.Find("PlayerColour");
        // hintBlip01 = GameObject.Find("HintColour_01");
        // hintBlip02 = GameObject.Find("HintColour_02");
    }


    public static void PaintGoalBlip(Color colour) {
        // Debug.Log("Getting colour");
        // goalBlip.material.color = colour;
        // goalBlipColour = colour;
        // SetGoalColourBlip(goalBlipColour);
        goalBlip.GetComponent<Image>().material.color = colour;
        // goalBlip.GetComponentInChildren<Image>().material.color = colour;
        SetGoalBlipColour();
    }

    // public static void PaintPlayerBlip(Color colour) {
    //     playerBlip.GetComponent<Image>().material.color = colour;
    // }

    private static void SetGoalBlipColour() {
        // goalColourBlip.GetComponent<Image>().material.color = goalBlipColour;
        goalBlipColour = goalBlip.GetComponent<Image>().material.color;
    }

    // Update is called once per frame
    void Update() {

    }

    // void OnUpdateSelected() {
    //     
    // }

}