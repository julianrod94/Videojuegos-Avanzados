public class IntermitentEnemySpawner : EnemySpawner {

    public float spawningWindow = 1;
    public float downTime = 1;

    void Start () {
        InvokeRepeating("StartSpawn",0,spawningWindow + downTime);
        InvokeRepeating("StopSpawn",spawningWindow,spawningWindow + downTime);
	}

    private void StartSpawn() { generateEnemies = true;  }
    private void StopSpawn()  { generateEnemies = false; }

}
