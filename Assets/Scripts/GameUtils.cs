using System.Collections;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    
    public static IEnumerator WaitForRowSlice() {
        yield return new WaitUntil(() => GameManager.firstRowFound == true);
    }
    
    public static IEnumerator WaitForColSlice() {
        yield return new WaitUntil(() => GameManager.firstColFound == true);
    }
    
    
    public static Vector3 RandomPosition(int minWidth, int maxWidth, int minHeight, int maxHeight) {
        // Debug.Log("Generating a random position");
        return new Vector3(Mathf.Round(Random.Range(minWidth, maxWidth)), 0,
            Mathf.Round(Random.Range(minHeight, maxHeight)));
    }
}
