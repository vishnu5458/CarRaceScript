using UnityEngine;
using System.Collections;

public class WeaponPoolManager : MonoBehaviour 
{
	#region Singleton
	private static WeaponPoolManager _instance;
	public static WeaponPoolManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<WeaponPoolManager> ();
			}
			return _instance;
		}
	}
	#endregion

	public GameObjectPool bulletArray;
	public GameObjectPool grenadeArray;
	public GameObjectPool seekerMissileArray;
	public GameObjectPool mineArray;

	// Use this for initialization
	void Awake () 
	{
		_instance = this;
	}
	
	public void ResetPool()
	{
		bulletArray.ReleaseAllInstances();
		grenadeArray.ReleaseAllInstances();
		seekerMissileArray.ReleaseAllInstances();
		mineArray.ReleaseAllInstances();
	}

	public Transform GetBullet()
	{
		return bulletArray.GetInstance();
	}

	public Transform GetGrenade()
	{
		return grenadeArray.GetInstance();
	}

	public Transform GetMissile()
	{
		return seekerMissileArray.GetInstance();
	}

	public Transform GetMine()
	{
		return mineArray.GetInstance();
	}
}
