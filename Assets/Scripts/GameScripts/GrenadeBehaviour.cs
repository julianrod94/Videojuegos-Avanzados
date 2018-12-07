using UnityEngine;

namespace GameScripts {
    public class GrenadeBehaviour: MonoBehaviour {
        public bool isExploding;
        public ParticleSystem explosionPrefab;
        public ParticleSystem _explosion;
        private bool exploded;

        private void Update() {
            if (!exploded && isExploding) {
                _explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                GetComponent<MeshRenderer>().enabled = false;
                exploded = true;
            }
        }

        private void OnDestroy() {
            if (_explosion != null) {
                Destroy(_explosion.gameObject);
            }
        }
    }
}