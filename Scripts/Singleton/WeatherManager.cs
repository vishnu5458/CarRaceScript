using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class WeatherDefnitionClass
{
	public Material skyboxMaterial;
	public Color lightColor;
	public float lightIntensity = 1.0f;
}

public class WeatherManager : MonoBehaviour 
{
	private bool isEmiting = false;
	private float rainPropability = 0.2f;
	private Transform thisTrans;
	private ParticleSystem currentParticle;

	public Light keyLight;
	public WeatherDefnitionClass[] _weatherDetail;
	public ParticleSystem[] trackParticleEffects;
	public AudioSource rainAudio;

	#region Singleton
	private static WeatherManager _instance;
	public static WeatherManager instance 
	{
		get 
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<WeatherManager> ();
			return _instance;
		}
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		thisTrans = transform;
		_instance = this;
	}

	public void InitWeatherManager(float dist, TrackType _trackType)
	{
		CancelInvoke("SetRain");

//		if(_trackType == TrackType.Desert)
//		{
//			RenderSettings.skybox = _weatherDetail[0].skyboxMaterial;
//			keyLight.color = _weatherDetail[0].lightColor;
//			keyLight.intensity = _weatherDetail[0].lightIntensity;
//			DynamicGI.UpdateEnvironment();
//			return;
//		}
//		else if(_trackType == TrackType.Grass)
//		{
//			RenderSettings.skybox = _weatherDetail[1].skyboxMaterial;
//			keyLight.color = _weatherDetail[1].lightColor;
//			keyLight.intensity = _weatherDetail[1].lightIntensity;
//			DynamicGI.UpdateEnvironment();
//
//			rainPropability = 0.2f;
//			currentParticle = trackParticleEffects[0];
//		}
//		else if(_trackType == TrackType.Mountain)
//		{
//			RenderSettings.skybox = _weatherDetail[2].skyboxMaterial;
//			keyLight.color = _weatherDetail[2].lightColor;
//			keyLight.intensity = _weatherDetail[2].lightIntensity;
//			DynamicGI.UpdateEnvironment();
//
//			return;
//		}
//		else if(_trackType == TrackType.Snow)
//		{
//			RenderSettings.skybox = _weatherDetail[3].skyboxMaterial;
//			keyLight.color = _weatherDetail[3].lightColor;
//			keyLight.intensity = _weatherDetail[3].lightIntensity;
//			DynamicGI.UpdateEnvironment();
//
//			rainPropability = 1.1f;
//			currentParticle = trackParticleEffects[1];
//		}
//		else
		{
//			Debug.Log("unchanged texture");
//			RenderSettings.skybox = _weatherDetail[0].skyboxMaterial;
//			keyLight.color = _weatherDetail[0].lightColor;
//			keyLight.intensity = _weatherDetail[0].lightIntensity;
//			DynamicGI.UpdateEnvironment();

			return;
		}

		Vector3 lp = new Vector3(0, 15, dist + 2);
		thisTrans.localPosition = lp;
	
//		SetRain();
		InvokeRepeating("SetRain",30,30);
	}

	void SetRain()
	{
		float chance = UnityEngine.Random.value;
		if(chance < rainPropability)
		{
			if(GlobalClass.SoundVoulme == 1)
				rainAudio.PlayDelayed(0.5f);
			currentParticle.Play();
			isEmiting = true;
		}
		else
		{
			rainAudio.Stop();
			currentParticle.Stop();
			isEmiting = false;
		}
//		Debug.Log(chance);
	}

	public void UpdateRainAngle(float speed)
	{
		if(!isEmiting)
			return;

		//angle -20 to 20
		Vector3 rainAngle = Vector3.zero;
		rainAngle.x = (2.0f / 5.0f) * Mathf.Abs(speed);

		thisTrans.localEulerAngles = rainAngle;

//		Debug.Log(rainAngle.x);
	}

	public void OnGameOver()
	{
		if(currentParticle == null)
			return;

		CancelInvoke("SetRain");
		rainAudio.Stop();
		isEmiting = false;
		currentParticle.Stop();
		currentParticle = null;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
