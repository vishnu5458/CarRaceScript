using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreen : ScreenBase
{
    #region Singleton

    private static LoadingScreen _instance;

    public static LoadingScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LoadingScreen>();
            }
            return _instance;
        }
    }

    #endregion

    private bool isLoadingComplete = false;
    private string loadingScreenSource;

    public bool isScreenActive;
    public GameObject pressSpaceGo;
    public GameObject tapToStartGo;
    public GameObject loadingBarGo;
    public Renderer[] loadingIndicator;

    public GameObject gamePlaySpecific;
    //	public Transform keyConfigTut;
    public Transform[] gamePlayTutorials;
    //	public Transform powerUpTut;
    public GameObject menuSpecific;

    //    private int currTutIndex = 0;
    //    private int lightIndex = 0;

    public Text waitForOppoText;
    public Camera orthoCamera;
    public PlayerDetailsMenu[] playerDetailsArray;
    public NetworkClientCars[] networkClientCarArray;
    public GameObject multiplayerGO;

    public GameObject startRaceGO;
    public Button startRaceButton;
    public Image startRaceImage;
    public Text startRaceText;

    public GameObject loadingIndicator1;
    private float waitTime = 1;
    private int tutSwapTimeout = 100;
    private int loadingTimer = 0;
    [SerializeField]
    private int maxPlayerCount;
    //    private Vector3 gameTutorialPos = new Vector3(0, -0.5f, -0.2f);
    private Vector3 outTutorialPos = new Vector3(-900, 0, 0);
    //	public UIProgressBar loadingBar;
    public Image load;

    bool fillImage = false;
    float timer = 0;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
        int i;
        foreach (NetworkClientCars _networkCars in networkClientCarArray)
        {
            for (i = 0; i < _networkCars.carsArray.Length; i++)
            {
                _networkCars.carsArray[i] = _networkCars.clientCarParent.GetChild(i);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public void InitLoadingScreen(string _source)
    {
        base.InitScreen();

        isLoadingComplete = false;

        loadingScreenSource = _source;

        loadingBarGo.SetActive(true);
        tapToStartGo.SetActive(false);
        pressSpaceGo.SetActive(false);
        multiplayerGO.SetActive(false);
        orthoCamera.enabled = false;

        CancelInvoke("ShowRaceButton");
        startRaceGO.SetActive(false);

        Time.timeScale = 1;

        GlobalClass.isPause = true;

        //		GameplayScreen.instance.HideGamePlayScreen();
        maxPlayerCount = 0;
        if (loadingScreenSource == "GamePlay")
        {
            waitTime = 1;
            gamePlaySpecific.SetActive(true);
            menuSpecific.SetActive(false);
            gamePlayTutorials[0].localPosition = Vector3.zero;
//            			keyConfigTut.localPosition = gameTutorialPos;
            //			powerUpTut.localPosition = outTutorialPos;
            tutSwapTimeout = 100;
        }
        else if (loadingScreenSource == "Menu")
        {
            waitTime = 1;
            gamePlaySpecific.SetActive(false);
            menuSpecific.SetActive(true);
        }
        else if (loadingScreenSource == "Multiplayer")
        {
            waitTime = 0.25f;
            gamePlaySpecific.SetActive(true);
            menuSpecific.SetActive(false);
            //			Debug.LogError ("Unknown Call Source");
        }

        isScreenActive = true;
        //		Hashtable ht = new Hashtable();
        //		ht.Add("time", .5f);
        //		ht.Add("islocal",true);
        //		ht.Add("ignoretimescale", true);
        //		ht.Add("y",0);
        //		iTween.MoveTo(gameObject, ht);	
        OnScreenLoadComplete();
    }

    //	public void SetLoadingScreen(string _source)
    //	{
    //
    //	}

    public override void OnScreenLoadComplete()
    {
        base.OnScreenLoadComplete();
        fillImage = true;
        Invoke("WaitBeforeLevelLoading", waitTime);
    }



    void WaitBeforeLevelLoading()
    {
        if (loadingScreenSource == "GamePlay")
        {
            MainDirector.instance.LoadTrackSceneCoroutine();
        }
        else if (loadingScreenSource == "Menu")
        {
            StartCoroutine(MainDirector.instance.LoadMenuSceneCouroutine());
        }
        else if (loadingScreenSource == "Multiplayer")
        {
            //			Debug.LogError ("Unknown Call Source");
            loadingBarGo.SetActive(false);
            isLoadingComplete = true;
            multiplayerGO.SetActive(true);
            foreach (Transform _trans in gamePlayTutorials)
                _trans.localPosition = outTutorialPos;

            //			keyConfigTut.localPosition = outTutorialPos;
            //			powerUpTut.localPosition = outTutorialPos;
            orthoCamera.enabled = true;
            Debug.Log("waiting for clients to join");
            UpdatePlayerList();
        }
    }

    public override void ExitScreen()
    {
        CancelInvoke();
        base.ExitScreen();
        orthoCamera.enabled = false;

        GlobalClass.isPause = false;
        Time.timeScale = 1;
        isScreenActive = false;

        loadingScreenSource = "";

        //		Hashtable ht = new Hashtable();
        //		ht.Add("time", .01f);
        //		ht.Add("islocal",true);
        //		ht.Add("ignoretimescale", true);
        //		ht.Add("y",15);
        //		iTween.MoveTo(gameObject, ht);	
    }
    //    void Update()
    //    {
    //        if (fillImage)
    //        {
    //            timer += Time.deltaTime;
    //            if (timer <= 2)
    //            {
    //                load.fillAmount = timer / 2;
    //            }
    //            else
    //            {
    //                timer = 0;
    //                fillImage = false;
    //            }
    //        }
    //    }
    protected override void UpdateInput()
    {
        base.UpdateInput();
        if (Time.frameCount % 15 == 0)
        {
//            RectTransform rotate = loadingIndicator1.GetComponent<RectTransform>();
//            rotate.Rotate(Vector3.forward, -45);
        }
        if (fillImage)
        {
            timer += Time.deltaTime;
            if (timer <= 2)
            {
                load.fillAmount = timer / 2;
            }
            else
            {
                timer = 0;
                fillImage = false;
            }
        }       

        if (loadingScreenSource == "GamePlay")
        {
            tutSwapTimeout--;
            if (tutSwapTimeout < 0)
            {
                tutSwapTimeout = 100;
//                gamePlayTutorials[currTutIndex].localPosition = outTutorialPos;
//                currTutIndex++;
//                if (currTutIndex > gamePlayTutorials.Length - 1)
//                    currTutIndex = 0;
//
//                gamePlayTutorials[currTutIndex].localPosition = gameTutorialPos;
            }
        }
        if (isLoadingComplete)
        {
            #if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WEBGL
            if (Input.anyKeyDown)
            {
                if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
                    HideLoadingScreen();
                //              else
                //                  PhotonRaceManager.instance.OnMasterClientPressSpace ();
            }
            #else
            if(Input.GetMouseButton(0))
            {
            HideLoadingScreen();
            }
            #endif
        }
    }

  

    public override void ButtonClick(Button _button)
    {
        base.ButtonClick(_button);
               			

        if (_button.name == "Start Race")
        {
            PhotonMenu.instance.OnStartRaceClicked();
        }
        else if (_button.name == "Leave Room")
        {
            PhotonMenu.instance.LeaveRoom();
            MultiplayerScreen.instance.InitScreen();
            ExitScreen();
        }

    }

    public void OnLoadingComplete()
    {
        if (loadingScreenSource == "GamePlay")
        {
            loadingBarGo.SetActive(false);
            isLoadingComplete = true;
            if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
            {
                #if UNITY_WEBPLAYER || UNITY_WEBGL
                pressSpaceGo.SetActive(true);
                #else
				tapToStartGo.SetActive(true);
                #endif
            }
			//			else if (GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
			//			{
			//				PhotonRaceManager.instance.StartRaceSession ();
			//			}
			else
            {
                Debug.Log("Waiting for Master client to press space");
            }
        }
        else if (loadingScreenSource == "Menu")
        {
            ExitScreen();
        }
        else
        {
            Debug.LogError("Unknown Call Source");
        }
    }

    void CheckToShowStartButton()
    {
        waitForOppoText.gameObject.SetActive(true);
        int playerCount = PhotonNetwork.playerList.Length;

        if (maxPlayerCount < playerCount)
        {
            maxPlayerCount = playerCount;
            if (IsInvoking("ShowRaceButton"))
                CancelInvoke("ShowRaceButton");
        }
        if (playerCount == 1)
        {
            //			Invoke ("ShowRaceButton", 12);
            //			ShowRaceButton ();
            startRaceGO.SetActive(false);
        }
        else if (playerCount == 2)
        {
            FadeOutRaceButton();
            Invoke("ShowRaceButton", 14);
            loadingTimer = 14;
        }
        else if (playerCount == 3)
        {
            FadeOutRaceButton();
            Invoke("ShowRaceButton", 9);
            loadingTimer = 9;
        }
        else
        {
            ShowRaceButton();
        }
    }

    void FadeOutRaceButton()
    {
        startRaceGO.SetActive(true);
        startRaceButton.enabled = false;
        Color _startColor = startRaceImage.color;
        _startColor.a = 0.5f;
        startRaceImage.color = _startColor;
        StartCoroutine("LoadingTimerCoroutine");
    }

    IEnumerator LoadingTimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            startRaceText.text = "START RACE \n in " + loadingTimer.ToString() + " sec";
            loadingTimer--;
        }
    }

    void ShowRaceButton()
    {
        StopCoroutine("LoadingTimerCoroutine");
        waitForOppoText.gameObject.SetActive(false);

        startRaceGO.SetActive(true);
        startRaceButton.enabled = true;
        Color _startColor = startRaceImage.color;
        _startColor.a = 1;
        startRaceImage.color = _startColor;
        startRaceText.text = "START RACE";

    }

    public void HideLoadingScreen()
    {
        GamePlayScreen.instance.InitScreen();
        GamePlayScreen.instance.InitialiseGame();
        ExitScreen();
        MainDirector.instance.OnHideLoadingPage();
        TrafficManager.instance.EnableTrafficCars();
    }

    public void UpdatePlayerList()
    {
        if (!isLoadingComplete)
            return;

        CheckToShowStartButton();
        //		Debug.Log ("updating");
        ClearPlayersGUI();
        int playerIndex = 0;
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            PlayerDetailsMenu playerMenu = playerDetailsArray[playerIndex];
            if (p.CustomProperties.ContainsKey("car"))
            {
                int carIndex = (int)p.CustomProperties["car"];
                int teamIndex = (int)p.CustomProperties["teamID"];
//                int modelIndex = carIndex % 2;// (int)p.customProperties["modelID"];
//                Debug.Log("player team index " + teamIndex + " Model Index : " + modelIndex + " Car Index : "+Mathf.FloorToInt(carIndex/2));
                Transform _clientCarTrans = networkClientCarArray[playerIndex].carsArray[carIndex].transform;
//                foreach (Renderer ren in _clientCarTrans.GetComponent<BikeSkinRenderer> ().skinRenderers)
//                    ren.material = TeamMaterialManager.instance.teamTextureArray[carIndex].modelClass[0].carTexture[teamIndex];
                _clientCarTrans.parent = playerMenu.carTrans;
                _clientCarTrans.localPosition = Vector3.zero;

                playerMenu.playerName.text = p.NickName.Trim();
                //				playerMenu.carTrans.GetComponent<Image>().sprite = carTextures[(int)p.customProperties["car"]];

            }
            playerIndex++;
        }
    }


    private void ClearPlayersGUI()
    {

        int i = 0;
        foreach (NetworkClientCars _playerPrefabs in networkClientCarArray)
        {
            for (i = 0; i < _playerPrefabs.carsArray.Length; i++)
            {
                _playerPrefabs.carsArray[i].transform.parent = _playerPrefabs.clientCarParent;
                _playerPrefabs.carsArray[i].transform.localPosition = Vector3.zero;
            }
        }
        foreach (PlayerDetailsMenu _menu in playerDetailsArray)
        {
            _menu.playerName.text = "";

        }
    }

   
}
