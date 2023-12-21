using UnityEngine;
using System.Collections;

public class BulletWeapon : WeaponBase 
{
	public MuzzleFlash[] muzzleFlash;

	protected override void Awake ()
	{
		if(gameObject.name.Contains("Chain"))
			thisWeaponType = WeaponType.ChainGun;
		else
			thisWeaponType = WeaponType.SingleShot;
		
		base.Awake ();
	}

	public override void FireTrigger ()
	{
		if(!isFireReady)
			return;

		Transform currFireTrans = firePosTrans[firePosIndex];
		muzzleFlash[firePosIndex].ShowFlash();

		Bullet _bullet =  WeaponPoolManager.instance.GetBullet().GetComponent<Bullet>();
		_bullet.trans.position = currFireTrans.position;
		_bullet.trans.forward = currFireTrans.forward;
		Vector3 _lea = _bullet.trans.localEulerAngles;
		if(isPlayerWeapon)	_lea.x = 0;
		_bullet.trans.localEulerAngles = _lea;
		_bullet.Fire(linkedVehicle.speed * angleDifference, targetLayer, linkedVehicle.isActiveVehicle, damage);

		AudioManager.instance.PlayWeaponSound(currFireTrans.position, thisWeaponType);

		base.FireTrigger ();


	}

	protected override void RaycastHitTarget (Transform hitTarget)
	{
		base.RaycastHitTarget (hitTarget);
	}

}
