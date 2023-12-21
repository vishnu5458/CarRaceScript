using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

[Serializable]
public class GamePlayObjects
{
	public GameObject multiPlayerPosGO;
	public GameObject[] singlePlayerGO;
	public Text currLapTextMesh;
	public Text totalLapTextMesh;

	public Text currLapTimerTextMesh;
	public Text lastLapTimeTextMesh;

	public Text currPosTextMesh;
	public Text totalVehiclesTextMesh;
	public Text speedometer;
	public Image nitroBar;
	public Image nitroIcon;
	//	public TextMesh playerAmmoCount;

	//	public GameObject[] powerUpGUI;
	public Transform playerPosDot;
	public Transform escortPosDot;
	public Transform buildingPosDot;
	public Transform[] aiPosDot;

	//	public Transform enemyHealthBase;
	//	public Transform arrestBarBase;
	//	public Transform bigHealthbarBase;
    public Text[] multiplayerClientPos;
	public GameObject wrongWayObject;

	//	public Transform playerHealthBarBase;
	//	public Transform aiHealthBarParent;
	//	public GameObject mineTutorial;
	public GameObject brakingTutorial;
    //  public FloatingHealth[] aiHealthBars;
}

public class GamePlayScreen : ScreenBase
{
    #region Singleton

    private static GamePlayScreen _instance;

