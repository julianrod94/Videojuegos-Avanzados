using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerCoordinator : MonoBehaviour {

	private static SpawnerCoordinator instance;

	private EnemySpawner[] spawners;
	public int enemies = 0;
	
	private float spawnRate = 0.5f;
	private float spawnWindow = 2.4f;
	private int i = 0;
	private bool spawning = false;
	private float initialTime;
	
	private SpawnerCoordinator() {
	}

	public static SpawnerCoordinator Instance {
		get { return instance; }
	}

	private void OnDestroy() {
		instance = null;
	}

	public void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		initialTime = Time.time;
		GameObject[] objects = GameObject.FindGameObjectsWithTag(Tags.spawn);
		IOrderedEnumerable<GameObject> list = new List<GameObject>(objects).OrderBy(o => o.transform.position.z);
		spawners = list.Select(o => o.GetComponent<EnemySpawner>()).ToArray();
	}

	private void Update() {
		if (!spawning) {
			StartCoroutine(spawn());
		}
	}

	IEnumerator spawn() {
		
		for (; i < spawners.Length; i++) {
			if (enemies > 25) {
				yield return new WaitForSeconds(0);
			} else {
				spawning = true;
				spawners[i].spawnRate = spawnRate;
				spawners[i].generateEnemies = true;
				yield return new WaitForSeconds(spawnWindow);
				spawners[i].generateEnemies = false;
				enemies += (int) Mathf.Ceil(spawnWindow / spawnRate);
				yield return new WaitForSeconds(4f);
			}
		}
		spawning = false;
		if (i == spawners.Length) {
			i = 0;
		}
		
		
	}
}
