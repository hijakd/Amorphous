using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MazeMenu : MonoBehaviour {

    private static RadioButtonGroup difficulty;
    private static RadioButtonGroup hints;
    private static Slider moveSpeed;
    public static float playerSpeed;
    // [SerializeField] private float playerSpeed;

    // private void Awake() {
        // moveSpeed.value = PlayerController.moveSpeed;
    // }

    private void OnEnable() {
        var uiDoc = GetComponent<UIDocument>();
        difficulty = uiDoc.rootVisualElement.Q("difficultySetting") as RadioButtonGroup;
        hints = uiDoc.rootVisualElement.Q("hintsSelector") as RadioButtonGroup;
        moveSpeed = uiDoc.rootVisualElement.Q("moveSpeedSlider") as Slider;
        playerSpeed = PlayerController.speed / 10;
        moveSpeed.value = playerSpeed;

    }


    private void FixedUpdate() {
        // if (!Mathf.Approximately(moveSpeed.value, playerSpeed)) {
            PlayerController.speed = moveSpeed.value * 10;
        // }
        
    }

}
