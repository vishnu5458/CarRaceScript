using UnityEngine;
using System.Collections;

public class HuntMode : GamePlaySource
{
	[Space(5)]
	[Header("Hunt Variables")]
	public int timeLimit = 120;

	private int closeIndex = -1;
	private float arrestPercent;

	// Update is called once per frame
	protected override void Update ()
	{
		if(!GlobalClass.isRaceStarted)
			return;

		bool isGameWon = true;
		for(int i = 0 ; i <  activeBots.Count; i++)
		{
			if(activeBots[i].carLife > 0 && !activeBots[i].isBotArrested)
			{
				isGameWon = false;

				// wont check for disabled or destroyed cars
				bool isSlowSpeed = false;
				if(Mathf.Abs(activeBots[i].speed) < 2.0)
					isSlowSpeed = true;
				
				if(activeBots[i].isCarEnabled && isSlowSpeed && Vector3.Distance(activeBots[i].trans.position, playerVehicle.trans.position) < 8)
				{
					closeIndex = i;
					arrestPercent += 0.0015f;
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

		if(isGameWon && !isTrackOver)
		{
			Debug.Log("call here");
			isTrackOver = true;
			isTrackWon = true;
			Invoke("GameOver",2);
		}

		base.Update();
	}

	#region InitGame
	public override void InitRaceSession ()
	{
		currentMode = GameMode.Hunt;
		initTime = timeLimit;
		base.InitRaceSession ();
	}
	
	public override void SetUpCars ()
	{
		base.SetUpCars ();
	}
	
	public override void PositionCars ()
	{
		base.PositionCars ();
	}
	
	public override void CountDownOver ()
	{
		base.CountDownOver ();
	}
	#endregion

	#region Timer
	public override void StartTimer ()
	{
//		timeDirection = 1;
//		currentTime = 0;
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
    public override void OnTrackComplete (VehicleBase _vehicle, float _time)
	{
//		_car.DisableCar();
//		_car.didFinish = true;
//		
		if(!_vehicle.isActiveVehicle)
		{ 
			_vehicle.ApplyHandBrake();
            GlobalClass.gameOverCause = "Enemy_Escaped";
			isTrackOver = true;
			GameOver();
		}
		base.OnTrackComplete (_vehicle, _time);
	}

	public override void GameOver ()
	{
		Debug.Log("game over called");
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
		}
		return base.GetCurrentScore ();
	}
	
	public override void EndRaceSession ()
	{
		CancelInvoke();
		base.EndRaceSession ();
	}
	
	public override void ResetCars ()
	{
		base.ResetCars ();
	}
	#endregion
}
