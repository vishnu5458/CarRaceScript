using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[Serializable]
public class GameModeObjects
{
    public GameObject[] timeTrialGO;
    public GameObject[] raceGO;
    public GameObject[] chaseGO;

    public GameObject[] huntGO;
    public GameObject[] destroyHQGO;
    public GameObject[] bossGO;
    public GameObject[] arrestGO;
    public GameObject[] escortGO;

    public void DisableGameObject()
    {
        foreach (GameObject go in timeTrialGO)
            go.SetActive(false);

        foreach (GameObject go in raceGO)
            go.SetActive(false);

        foreach (GameObject go in chaseGO)
            go.SetActive(false);

        foreach (GameObject go in huntGO)
            go.SetActive(false);

        foreach (GameObject go in destroyHQGO)
            go.SetActive(false);

        foreach (GameObject go in bossGO)
            go.SetActive(false);

        foreach (GameObject go in arrestGO)
            go.SetActive(false);

        foreach (GameObject go in escortGO)
            go.SetActive(false);
    }
}

[Serializable]
public class GameOverObjects
{
    public GameObject playAgainBtnGO;
    public GameObject nextBtnGO;
    public Text trackNum;
    public Image levelWin;
    public Image levelFail;
    public Image champWin;
    public Image champFail;
    public Text playerPos;
    public Text playerPosBonus;
    public Text totalTime;
    public Text bestTime;
    public Text levelScore;
    public Text coinCollectCount;
    public Text coinCollectBonus;

    //	public TextMesh playerName;
    public Text totalScore;
}

public class GameOverScreen : ScreenBase
{
    #region Singleton

    private static GameOverScreen _instance;

