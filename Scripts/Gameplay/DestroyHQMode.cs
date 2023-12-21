using UnityEngine;
using System.Collections;

public class DestroyHQMode : GamePlaySource
{
	[Space(5)]
	[Header("DestroyHQ Variables")]
	public int timeLimit = 120;
	public GameObject targetBuildPrefab;
	public Transform targetBuildTrans;

//	private bool isBuildingTutorialShown = true;
	private Building enemyBuilding;

	// Update is called once per frame
	protected override void Update () 
	{
		if(!GlobalClass.isRaceStarted)
			return;
		if(enemyBuilding.buildingLife < 0)
		{
			if(!isTrackOver)
			{
				Debug.Log("call here");
				isTrackOver = true;
				isTrackWon = true;
//				Invoke("GameOver",2);
				enemyBuilding.ExplodeBuilding();
			}
		}

//		if(!isBuildingTutorialShown)
//		{
//			if(Vector3.Distance(enemyBuilding.transform.position, playerBike.trans.position) < 20)
//			{
//				isBuildingTutorialShown = true;
//				StartCoroutine(GameplayScreen.instance.BuildingTutorial());
//			}
//		}

//		for(int i = 0 ; i <  activeBots.Count; i++)
//		{
//			if(activeBots[i].carLife > 0 && !activeBots[i].isBotArrested)
//			{
//				// wont check for disabled or destroyed cars
//				bool isSlowSpeed = false;
//				if(Mathf.Abs(activeBots[i].speed) < 2.0)
//					isSlowSpeed = true;
//
//				if(isSlowSpeed && Vector3.Distance(activeBots[i].trans.position, playerCar.trans.position) < 8)
//				{
//					closeIndex = i;
//					arrestPercent += 0.005f;
//					GameplayScreen.instance.UpdateArrestBar(arrestPercent);
//					activeBots[i].isBotClose = true;
//					if(arrestPercent > 1)
//					{
//						activeBots[i].OnArrested();
//						arrestPercent = 0;
////						MainDirector.GetCurrentGamePlay().AiCarKilled += 1;
////						activeBots[i].floatingHealthBar.SetHealth(0.01f);
////						activeBots[i].DisableCar();
////						GameplayScreen.instance.UpdateArrestBar(arrestPercent);
//					}
//				}
//				else if(i == closeIndex && arrestPercent != 0) // using index, to check case of multiple AI cars
//				{
//					closeIndex = -1;
//					activeBots[i].isBotClose = false;
//
//					arrestPercent = 0;
//					GameplayScreen.instance.UpdateArrestBar(arrestPercent);
//				}
//				else
//				{
//					activeBots[i].isBotClose = false;
//				}
//			}
//		}

		base.Update();
	}

	#region InitGame
	public override void InitRaceSession ()
	{
//		isBuildingTutorialShown = PlayerPrefs.HasKey("BuildingTutorialShown");
		currentMode = GameMode.DestroyHQ;
		initTime = timeLimit;
		base.InitRaceSession ();
	}
	
	public override void SetUpCars ()
	{
		base.SetUpCars ();
		GameObject enemyBuildGO = Instantiate(targetBuildPrefab) as GameObject;
		enemyBuildGO.transform.parent = MainDirector.instance.carParent;;
		enemyBuilding = enemyBuildGO.GetComponent<Building>();

	}
	
	public override void PositionCars ()
	{
		base.PositionCars ();
		enemyBuilding.transform.position = targetBuildTrans.position;
		enemyBuilding.transform.eulerAngles = targetBuildTrans.eulerAngles;

		Waypoint[] wpArray = transform.GetComponentsInChildren<Waypoint>();
		Waypoint buildWp = GetComponentInChildren<BotPlayerPosWP>().GetWP(wpArray, targetBuildTrans.position);
		enemyBuilding.InitBuilding(buildWp.id);
	}
	
	public override void CountDownOver ()
	{
		base.CountDownOver ();
	}
	#endregion

	#region Timer
	public override void StartTimer ()
	{
		currentTime = timeLimit;
		timeDirection = -1;
		base.StartTimer ();
	}

	protected override void TimerSecondTick ()
	{
		base.TimerSecondTick ();
		if(currentTime < 30)
			GamePlayScreen.instance.BlinkTimer();

		if(currentTime < 1)
		{
			GameOver();
            GlobalClass.gameOverCause = "Time_over";
		}
	}

	public override void StopTimer ()
	{
		base.StopTimer ();
	}
	#endregion

	#region GameOver
    public override void OnTrackComplete (VehicleBase _car, float _time)
	{
//		_car.DisableCar();
//		_car.didFinish = true;

//		if(_car.isPlayerCar)
//		{
//			_car.ApplyHandBrake();
//			GameOver();
//		}
		base.OnTrackComplete (_car, _time);
	}

	public override void GameOver ()
	{
		base.GameOver ();
	}

	public override ScoreObject GetCurrentScore ()
	{
		scoreObj = new ScoreObject();
//		scoreObj.levelScore = trackScore;
//		scoreObj.totalTime = currentTime;

		if(isTrackWon)
		{
			scoreObj.isTrackWon = true;
			scoreObj.playerHealth = playerVehicle.carLife/playerVehicle.initCarLife;

//			int timeRemaining = timeLimit - currentTime;
//			scoreObj.totalTime = timeRemaining;
//			scoreObj.totalScore = trackScore + (100 * timeRemaining) + Mathf.FloorToInt(scoreObj.playerHealth * 1000);
		}

		return base.GetCurrentScore ();
	}

	public override void EndRaceSession ()
	{
		enemyBuilding.OnGameEndSession();
		base.EndRaceSession ();
	}

	public override void ResetCars ()
	{
		base.ResetCars ();
	}
	#endregion
}
