using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
// ReSharper disable InconsistentNaming



public class MazeUI : MonoBehaviour {

    private Label timeText;
    private static VisualElement goalBlip;
    private static VisualElement playerBlip;
    private static VisualElement hintBlip01;
    private static VisualElement hintBlip02;
    private static VisualElement menuCell;
    private static StyleEnum<Visibility> visible;
    private static bool _b;

    void OnEnable() {
        // timeText = System.DateTime.Now.ToString("hh:mm:ss");
        var uiDoc = GetComponent<UIDocument>();
        timeText = uiDoc.rootVisualElement.Q("time") as Label;
        goalBlip = uiDoc.rootVisualElement.Q("goalColourBlip");
        playerBlip = uiDoc.rootVisualElement.Q("playerColourBlip");
        hintBlip01 = uiDoc.rootVisualElement.Q("hintColourBlip01");
        hintBlip02 = uiDoc.rootVisualElement.Q("hintColourBlip02");
        menuCell = uiDoc.rootVisualElement.Q("menuCell");
        // telemetryText = uiDoc.rootVisualElement.Q("telemetry") as Label;
    }

    public static void DisableMenu() {
        _b = visible == Visibility.Hidden;
        menuCell.style.visibility = visible;
    }
    public static void PaintGoalBlip(Color colour) {
        // Debug.Log("Getting colour");
        goalBlip.style.backgroundColor = colour;
    }

    public static void PaintPlayerBlip(Color color) {
        playerBlip.style.backgroundColor = color;
    }
    
    public static void PaintHintBlips(Color color01, Color color02) {
        if (color01 == Color.black && color02 == Color.black) {
            hintBlip01.style.opacity = 0;
            hintBlip02.style.opacity = 0;
        }
        else {
            hintBlip01.style.opacity = 1;
            hintBlip02.style.opacity = 1;
            
        }
        hintBlip01.style.backgroundColor = color01;
        hintBlip02.style.backgroundColor = color02;
        
    }

    public static void PaintPlayerBlipWhite() {
        playerBlip.style.backgroundColor = Color.white;
    }
    
    public static void PaintPlayerBlipBlack() {
        playerBlip.style.backgroundColor = Color.black;
    }
    
    void FixedUpdate() {
        timeText.text = DateTime.Now.ToString("HH:mm:ss");
    }

}

public class DataSource {

    public Color hintColour01 { get; set; }
    public bool hintsEnabled { get; set; }

}