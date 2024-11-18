using System.Collections;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public static IEnumerator WaitForRowSlice() {
        yield return new WaitUntil(() => GameManager.firstRowFound == true);
    }
    
    public static IEnumerator WaitForColSlice() {
        yield return new WaitUntil(() => GameManager.firstColFound == true);
    }
}
