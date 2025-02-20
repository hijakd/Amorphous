using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MazeUI : MonoBehaviour {
    UIDocument mainUiDoc;
    Label timeText;
    Label hint01BlipLabel;
    Label hint02BlipLabel;
    
    Button exitSettings;
    Button restartButton;
    Button settingsButton;
    Button quitButton;
    RadioButtonGroup difficulty;
    Slider playerSpeed;
    Toggle enableHints;
    VisualElement uiHeader;
    VisualElement settingsMenu;
    static VisualElement goalBlip;
    static VisualElement playerBlip;
    static VisualElement hintBlip01;
    static VisualElement hintBlip02;
    static VisualElement quitScreen;
    static VisualElement winScreen;

    bool showHints = true;
    bool showMenu;
    string timeString { get; set; }


    void OnEnable() {
        mainUiDoc = GetComponent<UIDocument>();
        VisualElement rootElement = mainUiDoc.rootVisualElement;
        
        uiHeader = rootElement.Q("uiHeader");
        timeText = rootElement.Q("time") as Label;
        goalBlip = rootElement.Q("goalBlip");
        playerBlip = rootElement.Q("playerBlip");
        hintBlip01 = rootElement.Q("hint01Blip");
        hintBlip02 = rootElement.Q("hint02Blip");
        settingsMenu = rootElement.Q("settingsMenu");
        quitScreen = rootElement.Q("quitScreen");
        winScreen = rootElement.Q("winScreen");
        exitSettings = rootElement.Q<Button>("exitSettings");
        restartButton = rootElement.Q<Button>("restartButton");
        settingsButton = rootElement.Q<Button>("settingsButton");
        quitButton = rootElement.Q<Button>("quitButton");
        hint01BlipLabel = rootElement.Q<Label>("hint01BlipLabel");
        hint02BlipLabel = rootElement.Q<Label>("hint02BlipLabel");
        difficulty = rootElement.Q<RadioButtonGroup>("difficulty");
        playerSpeed = rootElement.Q<Slider>("playerSpeed");
        enableHints = rootElement.Q<Toggle>("enableHints");
        
        GameManager.mazeData.SetTimeFormat(true);
        timeString = GameManager.mazeData.GetTimeFormat();
        exitSettings.clicked += ShowSettings;
        restartButton.clicked += OnRestartClicked;
        settingsButton.clicked += ShowSettings;
        quitButton.clicked += QuitGame;

        playerSpeed.RegisterCallback<ChangeEvent<float>>(evt => {
            GameManager.mazeData.playerSpeed = evt.newValue;
        });

        difficulty.RegisterCallback<ChangeEvent<int>>(evt => {
            GameManager.mazeData.difficulty = evt.newValue;
        });

        enableHints.RegisterCallback<ChangeEvent<bool>>(evt => {
            showHints = evt.newValue;
            if (showHints) {
                hintBlip01.style.display = DisplayStyle.Flex;
                hint01BlipLabel.style.display = DisplayStyle.Flex;
                hintBlip02.style.display = DisplayStyle.Flex;
                hint02BlipLabel.style.display = DisplayStyle.Flex;
            }
            else {
                hintBlip01.style.display = DisplayStyle.None;
                hint01BlipLabel.style.display = DisplayStyle.None;
                hintBlip02.style.display = DisplayStyle.None;
                hint02BlipLabel.style.display = DisplayStyle.None;
            }
        });
    }
    
    void FixedUpdate() {
        timeText.text = DateTime.Now.ToString(timeString);
    }
    
    public static void EnableWinScreen() {
        winScreen.style.display = DisplayStyle.Flex;
    }

    void ShowSettings() {
        if (!showMenu) {
            settingsMenu.style.display = DisplayStyle.Flex;
            showMenu = true;
        }
        else {
            settingsMenu.style.display = DisplayStyle.None;
            showMenu = false;
        }

        GameManager.PauseGame();
    }

    void QuitGame() {
        GameManager.PauseGame();
        quitScreen.style.display = DisplayStyle.Flex;
    }


    void OnRestartClicked() {
        GameManager.RestartGame();
    }

    public static void PaintGoal(Color goalColour) {
        goalBlip.style.backgroundColor = goalColour;
    }

    public static void PaintPlayer(Color playerColour) {
        playerBlip.style.backgroundColor = playerColour;
    }

    public static void PaintPlayerWhite() {
        playerBlip.style.backgroundColor = Color.white;
    }

    public static void PaintPlayerBlack() {
        playerBlip.style.backgroundColor = Color.black;
    }

    public static void PaintHints(Color hintColour01, Color hintColour02) {
        hintBlip01.style.backgroundColor = hintColour01;
        hintBlip02.style.backgroundColor = hintColour02;
    }


    /* END MazeUI */
}