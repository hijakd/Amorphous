using UnityEngine;

public class SpawnObject : MonoBehaviour {

    // spawn a given GameObject at a given position
    public static void Spawn(GameObject gameObject, Vector3 position) {
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }

    public static void Spawn(GameObject gameObject, Vector3 position, Quaternion rotation) {
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        Instantiate(gameObject);
    }

}