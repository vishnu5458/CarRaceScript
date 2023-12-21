using UnityEngine;
using System.Collections;

public class TrafficCarNavigation : MonoBehaviour 
{
//	private float waypointOffset;

	public TrafficCar thisCar;
	public TrafficWP currentWaypoint;

	public TrafficWP startWP;

	void Awake () 
	{
		thisCar = GetComponent<TrafficCar>();
	}



	public void InitWayPoints(TrafficWP startWp)
	{

		currentWaypoint = startWp;
		
//		Transform waypoint = currentWaypoint.thisTransform;
//		thisCar.target = new Vector3( waypoint.position.x, 0, waypoint.position.z);

	}

	void Update()
	{
		if(!thisCar.isCarEnabled)
			return;
	}

//	void OnTriggerEnter(Collider other)
//	{
//		if(!thisCar.isCarEnabled)
//			return;
//		
//		if(other.GetComponent<TrafficWP>())
//		{
//			OnWpTriggerEnter(other.GetComponent<TrafficWP>());
//		}
//		
//		//		if(other.GetComponent<DistWP>())
//		//		{
//		//			OnDistWpTriggerEnter(other.GetComponent<DistWP>());
//		//		}
//	}

	void OnWpTriggerEnter(TrafficWP closeWp)
	{
		TrafficWP closeWaypoint = closeWp;

		currentWaypoint = closeWaypoint.nextWaypoint.nextWaypoint;	
		

//		Transform waypointTrans = currentWaypoint.thisTransform;
//		thisCar.target = new Vector3( waypointTrans.position.x, 0, waypointTrans.position.z);

//		float x = waypointTrans.localScale.x;
//		SetWaypointOffset();
//		thisCar.target += waypointTrans.right  * x * waypointOffset;
	}
}
