using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour {

    public GameObject goalObject;
    public GameObject player;
    [SerializeField] private List<GameObject> cardinals;


    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    private int halfHeight;
    private int halfWidth;
    private int distance01;
    private int distance02;
    private int farCornerDistance;
    private int lcm01;
    private int lcm02;
    private int lcm03;
    private int combinedLCM;
    private Vector3 spawnPosition;
    private Vector3 goalPosition;
    private Vector3 destination01;
    private Vector3 midPoint01;
    private Vector3 farthestCorner;
    private Vector2 xMinMax;
    private Vector2 yMinMax;


    // visualize the corners of the grid in the Editor
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
    }

    private void Awake() {
        halfHeight = gridHeight / 2;
        halfWidth = gridWidth / 2;
        xMinMax.x = -halfWidth;
        xMinMax.y = halfWidth;
        yMinMax.x = -halfHeight;
        yMinMax.y = halfHeight;

        SpawnObject.Spawn(cardinals[0], new Vector3(xMinMax.x, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[1], new Vector3(xMinMax.y, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[2], new Vector3(xMinMax.y, 0f, yMinMax.x));
        SpawnObject.Spawn(cardinals[3], new Vector3(xMinMax.x, 0f, yMinMax.x));


        goalPosition = RandomPosition.Position(xMinMax, yMinMax);
        spawnPosition = RandomPosition.Position(xMinMax, yMinMax);
        destination01 = RandomPosition.Position(xMinMax, yMinMax);


        distance01 = Mathf.RoundToInt(Vector3.Distance(spawnPosition, destination01));
        midPoint01 = Vector3.Lerp(spawnPosition, destination01, 0.5f);
    }

    // Start is called before the first frame update
    void Start() {
        SpawnObject.Spawn(player, spawnPosition);
        SpawnObject.Spawn(goalObject, goalPosition);
        
        /* begin finding Lowest Common Multiples */
        // find far corner from midpoint01
        farthestCorner = FindFarCorner.Find(midPoint01, halfHeight, halfWidth);
        farCornerDistance = Mathf.RoundToInt(Vector3.Distance(midPoint01, farthestCorner));
        distance02 = distance01 + farCornerDistance;
        lcm01 = ReduceLcm(LCM_GCD.Lcm(distance01, farCornerDistance));
        lcm02 = ReduceLcm(LCM_GCD.Lcm(distance01, distance02));
        lcm03 = ReduceLcm(LCM_GCD.Lcm(farCornerDistance, distance02));
        combinedLCM = lcm01 + lcm02 + lcm03;
        /* end find LCM */
        
    }

    // Update is called once per frame
    void Update() {
    }

    private int ReduceLcm(int lcm) {
        int tmpLcm = lcm;
        while (tmpLcm >= halfWidth) {
            tmpLcm = tmpLcm / (halfWidth / 2);
        }

        return tmpLcm;
    }

}