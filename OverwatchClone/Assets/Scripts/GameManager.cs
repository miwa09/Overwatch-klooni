﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public SpawnMaster spawnMaster;
    public int intensity = 1;
    int minutes = 6;
    int seconds = 0;
    bool minuteHasPassed = false;
    float timer = 0;
    float secTicker = 1;
    int secondsPassed = 0;
    public Text clock;
    public Text gameOverUI;
    public Text resetUI;
    public Text doorUI;
    public float doorHP = 2000;
    public int playersDead = 0;
    public bool spawnEnemies = true;
    public bool finalStage = false;
    public int bossedDead = 0;
    bool paused = false;

    private void Update() {
        Clock();
        if (doorHP <= 0) {
            if (doorHP < 0) {
                doorHP = 0;
            }
            GameLost();
        }
        if (playersDead == 2) {
            GameLost();
        }
        if (minutes <= 0 && seconds <= 0) {
            minutes = 0;
            seconds = 0;
            finalStage = true;
        }
        if (finalStage) {
            if (bossedDead == 3) {
                GameWon();
            }
        }
        if ((Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.P)) && !paused) {
            GamePause();
        } else if ((Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.P)) && paused) {
            Time.timeScale = 1;
            gameOverUI.text = "";
            resetUI.text = "";
            paused = false;
        }

    }

    void Clock() {
        timer += Time.deltaTime;
        while (timer >= secTicker) {
            timer -= secTicker;
            secondsPassed++;
        }
        if (seconds < 60 && !minuteHasPassed && seconds != 0) {
            minutes--;
            minuteHasPassed = true;
        }
        if (secondsPassed > 0) {
            seconds = 60 - secondsPassed;
        }
        if (secondsPassed >= 60) {
            secondsPassed = 0;
            minuteHasPassed = false;
        }
        if (seconds < 10) {
            clock.text = "" + minutes + ":0" + seconds;
        } else clock.text = "" + minutes + ":" + seconds;
        doorUI.text = "" + doorHP;
    }

    void GameLost() {
        Time.timeScale = 0;
        gameOverUI.text = "Game Over!";
        resetUI.text = "Press 'Start' to restart";
        if (Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick2Button7)) {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
            gameOverUI.text = "";
            resetUI.text = "";
        }
    }

    void GameWon() {
        Time.timeScale = 0;
        gameOverUI.text = "Game Won!";
        resetUI.text = "Press 'Start' to go again";
        if (Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick2Button7)) {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
            gameOverUI.text = "";
            resetUI.text = "";
        }
    }

    void GamePause() {
        paused = true;
        Time.timeScale = 0;
        gameOverUI.text = "Paused!";
        resetUI.text = "Press 'Start' to continue";
    }
}
