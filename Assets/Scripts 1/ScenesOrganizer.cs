using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesOrganizer : MonoBehaviour {

    private static ScenesOrganizer instance;
    private static float timeStart;

    public static float lastTime = 0;

    public static ScenesOrganizer Instance {
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

    public void goToGame() {
        Cursor.visible = false;
        SceneManager.LoadScene("MainGame");
    }
    
    public void goToControls() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Controls");
    }

    public void goToEndGame() {
        AudioManager.Instance.gameOver();
        lastTime = getTime();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("GameOver");
    }
    
    public void goToVictory() {
        lastTime = getTime();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioManager.Instance.victory();
        SceneManager.LoadScene("VictoryScreen");
    }


    public float getTime() {
        return Time.time - timeStart;
    }

    public void restartTimer() {
        timeStart = Time.time;
    }

    public String getFormattedTime() {
        return formattedTime(getTime());
    }

    public String getFormattedLastTime() {
        return formattedTime(lastTime);
    }

    private String formattedTime(float time) {
        float seconds = time;
        int minutes = (int)seconds / 60;
        int remaining = (int) seconds - (minutes * 60);
        int ms = (int)(seconds*100) - (int)seconds * 100;
        return minutes.ToString("00") + ":" + remaining.ToString("00")  + ":" + ms.ToString("00") ;
    }
    
    
}
