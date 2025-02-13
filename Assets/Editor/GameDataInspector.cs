using System.Collections.Generic;
using AmorphousData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

public class GameDataInspector : EditorWindow {

    
    Sprite defaultIcon;
    static List<Waypoint> m_waypointDatabase = new();
    VisualElement gdiRoot;
    ColorField gdiGoalColour;
    ColorField gdiPlayerColour;
    ColorField gdiHintColour01;
    ColorField gdiHintColour02;
    IntegerField gdiHeight;
    IntegerField gdiWidth;

    [MenuItem("Amorphous/GameData Inspector")]
    public static void Init() {
        GameDataInspector gdInspector = GetWindow<GameDataInspector>();
        gdInspector.titleContent = new GUIContent("GameData Inspector");
        
        /* END Init */
    }

    public void CreateGUI() {
        gdiRoot = rootVisualElement;
        
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/GameDataInspector.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        gdiRoot.Add(rootFromUXML);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/GameDataInspectorUSS.uss");
        gdiRoot.styleSheets.Add(styleSheet);
        
        // var gData = new VisualElement();
        
        // gData = CreateInstance<GameData>();
        // gData.dataSourcePath = PropertyPath.FromName(nameof(GameData.goalColour));
        
        gdiGoalColour = gdiRoot.Q<ColorField>("gdiGoalColour");
        gdiPlayerColour = gdiRoot.Q<ColorField>("gdiPlayerColour");
        gdiHintColour01 = gdiRoot.Q<ColorField>("gdiHintColour01");
        gdiHintColour02 = gdiRoot.Q<ColorField>("gdiHintColour02");
        gdiHeight = gdiRoot.Q<IntegerField>("gdiHeight");
        gdiWidth = gdiRoot.Q<IntegerField>("gdiWidth");
        
        // gdiGoalColour.dataSourcePath = PropertyPath.FromName(nameof(GameData.goalColour));
        // gdiPlayerColour.dataSourcePath = PropertyPath.FromName(nameof(GameData.playerColour));
        // gdiHintColour01.dataSourcePath = PropertyPath.FromName(nameof(GameData.hintColour01));
        // gdiHintColour02.dataSourcePath = PropertyPath.FromName(nameof(GameData.hintColour02));
        // gdiHeight.dataSourcePath = PropertyPath.FromName(nameof(GameData.gridHeight));
        // gdiWidth.dataSourcePath = PropertyPath.FromName(nameof(GameData.gridWidth));

        // gdiGoalColour.dataSource = GameManager.mazeData.goalColour;

    }

    /* END WaypointDBase */
}
