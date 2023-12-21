using UnityEngine;
using System.Collections;

public class TrafficCollider : MonoBehaviour 
{
	private TrafficCar thisCar;

	void Awake()
	{
		bool isClassFound = false;
		Transform parentTrans = transform;
		while(!isClassFound)
		{
			parentTrans  = parentTrans.parent;

			if(parentTrans == null)
			{
				Debug.LogError("traffic car not found in parent");
				isClassFound = true;
				break;
			}
			else if(parentTrans.GetComponent<TrafficCar>())
			{
				thisCar = parentTrans.GetComponent<TrafficCar>();
				isClassFound = true;
				break;
			}
		}
	}

	public TrafficCar GetTrafficCar()
	{
		return thisCar;
	}
}
