using UnityEngine;
using System.Collections;

public class LaserWeapon : WeaponBase 
{
	LaserScript laserParticle;
	protected override void Awake ()
	{
		laserParticle = GetComponentInChildren<LaserScript>();
		thisWeaponType = WeaponType.LaserWeapon;
		base.Awake ();
	}

	public override void FireTrigger ()
	{
		if(!isFireReady)
			return;

		Transform currFireTrans = firePosTrans[firePosIndex];
		AudioManager.instance.PlayWeaponSound(currFireTrans.position, thisWeaponType);
		RaycastHit hit;
		if(Physics.SphereCast(currFireTrans.position, 3, currFireTrans.forward, out hit, 100, targetLayer))
		{
			Debug.Log("hit :  "+ hit.transform.name);
			RaycastHitTarget(hit.transform);
//			float dist = Vector3.Distance(currFireTrans.position, hit.point);
			laserParticle.ShowLaserEffect(currFireTrans, hit.point);
		}
		base.FireTrigger ();
	}

	protected override void RaycastHitTarget (Transform hitTarget)
	{
		base.RaycastHitTarget (hitTarget);

		GameObject obj = hitTarget.gameObject;
		Debug.Log("hit :  "+ hitTarget.name);
		if(obj.layer == 11 || obj.layer == 12)
		{
			obj.GetComponent<CarBase>().ApplyDamage(damage);
		}
		else if(obj.layer == 21)
		{
			obj.GetComponent<TrafficCar>().ApplyDamage(damage, linkedVehicle.isActiveVehicle);
		}
		else if(obj.layer == 10)
		{
			if(obj.GetComponent<Building>())
			{
				obj.GetComponent<Building>().ApplyDamage(damage);
			}
			else
			{
				bool isClassFound = false;
				Transform parentTrans = obj.transform;
				while(!isClassFound)
				{
					parentTrans  = parentTrans.parent;

					if(parentTrans == null)
					{
						Debug.LogError("building class not found in parent");
						isClassFound = true;
						break;
					}
					else if(parentTrans.GetComponent<Building>())
					{
						parentTrans.GetComponent<Building>().ApplyDamage(damage);
						isClassFound = true;
						break;
					}
				}
			}
		}
		else if(obj.layer == 8)
		{
			if(obj.GetComponent<Explosives>() != null)
				obj.GetComponent<Explosives>().ApplyDamage(damage);
		}
	}
}
