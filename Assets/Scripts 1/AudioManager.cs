using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AudioManager : MonoBehaviour {

    private static AudioManager instance;

    public AudioClip playerShootingAudio;
    public AudioClip enemyHittingAudio;
    public AudioClip playerHitAudio;
    public AudioClip enemyHitAudio;
    public AudioClip gameOverAudio;
    public AudioClip victoryAudio;

    private AudioManager() {
    }

    public static AudioManager Instance {
        get { return instance; }
    }
    
    public void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void playOn(AudioSource source, AudioClip clip, float volume, bool loop) {
        source.PlayOneShot(playerShootingAudio,volume);
    }

    public void playerShoot() {
        float volume = 0.05f;
        AudioSource source = PlayerCoordinator.Instance.player.GetComponent<AudioSource>();
        source.PlayOneShot(playerShootingAudio,volume);
    }

    public void playerHit() {
        float volume = 0.3f;
        AudioSource source = PlayerCoordinator.Instance.player.GetComponent<AudioSource>();
        source.PlayOneShot(playerHitAudio,volume);
    }
    
    public void enemyHitting(AudioSource source) {
        float volume = 0.2f;
        source.PlayOneShot(enemyHittingAudio,volume);
    }
    
    public void enemyHit(AudioSource source) {
        float volume = 0.3f;
        source.PlayOneShot(enemyHitAudio,volume);
    }

    public void gameOver() {
        float volume = 0.7f;
        GetComponent<AudioSource>().PlayOneShot(gameOverAudio,volume);
    }
    
    public void victory() {
        float volume = 0.5f;
        GetComponent<AudioSource>().PlayOneShot(victoryAudio,volume);
    }
}
