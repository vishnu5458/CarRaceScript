using UnityEngine;
using System.Collections;

public class CarResetTrigger : MonoBehaviour
{
  		
	public bool isHittingSomthin = true;
	[HideInInspector]
	public CarBase thisCar;
	public LayerMask targetLayer;

	[SerializeField]
	private int outOfTrackTimeCounter = 0;
	private Transform trans;
	
	// Use this for initialization
	void Start ()
	{
        thisCar = transform.parent.parent.GetComponent<CarBase> ();
		trans = transform;
	}

	//  void Update ()
	//  {
	//    if (!isHittingSomthin)
	//    {
	//      outOfTrackTimeCounter++; // may need to assign another variabe if constanst reset occur
	//      if (outOfTrackTimeCounter > 200)
	//      {
	//	outOfTrackTimeCounter = 0;
	//	thisCar.OnCarReset ();
	//      }
	//    }
	//  }

	void FixedUpdate ()
	{
		if (!thisCar.isActiveVehicle)
			return;
    
		if (Time.frameCount % 30 == 0) {
			Vector3 startPos = trans.position;
			Vector3 endpos = startPos - (trans.up * 10);

            if (!Physics.Linecast (startPos, endpos, targetLayer)) 
            {
                outOfTrackTimeCounter++;
				if (outOfTrackTimeCounter > 10) {
					outOfTrackTimeCounter = 0;
                    Debug.Log("Reset on CarResetTrigger");
					thisCar.OnVehicleReset ();
				}
			}
		}
	}
}
