using UnityEngine;
using UnityEngine.UIElements;

public class MazeUI : MonoBehaviour {

    private Label timeText;
    private static VisualElement blip;
    
    
    void OnEnable() {
        // timeText = System.DateTime.Now.ToString("hh:mm:ss");
        var uiDoc = GetComponent<UIDocument>();
        timeText = uiDoc.rootVisualElement.Q("time") as Label;
        blip = uiDoc.rootVisualElement.Q("colourBlip");
    }
    
    public static void PaintBlip(Color color) {
        blip.style.backgroundColor = color;
    }

    void FixedUpdate() {
        timeText.text = System.DateTime.Now.ToString("HH:mm:ss");
    }
}