    public static GamePlayScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GamePlayScreen>();
            }
            return _instance;
        }
    }

    #endregion

    public GamePlayObjects gamePlayObjects = new GamePlayObjects();

    [Space(10)]
    [Header("Game Modes")]
    public GameObject[] timeTrialGO;
    public GameObject[] raceGO;
    public GameObject[] chaseGO;
    public GameObject[] huntGO;
    public GameObject[] arrestGO;
    public GameObject[] bossGO;
    public GameObject[] destroyHQGO;
    public GameObject[] escortGO;


    //  private UIProgressBar enemyHQHealthBar;
    //  private UIProgressBar bigHealthBar;

    //  private UIProgressBar arrestBar;
    //  private SimpleBlinker arrestBarBlinker;

    private GameMode currGameMode;
    //  private UIProgressBar playerHealthBar;
    //  private SimpleBlinker playerHealthBarBlinker;
    private SimpleBlinker timeBlinker;
    private Vector3 initPos;

    public GameObject weaponTutorial;
    private bool isTutorialActive = false;
    private int tutorialID;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;

		initPos = trans.localPosition;
        GlobalClass.inputMode = GlobalClass.InputMode.TiltInput;

        //      bigHealthBar = gamePlayObjects.bigHealthbarBase.transform.GetChild(1).GetComponent<UIProgressBar>();
        //      arrestBar = gamePlayObjects.arrestBarBase.transform.GetChild(1).GetComponent<UIProgressBar>();
        //      playerHealthBar = gamePlayObjects.playerHealthBarBase.transform.GetChild(1).GetComponent<UIProgressBar>();
        //
        //      playerHealthBarBlinker = gamePlayObjects.playerHealthBarBase.GetComponent<SimpleBlinker>();
        //      arrestBarBlinker = gamePlayObjects.arrestBarBase.GetComponent<SimpleBlinker>();
        timeBlinker = gamePlayObjects.currLapTimerTextMesh.GetComponent<SimpleBlinker>();

        //      gamePlayObjects.aiHealthBars = gamePlayObjects.aiHealthBarParent.GetComponentsInChildren<FloatingHealth>();
    }

	// Use this for initialization
	protected override void Start()
	{
		base.Start();
    }

	public override void InitScreen()
	{
		base.InitScreen();
//		MiniMapCamera.miniMapCamera.EnableCamera();
        weaponTutorial.SetActive(false);
		OnScreenLoadComplete();
        trans.localPosition = new Vector3(0,0,950);
		//		foreach(FloatingHealth _flHealth in gamePlayObjects.aiHealthBars)
		//			_flHealth.SetVisibility(true);
	}

	public override void OnScreenLoadComplete()
	{
		base.OnScreenLoadComplete();
	}

    public void InitTutorial(int id)
    {
        tutorialID = id;
        Invoke("ShowTutorial", 0);
    }

    void ShowTutorial()
    {
        isTutorialActive = true;
        weaponTutorial.SetActive(true);

    }

    void HideTurorial()
    {
        isTutorialActive = false;
        weaponTutorial.SetActive(false);
        LocalDataManager.instance.SetTutorialCount(tutorialID);
    }

	public override void ExitScreen()
	{
//        MiniMapCamera.miniMapCamera.DisableCamera();
		//		foreach(FloatingHealth _flHealth in gamePlayObjects.aiHealthBars)
		//			_flHealth.SetVisibility(false);
		base.ExitScreen();
        trans.localPosition = initPos;
	}

	public void EndGame()
	{
		ExitScreen();
	}

	public void InitialiseGame()
	{ 

		foreach (GameObject go in timeTrialGO)
			go.SetActive(false);

		foreach (GameObject go in raceGO)
			go.SetActive(false);

		foreach (GameObject go in chaseGO)
			go.SetActive(false);

		foreach (GameObject go in huntGO)
			go.SetActive(false);

		foreach (GameObject go in arrestGO)
			go.SetActive(false);

		foreach (GameObject go in bossGO)
			go.SetActive(false);

		foreach (GameObject go in destroyHQGO)
			go.SetActive(false);

		foreach (GameObject go in escortGO)
			go.SetActive(false);

		currGameMode = MainDirector.GetCurrentGamePlay().currentMode;

		if (currGameMode == GameMode.TimeTrial)
		{
			foreach (GameObject go in timeTrialGO)
				go.SetActive(true);

		}
		else if (currGameMode == GameMode.Race)
		{
			foreach (GameObject go in raceGO)
				go.SetActive(true);

			if (GlobalClass.isCircuit)
			{
				gamePlayObjects.totalLapTextMesh.text = "/" + GlobalClass.totalLaps.ToString();
				gamePlayObjects.currLapTextMesh.text = "1";
			}
			else
			{
				gamePlayObjects.totalLapTextMesh.transform.parent.gameObject.SetActive(false);
			}
			if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
			{
				gamePlayObjects.multiPlayerPosGO.SetActive(false);
				gamePlayObjects.totalVehiclesTextMesh.text = "/" + (GlobalClass.totalBots).ToString(); 
				gamePlayObjects.currPosTextMesh.text = (GlobalClass.totalBots).ToString(); 
				foreach (GameObject obj in gamePlayObjects.singlePlayerGO)
					obj.SetActive(true);
			}
			else
			{
				gamePlayObjects.multiPlayerPosGO.SetActive(true);
				gamePlayObjects.totalVehiclesTextMesh.text = "/" + (PhotonNetwork.playerList.Length).ToString(); 
				UpdateMultiplayerClientPos();
				foreach (GameObject obj in gamePlayObjects.singlePlayerGO)
					obj.SetActive(false);
			}
		}
		else if (currGameMode == GameMode.Chase)
		{
			foreach (GameObject go in chaseGO)
				go.SetActive(true);

			//			arrestBarBlinker.DisableBlinker();
			//			arrestBar.Percentage = 0.01f;
		}
		else if (currGameMode == GameMode.Hunt || currGameMode == GameMode.Boss || currGameMode == GameMode.Arrest)
		{
			foreach (GameObject go in huntGO)
				go.SetActive(true);

			//			arrestBarBlinker.DisableBlinker();
			//			arrestBar.Percentage = 0.01f;
		}
		else if (currGameMode == GameMode.DestroyHQ)
		{
			foreach (GameObject go in destroyHQGO)
				go.SetActive(true);
			//			bigHealthBar.Percentage = 1.0f;
		}
		else if (currGameMode == GameMode.Escort)
		{
			foreach (GameObject go in escortGO)
				go.SetActive(true);
			//			bigHealthBar.Percentage = 1.0f;
		}

		gamePlayObjects.lastLapTimeTextMesh.gameObject.SetActive(false);

		UpdateTime(0);
        gamePlayObjects.currLapTimerTextMesh.enabled = true;
		AudioManager.instance.OnGamePlayLoad(); // play audio when count down starting

		timeBlinker.DisableBlinker();
        UpdateNitroMeter(0.0f);
		gamePlayObjects.nitroIcon.enabled = false;
		CountDown.countDownTimer.InitializeCountDown();

	}

	public void CountDownOver()
	{
		//		if(GlobalClass.gunUnlockedArray[5] && !PlayerPrefs.HasKey("MineTutorialShown"))
		//		{
		//			StartCoroutine(MineTutorial());
		//		}
	}

    public IEnumerator BrakingTutorial()
	{
		yield return new WaitForSeconds(0.5f);

		gamePlayObjects.brakingTutorial.SetActive(true);

		yield return new WaitForSeconds(4);

		gamePlayObjects.brakingTutorial.SetActive(false);
	}

	#region Update

	protected override void UpdateInput()
	{
		base.UpdateInput();

        if (GlobalClass.isGameRunning) //  || GlobalClass.isPause // added here cause mute should work even in game pause
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (GlobalClass.SoundVoulme == 1)
                {
                    GlobalClass.SoundVoulme = 0;
                    //                  CarManager.GetInstance().SetCarSounds(false);
                    MainDirector.GetCurrentGamePlay().SetCarSounds(false);
                }
                else
                {
                    GlobalClass.SoundVoulme = 1;
                    //                  CarManager.GetInstance().SetCarSounds(true);
                    MainDirector.GetCurrentGamePlay().SetCarSounds(true);
                }
                AudioManager.instance.PlayMuteBG();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (MainDirector.GetCurrentPlayer().GetComponent<CarBase>().isNitroSet)
                {
					UpdateNitroMeter(0.0f);
                    MainDirector.GetCurrentPlayer().GetComponent<CarBase>().BoostCollected(1);
                    AchievementManager.instance.OnNitroCollect();
                }
                if (isTutorialActive)
                {
                    HideTurorial();
                }
            }
			if (Input.GetKeyDown(KeyCode.C))
				MainDirector.GetCurrentPlayer().SwitchCamera();
            if (Input.GetKeyDown(KeyCode.R))
                MainDirector.GetCurrentPlayer().OnVehicleReset();
            if (Input.GetKeyDown(KeyCode.T) && Input.GetKeyDown(KeyCode.Y))
                MainDirector.GetCurrentPlayer().ApplySuperCheat();
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!GlobalClass.isPause)
                {
                    OnPause();
                }
            }

        }   



        if (!GlobalClass.isGameRunning)
            return;
              
		#if !UNITY_WEBPLAYER && !UNITY_WEBGL
		OnTouchInput();
		#endif
	}

	public void UpdateTime(int time)
	{
		System.TimeSpan t = System.TimeSpan.FromSeconds(time);
		gamePlayObjects.currLapTimerTextMesh.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds); 
	}

	public void UpdateSpeedoMeter(float speed)
	{
		gamePlayObjects.speedometer.text = Mathf.Abs(Mathf.FloorToInt(speed * 3.0f)).ToString();
	}

	public void SetLocalPlayerPosition(int pos)
	{
		gamePlayObjects.currPosTextMesh.text = pos.ToString(); 
	}

	public void SetMulPlayerPos(string name, int index)
	{
		gamePlayObjects.multiplayerClientPos[index].text = name;
	}

    public void UpdateNitroMeter(float percent)
    {
		gamePlayObjects.nitroBar.fillAmount = percent;
    }

    void UpdateReduceNitro(float percet)
    {
        gamePlayObjects.nitroBar.fillAmount = percet;
    }
	public void UpdateArrestBar(float percent)
	{
		//		arrestBar.Percentage = percent;
		//
		//		if(currGameMode == GameMode.Chase)
		//		{
		//			if(percent > 0.8f)
		//			{
		//				arrestBarBlinker.EnableBlinker();
		//			}
		//			else
		//			{
		//				arrestBarBlinker.DisableBlinker();
		//			}
		//		}
	}

	public void UpdateBigHealthBar(float percent)
	{
		//		bigHealthBar.Percentage = percent;
	}

	public void UpdateAmmoCount(int count)
	{
		//		gamePlayObjects.playerAmmoCount.text = count.ToString();
	}

	public void PlayerCarLapUp(int currLap)
	{
		Debug.Log(currLap);
		if (currLap == 1)
		{
			return;
		}

		gamePlayObjects.currLapTextMesh.text = currLap.ToString();

		gamePlayObjects.lastLapTimeTextMesh.gameObject.SetActive(true);
		System.TimeSpan t = System.TimeSpan.FromSeconds(MainDirector.GetCurrentPlayer().GetNavBase().lapTime);
		gamePlayObjects.lastLapTimeTextMesh.text = string.Format("{0:00} : {1:00}", t.Minutes, t.Seconds); 
		ScoreManager.instance.OnPlayerLapUp(MainDirector.GetCurrentPlayer().GetNavBase().lapTime);
		MainDirector.GetCurrentPlayer().GetNavBase().lapTime = 0;
		Invoke("HideLapTimer", 2);
	}

	public void PlayerCarHealthBarLerp(float health)
	{
		//		playerHealthBar.Percentage = health;
		//		
		//		if(health < 0.2f)
		//		{
		//			playerHealthBarBlinker.EnableBlinker();
		//		}
		//		else
		//		{
		//			playerHealthBarBlinker.DisableBlinker();
		//		}
	}

	#region Touch Input

	void OnTouchInput()
	{
		bool isInputPressed = false; 
		bool isSteeringPressed = false;
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; ++i)
		{
			Touch touch = Input.touches[i];
			Vector3 position = touch.position;
			Ray ray = thisCamera.ScreenPointToRay(position);
			RaycastHit hit;
			Transform currButton;

			if (Physics.Raycast(ray, out hit, 1000, layer))
			{
				//				isButtonTouched = true;
				currButton = hit.transform;

				VehicleInput currVehicleInput = MainDirector.GetCurrentPlayer().GetVehicleInput();
				//								Debug.Log("cal here on button : " + currButton.name);	
				if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
				{
					if (currButton.localScale == Vector3.one)
						currButton.localScale *= 0.95f;

					if (currButton.name == "Accelerator")
					{
						isInputPressed = true;
						currVehicleInput.Accell = 1;
					}

					if (currButton.name == "Brake")
					{
						isInputPressed = true;
						currVehicleInput.Accell = -1;
					}

					if (currButton.name == "Left Steer")
					{
						isSteeringPressed = true;
						currVehicleInput.Steer = -1;
					}
					else if (currButton.name == "Right Steer")
					{
						isSteeringPressed = true;
						currVehicleInput.Steer = 1;
					}
				}
				else if (touch.phase == TouchPhase.Ended)
				{
					if (currButton.localScale != Vector3.one)
						currButton.localScale = Vector3.one;

					if (currButton.name == "Pause")
					{
						OnPause();
					}

					if (currButton.name == "ResetCar")
					{
						MainDirector.GetCurrentPlayer().OnVehicleReset();
					}
				}
			}

		}

		if (!isInputPressed)
			MainDirector.GetCurrentPlayer().GetVehicleInput().Accell = 0;
		if (!isSteeringPressed)
			MainDirector.GetCurrentPlayer().GetVehicleInput().Steer = 0;

	}

	#endregion

	#endregion

	public void BlinkTimer()
	{
		timeBlinker.EnableBlinker();
	}

	public void SetWrongWay(bool status)
	{
		if (status == gamePlayObjects.wrongWayObject.activeSelf)
			return;

		if (status)
		{
			gamePlayObjects.wrongWayObject.SetActive(true);
			gamePlayObjects.wrongWayObject.GetComponent<SimpleBlinker>().EnableBlinker();
		}
		else
		{
			gamePlayObjects.wrongWayObject.SetActive(false);
			gamePlayObjects.wrongWayObject.GetComponent<SimpleBlinker>().DisableBlinker();
		}
	}

	public void UpdateMultiplayerClientPos()
	{
        foreach (Text _textMesh in gamePlayObjects.multiplayerClientPos)
			_textMesh.gameObject.SetActive(false);

		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			gamePlayObjects.multiplayerClientPos[i].gameObject.SetActive(true);
		}
		gamePlayObjects.totalVehiclesTextMesh.text = "/" + PhotonNetwork.playerList.Length.ToString();
	}

	void OnPause()
	{
		ExitScreen();
		GlobalClass.isPause = true;
		GlobalClass.isGameRunning = false;
		Time.timeScale = 0;
		PauseScreen.instance.InitScreen();
	}

	void HideLapTimer()
	{
		gamePlayObjects.lastLapTimeTextMesh.gameObject.SetActive(false);
	}


	public override void ButtonClick(Button _button)
	{
		base.ButtonClick(_button);

        if (_button.name == "Pause")
		{
			if (!GlobalClass.isPause)
			{
				OnPause();
			}
		}
		else if (_button.name == "ResetCar")
		{
			MainDirector.GetCurrentPlayer().OnVehicleReset();
		}
		else if (_button.name == "Camera")
		{
            MainDirector.GetCurrentPlayer().SwitchCamera();
		}
		else if (_button.name == "Home")
		{
			//					if(GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
			//					{
			//						GameManager.GetInstance().OnEndGame();
			//						GameManager.GetInstance().LoadMenu("MenuScreen");
			//						GameplayScreen.GetInstance().ExitScreen();
			//					}
			//					else
			//					{
			GlobalClass.isRaceStarted = false;
			PhotonNetwork.Disconnect();
			MainDirector.instance.UnloadTrack();
			MainDirector.instance.LoadMenu("MenuScreen");
			ExitScreen();
			//					}

		}
		
	}

	#region PowerUP

    public void SetNitro(bool status)
    {
		gamePlayObjects.nitroIcon.enabled = status;
    }
        

	//	public void SetPowerUp(PowerUpType _powType)
	//	{
	//		if(isFirstTime)
	//		{
	//			weaponTutorial.enabled = true;
	//			isFirstTime = false;
	//			isWeaponTutSet = true;
	//		}
	//
	//		foreach(GameObject go in powerUpGUI)
	//			go.SetActive(false);
	//
	//		if(_powType == PowerUpType.Nitro)
	//		{
	//			powerUpGUI[0].SetActive(true);
	//		}
	//		else if(_powType == PowerUpType.Blaster)
	//		{
	//			powerUpGUI[1].SetActive(true);
	//		}
	//		else if(_powType == PowerUpType.Twister)
	//		{
	//			powerUpGUI[2].SetActive(true);
	//		}
	//	}
	//
	//	public void OnPowerUpUse()
	//	{
	//		foreach(GameObject go in powerUpGUI)
	//			go.SetActive(false);
	//
	//		if(isWeaponTutSet)
	//		{
	//			isWeaponTutSet = false;
	//			weaponTutorial.enabled = false;
	//		}
	//	}
	//

	#endregion
}
