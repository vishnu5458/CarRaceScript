using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	#region Singleton

	private static AudioManager _instance;

	public static AudioManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<AudioManager>();
			}
			return _instance;
		}
	}

	#endregion

	public AudioClip menuBg;
	public AudioClip gamePlayBg;

	public AudioClip rollOverClip;
	public AudioClip mouseDownClip;

	public AudioClip bulletWeaponClip;
	public AudioClip[] bulletHitClips;

	public AudioClip grenadeLauncherClip;
	public AudioClip missileLauncherClip;
	public AudioClip laserWeaponClip;

	public AudioClip crashSound;
	public AudioClip[] blastSound;
	public AudioClip blasterClip;
	public AudioClip coinSound;
	public AudioClip nitroSound;
    public AudioClip carStart;

	public AudioSource BgSource;

	private float currBGVol = 1;
		
	// Use this for initialization
	void Awake()
	{
		_instance = this;
		BgSource = GetComponent<AudioSource>();
	}

	public void PlayCrashSoundAtPos(Vector3 pos, float vol)
	{
		AudioSource3D.PlayClip3DAtPoint(crashSound, pos, GlobalClass.SoundVoulme * vol);
	}

	public void PlayBlastSoundAtPos(Vector3 pos)
	{
		AudioSource3D.PlayClip3DAtPoint(blastSound[Random.Range(0, blastSound.Length)], pos, GlobalClass.SoundVoulme * 4);
	}

	public void PlayRollOverSound()
	{
		AudioSource3D.PlayClip2D(rollOverClip, GlobalClass.SoundVoulme);
	}

	public void PlayMouseClickSound()
	{
		AudioSource3D.PlayClip2D(mouseDownClip, GlobalClass.SoundVoulme);
	}

	public void PlayNitroCollectSound()
	{
		AudioSource3D.PlayClip2D(nitroSound, GlobalClass.SoundVoulme * 0.5f);
	}

	public void PlayCoinSound()
	{
		AudioSource3D.PlayClip2D(coinSound, GlobalClass.SoundVoulme);
	}

	public void PlayWeaponSound(Vector3 pos, WeaponType _type)
	{
//		if(_type == WeaponType.ChainGun || _type == WeaponType.SingleShot)
//			AudioSource3D.PlayClipAtPoint(bulletWeaponClip, pos, GlobalClass.SoundVoulme * 4);
//		else if(_type == WeaponType.GrenadeLauncher)
//			AudioSource3D.PlayClipAtPoint(grenadeLauncherClip, pos, GlobalClass.SoundVoulme * 4);
//		else if(_type == WeaponType.LaserWeapon)
//			AudioSource3D.PlayClipAtPoint(laserWeaponClip, pos, GlobalClass.SoundVoulme * 4);
//		else if(_type == WeaponType.MineLauncher)
//			AudioSource3D.PlayClipAtPoint(grenadeLauncherClip, pos, GlobalClass.SoundVoulme * 4);
//		else if(_type == WeaponType.RocketLaunch)
//			AudioSource3D.PlayClipAtPoint(missileLauncherClip, pos, GlobalClass.SoundVoulme * 4);
	}

	public void PlayBulletHitSound(Vector3 pos)
	{
		AudioSource3D.PlayClip3DAtPoint(bulletHitClips[Random.Range(0, bulletHitClips.Length)], pos, GlobalClass.SoundVoulme * 4);
	}

	public void PlayMuteBG()
	{
		if (GlobalClass.SoundVoulme == 1)
		{
			BgSource.volume = currBGVol;

			BgSource.loop = true;
			if (!BgSource.isPlaying)
				BgSource.Play();
		}
		else
		{
			BgSource.volume = 0;
			BgSource.Stop();
		}
	}

	public void FadeOutBg()
	{
		StartCoroutine(LerpFadeOut());
	}

	IEnumerator LerpFadeOut()
	{
		while (Mathf.Abs(BgSource.volume) > 0.3f)
		{
			BgSource.volume -= 0.2f;
			yield return null;
		}

		BgSource.volume = 0;
	}

	public void OnMenuLoad()
	{
		BgSource.Stop();
		BgSource.clip = menuBg;
        currBGVol = 1;
		PlayMuteBG();
	}

	public void OnGamePlayLoad()
	{
		BgSource.Stop();
		BgSource.clip = gamePlayBg;
		currBGVol = 0.6f;
		PlayMuteBG();
	}

    public void PlayCarStartSound()
    {
        AudioSource3D.PlayClip2D(carStart, GlobalClass.SoundVoulme*.4f);
    }

}
