using UnityEngine;
using EVP;
using System;
using System.Collections;
using System.Collections.Generic;

public enum GameMode
{
    TimeTrial,
    Race,
    Chase,
    Hunt,
    Arrest,
    Escort,
    DestroyHQ,
    Boss
}

[Serializable]
public class AiPrefab
{
    public CarDelayedTrigger delayTrigger;
    public GameObject aiPrefab;
}

[Serializable]
public class TrafficClass
{
    /// <summary>
    /// For Navigation Base spawn rate is based on this 
    /// </summary>
    [Tooltip("Spawn Rate of Traffic")]
    public int trafficSpread = 50;
    /// <summary>
    /// Number of iterations per SpawnTraffic Call
    /// </summary>
    [Tooltip("Number of iterations per SpawnTraffic Call")]
    public int iterationsPerSpawnCall = 2;
    /// <summary>
    /// offset for number of distanceWP in front of current player distanceWP Trigger
    /// </summary>
    [Tooltip("Default Offset Dist")]
    public int trafficOffset = 45;
    /// <summary>
    /// initial spawn offset array
    /// </summary>
    [Tooltip("Initial spawn offset array")]
    public int[] initTrafficOffset;
}

public class GamePlaySource : MonoBehaviour
{
    [HideInInspector]
    public bool isTrackWon = false;
    public GameMode currentMode;
    //	public AiPrefab[] aiPrefabArray;
    //	public GameObject playerPrefab;

    public int trackScore = 10000;

    [Space(5)]
    [Header("Traffic Object")]
    public TrafficClass _trafficClass = new TrafficClass();

    protected bool isGameOver;
    protected bool isTrackOver = false;
    protected int currentTime;
    protected int initTime;
    protected int timeDirection = 1;
    protected int _trafficCarKilled = 0;
    protected int _aiCarKilled = 0;
    protected int _coinCollected = 0;
    protected List<AICar> activeBots;
    protected List<VehicleBase> activeVehicle;
    protected ScoreObject scoreObj;
    protected VehicleBase playerVehicle;
    protected TrackManager thisTrackManager;

//    private int targetFrameCount = 0;
    private Transform playerCarTarget;

    public int TrafficCarKilled { get { return _trafficCarKilled; } set { _trafficCarKilled = value; } }

    public int AiCarKilled { get { return _aiCarKilled; } set { _aiCarKilled = value; } }

    public int CoinsCollected { get { return _coinCollected; } set { _coinCollected = value; } }

    void Awake()
    {
        activeBots = new List<AICar>();
        activeVehicle = new List<VehicleBase>();
    }
	
    // Update is called once per frame
    protected virtual void Update()
    {
        if (GlobalClass.isRaceStarted)
        {
            foreach (AICar _aiCar in activeBots)
                _aiCar.isActiveFrame = !_aiCar.isActiveFrame;// activeBots[targetFrameCount].isActiveFrame = false;

        }
    }

    #region InitGame

    public virtual void InitRaceSession()
    {
        GlobalClass.totalBots = 6;
        thisTrackManager = TrackManager.instance;
        GlobalClass.isCircuit = false;
        GlobalClass.totalLaps = 1;
        isTrackWon = false;
        isTrackOver = false;
        GlobalClass.gameOverCause = "";
    }

