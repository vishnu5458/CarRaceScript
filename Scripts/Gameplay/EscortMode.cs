using UnityEngine;
using System.Collections;

public class EscortMode : GamePlaySource
{
	[Space(5)]
	[Header("Escort Variables")]
	public int timeLimit = 120;
	public GameObject escortPrefab;

	private int closeIndex = -1;
	private float arrestPercent;
    private VehicleBase escortCar;
	// Update is called once per frame
	protected override void Update () 
	{
		if(!GlobalClass.isRaceStarted)
			return;

		for(int i = 0 ; i <  activeBots.Count; i++)
		{
            if(activeBots[i].carLife > 0 && !activeBots[i].isBotArrested) // && !activeBots[i].isEscortCar
			{
				// wont check for disabled or destroyed cars
				bool isSlowSpeed = false;
				if(Mathf.Abs(activeBots[i].speed) < 2.0)
					isSlowSpeed = true;

				if(activeBots[i].isCarEnabled && isSlowSpeed && Vector3.Distance(activeBots[i].trans.position, playerVehicle.trans.position) < 8)
				{
					closeIndex = i;
					arrestPercent += 0.003f;
					GamePlayScreen.instance.UpdateArrestBar(arrestPercent);
					activeBots[i].isBotBeingArrested = true;
					if(arrestPercent > 1)
					{
						activeBots[i].OnArrested();
						arrestPercent = 0;
//						MainDirector.GetCurrentGamePlay().AiCarKilled += 1;
//						activeBots[i].floatingHealthBar.SetHealth(0.01f);
//						activeBots[i].DisableCar();
//						arrestPercent = 0;
//						GameplayScreen.instance.UpdateArrestBar(arrestPercent);
					}
				}
				else if(i == closeIndex && arrestPercent != 0) // using index, to check case of multiple AI cars
				{
					closeIndex = -1;
					activeBots[i].isBotBeingArrested = false;

					arrestPercent = 0;
					GamePlayScreen.instance.UpdateArrestBar(arrestPercent);
				}
				else
				{
					activeBots[i].isBotBeingArrested = false;
				}
			}
		}

		if(escortCar.carLife < 0)
		{
			if(!isGameOver)
            {
                GlobalClass.gameOverCause = "EscortCar_Killed";
				GameOver();
            }
		}
		base.Update();
	}

	#region InitGame
	public override void InitRaceSession ()
	{
		currentMode = GameMode.Escort;
		initTime = timeLimit;
		base.InitRaceSession ();
	}
	
	public override void SetUpCars ()
	{
		base.SetUpCars ();
		GameObject escortGO = Instantiate(escortPrefab) as GameObject;
		escortGO.transform.parent = MainDirector.instance.carParent;;
        escortCar = escortGO.GetComponent<VehicleBase>();
		escortCar.GetNavBase().carPosDot = GamePlayScreen.instance.gamePlayObjects.escortPosDot;
//		escortCar.isEscortCar = true;
	}
	
	public override void PositionCars ()
	{
		base.PositionCars ();
		escortCar.PositionVehicleAndWayPoint(BotPlayerPosWP.instance.botPosition[GlobalClass.totalBots], BotPlayerPosWP.instance.trackEndWP); // here GlobalClass.totalBots 
																																		  // points to the last position in the botPosition array
																																		  // assign the bot pos of escort car in the last botPos
//		activeCars.Add(escortCar);
	}

    public override VehicleBase GetTargetCar ()
	{
		return escortCar;
	}
	
	public override void CountDownOver ()
	{
//		GameplayScreen.instance.gamePlayObjects.escortHealthBase.InitFloatingHealth(escortCar);
		base.CountDownOver ();
//		escortCar.EnableCar();
	}
	#endregion

	#region Timer
	public override void StartTimer ()
	{
//		timeDirection = 1;
//		currentTime = 0;
//		base.StartTimer ();
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

		if(_car == escortCar)
		{
			_car.DisableCar();
			_car.didFinish = true;
//			_car.ApplyHandBrake();
			isTrackWon = true;
			GameOver();
		}
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
			scoreObj.playerHealth = escortCar.carLife/escortCar.initCarLife;

		}

		return base.GetCurrentScore ();
	}

	public override void EndRaceSession ()
	{
		base.EndRaceSession ();
	}

	public override void ResetCars ()
	{
		base.ResetCars ();
	}
	#endregion
}
