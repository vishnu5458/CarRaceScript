using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseScreen : ScreenBase
{

	#region Singleton
	private static PauseScreen _instance;
	public static PauseScreen instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<PauseScreen> ();
			}
			return _instance;
		}
	}
	#endregion

	public GameObject tileControlObject;
	//	public TextMesh missionNumber;
	//	public TextMesh missionType;
	//	public TextMesh missionDesc;

//	private TextMesh tiltControlText;

	protected override void Awake()
	{
		base.Awake();
		_instance = this;
		//tiltControlText = tileControlObject.GetComponent<TextMesh>();
		//		initTween = GetComponent<iTweenEvent>();
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
	}

	public override void InitScreen()
	{
		#if !UNITY_WEBPLAYER && !UNITY_WEBGL
		if(GlobalClass.inputMode == GlobalClass.InputMode.TiltInput)
		{
		//isTiltControl = true;
		//tiltControlText.text = "Tilt Control : ON";
		}
		else
		{
		//isTiltControl = false;
		//tiltControlText.text = "Tilt Control : OFF";
		}
		#else
//		tileControlObject.SetActive(false);
		#endif
		base.InitScreen();
		//		CarManager.GetInstance().SetCarSounds(false);
		MainDirector.GetCurrentGamePlay().SetCarSounds(false);
		OnScreenLoadComplete();

		//		missionNumber.text = "Mission "+ (GlobalClass.currentTrackIndex + 1).ToString();
		//		switch(MainDirector.GetCurrentGamePlay().currentMode)
		//		{
		//		case GameMode.Arrest:
		//			missionType.text = "Arrest Mission";
		//			missionDesc.text = "Stop all the enemy cars in the current level.";
		//			break;
		//		case GameMode.Boss:
		//			missionType.text = "Boss Mission";
		//			missionDesc.text = "Stop/Destroy the enemy boss before he escapes.";
		//			break;
		//		case GameMode.Hunt:
		//			missionType.text = "Hunt Mission";
		//			missionDesc.text = "Destroy all the enemy cars in the current level.";
		//			break;
		//		case GameMode.Escort:
		//			missionType.text = "Escort Mission";
		//			missionDesc.text = "Escort the target vehicle until it reaches its target.";
		//			break;
		//		case GameMode.DestroyHQ:
		//			missionType.text = "Destroy Enemy Base Mission";
		//			missionDesc.text = "Destroy the enemy base before the time runs out.";
		//			break;
		//		}
	}

	public override void OnScreenLoadComplete()
	{
		base.OnScreenLoadComplete();
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
	}

	/*protected override void UpdateInput ()
	{
		base.UpdateInput();

		if(selectButton)
		{
			if(selectButton == _uiButton)
			{
				if(_uiButton.name == "Main Menu")
				{
					Time.timeScale = 1;
					GlobalClass.isPause = false;
					MainDirector.instance.UnloadTrack();
					MainDirector.instance.LoadMenu("MenuScreen");

					ExitScreen();
				}
				else if(_uiButton.name == "Resume")
				{
					Time.timeScale = 1;
					GlobalClass.isPause = false;
					GlobalClass.isGameRunning = true;
					MainDirector.instance.OnResume();
					ExitScreen();

					if(GlobalClass.SoundVoulme == 1)
					{
						//						CarManager.GetInstance().SetCarSounds(true);
						MainDirector.GetCurrentGamePlay().SetCarSounds(true);
					}
				}
				else if(_uiButton.name == "Play Again")
				{
					Time.timeScale = 1;
					GlobalClass.isPause = false;
					MainDirector.instance.UnloadTrack();

					//                    IdnetTracker.CustomEvent("Retry_Level_" + (GlobalClass.currentTrackIndex + 1).ToString());
					//                    IdnetTracker.CustomEvent("Retry_Level", (GlobalClass.currentTrackIndex + 1).ToString());

					MainDirector.instance.InitialiseGame();
					ExitScreen();
				}
				else if(_uiButton.name == "More Games" || _uiButton.name == "Sponsor Logo")
				{
					if(!GlobalClass.isHostedFromY8)
						Application.OpenURL(GlobalClass.SponsorLinkMenu);
				}
				else if(_uiButton.name == "InputControlButton")
				{
					isTiltControl = !isTiltControl;
					//					tiltControlText.text = isTiltControl? "Tilt Control : ON" : "Tilt Control : OFF";
					//					GlobalClass.inputMode = isTiltControl ? GlobalClass.InputMode.TiltInput : GlobalClass.InputMode.TouchInput;
					//					LocalDataManager.instance.TiltControl = isTiltControl ? 1 : 0;
					if(isTiltControl)
					{
						tiltControlText.text = "Tilt Control : ON";
						GlobalClass.inputMode = GlobalClass.InputMode.TiltInput;
						LocalDataManager.instance.TiltControl = 1;
						//						GameplayScreen.instance.DisableTouchSteer();
					}
					else
					{
						tiltControlText.text = "Tilt Control : OFF";
						GlobalClass.inputMode = GlobalClass.InputMode.TouchInput;
						LocalDataManager.instance.TiltControl = 0;
						//						GameplayScreen.instance.EnableTouchSteer();
					}
				}
				selectButton = null;
			}
		}
	}
	*/

	public override void ButtonClick(Button _button)
	{
		base.ButtonClick (_button);

		if(_button.name == "Main Menu")
		{
			Time.timeScale = 1;
//            CountDown.countDownTimer.CloseCountDownUpdate();
            GlobalClass.isPause = false;
            MainDirector.instance.UnloadTrack();
            MainDirector.instance.LoadMenu("MenuScreen");

            ExitScreen();
        }
        else if(_button.name == "Resume")
        {
            Time.timeScale = 1;
            GlobalClass.isPause = false;
            GlobalClass.isGameRunning = true;
            MainDirector.instance.OnResume();
            ExitScreen();

            if(GlobalClass.SoundVoulme == 1)
            {
                //                      CarManager.GetInstance().SetCarSounds(true);
                MainDirector.GetCurrentGamePlay().SetCarSounds(true);
            }
        }
        else if(_button.name == "Play Again")
        {
            Time.timeScale = 1;
			GlobalClass.isPause = false;
			MainDirector.instance.UnloadTrack();


			MainDirector.instance.InitialiseGame();
			ExitScreen();
		}
		else if(_button.name == "More Games" || _button.name == "Sponsor Logo")
		{
			if(!GlobalClass.isHostedFromY8)
				Application.OpenURL(GlobalClass.SponsorLinkMenu);
		}
		else if(_button.name == "InputControlButton")
		{
			
		}
	}




}
