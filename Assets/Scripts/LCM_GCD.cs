using UnityEngine;

public class LCM_GCD : MonoBehaviour {
    
    /* Find the Greatest Common Denominator/Divisor */
    static int Gcd(int int01, int int02) {
        int tempA = int01;
        int tempB = int02;

        while (int02 != 0) {
            int temp = tempB;
            tempB = tempA % tempB;
            tempA = temp;
        }

        return tempA;
    }
    
    /* find the Lowest Common Multiple */
    public static int Lcm(int dist01, int dist02) {
        // int result;
        int lcmTemp = dist01 / Gcd(dist01, dist02);
        int result = dist02 * lcmTemp;
        return result;
    }
}