    public virtual void SetUpCars()
    {
        int i;
        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
        {
//			int classIndex = (int)GlobalClass.currChampionshipMode;
//            GameObject aiPrefab = MainDirector.instance.aiBikePrefab [GlobalClass.currentCarIndex];
//            GameObject playerPrefab = MainDirector.instance.playerBikePrefab [GlobalClass.currentCarIndex];
            ChampionshipPointsTable[] _pointsTable = MainDirector.instance.pointsTable;
//            int currentClass = Mathf.FloorToInt(GlobalClass.currentCarIndex / 2) * 2;

            for (i = 0; i < GlobalClass.totalBots; i++)
            {
                CarBase _carBase;
                GameObject go;
                GameObject aiPrefab;
                if (_pointsTable[i].isActiveCar)
                {
                    GameObject playerPrefab = MainDirector.instance.playerBikePrefab[GlobalClass.currentCarIndex];
                    go = Instantiate(playerPrefab) as GameObject;
                    _carBase = go.GetComponent<CarBase>();
                    playerVehicle = go.GetComponent<VehicleBase>();
                    playerVehicle.GetComponent<NetworkCar>().enabled = false;
                    _carBase.nitroUpdateRate = 8;
//                    foreach (Renderer ren in _carBase.bikeSkinRenderer)
//                        ren.material = TeamMaterialManager.instance.teamTextureArray[GlobalClass.currentCarIndex].modelClass[0].carTexture[GlobalClass.currentTeamIndex];

                }
                else
                {

                    if (i < GlobalClass.currentCarIndex)
                    {
                        aiPrefab = MainDirector.instance.aiBikePrefab[i];

                        go = Instantiate(aiPrefab) as GameObject;
                        _carBase = go.GetComponent<CarBase>();
                        _carBase.nitroUpdateRate = UnityEngine.Random.Range(2, 5);
                        activeBots.Add(go.GetComponent<AICar>());
                        VehicleController carController = go.GetComponent<VehicleController>();
                        carController.maxSpeedForward += _pointsTable[i].speedBoost;
//                        foreach (Renderer ren in _carBase.bikeSkinRenderer)
//                        {
//                            ren.material = TeamMaterialManager.instance.teamTextureArray[i].modelClass[0].carTexture[GlobalClass.currentTrackIndex];
//
//                        }
                    }
                    else
                    {
                        aiPrefab = MainDirector.instance.aiBikePrefab[i + 1];

                        go = Instantiate(aiPrefab) as GameObject;
                        _carBase = go.GetComponent<CarBase>();
                        _carBase.nitroUpdateRate = UnityEngine.Random.Range(2, 5);
                        activeBots.Add(go.GetComponent<AICar>());
                        VehicleController carController = go.GetComponent<VehicleController>();
                        carController.maxSpeedForward += _pointsTable[i].speedBoost;
//                        foreach (Renderer ren in _carBase.bikeSkinRenderer)
//                        {
//                            ren.material = TeamMaterialManager.instance.teamTextureArray[i + 1].modelClass[0].carTexture[GlobalClass.currentTrackIndex];
//
//                        }
                    }
                    _carBase.GetNavBase().carPosDot =GamePlayScreen.instance.gamePlayObjects.aiPosDot[i];
//                    if(GlobalClass.currentTeamIndex > i)
//                    {
//                        foreach (Renderer ren in _carBase.bikeSkinRenderer)
//                            ren.material = TeamMaterialManager.instance.teamTextureArray[GlobalClass.currentCarIndex].modelClass[0].carTexture[i];
//                    }
//                    else if(GlobalClass.currentTeamIndex <= i)
//                    {
//                        foreach (Renderer ren in _carBase.bikeSkinRenderer)
//                            ren.material = TeamMaterialManager.instance.teamTextureArray[GlobalClass.currentCarIndex].modelClass[0].carTexture[i+1];
//                    }
                    //                       ren.material = TeamMaterialManager.instance.teamTextureArray[_pointsTable[i].teamID].modelClass[_pointsTable[i].carModel].carTexture[Mathf.FloorToInt(GlobalClass.currentCarIndex / 2)];
                }
                //              _bikeBase.GetComponentInChildren<BikeCharAnim>().charecterRender.material = TeamMaterialManager.instance.teamTextureArray[_pointsTable[i].teamID].charecTexture;

                _carBase.driverName = _pointsTable[i].name;
                go.transform.parent = MainDirector.instance.carParent;
                activeVehicle.Add(_carBase);
            }


        }
        else
        {
            PhotonRaceManager.instance.OnLoadConfirm();
            playerVehicle = PhotonRaceManager.instance.GetLocalCar();

        }

        MainDirector._currentPlayer = playerVehicle;
        PlayerCar _car = playerVehicle.GetComponent<PlayerCar>();
        playerCarTarget = _car.camFollowTarget;
        playerVehicle.GetNavBase().carPosDot = GamePlayScreen.instance.gamePlayObjects.playerPosDot;

//        MiniMapCamera.miniMapCamera.Init(playerVehicle.trans);


        SmoothFollow.instance.InitCamera(_car.camDist, _car.camHeight, _car.nitroCamDist, playerVehicle.body);
        WeatherManager.instance.InitWeatherManager(_car.camDist, thisTrackManager.thisTrackType);
    }

