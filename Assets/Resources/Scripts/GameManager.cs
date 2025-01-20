using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public ScriptableObject mazeData;
    [Range(0.24f, 0.76f)]
    public float randomVariance;

    private float xVal, zVal;
    private GameObject groundPlane;
    private int count, columnNumber, rowNumber, lastRowNumber;
    private Vector2 randVariance;
    private List<int> lcms;
    private List<int> distances;
    private List<Vector3> intersections, destinations, midpoint, drawnPath, sortedList, shortenedList;
    private Material wallMaterial;


    void Awake() {
        groundPlane = GameObject.Find("GroundPlane");
        distances = new List<int>();
        lcms = new List<int>();
        destinations = new List<Vector3>();
        drawnPath = new List<Vector3>();
        intersections = new List<Vector3>();
        midpoint = new List<Vector3>();
        shortenedList = new List<Vector3>();
        sortedList = new List<Vector3>();

        randVariance.x = randomVariance;
        randVariance.y = 1f - randomVariance;
        
        wallMaterial = Resources.Load<Material>("Materials/DryWall_mat");
    }
    
    
    void Start()
    {
        
    }
    
    
    void FixedUpdate() {
        
    }
}
