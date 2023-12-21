using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour 
{
//	public Camera guiCamera;
	[SerializeField]
	private Camera smoothFollowCamera;
	[SerializeField]
	private Camera hoodCamera;

	[HideInInspector]
	public bool isMineEquipped = false;
	[HideInInspector]
	public MineLauncher _mineLauncher;
	[HideInInspector]
	public WeaponBase equippedWeapon;

	public bool isTiltEnabled = false;

//	private bool isPowerUpActive = false;
//	private PowerUpType powerUpType;
	private AudioListener smoothFollowListener;
	private AudioListener hoodListener;
	private PlayerCar playerCar;
	
	static PlayerControl playerControl;

	public static PlayerControl GetInstance()
	{
		return playerControl;
	}

	void Awake()
	{
		playerControl = this;
//		Initialize();
		playerCar = GetComponent<PlayerCar>();
		hoodCamera = GetComponentInChildren<Camera>();
	}

	public void Initialize()
	{
//		equippedWeapon.linkedCar = playerCar;
		if(isMineEquipped)
			_mineLauncher.InitialiseWeapon(playerCar);
		
		equippedWeapon.InitialiseWeapon(playerCar);
		OnAmmoCollect(1);
		smoothFollowCamera = SmoothFollow.instance.GetSmoothFollowCam();		
		smoothFollowListener = smoothFollowCamera.GetComponent<AudioListener>();
		smoothFollowCamera.enabled = true;
		smoothFollowListener.enabled = true;

		hoodListener = hoodCamera.GetComponent<AudioListener>();
	}
	
	void Update()
	{

		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_WEBGL)	

		if(playerCar.isCarEnabled)
		{
			
			if(Input.GetKey(KeyCode.Space))
			{
				equippedWeapon.FireTrigger();
//				equippedWeapon.TurnTurret(transform);
			}
			
			if(Input.GetKeyDown(KeyCode.R))
			{
                playerCar.OnVehicleReset();
			}
		
//			if(Input.GetKeyDown(KeyCode.C))
//			{
//				SwitchCamera();
//			}

			if(Input.GetKeyDown(KeyCode.X))
			{
				DropMine();
			}

			if(Input.GetKey(KeyCode.C))
			{
				if(!hoodCamera.enabled)
				{
					smoothFollowCamera.enabled = false;
					smoothFollowListener.enabled = false;
					hoodCamera.enabled = true;
					hoodListener.enabled = true;
				}
			}
			else
			{
				if(!smoothFollowCamera.enabled)
				{
					smoothFollowCamera.enabled = true;
					smoothFollowListener.enabled = true;
					hoodCamera.enabled = false;
					hoodListener.enabled = false;
				}
			}
		}	
#else
		if(isTiltEnabled)
		{
			playerCar.OnMobileSteering(Input.acceleration.x * 2);
		}
#endif
	}

	public void OnAmmoCollect(int multiplier)
	{
		int ammoCount = 0;

		switch(equippedWeapon.thisWeaponType)
		{
		case WeaponType.SingleShot : ammoCount = 20;
			break;
		case WeaponType.GrenadeLauncher : ammoCount = 10;
			break;
		case WeaponType.ChainGun : ammoCount = 50;
			break;
		case WeaponType.LaserWeapon : ammoCount = 10;
			break;
		case WeaponType.RocketLaunch : ammoCount = 8;
			break;
		}

		ammoCount *= multiplier;
		equippedWeapon.OnAmmoCollect(ammoCount);
	}

	public void SwitchCamera()
	{
		if(smoothFollowCamera.enabled)
		{
			smoothFollowCamera.enabled = false;
			smoothFollowListener.enabled = false;
			hoodCamera.enabled = true;
			hoodListener.enabled = true;
		}
		else
		{
			smoothFollowCamera.enabled = true;
			smoothFollowListener.enabled = true;
			hoodCamera.enabled = false;
			hoodListener.enabled = false;
		}
	}

	public void DropMine()
	{
		if(isMineEquipped)
			_mineLauncher.FireTrigger();
	}

//	public void OnPowerUpCollect(PowerUpType _powType)
//	{
//		isPowerUpActive = true;
//		powerUpType = _powType;
//		GameplayScreen.instance.SetPowerUp(_powType);
//	}

//	void ActivatePowerUp()
//	{
//		if(!isPowerUpActive)
//			return;
//
//		isPowerUpActive = false;
//		if(powerUpType == PowerUpType.Nitro)
//		{
//			playerCar.BoostCollected(1);
//		}
//		GameplayScreen.instance.OnPowerUpUse();
//	}
	
	void Stop()
	{
		this.enabled = false;
	}
	
	void Resume()
	{
		this.enabled = true;
	}

	public void EnableTiltControl()
	{
		isTiltEnabled = true;
	}

	public void DisableTiltControl()
	{
		isTiltEnabled = false;
	}
}



