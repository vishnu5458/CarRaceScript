using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarScreen : ScreenBase
{
	#region Singleton
	private static CarScreen _instance;
	public static CarScreen instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<CarScreen> ();
			}
			return _instance;
		}
	}
	#endregion

//	public GameObject[] soundUI;

	public bool isDataChanged = false;

	public Transform platformTrans;
	public Transform carParent;

	public Camera garageCamera;
	public GarageCars[] carsGO;	
    public GameObject[] playerAvatars;
//	public Transform[] gunsGO;

	public GameObject[] lockTexts;
	public Light[] garageLights;

	public TextMesh carName;
//	public Renderer[] engineUpgradeRenderer;
//	public Renderer[] armorUpgradeRenderer;
//	public TextMesh playerCredit;
//	public SpriteRenderer levelUpIcon;
//	public TextMesh levelUpText1;
//	public TextMesh levelUpText2;
//	public Sprite[] rankIcons;
//	public TextMesh playerRank;
//  public GameObject upgradeTutorialGO;
//	public Transform upgradeButton;
//  private bool isTutorialShown = false;

	public GameObject lockedTexture;

	private int noCars;
	private int currentCarIndex;
//	private Transform initGunParent;
	
	protected override void Awake()
	{
		base.Awake();
		_instance = this;

//		initGunParent = gunsGO[0].parent;
		noCars = carsGO.Length;
	}
	
	protected override void Start () 
	{
		base.Start();
	}

    public override void InitScreen()
	{
		base.InitScreen();

//		playerCredit.text = GlobalClass.totalCredits.ToString();
/*        if(GlobalClass.currentAvatar == AvatarType.Boy)
        {
            playerAvatars[0].SetActive(true);
            playerAvatars[1].SetActive(false);
        }
        else
        {
            playerAvatars[0].SetActive(false);
            playerAvatars[1].SetActive(true);
        }*/
		garageCamera.enabled = true;

		for (int i = 0; i < carsGO.Length; i++)
		{
			OffStage (i);
//			if (GlobalClass.carUnlockedArray [i] || GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
//				carsGO [i].UnlockCar ();
//			else
//				carsGO [i].LockCar ();
		}

		if (GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
			currentCarIndex = Random.Range (0, 5);//GlobalClass.currentCarIndex;
		else
			currentCarIndex = GlobalClass.currentCarIndex;

		OnScreenLoadComplete();
	}

	public override void OnScreenLoadComplete()
	{
		base.OnScreenLoadComplete();
        ToStage(currentCarIndex, 30);
        		
	}


	public override void ExitScreen()
	{
		base.ExitScreen();
		garageCamera.enabled = false;
		if(isDataChanged)
			MainDirector.instance.SaveData();
	}

	public override void ButtonClick(Button _button)
	{
		base.ButtonClick(_button);
//		platformTrans.Rotate(Vector3.up * Time.deltaTime * 10);


				
				if(_button.name == "Next")
				{
					if(!GlobalClass.carUnlockedArray[currentCarIndex] && GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
						return;
					ExitScreen();
					if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
						TrackScreen.instance.InitScreen ();
					else
						MultiplayerScreen.instance.InitScreen ();
					GlobalClass.currentCarIndex = currentCarIndex;
				}
//				else if(_uiButton.name == "Gun")
//				{
//    				ShowGunUI();
//				}
//				else if(_uiButton.name == "Upgrade")
//				{
//					if(GlobalClass.carDetailsArray[currentCarIndex].isUnlocked)
//						ShowUpgradeUI();
//					else
//						Debug.Log("car locked");
//				}
                else if(_button.name == "Tut Next")
                {
                    TutorialManager.instance.OnNextClick();
                }
                else if(_button.name == "Tut Skip")
                {
                    TutorialManager.instance.OnSkipClick();
                }
				else if(_button.name == "Arrow L")
				{
					PrevCar();
				}
				else if(_button.name == "Arrow R")
				{
					NextCar();
				}
				else if(_button.name == "Back")
				{
					GlobalClass.currentCarIndex = currentCarIndex;
					ExitScreen();
					MenuScreen.instance.InitScreen();
				}
				else if(_button.name == "More Games" || _button.name == "Sponsor Logo")
				{
					if(!GlobalClass.isHostedFromY8)
						Application.OpenURL(GlobalClass.SponsorLinkMenu);

		}

//		if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.K))
//		{
//			GlobalClass.carDetailsArray[currentCarIndex].isUnlocked = true;
//		}
//
//		if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.L))
//		{
//			GlobalClass.totalCredits += 10000;
//			playerCredit.text = GlobalClass.totalCredits.ToString();
//		}

		if(Input.GetMouseButtonDown(0))
		{
//			Ray ray;
//			RaycastHit hit;
//			Vector3 position = Input.mousePosition;
//			ray = thisCamera.ScreenPointToRay(position);
			
//			if (Physics.Raycast(ray, out hit,100,layer))
//			{
//				CarPanelObject obj = hit.transform.GetComponent<CarPanelObject>();
//				if(obj && GlobalClass.carUnlockedArray[currentIndex])
//				{
//					carsGO[currentIndex].SetCarAndRimTexture(obj);
//				}
//			}
		}
	}

	void PrevCar()
	{
		OffStage(currentCarIndex);
		currentCarIndex--;
		
		if(currentCarIndex < 0)
			currentCarIndex = noCars - 1;
		
		ToStage(currentCarIndex, -20);
	}
	
	void NextCar()
	{
		OffStage(currentCarIndex);
		currentCarIndex++;
		
		if(currentCarIndex > noCars - 1)
			currentCarIndex = 0;
		
		ToStage(currentCarIndex, 20);
	}
	
	void OffStage(int carIndex)
	{
		carsGO[carIndex].trans.parent = carParent;
		carsGO[carIndex].trans.position = carsGO[carIndex].initPos;
		lockTexts[carIndex].SetActive(false);
	}
	
	void ToStage(int carIndex, float fromPosition)
	{
        carName.text = carsGO[carIndex].carName;    
		carsGO[carIndex].trans.parent = platformTrans;
		carsGO[carIndex].trans.localPosition = Vector3.zero;
		carsGO[carIndex].trans.localEulerAngles = Vector3.zero;

		if(GlobalClass.carUnlockedArray[carIndex] || GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
		{
			lockTexts[carIndex].SetActive(false);
			lockedTexture.SetActive(false);
			foreach(Light lts in garageLights)
				lts.enabled = true;
		}
		else
		{
			lockTexts[carIndex].SetActive(true);
			lockedTexture.SetActive(true);
			foreach(Light lts in garageLights)
				lts.enabled = false;
		}

//		SetGunObject(GlobalClass.currentGunIndex);
//		UpdateTexts(carIndex);

	}

//	void UpdateTexts(int _currIndex)
//	{
//		carName.text = carsGO[_currIndex].carName;
//		foreach(Renderer ren in engineUpgradeRenderer)
//			ren.enabled = false;
//		foreach(Renderer ren in armorUpgradeRenderer)
//			ren.enabled = false;
//
//		if(!GlobalClass.carDetailsArray[currentCarIndex].isUnlocked)
//			return;
//		
//		int i;
//		int currEngineUpgrade = GlobalClass.carDetailsArray[_currIndex].engineUpgrade;
//		engineUpgradeRenderer[0].enabled = true;
//
//		for(i = 1; i <= currEngineUpgrade; i++)
//		{
//			engineUpgradeRenderer[i].enabled = true;
//		}
//
//		int currArmorUpgrade = GlobalClass.carDetailsArray[_currIndex].ArmorUpgrade;
//		armorUpgradeRenderer[0].enabled = true;
//
//		for(i = 1; i <= currArmorUpgrade; i++)
//		{
//			armorUpgradeRenderer[i].enabled = true;
//		}
//	}
}
