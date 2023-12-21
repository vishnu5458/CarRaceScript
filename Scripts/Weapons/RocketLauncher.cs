using UnityEngine;
using System.Collections;

public class RocketLauncher : WeaponBase 
{

	protected override void Awake ()
	{
		thisWeaponType = WeaponType.RocketLaunch;
		base.Awake ();
	}

	public override void FireTrigger ()
	{
		if(!isFireReady)
			return;

		Transform currFireTrans = firePosTrans[firePosIndex];
		AudioManager.instance.PlayWeaponSound(currFireTrans.position, thisWeaponType);

		SeekerMissile _missile = WeaponPoolManager.instance.GetMissile().GetComponent<SeekerMissile>();
		_missile.trans.position = currFireTrans.position;
		_missile.trans.forward = currFireTrans.forward;
		Vector3 _lea = _missile.trans.localEulerAngles;
		_lea.x = 0;//-20;
		_missile.trans.localEulerAngles = _lea;
		_missile.Fire(linkedVehicle.speed * angleDifference, targetLayer, linkedVehicle.isActiveVehicle, damage);

		base.FireTrigger ();
	}

	protected override void RaycastHitTarget (Transform hitTarget)
	{
		base.RaycastHitTarget (hitTarget);
	}
}
