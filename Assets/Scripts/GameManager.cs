using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager = null;
    [SerializeField] private NPCManager npcManager = null;
    [SerializeField] private ClearScoreManager clearScoreManager = null;
    [SerializeField] private UserController userController = null;
    [SerializeField] private BuildingMapManager buildingMapManager = null;
    [SerializeField] private SoundsManager soundsManager = null;

    private void Awake()
    {
        Application.targetFrameRate = 30;


        timeManager.onNight += () =>
        {
            npcManager.SleepAll();
        };

        npcManager.onGameEnd += (isWin) =>
        {
            if (isWin)
                GameWin();
            else
                GameOver();
        };
    }

    private void GameOver()
    {
        clearScoreManager.SetScore(false);
        timeManager.StopTimeScale();
        userController.GameOver();
        soundsManager.PlayAudio(8);
    }

    private void GameWin()
    {
        StartCoroutine("GameWinCoroutine");
        userController.GameOver();
        soundsManager.PlayAudio(7);
    }

    private IEnumerator GameWinCoroutine()
    {
        buildingMapManager.LaunchRocket();
        yield return new WaitForSeconds(3.5f);
        timeManager.StopTimeScale();
        clearScoreManager.SetScore(true);
    }
}
