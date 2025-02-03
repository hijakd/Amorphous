using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using AmorphousData;

public class MazeUI : MonoBehaviour {

    // GameManager mazeData;
    UIDocument mainUiDoc;
    Label timeText;
    VisualElement settingsButton;
    static VisualElement goalBlip;
    static VisualElement playerBlip;
    static VisualElement hintBlip01;
    static VisualElement hintBlip02;
    VisualElement uiHeader;
    VisualElement settingsMenu;

    string timeString { get; set; }


    void OnEnable() {
        // mazeData = ScriptableObject.CreateInstance<GameData>();
        SetVisualElements();
        GameManager.mazeData.SetTimeFormat(true);
        timeString = GameManager.mazeData.GetTimeFormat();
    }


    void FixedUpdate() {
        timeText.text = DateTime.Now.ToString(timeString);
    }


    void SetVisualElements() {
        mainUiDoc = GetComponent<UIDocument>();
        VisualElement rootElement = mainUiDoc.rootVisualElement;
        uiHeader = rootElement.Q("uiHeader");
        timeText = rootElement.Q("time") as Label;
        goalBlip = rootElement.Q("goalBlip");
        playerBlip = rootElement.Q("playerBlip");
        hintBlip01 = rootElement.Q("hint01Blip");
        hintBlip02 = rootElement.Q("hint02Blip");
    }


    public static void PaintGoal(Color goalColour) {
        goalBlip.style.backgroundColor = goalColour;
    }

    public static void PaintPlayer(Color playerColour) {
        playerBlip.style.backgroundColor = playerColour;
    }
    
    public static void PaintPlayerWhite() {
        playerBlip.style.backgroundColor = Color.white;
        // GameManager.mazeData.playerIsWhite = true;
    }
    
    public static void PaintPlayerBlack() {
        playerBlip.style.backgroundColor = Color.black;
        // GameManager.mazeData.playerIsWhite = false;
    }
    
    public static void PaintHints(Color hintColour01, Color hintColour02) {
        hintBlip01.style.backgroundColor = hintColour01;
        hintBlip02.style.backgroundColor = hintColour02;
    }



}