    public void OnNewNetworkCar(VehicleBase _vehicle, int racePos, int carIndex, int teamIndex)
    {
        Debug.Log(teamIndex);
        GamePlayScreen.instance.SetMulPlayerPos(_vehicle.driverName, racePos);
        //      activeCars.Insert (index, _car);
        activeVehicle.Add(_vehicle);
        if (!_vehicle.isActiveVehicle)
		{
            _vehicle.GetNavBase().carPosDot = GamePlayScreen.instance.gamePlayObjects.aiPosDot[racePos];
		}

        CarBase _currCarBase = _vehicle.GetComponent<CarBase>();
        _currCarBase.nitroUpdateRate = 8;

//        int modelIndex = carIndex % 2;
//        ChampionshipPointsTable[] _pointsTable = MainDirector.instance.pointsTable;
        foreach (Renderer ren in _currCarBase.bikeSkinRenderer)
            ren.material = TeamMaterialManager.instance.teamTextureArray[carIndex].modelClass[0].carTexture[teamIndex];

        _vehicle.PositionVehicleAndWayPoint(BotPlayerPosWP.instance.botPosition[racePos], BotPlayerPosWP.instance.trackEndWP);

        _vehicle.body.isKinematic = false;
        _vehicle.InitCar();
        _vehicle.ApplyHandBrake();
    }

    public void RemoveCarFromList(VehicleBase removedVehicle)
    {
        GamePlayScreen.instance.UpdateMultiplayerClientPos();
        activeVehicle.Remove(removedVehicle);
    }

    public virtual void PositionCars()
    {
        int i;
        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
        {
            for (i = 0; i < GlobalClass.totalBots; i++)
            {
                if(i % 2 == 0)
                {
                    activeVehicle[i].isActiveFrame = true;                    
                }
                activeVehicle[i].PositionVehicleAndWayPoint(BotPlayerPosWP.instance.botPosition[i], BotPlayerPosWP.instance.trackEndWP);
            }

//            for (i = 0; i < _trafficClass.initTrafficOffset.Length; i++)
//                TrafficManager.instance.SpawnTrafficCar(_trafficClass.initTrafficOffset[i], _trafficClass.iterationsPerSpawnCall, true);
        }

        PlayerCar _car = playerVehicle.GetComponent<PlayerCar>();
        SmoothFollow.instance.trans.position = _car.camFollowTarget.position + (playerVehicle.transform.forward * -_car.camDist) + (playerVehicle.transform.up * _car.camHeight);// trackPieces[currentTrack].initCameraPosition.position;
        SmoothFollow.instance.trans.LookAt(playerCarTarget);
    }



    public virtual VehicleBase GetTargetCar()
    {
        return playerVehicle; // normal chase scenario, defaluts target car to player car. ie no inheritance
    }

    public virtual void CountDownOver()
    {
        for (int i = 0; i < activeVehicle.Count; i++)
        {
//            if(!activeCars[i].isPlayer && activeCars[i].isDamage)// && !activeCars[i].isEscortCar)
//			{
//				GameplayScreen.instance.gamePlayObjects.aiHealthBars[i].InitFloatingHealth(activeCars[i]);
//			}
//			if(!activeCars[i].isDelayedStart)
            activeVehicle[i].EnableVehicle();
        }


//		if (GlobalClass.SoundVoulme == 1)
//			SetCarSounds(true);
//		else
//			SetCarSounds(false);

        SmoothFollow.instance.GetSmoothFollowCam().enabled = true;
        SmoothFollow.instance.target = playerCarTarget;
        StartTimer();
    }

    public void SetCarSounds(bool status)
    {
        foreach (CarBase cars in activeVehicle)
            cars.SetSound(status);
        TrafficManager.instance.SetTrafficCarSound(status);
    }

