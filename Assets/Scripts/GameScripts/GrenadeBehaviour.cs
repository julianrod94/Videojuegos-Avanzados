using UnityEngine;

namespace GameScripts {
    public class GrenadeBehaviour: MonoBehaviour {
        public bool isExploding;
        public ParticleSystem explosion;
        private bool exploded;
        private float timeSinceExplotion;

        private void Update() {
            if (!exploded && isExploding) {
                explosion.Play();
                exploded = true;
                timeSinceExplotion = Time.time;
            }

            if (Time.time - timeSinceExplotion > 1) {
                Destroy(explosion);
                Destroy(this);
            }
        }
    }
}