using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public GameObject enemy;
    public float spawnRate = 0.8f;
    public bool randomOrientation = false;

    private bool _generateEnemies = false;
    public bool generateEnemies {
        get { return _generateEnemies; }
        set {
            _generateEnemies = value;
            if (value) {
                InvokeRepeating("SpawnEnemy", 0, spawnRate);
            } else {
                CancelInvoke("SpawnEnemy");
            }
        }
    }

    void SpawnEnemy() {
        var position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        var rotation = randomOrientation ? Random.rotation : transform.rotation;
        Instantiate(enemy, position, rotation);
    }

}
