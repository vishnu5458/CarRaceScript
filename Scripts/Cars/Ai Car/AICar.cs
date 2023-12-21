using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICar : CarBase 
{
	[HideInInspector]
	public bool isBotBeingArrested = false; // work around; initally set in chaseAi 
	public bool isBotArrested = false;

	public float oADistance;
	public float oAWidth;
	public bool isCatchUpEnabled = true;

	protected bool isReverse = false;
	protected int reverseTimeOut;
	protected int carStopTimeOut = 150;
	protected float magnitude;

	private bool isWeaponPresent = false;
	private bool isFireReady = false;
	[SerializeField] private float fireSessionTime = 5;
	[SerializeField] private float fireIntervalTime = 7;
	private float initFireSessionTime;
	private Transform targetTrans;
	[SerializeField]
	private List<WeaponBase> equippedWeapons = new List<WeaponBase>();
	private bool isMinePresent = false;
	private MineLauncher mineLauncher;

	protected override void Awake()
	{
		base.Awake();
		isPlayer = false;
		if(GetComponentInChildren<WeaponBase>() != null)
		{
			isWeaponPresent = true;
			initFireSessionTime = fireSessionTime;
			WeaponBase[] tempWeaponArray = GetComponentsInChildren<WeaponBase>();
			for(int i = 0; i < tempWeaponArray.Length; i++)
			{
				if(!tempWeaponArray[i].GetComponent<MineLauncher>())
				{
					equippedWeapons.Add(tempWeaponArray[i]);
				}
			}
//			equippedWeapons = GetComponentsInChildren<WeaponBase>();
		}

		if(GetComponentInChildren<MineLauncher>() != null)
		{
			isMinePresent = true;
			mineLauncher = GetComponentInChildren<MineLauncher>();
		}
	}
		
	public override void InitCar ()
	{
		base.InitCar ();
	}

	public override void EnableVehicle ()
	{
		base.EnableVehicle ();
		if(isWeaponPresent)
		{
			targetTrans =  MainDirector.GetCurrentGamePlay().GetTargetCar().trans;
			foreach(WeaponBase _weapons in equippedWeapons)
				_weapons.InitialiseWeapon(this);
			StartCoroutine("TurnTurretToPlayer");
		}
		if(isMinePresent)
			mineLauncher.InitialiseWeapon(this);
	}

	IEnumerator TurnTurretToPlayer()
	{
		while(isCarEnabled)
		{
			yield return new WaitForSeconds(fireIntervalTime);

			if(isMinePresent)
			{
				mineLauncher.FireTrigger();
			}
			float dist = Vector3.Distance(trans.position, targetTrans.position);
			if(dist < 20)
			{
				foreach(WeaponBase _weapons in equippedWeapons)
					_weapons.TurnTurret(targetTrans);
			}
		}
	}

	public IEnumerator OnTurretTrunComplete()
	{
		yield return new WaitForSeconds(1);

		fireSessionTime = initFireSessionTime;
		isFireReady = true;
	}

	protected override void Update()
	{
		base.Update();
	}
	
	protected override void FixedUpdate () 
	{
		if(isFireReady)
		{
			fireSessionTime -= Time.deltaTime;
			if(fireSessionTime < 0)
				isFireReady = false;
			
			foreach(WeaponBase _weapons in equippedWeapons)
				_weapons.FireTrigger();

		}
		base.FixedUpdate();
	}

	protected override void UpdatePowerTrain ()
	{
		base.UpdatePowerTrain ();
	} 

	protected override void UpdateSteering ()
	{
		base.UpdateSteering ();
	}
	
	public override void OnVehicleReset()
	{
//		Debug.Log("Resetting");
		if(isBotBeingArrested)
			return;

		carStopTimeOut = 150;
		isReverse = false;

		base.OnVehicleReset();
	}
	
	public override void OnGameEnd()
	{
//		DisableColliderTigger();
		base.OnGameEnd();
	}
	
	protected virtual void OnReverseTrigger()
	{
		isReverse = true;
		reverseTimeOut = 100;
	}

}
