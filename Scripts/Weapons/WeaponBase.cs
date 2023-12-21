using UnityEngine;
using System.Collections;

public enum WeaponType
{
	SingleShot,
	GrenadeLauncher,
	ChainGun,
	LaserWeapon,
	RocketLaunch,
	MineLauncher
}

public class WeaponBase : MonoBehaviour 
{
	public WeaponType thisWeaponType;
	public LayerMask targetLayer;
	public Transform[] firePosTrans;
	[HideInInspector]
	public VehicleBase linkedVehicle;

	protected bool isFireReady = true;
	protected bool isPlayerWeapon = false;
	[SerializeField]
	protected float fireRate = 10;
	[SerializeField]
	protected float damage = 10;
	protected float angleDifference = 0;
	protected int firePosIndex;
	[SerializeField]
	protected int ammoCount = 0;
	protected Transform trans;

	private float fireIntervalDT;
	private Transform target;

	// Use this for initialization
	protected virtual void Awake () 
	{
		trans = transform;
		firePosIndex = 0;
	}

	public void InitialiseWeapon(VehicleBase _linkedCar)
	{
		linkedVehicle = _linkedCar;
		isPlayerWeapon = _linkedCar.isActiveVehicle;
		if(!isPlayerWeapon || thisWeaponType == WeaponType.MineLauncher)	ammoCount = 1;
		angleDifference = 1 - ((Vector3.Angle(linkedVehicle.trans.forward, trans.forward))/90);
	}

	public void OnAmmoCollect(int _ammo)
	{
		ammoCount += _ammo;
		GamePlayScreen.instance.UpdateAmmoCount(ammoCount);
	}

	// Update is called once per frame
	void Update () 
	{
		if(!isFireReady && ammoCount > 0)
		{
			fireIntervalDT--;
			if(fireIntervalDT < 0)
			{
				WeaponReady();
			}
		}
	}

	protected virtual void WeaponReady()
	{
		isFireReady = true;
	}

	//only for ai car
	public void TurnTurret(Transform _target)
	{
		target = _target;
		StartCoroutine(SmoothTurnTurretCoroutine());
	}

	IEnumerator SmoothTurnTurretCoroutine()
	{
		Quaternion rotation = Quaternion.LookRotation(target.position - trans.position);
		while(Mathf.Abs(rotation.eulerAngles.y - trans.rotation.eulerAngles.y) > 0.1f)
		{
			yield return new WaitForEndOfFrame();

			rotation = Quaternion.LookRotation(target.position - trans.position);
			rotation.x = 0;
			rotation.z = 0;
			trans.rotation = Quaternion.Slerp(trans.rotation, rotation, Time.deltaTime * 3);
		}
		angleDifference = 1 - ((Vector3.Angle(linkedVehicle.trans.forward, trans.forward))/90);
		StartCoroutine(linkedVehicle.GetComponent<AICar>().OnTurretTrunComplete());
	}

	public virtual void FireTrigger()
	{
		if(!isFireReady)
			return;

		if(isPlayerWeapon && thisWeaponType != WeaponType.MineLauncher)
		{
			if(ammoCount < 1)
				return;
			ammoCount--;
			GamePlayScreen.instance.UpdateAmmoCount(ammoCount);
		}

		isFireReady = false;
		fireIntervalDT = fireRate;

		firePosIndex++;
		if(firePosIndex > firePosTrans.Length - 1)
			firePosIndex = 0;


//		Bullet _bullet =  WeaponPoolManager.instance.GetBullet().GetComponent<Bullet>();
//		_bullet.trans.position = currFireTrans.position;
//		_bullet.trans.forward = currFireTrans.forward;
//		Vector3 _lea = _bullet.trans.localEulerAngles;
//		_lea.x = 0;
//		_bullet.trans.localEulerAngles = _lea;
//		_bullet.Fire(linkedCar.speed * angleDifference, targetLayer);

//		RaycastHit hit;
//		if(Physics.SphereCast(currFireTrans.position, 3, currFireTrans.forward, out hit, 100, targetLayer))
//		{
//			Debug.Log("hit :  "+ hit.transform.name);
//			RaycastHitTarget(hit.transform);
//		}
	}

	protected virtual void RaycastHitTarget(Transform hitTarget)
	{
		
	}

}
