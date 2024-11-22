using UnityEngine;

public class GUtils : MonoBehaviour
{
    static Vector3 rPos;

    public static Vector3 RandomPosition(int xMin, int xMax, int yMin, int yMax) {
        rPos = new Vector3(Mathf.Round(Random.Range(xMin, xMax)), 0, Mathf.Round(Random.Range(yMin, yMax)));
        return rPos;
    }
}
