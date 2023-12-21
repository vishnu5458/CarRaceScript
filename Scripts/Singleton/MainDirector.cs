using UnityEngine;
using UnityEngine.SceneManagement;

//using System;
using System.Collections;
using System.Collections.Generic;

public class MainDirector : MonoBehaviour
{
    #region Singleton

    private static MainDirector _instance;

    public static MainDirector instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MainDirector>();
            }
            return _instance;
        }
    }

    #endregion

    private static GamePlaySource _currGameplay;
    public static VehicleBase _currentPlayer;

    private const string DoNotDestroyTag = "DoNotDestroy";

    private bool isInit = true;
    private int currentTrack;
    private string menuLoadCallBack = "";
    private TrackManager _currTrackManager;
    [SerializeField] 
    private string currLoadedScene;
    //	private AsyncOperation loadTrackSceneAsync = null;
    [SerializeField] 
    private ScreenFade screenFade;
    private int currentTrackObject;

    public Color fogColor;
    public ChampionshipPointsTable[] pointsTable;

    public GameObject[] playerBikePrefab;
    public GameObject[] aiBikePrefab;
    //	public GameObject[] playerAvatars;
    //	public RuntimeAnimatorController[] playerAnimators;

    public Transform carParent;
    public Transform keyLightTransform;


    #region AppInitialise

    void Awake()
    {        
//        PlayerPrefs.DeleteAll();
        _instance = this;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        Time.timeScale = 1;

//		GlobalClass.isGameOver = false;
        GameObject[] levelGo = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in levelGo)
        {
            if (go != null && go.transform.parent == null)// && !go.CompareTag(DoNotDestroyTag) && !go.CompareTag("MainCamera"))
				DontDestroyOnLoad(go);
            
            //				go.tag = DoNotDestroyTag;
        }


        GlobalClass.championshipUnlockedArray = new bool[3];
        GlobalClass.trackUnlockedArray = new bool[6]; 
        GlobalClass.carUnlockedArray = new bool[6];
        GlobalClass.trackRecordArray = new int[10];
		
        GlobalClass.trackUnlockedArray[0] = true;
        GlobalClass.carUnlockedArray[0] = true;
        GlobalClass.championshipUnlockedArray[0] = true;

