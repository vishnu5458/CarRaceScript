using UnityEngine;
using System.Collections;

public class DistWpDetect : MonoBehaviour 
{

	public CarBase thisCar;

	// Use this for initialization
	void Start () 
	{
		thisCar = GetComponent<CarBase>();
	}

//	void OnTriggerEnter(Collider other)
//	{
//		if(!thisCar.isCarEnabled)
//			return;
//		
//		if(other.GetComponent<DistWP>())
//		{
//			thisCar.GetNavBase().OnDistWpTriggerEnter(other.GetComponent<DistWP>());
//		}
//	}

}
