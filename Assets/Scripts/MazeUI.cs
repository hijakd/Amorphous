using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable InconsistentNaming

public class MazeUI : MonoBehaviour {

    private Label timeText;
    private static VisualElement goalBlip;
    private static VisualElement playerBlip;
    
    
    void OnEnable() {
        // timeText = System.DateTime.Now.ToString("hh:mm:ss");
        var uiDoc = GetComponent<UIDocument>();
        timeText = uiDoc.rootVisualElement.Q("time") as Label;
        goalBlip = uiDoc.rootVisualElement.Q("goalColourBlip");
        playerBlip = uiDoc.rootVisualElement.Q("playerColourBlip");
    }
    
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