//        Application.targetFrameRate = 30;
    }

    public void WaitBeforeLoadingData()
    {
        
//		Invoke("LoadInitData",0.2f);
        LoadInitData();
        Destroy(PreloaderScreen.instance.gameObject);
    }

    void LoadInitData()
    {
        screenFade.ShowFadeOutEffect();
        LoadMenu("MenuScreen");
        GlobalClass.playerProfile = GlobalClass.Profile.Local;
    }

    // called from idnet when get is completed
    public void OnDataLoadComplete()
    {
//		Debug.Log("State 0");
//		GlobalClass.gunUnlockedArray[0] = true;
        GlobalClass.trackUnlockedArray[0] = true;
        GlobalClass.carUnlockedArray[0] = true;
        GlobalClass.championshipUnlockedArray[0] = true;

        if (GlobalClass.unlockAll)
        {
            Debug.Log("unlock cars");
            for (int i = 0; i < GlobalClass.trackUnlockedArray.Length; i++)
                GlobalClass.trackUnlockedArray[i] = true;

            for (int i = 0; i < GlobalClass.carUnlockedArray.Length; i++)
                GlobalClass.carUnlockedArray[i] = true;

            for (int i = 0; i < GlobalClass.championshipUnlockedArray.Length; i++)
                GlobalClass.championshipUnlockedArray[i] = true;
        }
        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
            TeamSelectionScreen.instance.InitScreen();
        else
        {
            TeamSelectionScreen.instance.InitScreen();
        }
        MenuScreen.instance.ExitScreen(); 
    }

    #endregion

    public void OnResume()
    {
        GamePlayScreen.instance.InitScreen();
    }

    #region Initialise Menu

    public void LoadMenu(string _menuLoadCallBack)
    {
        
        menuLoadCallBack = _menuLoadCallBack;
        if (!isInit)
        {
            LoadingScreen.instance.InitLoadingScreen("Menu");
        }
        else
        {
            StartCoroutine("LoadMenuSceneCouroutine");
            isInit = false;
        }

    }

    public IEnumerator LoadMenuSceneCouroutine()
    {
        currLoadedScene = "Menu CarSel";
       
        //	AsyncOperation loadMenuAsync = Application.LoadLevelAdditiveAsync (currLoadedScene);
        SceneManager.LoadScene(currLoadedScene);
//		AsyncOperation loadMenuAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(currLoadedScene);
//		while (!loadMenuAsync.isDone) {
//			//			Debug.Log(loadLevelAsync.progress);
        yield return 0;
//		}
		
        OnMenuLoadComplete();
    }

    void OnMenuLoadComplete()
    {
        //initial loading completed

        //GameObject.Find("MenuObject").transform.parent = GuiController.instance.transform;
        GameObject.Find("Menu Canvas").GetComponent<Canvas>().worldCamera = GuiController.GetGuiCam();

        switch (menuLoadCallBack)
        {
            case  "MenuScreen":
                if (GlobalClass.isChampionship)
                {
                    ChampionshipProgressScreen.instance.InitScreen();
                }
                else
                {
                    MenuScreen.instance.InitScreen();
                    AudioManager.instance.OnMenuLoad();
                }
                break;
            case "CarScreen":
                //CarScreen.instance.InitScreen();
                if (GlobalClass.isChampionship)
                {
                    ChampionshipProgressScreen.instance.InitScreen();
                }
                else if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
                {
                    TeamSelectionScreen.instance.InitScreen();
                    AudioManager.instance.OnMenuLoad();
                }
                else
                {
                    MenuScreen.instance.InitScreen();
                    AudioManager.instance.OnMenuLoad();
                }
                break;
        }
		
        menuLoadCallBack = "";

        if (LoadingScreen.instance.isScreenActive)
            LoadingScreen.instance.OnLoadingComplete();
    }

    #endregion

    #region Initialize Game

    public void InitialiseGame(int _currentTrackObject)
    {
        currentTrackObject = _currentTrackObject;
        InitialiseGame();
        RemoveUnUsedAssets();
    }

    public void InitialiseGame()
    {
        currentTrack = GlobalClass.currentTrackIndex;
		
        GlobalClass.isRaceStarted = false;
        GlobalClass.isGameRunning = true;
        SmoothFollow.instance.target = null;
		
        GlobalClass.currentLap = 0;
        LoadingScreen.instance.InitLoadingScreen("GamePlay");
    }

    public void LoadTrackSceneCoroutine()
    {
        
        currLoadedScene = "Track " + (currentTrackObject + 1).ToString();
        if (GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
            PhotonNetwork.LoadLevel(currLoadedScene);
        else
            SceneManager.LoadScene(currLoadedScene);
        //loadTrackSceneAsync = Application.LoadLevelAdditiveAsync (currLoadedScene);
        //while (!loadTrackSceneAsync.isDone) {
//			Debug.Log(loadLevelAsync.progress);
        //	yield return 0;
        //}
		
        //initial loading completed
        Invoke("OnSceneLoadComplete", 1);
//		OnSceneLoadComplete ();
    }

    void OnSceneLoadComplete()
    {
//		loadTrackSceneAsync = null;
//        RenderSettings.fog = true;
//        RenderSettings.fogMode = FogMode.Exponential;
//        RenderSettings.fogColor = fogColor;
//        RenderSettings.fogDensity = 0.005f;
        //		Debug.Log("on level load complete");
        _currGameplay = GameObject.FindObjectOfType<GamePlaySource>();

        _currTrackManager = TrackManager.instance;
        _currTrackManager.InitializeTrack();
//        keyLightTransform.localEulerAngles = _currTrackManager.lightAngle;
    }

    public void TrackLoadCompleted()
    {
        //reset stuff
        _currGameplay.InitRaceSession();
        ScoreManager.instance.Reset();
		
        // instantiate Cars
        _currGameplay.SetUpCars();

        StartCoroutine(WaitForCarSetup());
    }

    IEnumerator WaitForCarSetup() // wait for 1 second for all the instantiated scripts to call Start()
    {
        yield return new WaitForSeconds(0.5f);

        _currGameplay.PositionCars();
//		CarManager.GetInstance ().PositionCars ();
        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
        {
//			_currentPlayer.GetPlayerControl().Initialize();
        }
        else
        {
            PhotonRaceManager.instance.LoadCompleted();
        }

//		PlayerControl.GetInstance().Initialize();
//		trackObject.InitTrafficCars();

        bool isHighQuality = false;
        if (!GlobalClass.is64Bit)
        {
            QualitySettings.SetQualityLevel(1, false);
        }
        else
        {
            QualitySettings.SetQualityLevel(3, false);
        }


        Debug.Log("Quality:" + QualitySettings.GetQualityLevel());
        if (QualitySettings.GetQualityLevel() >= 3)
            isHighQuality = true;    
        else
            isHighQuality = false;

        Camera[] _sceneCameras = FindObjectsOfType(typeof(Camera)) as Camera[];
        foreach (Camera _cams in _sceneCameras)
        {
            if (!_cams.orthographic)
            {
                //                        _cams.gameObject.GetComponent<FXAA>().enabled = isHighQuality;
                _cams.gameObject.GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>().enabled = isHighQuality;
//                _cams.gameObject.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>().enabled = isHighQuality;
            }
        }

        Invoke("LevelLoadingComplete", 1.0f);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    void LevelLoadingComplete()
    {
//
        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
        {
            LoadingScreen.instance.OnLoadingComplete();
        }
    }

    public void OnHideLoadingPage()
    {
//		trackObject.StartTrafficCars();
        if (GlobalClass.SoundVoulme == 1)
            GetCurrentGamePlay().SetCarSounds(true);
        else
            GetCurrentGamePlay().SetCarSounds(false);
    }

    public void CountDownOver()
    {
        _currGameplay.CountDownOver();
//		CarManager.GetInstance().CountDownOver();
        GamePlayScreen.instance.CountDownOver();
        GlobalClass.isRaceStarted = true;
		
    }

    #endregion

    public void PlayerCarLapUp()
    {

        GlobalClass.currentLap++;
        GamePlayScreen.instance.PlayerCarLapUp(GlobalClass.currentLap);
        if (GlobalClass.currentLap > GlobalClass.totalLaps)
        { //added extra condition here to not show race over on car death
            if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
            {
                _currGameplay.OnTrackComplete(_currentPlayer, _currentPlayer.GetNavBase().totalTime);
//				OnGameOver();
            }
            else
            {
                PhotonRaceManager.instance.LocalCarGameFinish();
                GamePlayScreen.instance.ExitScreen();
            }
        }
    }

    #region  GameOver

    public void OnGameOver()
    {

        GamePlayScreen.instance.EndGame();
        AudioManager.instance.FadeOutBg();

//		Invoke("WaitForGameOverScreen", 1.5f);
        GameOverEffect.instance.SetGameOverStatus(_currGameplay.isTrackWon);
    }

    public void WaitForGameOverEffect()
    {
        ScoreObject scoreObj = _currGameplay.GetCurrentScore();

        if ((scoreObj.isTrackWon || GlobalClass.isChampionship) && GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
        {
//            IdnetManager.instance.PostAchievements("winOneTrack");
            if (currentTrack != 5)
            {
                GlobalClass.carUnlockedArray[currentTrack + 1] = true;
                GlobalClass.trackUnlockedArray[currentTrack + 1] = true;
            }
            if (GlobalClass.isChampionship)
            {
                
                int currTrackIndex;
                if (GlobalClass.currChampionshipMode == ChampionshipModes.GT1)
                {
                    currTrackIndex = System.Array.IndexOf(GlobalClass.class250Tracks, GlobalClass.currentTrackIndex);
                }
                else if (GlobalClass.currChampionshipMode == ChampionshipModes.GT2)
                {
                    currTrackIndex = System.Array.IndexOf(GlobalClass.class600Tracks, GlobalClass.currentTrackIndex);
                }
                else
                {
                    currTrackIndex = System.Array.IndexOf(GlobalClass.class1000Tracks, GlobalClass.currentTrackIndex);
                }

                GlobalClass.champProgressArray[currTrackIndex] = true;

                bool isChampionshipCompleted = true;
                foreach (bool _trackCompleted in GlobalClass.champProgressArray)
                {
                    if (!_trackCompleted)
                        isChampionshipCompleted = false;
                }
                if (isChampionshipCompleted)
                {
                    SortChampionshipTable();
                    if (pointsTable[0].isActiveCar)
                    {
                        if (GlobalClass.currChampionshipMode == ChampionshipModes.GT1)
                        {
                            //achievement post here
                            GlobalClass.championshipUnlockedArray[1] = true;
                        }
                        else if (GlobalClass.currChampionshipMode == ChampionshipModes.GT2)
                        {
                            //achievement post here
                            GlobalClass.championshipUnlockedArray[2] = true;
                        }
                        else
                        {
                            //achievement post here
                        }
                        scoreObj.championshipinProgress = 2;
                        Debug.Log("championship completed");
                    }
                    else
                    {
                        scoreObj.championshipinProgress = 3;
                        Debug.Log("championship failed");
                    }
                }
                else
                {
                    scoreObj.championshipinProgress = 1;
                }

            }

            scoreObj.timeBonus = (300 - scoreObj.totalTime) * 50;
            if (GlobalClass.trackRecordArray[currentTrack] == 0 || scoreObj.totalTime < GlobalClass.trackRecordArray[currentTrack])
            {
                scoreObj.timeBonus += 1000;
                GlobalClass.trackRecordArray[currentTrack] = scoreObj.totalTime;
            }
            if(GlobalClass.currentTrackIndex < 5 && GlobalClass.currentCarIndex < 5)
            {
                GlobalClass.currentTrackIndex += 1;
                GlobalClass.currentCarIndex += 1;
            }
        }

        scoreObj.totalScore = Mathf.Clamp(scoreObj.levelScore + scoreObj.timeBonus + scoreObj.positionBonus + scoreObj.coinCollectedBonus, 0, int.MaxValue);
        GlobalClass.highScore = scoreObj.totalScore;

//		scrObj.bestLapTime = ScoreManager.instance.bestLapTime;
//		scrObj.totalTime = ScoreManager.instance.totalLapTime;
        scoreObj.trackRecord = GlobalClass.trackRecordArray[currentTrack];

        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
            SaveData();

        _currGameplay.ResetCars();

        GameOverScreen.instance.InitScreen();
        GameOverScreen.instance.InitGameOverValues(scoreObj);
    }

    public void UnloadTrack() // called from GameOverScreen or PauseScreen to unload track 
    {
//        MiniMapCamera.miniMapCamera.Dealloc();
        WeatherManager.instance.OnGameOver();
        _currGameplay.EndRaceSession();
        RemoveUnUsedAssets();
    }

    public void RemoveUnUsedAssets()
    {
        RemovePrevScene();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    void RemovePrevScene()//string sceneName
    {
//		UnityEngine.SceneManagement.SceneManager.UnloadScene(currLoadedScene);
        if (currLoadedScene == "Menu CarSel")
        {
            //			GameObject.Destroy (GameObject.Find ("MenuObject"));
        }
        else
        {
//			CarManager.GetInstance ().DestroyCars ();
            //			GameObject.Destroy(LevelScreen.instance.gameObject);
        }
    }

    #endregion

    public void SaveData()
    {
        if (GlobalClass.isChampionship)
            GlobalClass.championshipPointTable = pointsTable;

        if (GlobalClass.playerProfile == GlobalClass.Profile.Local)
            LocalDataManager.instance.Save();
    }

    public static GamePlaySource GetCurrentGamePlay()
    {
        return _currGameplay;
    }

    public static VehicleBase GetCurrentPlayer()
    {
        return _currentPlayer;
    }

    public void ResetChampionship()
    {
        GlobalClass.isChampionship = false;
        GlobalClass.newChampionship = true;
        GlobalClass.champProgressArray = new bool[]{ false, false, false, false, false, false };
        for (int i = 0; i < 6; i++)
        {
            ChampionshipPointsTable _tempTable = new ChampionshipPointsTable();
            _tempTable.isActiveCar = false;
            _tempTable.name = "";
            _tempTable.teamID = 0;
            _tempTable.points = 0;
            _tempTable.speedBoost = 0;
            pointsTable[i] = _tempTable;
        }
        GlobalClass.championshipPointTable = pointsTable;
    }

    public void SetTableDetails(string driverName, int finalPosition)
    {
        
        foreach (ChampionshipPointsTable _table in pointsTable)
        {
            if (_table.name == driverName)
            {
                _table.points += (9 - finalPosition) * 10;
            }
        }
            
    }

    public void SortChampionshipTable()
    {
        ChampionshipPointsTable tempPointsTable;
        for (int i = 0; i < pointsTable.Length; i++)
        {
            for (int j = 0; j < pointsTable.Length - 1 - i; j++)
            {
                if (pointsTable[j].points < pointsTable[j + 1].points)
                { 
                    tempPointsTable = pointsTable[j];   
                    pointsTable[j] = pointsTable[j + 1];
                    pointsTable[j + 1] = tempPointsTable;
                }
            }
        }
        for (int i = 0; i < pointsTable.Length; i++)
        {
            pointsTable[i].rank = i;
        }
    }

    public TrackManager GetCurrentTrackManager()
    {
        return _currTrackManager;
    }

}
