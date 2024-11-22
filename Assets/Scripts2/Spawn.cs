using UnityEngine;

public class Spawn : MonoBehaviour
{

    public static void Object(GameObject gameObject, Vector3 position) {
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }
}
