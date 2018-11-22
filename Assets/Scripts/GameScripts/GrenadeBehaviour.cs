using UnityEngine;

namespace GameScripts {
    public class GrenadeBehaviour: MonoBehaviour {
        public bool isExploding;
        public ParticleSystem explosion;
        public ParticleSystem _explosion;
        private bool exploded;
        private float timeSinceExplotion;

        private void Update() {
            if (!exploded && isExploding) {
                _explosion = Instantiate(explosion);
                exploded = true;
                timeSinceExplotion = Time.time;
            }
        }

        private void OnDestroy() {
            if (_explosion != null) {
                Destroy(_explosion.gameObject);
            }
        }
    }
}