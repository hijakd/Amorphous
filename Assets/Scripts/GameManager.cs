using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public GameObject goalObject;
    public GameObject player;
    public GameObject floorTile;
    public List<GameObject> waypoints;
    
    [SerializeField] private List<GameObject> cardinals;
    [SerializeField] private List<Vector3> selectableCoords;
    [SerializeField] private List<Vector3> pathOne;
    private List<Vector3> shortenedPathOne;

    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    private int halfHeight;
    private int halfWidth;
    private int count;
    [SerializeField] private int distance01;
    [SerializeField] private int distance02;
    [SerializeField] private int farCornerDistance;
    [SerializeField] private int lcm01;
    [SerializeField] private int lcm02;
    [SerializeField] private int lcm03;
    [SerializeField] private int combinedLCM;
    [SerializeField] private Vector3 spawnPosition;
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
        Gizmos.color = Color.blue;
        for (int i = 0; i < pathOne.Count - 1; i++) {
            Gizmos.DrawLine(pathOne[i], pathOne[i + 1]);
        }
    }

    private void Awake() {
        halfHeight = gridHeight / 2;
        halfWidth = gridWidth / 2;
        xMinMax.x = -halfWidth;
        xMinMax.y = halfWidth;
        yMinMax.x = -halfHeight;
        yMinMax.y = halfHeight;

        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        SpawnObject.Spawn(cardinals[0], new Vector3(xMinMax.x, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[1], new Vector3(xMinMax.y, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[2], new Vector3(xMinMax.y, 0f, yMinMax.x));
        SpawnObject.Spawn(cardinals[3], new Vector3(xMinMax.x, 0f, yMinMax.x));
        
        goalPosition = RandomPosition.Position(xMinMax, yMinMax);
        spawnPosition = RandomPosition.Position(xMinMax, yMinMax);
        destination01 = RandomPosition.Position(xMinMax, yMinMax);

        /* this is used for 'random' vector triangulation */
        selectableCoords.Add(new Vector3(0, 0, 0));
        selectableCoords.Add(cardinals[0].transform.position);
        selectableCoords.Add(cardinals[1].transform.position);
        selectableCoords.Add(cardinals[2].transform.position);
        selectableCoords.Add(cardinals[3].transform.position);
        selectableCoords.Add(spawnPosition);
        selectableCoords.Add(goalPosition);
        selectableCoords.Add(destination01);
        
        distance01 = Mathf.RoundToInt(Vector3.Distance(spawnPosition, destination01));
        midPoint01 = Vector3.Lerp(spawnPosition, destination01, 0.5f);
    }

    // Start is called before the first frame update
    void Start() {
        SpawnObject.Spawn(player, spawnPosition);
        SpawnObject.Spawn(goalObject, goalPosition);
        SpawnObject.Spawn(waypoints[0], destination01);
        
        /* begin finding Lowest Common Multiples */
        /** LCM's will be used to determine the number of intersections between two points **/
        // find the far corner from midpoint01
        farthestCorner = FindFarCorner.Find(midPoint01, halfHeight, halfWidth);
        farCornerDistance = Mathf.RoundToInt(Vector3.Distance(midPoint01, farthestCorner));
        distance02 = distance01 + farCornerDistance;
        lcm01 = ReduceLcm(LCM_GCD.Lcm(distance01, farCornerDistance));
        lcm02 = ReduceLcm(LCM_GCD.Lcm(distance01, distance02));
        lcm03 = ReduceLcm(LCM_GCD.Lcm(farCornerDistance, distance02));        
        
        combinedLCM = lcm01 + lcm02 + lcm03;
        /* end find LCM */

        count = 0;
        pathOne.Add(spawnPosition);
        while (count < combinedLCM) {
            int select01 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            int select02 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            Vector3 coord01 = selectableCoords[select01];
            Vector3 coord02 = selectableCoords[select02];
            pathOne.Add(TriangulateV.Position(coord01, coord02, distance02, xMinMax, yMinMax));
            count++;
        }
        pathOne.Add(destination01);

        /* remove duplicate values from pathOne */
        shortenedPathOne = new List<Vector3>(ShortenList(pathOne));
        
        count = 0;
        while (count < shortenedPathOne.Count) {
            SpawnObject.Spawn(floorTile, shortenedPathOne[count]);
            count++;
        }
    }

    // Update is called once per frame
    void Update() {
    }

    /* reducing the value of LCM to ensure there is a reasonably usable number */
    private int ReduceLcm(int lcm) {
        int tmpLcm = lcm;
        while (tmpLcm >= halfWidth) {
            tmpLcm = tmpLcm / halfWidth;
        }

        return tmpLcm;
    }

    /* remove duplicate Vector3's from a List<Vector3> */
    private List<Vector3> ShortenList(List<Vector3> pathList) {
        HashSet<Vector3> tmpList = new HashSet<Vector3>(pathList);
        List<Vector3> shortList = tmpList.ToList();
        return shortList;
    }
}