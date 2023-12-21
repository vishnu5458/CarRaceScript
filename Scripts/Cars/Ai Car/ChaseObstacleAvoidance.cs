using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class ChaseAntennaTrans
{
	public Transform centerA;
	public Transform centerB;
	public Transform centerEndA;
	public Transform centerEndB;
	public Transform left;
	public Transform leftEnd;
	public Transform right;
	public Transform rightEnd;
}


public class ChaseObstacleAvoidance : AIObstacleAvoidance 
{
	public float smashLength = 5;
	public float smashSideWidth = 2;
	public float smashWidth = 2;

	public ChaseAntennaTrans chaseAntenna = new ChaseAntennaTrans();

	public LayerMask playerLayer = -1;

	protected override void Start ()
	{
		base.Start ();

		float obstacleAvoidanceWidth = smashWidth; //(smashLength * 0.66f)
		Vector3 leftDirection = chaseAntenna.left.position + antennaTrans.center.TransformDirection((Vector3.left * obstacleAvoidanceWidth) + (Vector3.forward * smashSideWidth));
		Vector3 rightDirection = chaseAntenna.right.position + antennaTrans.center.TransformDirection((Vector3.right * obstacleAvoidanceWidth) + (Vector3.forward * smashSideWidth));
		
		chaseAntenna.leftEnd.position = leftDirection;
		chaseAntenna.rightEnd.position = rightDirection;
		
		chaseAntenna.centerEndA.position = chaseAntenna.centerA.position + chaseAntenna.centerA.TransformDirection(Vector3.forward * smashLength);
		chaseAntenna.centerEndB.position = chaseAntenna.centerB.position + chaseAntenna.centerB.TransformDirection(Vector3.forward * smashLength);
		
//		antennaTrans.leftEnd.transform.position = antennaTrans.left.transform.position + antennaTrans.left.transform.TransformDirection(Vector3.forward * aiCar.oADistance);
//		antennaTrans.rightEnd.transform.position = antennaTrans.right.transform.position + antennaTrans.right.transform.TransformDirection(Vector3.forward * aiCar.oADistance);

	}

    public override SensorData Sensors(float _speed)
	{
		SensorData _sensorData = new SensorData();
//		float avoidSenstivity = 0;

		#region Obstacle Avoidance 
		RaycastHit hit;
		if (Physics.Linecast(antennaTrans.right.position, antennaTrans.rightEnd.position, out hit, visibleLayers))
		{
			_sensorData.targetAngle -= 1; 
			Debug.DrawLine(antennaTrans.right.position, hit.point,Color.white);
		}
		else if (Physics.Linecast(antennaTrans.right.position, antennaTrans.rightEnd_A.position, out hit,  visibleLayers))
		{
			_sensorData.targetAngle -= 0.5f; 
			Debug.DrawLine(antennaTrans.right.position ,hit.point,Color.white);
		}
		if (Physics.Linecast(antennaTrans.left.position, antennaTrans.leftEnd.position, out hit,  visibleLayers))
		{
			_sensorData.targetAngle += 1; 
			Debug.DrawLine(antennaTrans.left.position, hit.point,Color.white);
		}
		else if (Physics.Linecast(antennaTrans.left.position, antennaTrans.leftEnd_A.position, out hit,  visibleLayers))
		{
			_sensorData.targetAngle += 0.5f;
			Debug.DrawLine(antennaTrans.left.position, hit.point,Color.white);
		}
		if (Physics.Linecast(antennaTrans.center.position, antennaTrans.centerEnd.position, out hit,  visibleLayers))
		{
//			isBreaking = true;
			if (_sensorData.targetAngle == 0)
			{
				if (hit.normal.x < 0 )
					_sensorData.targetAngle = 1;
				else 
					_sensorData.targetAngle = -1;
			}
			Debug.DrawLine(antennaTrans.center.position, hit.point, Color.white);
		}
		else
		{
//			isBreaking = false;
		}
		#endregion

		if (Physics.Linecast(chaseAntenna.centerA.position, chaseAntenna.centerEndA.position, out hit,  playerLayer) || Physics.Linecast(chaseAntenna.centerB.position, chaseAntenna.centerEndB.position, out hit,  playerLayer))
		{
			_sensorData.smashDirection = 0;
			Debug.DrawLine(antennaTrans.left.position, hit.point,Color.red);
		}
		else if (Physics.Linecast(chaseAntenna.left.position, chaseAntenna.leftEnd.position, out hit,  playerLayer))
		{
			_sensorData.smashDirection = -1;
			Debug.DrawLine(antennaTrans.left.position, hit.point,Color.red);
		}
		else if (Physics.Linecast(chaseAntenna.right.position, chaseAntenna.rightEnd.position, out hit,  playerLayer))
		{
			_sensorData.smashDirection = 1;
			Debug.DrawLine(antennaTrans.right.position, hit.point,Color.red);
		}

		return _sensorData;
	}

	public override void OnDrawGizmos ()
	{
		base.OnDrawGizmos ();
	
		Vector3 forwardDirection = chaseAntenna.centerA.TransformDirection(Vector3.forward * smashLength);
		
//		float obstacleAvoidanceWidth = aiCar.oAWidth;               
		
		Vector3 leftDirection = chaseAntenna.centerA.TransformDirection((Vector3.left * smashWidth) + (Vector3.forward * smashSideWidth));
		Vector3 rightDirection = chaseAntenna.centerB.TransformDirection((Vector3.right * smashWidth) + (Vector3.forward * smashSideWidth));

		Debug.DrawRay(chaseAntenna.left.position, leftDirection, Color.white);
		Debug.DrawRay(chaseAntenna.right.position, rightDirection, Color.white);
		Debug.DrawRay(chaseAntenna.centerA.position, forwardDirection, Color.white);
		Debug.DrawRay(chaseAntenna.centerB.position, forwardDirection, Color.white);
	}
}
