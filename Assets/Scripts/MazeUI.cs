using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable InconsistentNaming

public class MazeUI : MonoBehaviour {

    private Label timeText;
    private static VisualElement goalBlip;
    private static VisualElement playerBlip;
    // private static Label telemetryText;
    // private static string dataTmp = null;
    
    void OnEnable() {
        // timeText = System.DateTime.Now.ToString("hh:mm:ss");
        var uiDoc = GetComponent<UIDocument>();
        timeText = uiDoc.rootVisualElement.Q("time") as Label;
        goalBlip = uiDoc.rootVisualElement.Q("goalColourBlip");
        playerBlip = uiDoc.rootVisualElement.Q("playerColourBlip");
        // telemetryText = uiDoc.rootVisualElement.Q("telemetry") as Label;
    }

    // public static void DisplayTelemetry(String data) {
    //     telemetryText.text = data;
    //     Debug.Log("telemetry data:\n" + data);
    // }
    //
    // public static void DisplayTelemetry(List<Vector3> data) {
    //     
    //     foreach (var vector in data) {
    //         dataTmp += vector + "\n";
    //     }
    //     
    //     Debug.Log("telemetry data:\n" + dataTmp);
    // }
    //
    // public static void DisplayTelemetry(int data) {
    //     telemetryText.text = data.ToString();
    //     Debug.Log("telemetry data:\n" + data);
    // }
    //
    // public static void DisplayTelemetry(float data) {
    //     telemetryText.text = data.ToString();
    //     Debug.Log("telemetry data:\n" + data);
    // }
    
    public static void PaintGoalBlip(Color colour) {
        // Debug.Log("Getting colour");
        goalBlip.style.backgroundColor = colour;
    }

    public static void PaintPlayerBlip(Color color) {
        playerBlip.style.backgroundColor = color;
    }
    
    void FixedUpdate() {
        timeText.text = System.DateTime.Now.ToString("HH:mm:ss");
    }
}