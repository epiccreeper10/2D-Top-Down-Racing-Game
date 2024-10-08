﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceController : MonoBehaviour
{

    public static RaceController instance;

    public TextMeshProUGUI timer;
    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI lastTime;
    public TextMeshProUGUI wrongDirection;
    private float raceTime;
    public bool raceStarted { get; private set; }
    private float bestRaceTime;
    private float lastRaceTime;

    int raceMinutes;
    int raceSeconds;
    float raceMiliseconds;

    public float BestRaceTime
    {
        get => bestRaceTime;
        set
        {
            bestRaceTime = value;
            PlayerPrefs.SetFloat("BestRace" + CanvasController.RaceId, value);
        }
    }

    public float LastRaceTime
    {
        get => lastRaceTime;
        set
        {
            lastRaceTime = value;
            PlayerPrefs.SetFloat("LastRace" + CanvasController.RaceId, value);
        }
    }

    private void Awake()
    {
        instance = this;
        raceStarted = false;        
    }

    public void OnRaceEnable()
    {
        BestRaceTime = PlayerPrefs.GetFloat("BestRace" + CanvasController.RaceId, 0);
        SetBestRaceText();
    }

    private void Update()
    {
        if (raceStarted)
        {
            raceTime += Time.deltaTime;
            raceMinutes = Mathf.FloorToInt(raceTime / 60F);
            raceSeconds = Mathf.FloorToInt(raceTime - raceMinutes * 60);
            raceMiliseconds = (raceTime - raceMinutes * 60 - raceSeconds) * 1000;
            timer.text = string.Format("{0:0}:{1:00}:{2:000}", raceMinutes, raceSeconds, raceMiliseconds);
            wrongDirection.enabled = CarController.instance.wrongDirection;
        }
    }

    public void StartRace()
    {
        if ((raceTime < BestRaceTime || BestRaceTime == 0 ) && raceTime > 1.0f)
        {
            BestRaceTime = raceTime;
            SetBestRaceText();
        }
        LastRaceTime = raceTime;
        SetLastRaceText();        
        wrongDirection.enabled = false;
        raceStarted = false;
        //Set raceStarted with delay so player can se he's previous time
        Invoke("StartRaceCounter", 1.0f);
    }

    private void StartRaceCounter()
    {
        raceStarted = true;
        raceTime = 1.0f;
    }

    private void SetBestRaceText()
    {
        int minutes = Mathf.FloorToInt(BestRaceTime / 60F);
        int seconds = Mathf.FloorToInt(BestRaceTime - minutes * 60);
        float miliseconds = (BestRaceTime - minutes * 60 - seconds) * 1000;
        bestTime.text = "Best - " + string.Format("{0:0}:{1:00}:{2:000}", minutes, seconds, miliseconds);
    }

    private void SetLastRaceText()
    {
        int minutes = Mathf.FloorToInt(LastRaceTime / 60F);
        int seconds = Mathf.FloorToInt(LastRaceTime - minutes * 60);
        float miliseconds = (LastRaceTime - minutes * 60 - seconds) * 1000;
        lastTime.text = "Last - " + string.Format("{0:0}:{1:00}:{2:000}", minutes, seconds, miliseconds);
    }

    public void ResetRace()
    {
        CancelInvoke("StartRaceCounter");
        raceStarted = false;
        raceTime = 0;
        timer.text = "0:00:000";        
    }
}
