using UnityEngine;
using System.Collections;

public class SimpleCarAudio : MonoBehaviour {

	// This script reads some of the car's current properties and plays sounds accordingly.
	// The engine sound can be a simple single clip which is looped and pitched, or it
	// can be a crossfaded blend of four clips which represent the timbre of the engine
	// at different RPM and Throttle state.
	
	// the engine clips should all be a steady pitch, not rising or falling.
	
	// when using four channel engine crossfading, the four clips should be:
	// lowAccelClip : The engine at low revs, with throttle open (i.e. begining acceleration at very low speed)
	// highAccelClip : Thenengine at high revs, with throttle open (i.e. accelerating, but almost at max speed)
	// lowDecelClip : The engine at low revs, with throttle at minimum (i.e. idling or engine-braking at very low speed)
	// highDecelClip : Thenengine at high revs, with throttle at minimum (i.e. engine-braking at very high speed)
	
	// For proper crossfading, the clips pitches should all match, with an octave offset between low and high.
	
	
	public AudioClip highAccelClip;                                                     // Audio clip for high acceleration
	public float pitchMultiplier = 1f;                                                  // Used for altering the pitch of audio clips
	public float lowPitchMin = 1f;                                                      // The lowest possible pitch for the low sounds
	public float lowPitchMax = 6f;                                                      // The highest possible pitch for the low sounds
	public float highPitchMultiplier = 0.25f;                                           // Used for altering the pitch of high sounds
	public float maxRolloffDistance = 500;                                              // The maximum distance where rollof starts to take place


//	AudioSource highAccel;                                                              // Source for the high acceleration sounds
//	bool startedSound;                                                                  // flag for knowing if we have started sounds
	TrafficCar _trafficCar;                                                        // Reference to car we are controlling
	
	bool isSoundEnabled = false;
	
	public TrafficCar trafficCar 
	{
		get 
		{
			if(!_trafficCar)
				_trafficCar = GetComponent<TrafficCar>();
			return _trafficCar;
		}
	}
	
	void Start()
	{
		isSoundEnabled = true;
	}
	
	public void SetSoundStatus(bool status)
	{
		isSoundEnabled = status;
	}
	
	public void StartSound () 
	{
		if(!isSoundEnabled)
			return;
		
		// setup the simple audio source
		SetUpEngineAudioSource(highAccelClip);
			

		// flag that we have started the sounds playing
//		startedSound = true;
	}
	
	public void StopSound()
	{
		//Destroy all audio sources on this object:
		foreach (var source in GetComponents<AudioSource>()) {
			Destroy (source);
		}
		
//		startedSound = false;
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//		if(!trafficCar.isCarEnabled)
//			return;
//
//		if (startedSound) {
//			
//			// The pitch is interpolated between the min and max values, according to the car's revs.
//			float pitch = ULerp (lowPitchMin,lowPitchMax,trafficCar.RevsFactor);
//			
//			// clamp to minimum pitch (note, not clamped to max for high revs while burning out)
//			pitch = Mathf.Min(lowPitchMax,pitch);
//			
//			// for 1 channel engine sound, it's oh so simple:
//			highAccel.pitch = pitch*pitchMultiplier*highPitchMultiplier;
//			highAccel.volume = 1;
//		}
//	}
	
	
	// sets up and adds new audio source to the gane object
	void SetUpEngineAudioSource(AudioClip clip)
	{
		// create the new audio source component on the game object and set up its properties
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.volume = 0;
		source.loop = true;
		
		// start the clip from a random point
		source.time = Random.Range(0f, clip.length);
		source.Play();
		source.minDistance = 5;
		source.maxDistance = maxRolloffDistance;
		source.dopplerLevel = 0;
//		return source;
	}
	
}
