using UnityEngine;

public class RandomPosition : MonoBehaviour {

    static Vector3 rPostion;
    
    public static Vector3 Position(Vector2 xMinMax, Vector2 yMinMax) {

        rPostion = new Vector3(Mathf.Round(Random.Range(xMinMax.x, xMinMax.y)), 0f, Mathf.Round(Random.Range(yMinMax.x, yMinMax.y)));
        return rPostion;
    }

}
