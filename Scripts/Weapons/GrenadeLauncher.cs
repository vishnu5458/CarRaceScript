using UnityEngine;
using System.Collections;

public class GrenadeLauncher : WeaponBase 
{
	public MuzzleFlash[] muzzleFlash;

	protected override void Awake ()
	{
		thisWeaponType = WeaponType.GrenadeLauncher;
		base.Awake ();
	}

	public override void FireTrigger ()
	{
		if(!isFireReady)
			return;

		Transform currFireTrans = firePosTrans[firePosIndex];
		muzzleFlash[firePosIndex].ShowFlash();
		AudioManager.instance.PlayWeaponSound(currFireTrans.position, thisWeaponType);

		Grenade _grenade =  WeaponPoolManager.instance.GetGrenade().GetComponent<Grenade>();
		_grenade.trans.position = currFireTrans.position;
		_grenade.trans.forward = currFireTrans.forward;
		Vector3 _lea = _grenade.trans.localEulerAngles;
		_lea.x = 0;//-20;
		_grenade.trans.localEulerAngles = _lea;
		_grenade.Fire(linkedVehicle.speed * angleDifference, targetLayer, linkedVehicle.isActiveVehicle, damage);

		base.FireTrigger ();
	}

	protected override void RaycastHitTarget (Transform hitTarget)
	{
		base.RaycastHitTarget (hitTarget);
	}
}
