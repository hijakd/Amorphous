using UnityEngine;

public class LCM_GCD : MonoBehaviour {
    
    /* Find the Greatest Common Denominator/Divisor */
    static int Gcd(int int01, int int02) {
        int tempA = int01;
        int tempB = int02;
        
        // Debug.Log("start GCD");
        // Debug.Log("GCD: tempA: " + tempA + " tempB: " + tempB);
        while (tempB > 0) {
            // Debug.Log("GCD loop: tempA: " + tempA + " tempB: " + tempB);
            int temp = tempB;
            tempB = tempA % tempB;
            tempA = temp;
        }

        return tempA;
    }
    
    /* find the Lowest Common Multiple */
    public static int Lcm(int dist01, int dist02) {
        // int result;
        // Debug.Log("start LCM");
        // Debug.Log("LCM; int a: " + dist01 + " int b: " + dist02);
        int lcmTemp = dist01 / Gcd(dist01, dist02);
        int result = dist02 * lcmTemp;
        return result;
    }
}