// ReSharper disable RedundantUsingDirective

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointDBase : EditorWindow {

    Sprite defaultIcon;

    [MenuItem("Amorphous/Waypoint Database")]
    public static void Init() {
        WaypointDBase waypointDb = GetWindow<WaypointDBase>();
        waypointDb.titleContent = new GUIContent("Waypoint Database");
        // waypointDb.titleContent = new GUIContent("GameData Inspector");
        
        /* END Init */
    }

    // public void CreateGUI() {
    //     // ReSharper disable once UnusedVariable
    //     var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/WaypointDatabase.uxml");
    // }

    /* END WaypointDBase */
}