    public void InitializePlayerControl()
    {
        foreach (VehicleBase _car in activeVehicle)
        {
            if (_car.isActiveVehicle)
            {
//				_car.GetPlayerControl().Initialize();
            }
            else
            {
//				_car.GetPlayerControl().enabled = false;
            }
        }
    }

    #endregion

    #region Timer

    public virtual void StartTimer()
    {
        isGameOver = false;
        StartCoroutine(CheckTime());
    }

    IEnumerator CheckTime()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(1);
            TimerSecondTick();
        }
    }

    protected virtual void TimerSecondTick()
    {
        if (isGameOver)
            return;
        foreach (CarBase _cars in activeVehicle)
            _cars.OnSecondsTick();

        currentTime += 1 * timeDirection;

        if (!GlobalClass.isCircuit)
            GamePlayScreen.instance.UpdateTime(currentTime);
        else
            GamePlayScreen.instance.UpdateTime(playerVehicle.GetNavBase().lapTime);
    }

    public virtual void StopTimer()
    {
//		currentTime = 0;
        isGameOver = true;
        StopCoroutine("CheckTime");
    }

    #endregion

    #region GameOver


    public virtual void OnTrackComplete(VehicleBase _car, float _time)
    {

    }

    public virtual void GameOver()
    {
        if (isGameOver)
            return;
        StopTimer();
        MainDirector.instance.OnGameOver();
        GlobalClass.isGameRunning = false;
    }

    public virtual ScoreObject GetCurrentScore()
    {
        scoreObj.coinCollectedBonus = _coinCollected * 500;
        if (scoreObj.isTrackWon || GlobalClass.isChampionship)
        {
            if (GlobalClass.currentTrackIndex == 0)
                scoreObj.levelScore = trackScore;
            else if (GlobalClass.currentTrackIndex == 1)
                scoreObj.levelScore = trackScore+1000;
            else if (GlobalClass.currentTrackIndex == 2)
                scoreObj.levelScore = trackScore+2000;
            else if (GlobalClass.currentTrackIndex == 3)
                scoreObj.levelScore = trackScore+4000;
            else if (GlobalClass.currentTrackIndex == 4)
                scoreObj.levelScore = trackScore+5000; 
            else if (GlobalClass.currentTrackIndex == 5)
                scoreObj.levelScore = trackScore+7000;


//          scoreObj.healthBonus = Mathf.FloorToInt(scoreObj.playerHealth * 200);
//          scoreObj.trafficCarsKilledBonus = _trafficCarKilled * 50;
//          scoreObj.aiCarsKilledBonus = _aiCarKilled * 500;
        }
		
        scoreObj.gameMode = currentMode;
        return scoreObj;
    }

    public virtual void EndRaceSession()
    {
        scoreObj = null;
        SmoothFollow.instance.target = null;
		
        ResetCars();
        StopTimer();
        GlobalClass.isRaceStarted = false;

        while (MainDirector.instance.carParent.childCount > 0)
        {
            DestroyImmediate(MainDirector.instance.carParent.GetChild(0).gameObject);
        }

		
        if (CountDown.countDownTimer.IsTimerRunning)
        {
            CountDown.countDownTimer.CancelCountDown();
        }
		
//		if(currentTrack != null)
//			currentTrack.ResetTrafficCars();
        activeBots.Clear();
        activeVehicle.Clear();

        PowerupManager.instance.ResetPowerUp();
        TrafficManager.instance.ResetTrafficManager();
    }

    public virtual void ResetCars()
    {
        if (playerVehicle.body.isKinematic)
            return;
		
        for (int i = 0; i < activeVehicle.Count; i++)
        {
            if (!activeVehicle[i].body.isKinematic)
            {
                activeVehicle[i].OnGameEnd();
            }
        }
		
        SmoothFollow.instance.GetSmoothFollowCam().enabled = false;
    }

    #endregion

    public VehicleBase GetPlayerVehicle()
    {
        return playerVehicle;
    }

    public List<VehicleBase> GetActiveCars()
    {
        return activeVehicle;
    }

    public List<DistWP> GetAIWaypoints()
    {
        List<DistWP> dWP = new List<DistWP>();
        foreach (AICar _aiCar in activeBots)
            dWP.Add(_aiCar.GetCurrentDistWP());

        return dWP;
    }

    public List<TrafficWP> GetCloseAITrafficWP()
    {
        List<TrafficWP> traffWPArr = new List<TrafficWP>();
        foreach (AICar _aiCar in activeBots)
            traffWPArr.Add(_aiCar.GetCloseTrafficWP());

        return traffWPArr;
    }

    #region EditorCode

    public void FindWPNo()
    {
//		_wayPointClass.startWaypointNo = _wayPointClass.startWaypoint.id;
//		_wayPointClass.endWaypointNo = _wayPointClass.endWaypoint.id;
//
//		_wayPointClass.startDistWPNo = _wayPointClass.startDistWP.id;

        BotPlayerPosWP botPlayerPos = GetComponentInChildren<BotPlayerPosWP>();
	
//		botPlayerPos.trackStartWPNo = botPlayerPos.trackStartWP.id;
        botPlayerPos.trackEndWPNo = botPlayerPos.trackEndWP.id;
        botPlayerPos.trackStartDistWPNo = botPlayerPos.trackStartDistWP.id;

//		botPlayerPos.playerPosition.startWPNo = botPlayerPos.playerPosition.startWP.id;
        botPlayerPos.playerPosition.playerStartWPNo = botPlayerPos.playerPosition.playerStartWP.id;

        foreach (PosWP _posWP in botPlayerPos.botPosition)
        {
//			_posWP.startWPNo = _posWP.startWP.id;
            _posWP.playerStartWPNo = _posWP.playerStartWP.id;
        }
    }

    public void FindWPObject()
    {
        BotPlayerPosWP botPlayerPos = GetComponentInChildren<BotPlayerPosWP>();

//		Waypoint[] wpArray = transform.GetComponentsInChildren<Waypoint>();
        DistWP[] _distWPArray = transform.GetComponentsInChildren<DistWP>();

//		foreach (Waypoint _wp in wpArray)
//		{
//			if (_wp.id == botPlayerPos.trackStartWPNo)
//				botPlayerPos.trackStartWP = _wp;
//			if (_wp.id == botPlayerPos.trackEndWPNo)
//				botPlayerPos.trackEndWP = _wp;
//		}

        foreach (DistWP _dWP in _distWPArray)
        {

            if (_dWP.id == botPlayerPos.trackStartDistWPNo)
                botPlayerPos.trackStartDistWP = _dWP;

            if (_dWP.id == botPlayerPos.trackEndWPNo)
                botPlayerPos.trackEndWP = _dWP;
        }
		
        if (Application.isPlaying && botPlayerPos.playerPosition.playerStartWPNo == -1)
        {
//			botPlayerPos.playerPosition.startWP = botPlayerPos.trackStartWP;
            botPlayerPos.playerPosition.playerStartWP = botPlayerPos.trackStartDistWP;
        }
        else
        {
//			foreach (Waypoint _wp in wpArray)
//			{
//				if (_wp.id == botPlayerPos.playerPosition.startWPNo)
//					botPlayerPos.playerPosition.startWP = _wp;
//			}
            foreach (DistWP _dWP in _distWPArray)
            {
                if (_dWP.id == botPlayerPos.playerPosition.playerStartWPNo)
                    botPlayerPos.playerPosition.playerStartWP = _dWP;
            }
        }

        foreach (PosWP _posWP in botPlayerPos.botPosition)
        {
            if (Application.isPlaying && _posWP.playerStartWPNo == -1)
            {
//				_posWP.startWP = botPlayerPos.trackStartWP;
                _posWP.playerStartWP = botPlayerPos.trackStartDistWP;
            }
            else
            {
//				foreach (Waypoint _wp in wpArray)
//				{
//					if (_wp.id == _posWP.startWPNo)
//						_posWP.startWP = _wp;
//				}
				
                foreach (DistWP _dWP in _distWPArray)
                {
                    if (_dWP.id == _posWP.playerStartWPNo)
                        _posWP.playerStartWP = _dWP;
                }
            }
        }

    }

    #endregion

}
