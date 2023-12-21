using UnityEngine;
using System.Collections;

public class MineLauncher : WeaponBase
{
	private GameObject mineDispObject;

	protected override void Awake ()
	{
		thisWeaponType = WeaponType.MineLauncher;
		base.Awake ();
		mineDispObject = trans.GetChild(0).gameObject;
	}

	public override void FireTrigger ()
	{
		if(!isFireReady)
			return;

		Transform currFireTrans = firePosTrans[firePosIndex];
		AudioManager.instance.PlayWeaponSound(currFireTrans.position, thisWeaponType);

		Mine _mine = WeaponPoolManager.instance.GetMine().GetComponent<Mine>();
		_mine.trans.position = currFireTrans.position;
		_mine.trans.forward = currFireTrans.forward;
		Vector3 _lea = _mine.trans.localEulerAngles;
		_lea.x = 0;
		_mine.trans.localEulerAngles = _lea;
		_mine.Fire(targetLayer, linkedVehicle.isActiveVehicle, damage);

		mineDispObject.SetActive(false);

		base.FireTrigger ();
	}

	protected override void WeaponReady ()
	{
		mineDispObject.SetActive(true);
		base.WeaponReady ();
	}
}
