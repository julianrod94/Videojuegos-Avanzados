using UnityEngine;

namespace GameScripts {
    public class GrenadeBehaviour: MonoBehaviour {
        public bool isExploding;
        public ParticleSystem explosion;
        public ParticleSystem _explosion;
        private bool exploded;
        private float timeSinceExplotion;

        private void Start() {
            _explosion = Instantiate(explosion);
        }

        private void Update() {
            if (!exploded && isExploding) {
                _explosion.Play();
                exploded = true;
                timeSinceExplotion = Time.time;
            }

            if (Time.time - timeSinceExplotion > 1) {
                Destroy(_explosion.gameObject);
                Destroy(gameObject);
            }
        }
    }
}