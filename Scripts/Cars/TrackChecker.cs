using UnityEngine;
using System.Collections;

public class TrackChecker : MonoBehaviour
{
	public LayerMask visibleLayers = -1;
	private int updateCounter;
	private int outIteration;
	private Transform trans;
	private CarBase thisCar;

	void Start () 
	{
		trans = transform;
		updateCounter = Random.Range(0, 20);
		thisCar = trans.parent.GetComponent<CarBase>();
	}

	void FixedUpdate () 
	{
		if(!thisCar.isCarEnabled)
			return;

		updateCounter ++;
		if(updateCounter % 20 == 0)
		{
			Vector3 pos1 = trans.position;
			pos1.y += 2;
			Vector3 pos2 = pos1;
			pos2.y -= 10;

			if(!Physics.Linecast(pos1, pos2, visibleLayers))
			{
//				if(thisCar.isPlayerCar)
//					Debug.Log("out iteration : " + outIteration);
				outIteration ++;
                if (outIteration > 15)
                {
                    Debug.Log("TrackCheck");
                    thisCar.OnVehicleReset();
                }
			}
			else
			{
				outIteration = 0;
			}
		}
	}
}
