using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class MenuScreen : ScreenBase
{
	#region Singleton

	private static MenuScreen _instance;

	public static MenuScreen instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<MenuScreen>();
			}
			return _instance;
		}
	}

	#endregion

	//	bool isTiltControl = true;

	public GameObject[] soundUI;

	public GameObject[] hideDuringInit;

	public GameObject menuScreenGO;
	public GameObject loginScreenGO;
	public GameObject nameScreenGO;
	public GameObject championshipGO;
	//	public GameObject tileControlObject;

	public GameObject onlineObj;
	public GameObject offlineObj;
	public Text idNetPlayerName;

	public int currState;

	//	#if !UNITY_WEBPLAYER || UNITY_WEBGL
	//	private TextMesh tiltControlText;
	//	#endif

    public GameObject playerNameGO;
	public Text playerName;

	// Use this for initialization
	protected override void Awake()
	{
		_instance = this;
		base.Awake();
		currState = 0;
        playerNameGO.SetActive(false);
       
	}

	protected override void Start()
	{
		base.Start();
	}

	public override void InitScreen()
	{
//        Debug.Log("currState::"+currState);
//		if(!IdnetManager.instance.isLoginShown)
//			nameBG.SetActive(false);
//		else
//		{
//			SetPlayerName();
//			nameBG.SetActive(true);
//		}
		menuScreenGO.SetActive(false);
		nameScreenGO.SetActive(false);
		loginScreenGO.SetActive(false);
		championshipGO.SetActive(false);

		if (currState == 0)
		{
			menuScreenGO.SetActive(true);
			foreach (Transform child in menuScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().InitialseCompement(true);
			}
		}
		else if (currState == 1)
		{
			ShowLoginUI();
		}
		else if (currState == 2)
		{
			ShowQuickorChamp();
		}
		else if (currState == 3)
		{
			ShowPlayerNameUI();
		}
		if (GlobalClass.SoundVoulme == 1)
		{
			soundUI[0].SetActive(true);
			soundUI[1].SetActive(false);
		}
		else
		{
			soundUI[1].SetActive(true);
			soundUI[0].SetActive(false);
		}


		base.Start();
		base.InitScreen();

//		transform.position = initPosition;

		OnScreenLoadComplete();
	}

	public override void OnScreenLoadComplete()
	{
		base.OnScreenLoadComplete();
	}

	public override void ExitScreen()
	{
		if (currState == 0)
		{
			foreach (Transform child in menuScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
		}
		else if (currState == 1)
		{
			foreach (Transform child in loginScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
		}
		else if (currState == 2)
		{
			foreach (Transform child in championshipGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
		}
		else if (currState == 3)
		{
			foreach (Transform child in nameScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
		}

		base.ExitScreen();
	}

	public override void ButtonClick(Button _button)
	{
//        Debug.Log("Button click : " + _button.name);
		base.ButtonClick(_button);
					 	
		if (_button.name == "Casual")
		{
			GlobalClass.gameMode = GlobalClass.GameMode.SinglePlayer;
			InitGame();	

		}
		else if (_button.name == "Multiplayer")
		{
			GlobalClass.isChampionship = false;
			GlobalClass.gameMode = GlobalClass.GameMode.Multiplayer;
			PhotonMenu.instance.InitPhoton();
			InitGame();	
		}
		else if (_button.name == "Achievements")
		{
			ExitScreen();
			foreach (Transform child in menuScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
					
			AchievementScreen.instance.InitScreen();
		}
		else if (_button.name == "Sound")
		{
			if (GlobalClass.SoundVoulme == 1)
			{
				GlobalClass.SoundVoulme = 0;
				soundUI[1].SetActive(true);
				soundUI[0].SetActive(false);
			}
			else
			{
				GlobalClass.SoundVoulme = 1;
				soundUI[0].SetActive(true);
				soundUI[1].SetActive(false);
			}
			AudioManager.instance.PlayMuteBG();
		}
		else if (_button.name == "Credits")
		{
			ExitScreen();

			CreditsScreen.instance.InitScreen();
		}
		else if (_button.name == "Sponsor Logo")
		{
			if (!GlobalClass.isHostedFromY8)
				Application.OpenURL(GlobalClass.SponsorLinkMenu);
		}
		else if (_button.name == "More Games")
		{
			if (!GlobalClass.isHostedFromY8)
				Application.OpenURL(GlobalClass.SponsorLinkMoreGames);
		}
		else if (_button.name == "Studd Logo")
		{
			Application.OpenURL("http://www.studdgames.com/");
		}
		else if (_button.name == "Id Net")
		{
			Application.OpenURL(GlobalClass.IDNet);
		}
		else if (_button.name == "Online")
		{
			isUpdateInput = false;
			foreach (Transform child in loginScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
			loginScreenGO.SetActive(false);

			GlobalClass.playerProfile = GlobalClass.Profile.Online;
            currState = 0;
		}
		else if (_button.name == "Local")
		{
			foreach (Transform child in loginScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
			loginScreenGO.SetActive(false);

            currState = 0;
			GlobalClass.playerProfile = GlobalClass.Profile.Local;
			if (LocalDataManager.instance.IsFirstTime() && GlobalClass.playerProfile == GlobalClass.Profile.Local)
			{ 
				ShowPlayerNameUI();
			}
			else
			{
				LocalDataManager.instance.InitLocalDataManager();
			}
		}
		else if (_button.name == "NameNext")
		{
//					GlobalClass.inputMode = isTiltControl ? GlobalClass.InputMode.TiltInput : GlobalClass.InputMode.TouchInput;
//					LocalDataManager.instance.TiltControl = isTiltControl ? 1 : 0;

			foreach (Transform child in nameScreenGO.transform)
			{
				if (child.GetComponent<GuiMove>())
					child.GetComponent<GuiMove>().Reset(true);
			}
			nameScreenGO.SetActive(false);
			LocalDataManager.instance.PlayerName = NameBar.nameBar.GetName();
			LocalDataManager.instance.InitLocalDataManager();
            currState = 0;
            playerNameGO.SetActive(true);
			playerName.text = GlobalClass.PlayerName;
		}
		else if (_button.name == "Championship")
		{
			GlobalClass.isChampionship = true;
			if (GlobalClass.newChampionship)
				ClassSelectionScreen.instance.InitScreen();
			else
			{
				MainDirector.instance.pointsTable = GlobalClass.championshipPointTable;
                TeamSelectionScreen.instance.teamNames = GlobalClass.teamNameArray;
				ChampionshipProgressScreen.instance.InitScreen();
			}
            currState = 0;
    

			ExitScreen();
		}
		else if (_button.name == "QuickRace")
		{
			GlobalClass.isChampionship = false;
			ClassSelectionScreen.instance.InitScreen();
            currState = 0;
			ExitScreen();
		}
		else if (_button.name == "InputControlButton")
		{
			#if !UNITY_WEBPLAYER && !UNITY_WEBGL
					//isTiltControl = !isTiltControl;
					//tiltControlText.text = isTiltControl? "Tilt Control : ON" : "Tilt Control : OFF";;
			#endif
				
		}

	}



	void InitGame()
	{
		foreach (Transform child in menuScreenGO.transform)
		{
			if (child.GetComponent<GuiMove>())
				child.GetComponent<GuiMove>().Reset(true);
		}
		menuScreenGO.SetActive(false);
		
//			if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
//				ShowQuickorChamp();
//			else
//			{
                TeamSelectionScreen.instance.InitScreen();
				ExitScreen(); 
//			}
	}

	public void ShowLoginUI()
	{
		currState = 1;
		loginScreenGO.SetActive(true);
		foreach (Transform child in loginScreenGO.transform)
		{
			if (child.GetComponent<GuiMove>())
				child.GetComponent<GuiMove>().InitialseCompement(true);
		}

			offlineObj.SetActive(true);
			onlineObj.SetActive(false);
			Debug.Log("no player logged in ");
	}

	public void ShowPlayerNameUI()
	{
		currState = 3;
		nameScreenGO.SetActive(true);
		// additional condition here to check login
		//only in case of local save
		foreach (Transform child in nameScreenGO.transform)
		{
			if (child.GetComponent<GuiMove>())
				child.GetComponent<GuiMove>().InitialseCompement(true);
		}
		NameBar.nameBar.InitNameBar();

	}

	public void ShowQuickorChamp()
	{
		currState = 2;
		championshipGO.SetActive(true);
		isUpdateInput = true;
        playerNameGO.SetActive(true);
		playerName.text = GlobalClass.PlayerName;
		foreach (Transform child in championshipGO.transform)
		{
			if (child.GetComponent<GuiMove>())
				child.GetComponent<GuiMove>().InitialseCompement(true);
		}
	}

	public void SetPlayerName()
	{
        playerNameGO.SetActive(true);
		playerName.text = GlobalClass.PlayerName;
	}

	public void EnableInputAndLoginGUI()
	{
		isUpdateInput = true;
		loginScreenGO.SetActive(true);
		foreach (Transform child in loginScreenGO.transform)
		{
			if (child.GetComponent<GuiMove>())
				child.GetComponent<GuiMove>().InitialseCompement(true);
		}
	}
}
