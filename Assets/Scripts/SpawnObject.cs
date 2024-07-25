using UnityEngine;

public class SpawnObject : MonoBehaviour {

    // spawn a given GameObject at a given position
    public static void Spawn(GameObject gameObject, Vector3 position) {
        gameObject.transform.position = position;
        Instantiate(gameObject);
    }
}