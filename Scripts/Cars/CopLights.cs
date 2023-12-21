using UnityEngine;
using System.Collections;

public class CopLights : MonoBehaviour 
{
	public int lightingInterval = 10;
	public GameObject[] loadingIndicator;
	public AudioSource sirenAudio;

	private int lightIndex = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.frameCount % lightingInterval == 0)
		{
			loadingIndicator[lightIndex].SetActive(false);
			lightIndex++;
			if(lightIndex > loadingIndicator.Length -1)
				lightIndex = 0;
			loadingIndicator[lightIndex].SetActive(true);
		}
	}

	public void SetSound(bool status)
	{
//		Debug.Log("set sound as: "+ status);
		if(status)
			sirenAudio.Play();
		else
			sirenAudio.Stop();
	}
}