    public static GameOverScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameOverScreen>();
            }
            return _instance;
        }
    }

    #endregion

    //	public GameObject multiplayerGO;

    public PositionBox[] positionBoxArray;

    public GameOverObjects gameOverObj = new GameOverObjects();

    [Space(15)]
    [Header("Game Modes")]
    public GameModeObjects modeObjects;

    //	public GameObject gameOverSeal;
    public bool isChampionshipResultShown = false;
    // public EllipsoidParticleEmitter[] confettiParticleEmitter;
    private ScoreObject prevScoreObj;

    public Transform[] pages;
    public GameObject leftArrow;
    public GameObject rightArrow;

    public GameObject[] mobileObjects;

    private Vector3 exitPos = new Vector3(-1000, 0, 0);

    protected override void Awake()
    {
        base.Awake();
        _instance = this;

        #if UNITY_WEBPLAYER || UNITY_WEBGL
        foreach (GameObject go in mobileObjects)
            go.SetActive(false);
        #else
		foreach(GameObject go in mobileObjects)
		go.SetActive(true);
        #endif

        foreach (PositionBox _pbox in positionBoxArray)
        {
            _pbox.nameBox = _pbox.transform.GetChild(1).GetComponent<Text>();
            _pbox.timeBox = _pbox.transform.GetChild(2).GetComponent<Text>();
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public override void InitScreen()
    {
        base.InitScreen();
        OnScreenLoadComplete();
    }

    public void InitGameOverValues(ScoreObject scoreObj)
    {
        modeObjects.DisableGameObject();
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        pages[0].localPosition = Vector3.zero;
        pages[1].localPosition = exitPos;

        gameOverObj.trackNum.text = "Track " + (GlobalClass.currentTrackIndex + 1).ToString();

        isChampionshipResultShown = false;
        if (scoreObj.isTrackWon)
        {
            gameOverObj.levelWin.enabled = true;
            gameOverObj.levelFail.enabled = false;

        }
        else
        {
            gameOverObj.levelWin.enabled = false;
            gameOverObj.levelFail.enabled = true;
        }
               
        if (scoreObj.gameMode == GameMode.TimeTrial)
        {
            foreach (GameObject go in modeObjects.timeTrialGO)
                go.SetActive(true);

            scoreObj.playerHealth *= 100;
        }
        else if (scoreObj.gameMode == GameMode.Race)
        {
            foreach (GameObject go in modeObjects.raceGO)
                go.SetActive(true);

            rightArrow.SetActive(true);

            switch (scoreObj.playerPos)
            {
                case 1:
                    gameOverObj.playerPos.text = "1 st";
                    break;
                case 2:
                    gameOverObj.playerPos.text = "2 nd";
                    break;
                case 3:
                    gameOverObj.playerPos.text = "3 rd";
                    break;
                case 4:
                    gameOverObj.playerPos.text = "4 th";
                    break;
                case 5:
                    gameOverObj.playerPos.text = "5 th";
                    break;
                case 6:
                    gameOverObj.playerPos.text = "6 th";
                    break;
                case 7:
                    gameOverObj.playerPos.text = "7 th";
                    break;
                case 8:
                    gameOverObj.playerPos.text = "8 th";
                    break;
            }

            gameOverObj.playerPosBonus.text = scoreObj.positionBonus.ToString();

            RaceMode _raceMode = MainDirector.GetCurrentGamePlay().GetComponent<RaceMode>();
            int playerCount;

            if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
            {
                gameOverObj.playAgainBtnGO.SetActive(true);
                gameOverObj.nextBtnGO.SetActive(true);
                playerCount = positionBoxArray.Length;
                //				multiplayerGO.SetActive(true);
            }
            else
            {
                gameOverObj.playAgainBtnGO.SetActive(false);
                //				multiplayerGO.SetActive(false);
                playerCount = _raceMode.completedCarsTime.Count;

            }

            foreach (PositionBox pos in positionBoxArray)
                pos.gameObject.SetActive(false);

            for (int i = 0; i < playerCount; i++)
            {
                positionBoxArray[i].gameObject.SetActive(true);
                positionBoxArray[i].nameBox.text = (string)_raceMode.completedCarsNames[i];

                int totTime = (int)_raceMode.completedCarsTime[i];
                if (totTime != 0)
                {
                    System.TimeSpan t = System.TimeSpan.FromSeconds(totTime);
                    //              string format = "HH:"
                    positionBoxArray[i].timeBox.text = t.Minutes.ToString() + " : " + t.Seconds.ToString();
                }
                else
                {
                    //              totTime = (int)CarManager.GetInstance().completedCarsTime[i -1];
                    //              CarManager.GetInstance().completedCarsTime.Insert(i , totTime + Random.Range(1 , 10));
                    //              
                    //              int newTime =  (int)CarManager.GetInstance().completedCarsTime[i];
                    //              
                    //              System.TimeSpan t = System.TimeSpan.FromSeconds(newTime);
                    //              positionBoxArray[i].timeBox.text =  t.Minutes.ToString() + " : " + t.Seconds.ToString();
                    positionBoxArray[i].timeBox.text = "...";
                }
            }

            if (scoreObj.championshipinProgress == 1)
            {
                gameOverObj.playAgainBtnGO.SetActive(false);
                gameOverObj.levelWin.enabled = false;
                gameOverObj.levelFail.enabled = false;
                gameOverObj.champWin.enabled = false;
                gameOverObj.champFail.enabled = false;
            }
            else if (scoreObj.championshipinProgress == 2)
            {
                //celebrations here
                // foreach (EllipsoidParticleEmitter confitti in confettiParticleEmitter)
                //     confitti.emit = true;
                ShowFinalResult(scoreObj);
            }
            else if (scoreObj.championshipinProgress == 3)
            {
                ShowFinalResult(scoreObj);
            }
            else
            {
                gameOverObj.champWin.enabled = false;
                gameOverObj.champFail.enabled = false;
            }

        }
        else if (scoreObj.gameMode == GameMode.Chase)
        {
            foreach (GameObject go in modeObjects.chaseGO)
                go.SetActive(true);
        }
        else if (scoreObj.gameMode == GameMode.Hunt || scoreObj.gameMode == GameMode.Boss || scoreObj.gameMode == GameMode.Arrest || scoreObj.gameMode == GameMode.DestroyHQ)
        {
            foreach (GameObject go in modeObjects.huntGO)
                go.SetActive(true);
        }
        else if (scoreObj.gameMode == GameMode.Escort)
        {
            foreach (GameObject go in modeObjects.escortGO)
                go.SetActive(true);
        }

        System.TimeSpan _t;
        _t = System.TimeSpan.FromSeconds(scoreObj.trackRecord);
        gameOverObj.bestTime.text = string.Format("{0:00} : {1:00}", _t.Minutes, _t.Seconds); 

        _t = System.TimeSpan.FromSeconds(scoreObj.totalTime);
        gameOverObj.totalTime.text = string.Format("{0:00} : {1:00}", _t.Minutes, _t.Seconds); 

//        gameOverObj.coinCollectCount.text = (scoreObj.coinCollectedBonus / 500).ToString() + " x 500";
        gameOverObj.coinCollectBonus.text = scoreObj.coinCollectedBonus.ToString();


        gameOverObj.levelScore.text = scoreObj.levelScore.ToString();
        gameOverObj.totalScore.text = GlobalClass.highScore.ToString();

        AudioManager.instance.OnMenuLoad();
        Debug.Log("Game Over Screen Done");

     
        //		gameOverObj.playerName.text = "Name : " + GlobalClass.PlayerName;
    }

    public void SetPlayerName()
    {
        //		gameOverObj.playerName.text = "Name : " + GlobalClass.PlayerName;
    }

    public override void OnScreenLoadComplete()
    {
        base.OnScreenLoadComplete();
    }

    //master-client only; need to trigger gameover for all clients
    public void CheckForGameOver()
    {
        if (IsInvoking("ShowNextButton"))
            CancelInvoke("ShowNextButton");
        Debug.Log("Hiding next button");
        gameOverObj.nextBtnGO.SetActive(false);

        int playersRemain = PhotonNetwork.playerList.Length - PhotonRaceManager.instance.loadedPlayers;
        if (playersRemain == 3)
        {
            Invoke("ShowNextButton", 14);
        }
        else if (playersRemain == 2)
        {
            Invoke("ShowNextButton", 9);
        }
        else if (playersRemain == 1)
        {
            Invoke("ShowNextButton", 9);
        }
        else
        {
            ShowNextButton();
        }
    }

    private void ShowNextButton()
    {
        gameOverObj.nextBtnGO.SetActive(true);
    }

    public override void ExitScreen()
    {
        base.ExitScreen();
    }

    public override void ButtonClick(Button _button)
    {
        base.ButtonClick(_button);
        Debug.Log("Button Clicked");

        if (_button.name == "Next")
        {
            if (!isChampionshipResultShown)
            {
                Time.timeScale = 1;
                ExitScreen();
                MainDirector.instance.UnloadTrack();
                if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
                {
                    MainDirector.instance.LoadMenu("CarScreen");
                }
                else
                {
                    //						PhotonRaceManager.instance.loadedPlayers = 0;
                    PhotonMenu.instance.DisconnectFromPhoton();
                    MainDirector.instance.LoadMenu("MenuScreen");
                }
            }
            else
                OnChampNextClick();
        }
        else if (_button.name == "Play Again")
        {
            Time.timeScale = 1;
            MainDirector.instance.UnloadTrack();

            MainDirector.instance.InitialiseGame();
            ExitScreen();
        }
        else if (_button.name == "Share")
        {
            //					AdmobManager.instance.Share();
        }
        else if (_button.name == "Arrow L")
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);

            Hashtable ht = new Hashtable();
            ht.Add("position", Vector3.zero);
            ht.Add("time", .5f);
            ht.Add("islocal", true);
            ht.Add("ignoretimescale", true);
            iTween.MoveTo(pages[0].gameObject, ht);

            Hashtable ht1 = new Hashtable();
            ht1.Add("position", exitPos);
            ht1.Add("time", .5f);
            ht1.Add("islocal", true);
            ht1.Add("ignoretimescale", true);
            iTween.MoveTo(pages[1].gameObject, ht1);

            pages[1].localPosition = exitPos;

        }
        else if (_button.name == "Arrow R")
        {
            rightArrow.SetActive(false);
            leftArrow.SetActive(true);


            Hashtable ht = new Hashtable();
            ht.Add("position", Vector3.zero);
            ht.Add("time", .5f);
            ht.Add("islocal", true);
            ht.Add("ignoretimescale", true);
            iTween.MoveTo(pages[1].gameObject, ht);

            Hashtable ht1 = new Hashtable();
            ht1.Add("position", -exitPos);
            ht1.Add("time", .5f);
            ht1.Add("islocal", true);
            ht1.Add("ignoretimescale", true);
            iTween.MoveTo(pages[0].gameObject, ht1);
        }

    }

    void ShowFinalResult(ScoreObject _scrOBj)
    {
        gameOverObj.playAgainBtnGO.SetActive(false);
        isChampionshipResultShown = true;
        gameOverObj.levelWin.enabled = false;
        gameOverObj.levelFail.enabled = false;

        prevScoreObj = _scrOBj;
        if (prevScoreObj.championshipinProgress == 2)
        {
            _scrOBj.isTrackWon = true;
            gameOverObj.champWin.enabled = true;
            gameOverObj.champFail.enabled = false;
        }
        else if(prevScoreObj.championshipinProgress == 3)
        {
            _scrOBj.isTrackWon = false;
            gameOverObj.champFail.enabled = true;
            gameOverObj.champWin.enabled = false;
        }
        prevScoreObj.championshipinProgress = -1;
        pages[1].localPosition = Vector3.zero;
        pages[0].localPosition = exitPos;

        for (int i = 0; i < 8; i++)
        {
            positionBoxArray[i].gameObject.SetActive(true);
            positionBoxArray[i].nameBox.text = MainDirector.instance.pointsTable[i].name;
            positionBoxArray[i].timeBox.text = MainDirector.instance.pointsTable[i].points.ToString();
        }
        Invoke("HideConfettiEffect",2);
        MainDirector.instance.ResetChampionship();
        MainDirector.instance.SaveData();
    }

    void HideConfettiEffect()
    {
        // foreach (EllipsoidParticleEmitter confitti in confettiParticleEmitter)
        //     confitti.emit = false;
    }

    void OnChampNextClick()
    {
        isChampionshipResultShown = false;
        InitGameOverValues(prevScoreObj);
    }

}


