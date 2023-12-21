using UnityEngine;
using System.Collections;

public class TimeTrialMode : GamePlaySource
{

	[Space(5)]
	[Header("Time Trial Variables")]
	public int timeLimit = 120;

	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
	}

	#region InitGame
	public override void InitRaceSession ()
	{
		currentMode = GameMode.TimeTrial;
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
		timeDirection = -1;
		currentTime = timeLimit;
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
		_car.ApplyHandBrake();
		_car.DisableCar();
		_car.didFinish = true;
		isTrackWon = true;
		GameOver();

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
		int timeRemaining = timeLimit - currentTime;
		scoreObj.totalTime = timeRemaining;

		if(isTrackWon)
		{
			scoreObj.isTrackWon = true;

			scoreObj.playerHealth = playerVehicle.carLife/playerVehicle.initCarLife;
			scoreObj.totalCredits = trackScore + (100 * timeRemaining) + Mathf.FloorToInt(scoreObj.playerHealth * 1000);
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
