using UnityEngine;
using System.Collections;

public class RaceMode : GamePlaySource
{
    [Space(5)]
    [Header("Race Variables")]
    public bool isCircuit = false;
    public int totalLaps;

    public ArrayList completedCarsNames = new ArrayList();
    public ArrayList completedCarsTime = new ArrayList();

    //	private int finalStanding = 0;

    // Update is called once per frame
    protected override void Update()
    {
        if (!GlobalClass.isGameRunning)
            return;
		
        if (GlobalClass.isRaceStarted)
        {
            //			activeCars.Sort(
            //				delegate(CarBase a, CarBase b)
            //				{
            //				return b.navBase.positionWeight.CompareTo(a.navBase.positionWeight);
            //				}
            //			);
            //			// script here for car position
            VehicleBase tempCar;
            for (int i = 0; i < activeVehicle.Count; i++)
            {
                for (int j = 0; j < activeVehicle.Count - 1 - i; j++)
                {
                    if (activeVehicle[j].GetNavBase().positionWeight < activeVehicle[j + 1].GetNavBase().positionWeight)
                    { 
                        tempCar = activeVehicle[j];   
                        activeVehicle[j] = activeVehicle[j + 1];
                        activeVehicle[j + 1] = tempCar;
                    }
                }
            }
			
            for (int i = 0; i < activeVehicle.Count; i++)
            {
                if (GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
                    GamePlayScreen.instance.SetMulPlayerPos(activeVehicle[i].driverName, i);
                if (activeVehicle[i].isActiveVehicle)
                {
                    GamePlayScreen.instance.SetLocalPlayerPosition(i + 1);
                }
            }
        }

        base.Update();
    }

    #region InitGame

    public override void InitRaceSession()
    {
        currentMode = GameMode.Race;
        completedCarsNames.Clear();
        completedCarsTime.Clear();
        base.InitRaceSession();
        GlobalClass.isCircuit = isCircuit;
        if (isCircuit)
        {
            if (!GlobalClass.isChampionship || GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
            {
                totalLaps = GlobalClass.lapCount;
            }
            GlobalClass.totalLaps = totalLaps;
        }
    }

    public override void SetUpCars()
    {
        base.SetUpCars();
    }

    public override void PositionCars()
    {
        base.PositionCars();
    }

    public override void CountDownOver()
    {
        base.CountDownOver();
    }

    #endregion

    #region Timer

    public override void StartTimer()
    {
        timeDirection = 1;
        currentTime = 0;
        base.StartTimer();
    }

    protected override void TimerSecondTick()
    {
        base.TimerSecondTick();
    }

    public override void StopTimer()
    {
        base.StopTimer();
    }

    #endregion

    #region GameOver

    public override void OnTrackComplete(VehicleBase _vehicle, float _time)
    {

        if (_vehicle.GetNavBase().currentLap > totalLaps)
        {

            if (completedCarsNames.Contains(_vehicle.driverName)) //condition to check if array already contains finished car
            {
                int index = completedCarsNames.IndexOf(_vehicle.driverName);

                completedCarsNames.Insert(index, _vehicle.driverName);
                completedCarsTime.Insert(index, Mathf.FloorToInt(_time));
            }
            else
            {
                completedCarsNames.Add(_vehicle.driverName);
                completedCarsTime.Add(Mathf.FloorToInt(_time));
            }

            _vehicle.finalPosition = completedCarsNames.Count;
            MainDirector.instance.SetTableDetails(_vehicle.driverName, _vehicle.finalPosition);
            _vehicle.DisableCar();
            _vehicle.didFinish = true;

            if (_vehicle.isActiveVehicle)
            {
//					_car.ApplyHandBrake();
 //               if (_vehicle.finalPosition == 1)
//                    _vehicle.GetComponentInChildren<BikeCharAnim>().PlayFinishAnimation();
                GameOver();
            }
        }
        else
        {
            if (_vehicle.isActiveVehicle)
                GamePlayScreen.instance.PlayerCarLapUp(_vehicle.GetNavBase().currentLap);
        }

        base.OnTrackComplete(_vehicle, _time);
    }

    public override void GameOver()
    {
//		finalStanding = playerVehicle.finalPosition;
        if (playerVehicle.finalPosition <= 3 && GlobalClass.gameMode==GlobalClass.GameMode.SinglePlayer)
        {
            isTrackWon = true;
        }
        else if(playerVehicle.finalPosition <= 1)
        {
            isTrackWon = true;
        }


        for (int i = 0; i < activeBots.Count; i++)
        {
            if (!activeBots[i].didFinish)
            {
                completedCarsNames.Add(activeBots[i].driverName);
                MainDirector.instance.SetTableDetails(activeBots[i].driverName, completedCarsTime.Count + 1);
                completedCarsTime.Add(0);
                activeBots[i].didFinish = true;
            }
        }

        base.GameOver();
    }

    public override ScoreObject GetCurrentScore()
    {
        scoreObj = new ScoreObject();
//		scoreObj.levelScore = trackScore;
        scoreObj.playerPos = playerVehicle.finalPosition;
        scoreObj.totalTime = currentTime;
        scoreObj.positionBonus = (activeVehicle.Count - playerVehicle.finalPosition) * 1000;

        if (isTrackWon)
        {
            scoreObj.isTrackWon = true;
        }
		
        return base.GetCurrentScore();
    }

    public override void EndRaceSession()
    {
        base.EndRaceSession();
    }

    public override void ResetCars()
    {
        base.ResetCars();
    }

    #endregion
}
