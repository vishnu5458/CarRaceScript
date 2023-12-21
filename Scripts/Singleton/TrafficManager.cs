using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TrafficDirection
{
	LHS,
	RHS
}

public class TrafficManager : MonoBehaviour 
{
	#region Singleton
	private static TrafficManager _instance;
	public static TrafficManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<TrafficManager> ();
			}
			return _instance;
		}
	}
	#endregion

	public GameObjectPool[] prefabArray;
	public List<TrafficCar> LHSactiveTrafficCars;
	public List<TrafficCar> RHSactiveTrafficCars;

	private bool shouldSpawnCars = false;
//	[SerializeField]
	private int trafficInterval;

	void Awake () 
	{
		_instance = this;
		LHSactiveTrafficCars = new List<TrafficCar>();
		RHSactiveTrafficCars = new List<TrafficCar>();

		prefabArray = GetComponentsInChildren<GameObjectPool>();
	}

	public void InitTrafficCars()
	{
		shouldSpawnCars = true;
		trafficInterval = 7;
//		StartCoroutine(FadeIn());
	}

	public void ResetTrafficManager()
	{
		shouldSpawnCars = false;

		LHSactiveTrafficCars.Clear();
		RHSactiveTrafficCars.Clear();

		foreach(GameObjectPool pool in prefabArray)
			pool.ReleaseAllInstances();
	}

	public void ReleaseTrafficCar(TrafficCar _trafficCar)
	{
		if(_trafficCar.thisDirection == TrafficDirection.LHS)
			LHSactiveTrafficCars.Remove(_trafficCar);
		else
			RHSactiveTrafficCars.Remove(_trafficCar);

	}

//	IEnumerator FadeIn()
//	{
//		while(shouldSpawnCars)
//		{
//			yield return new WaitForSeconds(15);
//
//		}
//	}

	public void SpawnTrafficCar()
	{
		SpawnTrafficCar(45);
	}

	public void SpawnTrafficCar(int offset)
	{
		SpawnTrafficCar(offset, 3);
	}

	public void SpawnTrafficCar(int offset, int maxCount, bool isInit = false)
	{
		if(!shouldSpawnCars)
			return;
		/*
		 * -- check on total number of traffic cars
		 * -- check for Traffic WP LHS and RHS
		 * -- check for AI Cars WP (sphere cast?)
		 */

		TrafficWP playerDWP = MainDirector.GetCurrentPlayer().GetCloseTrafficWP();
        List<TrafficWP> aiWParray = MainDirector.GetCurrentGamePlay().GetCloseAITrafficWP();

		int targetWP = playerDWP.id + offset;
		int count = 0;
//		Debug.Log(targetWP + "  " + offset);

		foreach(TrafficCar _trafficCar in RHSactiveTrafficCars)
		{
            if(targetWP <= _trafficCar.currTrafficWP.id)
			{
//				Debug.Log("traffic spawn conflicts with running Traffic Cars + RHS "+ targetWP);
                targetWP = _trafficCar.currTrafficWP.id + 5;
			}
		}
		foreach(TrafficCar _trafficCar in LHSactiveTrafficCars)
		{
            if(targetWP <= _trafficCar.currTrafficWP.id)
			{
//				Debug.Log("traffic spawn conflicts with running Traffic Cars + LHS  "+ targetWP);
                targetWP = _trafficCar.currTrafficWP.id + 5;
			}
		}
//		Debug.Log("Target WP  " + targetWP);
		while(count < maxCount)
		{
//			foreach(TrafficCar _trafficCar in RHSactiveTrafficCars)
//			{
//				if(targetWP == _trafficCar.currentWaypoint.id)
//				{
//					Debug.Log("traffic spawn conflicts with running Traffic Cars + RHS "+ targetWP);
//					Debug.Break();
//					targetWP += 10;
//				}
//			}
//
//			foreach(TrafficCar _trafficCar in LHSactiveTrafficCars)
//			{
//				if(targetWP == _trafficCar.currentWaypoint.id)
//				{
//					Debug.Log("traffic spawn conflicts with running Traffic Cars + LHS  "+ targetWP);
//					Debug.Break();
//					targetWP += 10;
//				}
//			}

            foreach(TrafficWP _aiTraffWP in aiWParray)
			{
				if(targetWP >= _aiTraffWP.id && targetWP < (_aiTraffWP.id + 10))
				{
					Debug.Log("traffic spawn conflicts with AI");
					targetWP += _aiTraffWP.id + 10;
				}
			}

//			Debug.Log("Set traffic car at " + targetWP);
			int _rand;
            TrafficWP _startWP = TrackManager.instance.GetTrafficWPForID(targetWP);

			if(_startWP == null)
				return;
			if(Random.value > 0.5f)
			{
				_rand = Random.Range(0, prefabArray.Length);
				TrafficCar _car1 = prefabArray[_rand].GetInstance().GetComponent<TrafficCar>();

				if(!_car1.InitWayPoints(_startWP, TrafficDirection.LHS)) // returning if next checkpoint is null
					return;
//				_car1.EnableCar();
				LHSactiveTrafficCars.Add(_car1);
			}

			if(Random.value > 0.5f)
			{
				_rand = Random.Range(0, prefabArray.Length);
				TrafficCar _car2 = prefabArray[_rand].GetInstance().GetComponent<TrafficCar>();
				if(!_car2.InitWayPoints(_startWP, TrafficDirection.RHS))  // returning if next checkpoint is null
					return;
//				_car2.EnableCar();
				RHSactiveTrafficCars.Add(_car2);
			}

			targetWP += trafficInterval;
			count++;
//			Debug.Log(count + "  " +targetWP);
		}

		if(!isInit)
			EnableTrafficCars();
//		Debug.Break();
	}

	public void EnableTrafficCars()
	{
		int i;
		for(i = 0; i < LHSactiveTrafficCars.Count; i++)
			LHSactiveTrafficCars[i].EnableCar();
		for(i = 0; i < RHSactiveTrafficCars.Count; i++)
			RHSactiveTrafficCars[i].EnableCar();

//		foreach(TrafficCar _cars in LHSactiveTrafficCars)
//			_cars.EnableCar();
//		
//		foreach(TrafficCar _cars in RHSactiveTrafficCars)
//			_cars.EnableCar();
	}

	public void SetTrafficCarSound(bool soundStatus)
	{
//		bool soundStatus = GlobalClass.SoundVoulme == 1 ? true : false;

		int i;
		for(i = 0; i < LHSactiveTrafficCars.Count; i++)
			LHSactiveTrafficCars[i].SetSound(soundStatus);
		for(i = 0; i < RHSactiveTrafficCars.Count; i++)
			RHSactiveTrafficCars[i].SetSound(soundStatus);

//		foreach(TrafficCar _cars in LHSactiveTrafficCars)
//			_cars.SetSound(soundStatus);
//
//		foreach(TrafficCar _cars in RHSactiveTrafficCars)
//			_cars.SetSound(soundStatus);
	}
}
