using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public GameObject goalObject;
    public GameObject player;
    public GameObject floorTile;
    public GameObject floorTile2;
    public GameObject otherPiece;
    public int gridHeight;
    public int gridWidth;
    public List<GameObject> waypoints;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI gameOverText;
    public bool isGameActive;
    public Button restartButton;
    public GameObject titleScreen;
    

    [SerializeField] private List<GameObject> cardinals;
    // private List<GameObject> cardinals;
    [SerializeField] private List<Vector3> selectableCoords;
    [SerializeField] private List<Vector3> LegOneIntersections;
    [SerializeField] private List<Vector3> shortenedLegOneIntersections;
    [SerializeField] private List<Vector3> drawnPath;
    
    private float modifier = 1.0f;
    
    private int halfHeight;
    private int halfWidth;
    private int count;
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
    [SerializeField] private List<Vector3> destinations;
    private Vector3 midPoint01;
    [SerializeField] private List<Vector3> midPoints;
    private Vector3 pyramidPos;
    private Vector3 pyramidPos02;
    private Vector3 farthestCorner;
    public static Vector2 xMinMax;
    public static Vector2 yMinMax;
    private Vector2 randomVariance;

    // public List<Vector2> vectorAngles; // for testing Vector3.Angle/SignedAngle() to find a path direction


    private void OnDrawGizmos() {
        // visualize the corners of the grid in the Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));
        Gizmos.DrawWireCube(new Vector3(-gridWidth / 2, 1f, -gridHeight / 2), new Vector3(0.75f, 2f, 0.75f));

        // visualize paths between 'intersections'
        Gizmos.color = Color.blue;
        for (int i = 0; i < LegOneIntersections.Count - 1; i++) {
            Gizmos.DrawLine(LegOneIntersections[i], LegOneIntersections[i + 1]);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(spawnPosition, pyramidPos);
        Gizmos.DrawLine(pyramidPos, destination01);
    }

    private void Awake() {
        halfHeight = gridHeight / 2;
        halfWidth = gridWidth / 2;
        xMinMax.x = -halfWidth;
        xMinMax.y = halfWidth;
        yMinMax.x = -halfHeight;
        yMinMax.y = halfHeight;
        randomVariance.x = 0.42f;
        randomVariance.y = 0.58f;

        /* cardinals are the corners of the grid & used for boundary calculations */
        // cardinals.Capacity = 4;
        SpawnObject.Spawn(cardinals[0], new Vector3(xMinMax.x, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[1], new Vector3(xMinMax.y, 0f, yMinMax.y));
        SpawnObject.Spawn(cardinals[2], new Vector3(xMinMax.y, 0f, yMinMax.x));
        SpawnObject.Spawn(cardinals[3], new Vector3(xMinMax.x, 0f, yMinMax.x));

        goalPosition = RandomPosition.Position(xMinMax, yMinMax);
        spawnPosition = RandomPosition.Position(xMinMax, yMinMax);
        destination01 = RandomPosition.Position(xMinMax, yMinMax);
        for (int i = 0; i < waypoints.Count; i++) {
            destinations.Add(RandomPosition.Position(xMinMax, yMinMax));
        }
        

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
        midPoint01 = Vector3.Lerp(spawnPosition, destinations[0], 0.5f);
        midPoints.Add(Vector3.Lerp(spawnPosition, destinations[0], 0.5f));
        midPoints.Add(Vector3.Lerp(destinations[0], destinations[1], 0.5f));
        midPoints.Add(Vector3.Lerp(destinations[1], destinations[2], 0.5f));
        midPoints.Add(Vector3.Lerp(destinations[2], destinations[3], 0.5f));
        
    }

    // Start is called before the first frame update
    void Start() {
        // SpawnObject.Spawn(player, spawnPosition);
        player.transform.position = spawnPosition;
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
        LegOneIntersections.Add(spawnPosition);
        while (count < combinedLCM) {
            int select01 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            int select02 = Mathf.RoundToInt(Random.Range(0, selectableCoords.Count));
            Vector3 coord01 = selectableCoords[select01];
            Vector3 coord02 = selectableCoords[select02];

            // pathOne.Add(TriangulateV.Position(coord01, coord02, distance02, xMinMax, yMinMax));
            LegOneIntersections.Add(TriangulateVectors.Position(coord01, coord02, Random.Range(0.42f, 0.58f)));
            count++;
        }

        LegOneIntersections.Add(destination01);

        /* remove duplicate values from pathOne */
        shortenedLegOneIntersections = new List<Vector3>(ShortenList(LegOneIntersections));

        count = 0;
        while (count < shortenedLegOneIntersections.Count) {
            SpawnObject.Spawn(floorTile, shortenedLegOneIntersections[count]);
            count++;
        }

        // pyramidPos = TriangulateV.Position(spawnPosition, destination01, xMinMax, yMinMax);
        // SpawnObject.Spawn(otherPiece, pyramidPos);


        // currently 'pyramids' are used for testing/visualising positions
        // pyramidPos = Vector3.Min(shortenedLegOneIntersections[0], shortenedLegOneIntersections[1]);
        // pyramidPos02 = Vector3.Max(shortenedLegOneIntersections[0], shortenedLegOneIntersections[1]);
        // Debug.Log("printing 'Min' of pos1 & pos2: " + pyramidPos);
        // Debug.Log("printing 'Max' of pos1 & pos2: " + pyramidPos02);

        // SpawnObject.Spawn(otherPiece, pyramidPos);
        // SpawnObject.Spawn(otherPiece, pyramidPos02);

        count = 0;
        while (count < shortenedLegOneIntersections.Count) {
            // Debug.Log("\nCount equals: " + count + "\n" + "Array length is: " + shortenedLegOneIntersections.Count);
            // Debug.Log("\nArray index a: " + count + "\n" + "Array index b: " + (count + 1));
            if ((count + 1) < shortenedLegOneIntersections.Count) {
                CheckOrientation.CheckHorizontal(shortenedLegOneIntersections[count],
                                shortenedLegOneIntersections[count + 1], drawnPath);
                CheckOrientation.CheckVertical(drawnPath[drawnPath.Count - 1], shortenedLegOneIntersections[count + 1],
                            drawnPath);
            }
            else {
                CheckOrientation.CheckHorizontal(shortenedLegOneIntersections[count],
                    shortenedLegOneIntersections[count], drawnPath);
                CheckOrientation.CheckVertical(drawnPath[drawnPath.Count - 1], shortenedLegOneIntersections[count],
                    drawnPath);
            }
            

            
            count++;
        }


        count = 0;
        while (count < drawnPath.Count) {
            SpawnObject.Spawn(floorTile2, drawnPath[count]);
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

    public void GameOver() {
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
    }

    public void EndLevel() {
        Debug.Log("display winText");
        winText.gameObject.SetActive(true);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame() {
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
    }
    
    /*public void StartGame(int difficulty) {
        modifier /= difficulty;
        isGameActive = true;
        titleScreen.gameObject.SetActive(false);
    }*/

}