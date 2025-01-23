using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable InconsistentNaming

public class MazeUI : MonoBehaviour {

    private Label timeText;
    private static VisualElement goalBlip;
    private static VisualElement playerBlip;
    private static VisualElement hintBlip01;
    private static VisualElement hintBlip02;
    
    void OnEnable() {
        var uiDoc = GetComponent<UIDocument>();
        timeText = uiDoc.rootVisualElement.Q("time") as Label;
        goalBlip = uiDoc.rootVisualElement.Q("goalColourBlip");
        playerBlip = uiDoc.rootVisualElement.Q("playerColourBlip");
        hintBlip01 = uiDoc.rootVisualElement.Q("hintColourBlip01");
        hintBlip02 = uiDoc.rootVisualElement.Q("hintColourBlip02");
    }
    
    public static void PaintGoalBlip(Color colour) {
        goalBlip.style.backgroundColor = colour;
    }

    public static void PaintPlayerBlip(Color color) {
        playerBlip.style.backgroundColor = color;
    }
    
    public static void PaintHintBlips(Color color01, Color color02) {
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