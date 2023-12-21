using UnityEngine;
using System.Collections;

public class CarDelayedTrigger : MonoBehaviour 
{
	public CarBase linkedCar;
	private bool isTriggerActive = false;

	public void InitTrigger()
	{
		isTriggerActive = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if(!isTriggerActive)
			return;

        if(other.gameObject.layer == 11 || other.GetComponent<PlayerBike>())
		{
			Debug.Log(other.name);
			isTriggerActive = false;
			
			if(!linkedCar.isCarEnabled)
				linkedCar.EnableVehicle();
		}
	}
}
