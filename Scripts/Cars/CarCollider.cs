using UnityEngine;
using System.Collections;

public class CarCollider : MonoBehaviour 
{
	private CarBase thisCar;

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
			else if(parentTrans.GetComponent<CarBase>())
			{
				thisCar = parentTrans.GetComponent<CarBase>();
				isClassFound = true;
				break;
			}
		}
	}

	public CarBase GetCarBase()
	{
		return thisCar;
	}
}
