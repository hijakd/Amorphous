using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static IEnumerator WaitForRowSlice() {
        yield return new WaitUntil(() => GameManager.firstRowFound == true);
    }
    
    public static IEnumerator WaitForColSlice() {
        yield return new WaitUntil(() => GameManager.firstColFound == true);
    }
    
    /** remove duplicate Vector3's from a List<Vector3> */
    public static List<Vector3> ShortenList(List<Vector3> pathList) {
        HashSet<Vector3> tmpList = new HashSet<Vector3>(pathList);
        List<Vector3> shortList = tmpList.ToList();
        return shortList;
    }
    
    
}
