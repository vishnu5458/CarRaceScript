using UnityEngine;
using System.Collections;

public class ChaseAi : AICar 
{
	public float smashForce = 20;

	private bool isEnabled = false;
	private bool isSmashReady = false;
	private bool isArresting = false;
	private int aiOffsetDir = 0;
//	private float arrestPercent = 0.0f;
    private VehicleBase _targetCar;
	private ChaseObstacleAvoidance thisObstaceAvoidance;

	protected override void Awake ()
	{
		thisObstaceAvoidance = gameObject.GetComponentInChildren<ChaseObstacleAvoidance>();
		base.Awake ();
	}
	
	public override void InitCar ()
	{
		base.InitCar ();
		isEnabled = true;
//		arrestPercent = 0.0f;
		_targetCar = MainDirector.GetCurrentGamePlay().GetTargetCar();// PlayerCar.playerCar;
		StartCoroutine(RandomiseAIOffset());
	}

	IEnumerator RandomiseAIOffset()
	{
		while(isEnabled)
		{
			yield return new WaitForSeconds(10);

			isSmashReady = true;
			float rand = Random.value;
			if(rand > 0.66f)
				aiOffsetDir = -1;
			else if(rand > 0.33f)
				aiOffsetDir = 0;
			else
				aiOffsetDir = 1;
		}
	}

	protected override void Update ()
	{
		if(!isCarEnabled)
			return;	

		Vector3 relativePosition;

		Vector3 targetPos = _targetCar.trans.position;
		targetPos.x += aiOffsetDir * 2;
		Vector3 targetRelative = trans.InverseTransformPoint(targetPos);

		isReverse = false;

		if(targetRelative.z > 0 && targetRelative.z < 30) // AI Chase car is behind target
		{
//			Debug.Log("target set to target");
			relativePosition = trans.InverseTransformPoint(targetPos);
		}
		else if(targetRelative.z < 0) // AI Chase car is ahead of the target
		{
//			Debug.Log("Reverse");
			isReverse = true;
			relativePosition = trans.InverseTransformPoint(target);
		}
		else // player car is too far away, use default target
		{
			relativePosition = trans.InverseTransformPoint(target);
		}

		bool isSlowSpeed = false;
		if(Mathf.Abs(speed) < 5.0)
			isSlowSpeed = true;

		// Disabling arrest bar for Highway Squad
//		if(isSlowSpeed && Vector3.Distance(trans.position, targetPos) < 5)
//		{
//			isArresting = true;
//			arrestPercent += 0.005f;
//			GameplayScreen.instance.UpdateArrestBar(arrestPercent);
//
//			if(arrestPercent > 1)
//			{
//				MainDirector.GetCurrentGamePlay().GameOver();
//				DisableCar();
//			}
//		}
//		else
//		{
//			isArresting = false;
//			if(arrestPercent > 0)
//			{
//				arrestPercent -= 0.05f;
//				vehicleController.ReleaseHandBrake();
//				GameplayScreen.instance.UpdateArrestBar(arrestPercent);
//			}
//		}

		magnitude = relativePosition.z;  

		steer = relativePosition.x / magnitude;
		acce = relativePosition.z / magnitude;
		acce = Mathf.Abs(acce);
		
//		DebugLog("rel pos : "+relativePosition.z+" magnitude : " + magnitude + " Acce : "+ acce+ " Steer : "+ steer);
		if(!isArresting)// && Mathf.Abs(relativePosition.z) < 0) // > 5
		{
			if(isSlowSpeed) // Mathf.Abs(speed) < 3.0f
			{
				carStopTimeOut--;
				if(carStopTimeOut < 0)
				{
					Debug.Log("reset on slow speed");
					OnVehicleReset();
				}
			}
			else if(relativePosition.z < 0 && !isReverse) // is facing the wrong direction
			{
				carStopTimeOut --;
				if(carStopTimeOut < 0)
				{
					Debug.Log("reset on relative");
					OnVehicleReset();
				}
			}
			else 
			{
				carStopTimeOut = 150;
			}
		}

		if(isReverse)
		{
			acce *= -1.0f;
			steer = 0; // *= -1f;
			
//			reverseTimeOut--;
//			
//			if(reverseTimeOut < 0)
//				isReverse = false;
		}
		base.Update ();
	}
	
	protected override void FixedUpdate ()
	{
		if(!isCarEnabled)
			return;	
		if(!isArresting)
			base.FixedUpdate ();
		else
			vehicleController.ApplyHandBrake();
	}
	
	protected override void UpdatePowerTrain ()
	{
		vehicleController.AiCatchUpSpeed = 0;
		if(isCatchUpEnabled)
		{
			float distDiff = _targetCar.GetNavBase().positionWeight - GetNavBase().positionWeight;
			
			if(Mathf.Abs(distDiff) > 5)
			{
				float catchUpSpeed = distDiff;
				catchUpSpeed = Mathf.Clamp(catchUpSpeed, -20, 20);
				vehicleController.AiCatchUpSpeed = catchUpSpeed;
			}
			else
				vehicleController.AiCatchUpSpeed = 0;
		}
		
		base.UpdatePowerTrain ();
	}
	
	protected override void UpdateSteering ()
	{
        SensorData _sensorData = thisObstaceAvoidance.Sensors(speed);
		if(_sensorData.targetAngle != 0)
		{
			if (steer < _sensorData.targetAngle)
			{
				steer = steer + (Time.deltaTime * 50); //200
				if (steer > _sensorData.targetAngle)
				{
					steer = _sensorData.targetAngle;
				}
			}
			else if (steer > _sensorData.targetAngle)
			{
				steer = steer - (Time.deltaTime * 50); //200
				if (steer < _sensorData.targetAngle)
				{
					steer = _sensorData.targetAngle;
				}
			}
		}
		if(isSmashReady && !isArresting && _sensorData.smashDirection != -2 && speed > 10)
		{
			isSmashReady = false;
			Debug.Log("SMASHING : " + _sensorData.smashDirection);
			Vector3 forceDir = trans.TransformDirection(_targetCar.trans.position);
			body.AddForce(forceDir * smashForce, ForceMode.Impulse);
		}
		
		base.UpdateSteering ();
	}
	
	public override void OnVehicleReset ()
	{
		base.OnVehicleReset ();
	}
	
	public override void OnGameEnd ()
	{
		isEnabled = false;
		base.OnGameEnd ();
	}
	
	protected override void OnTriggerEnter (Collider other)
	{
		base.OnTriggerEnter (other);
	}
	
	protected override void OnCollisionEnter (Collision other)
	{
		base.OnCollisionEnter (other);
	}
}
