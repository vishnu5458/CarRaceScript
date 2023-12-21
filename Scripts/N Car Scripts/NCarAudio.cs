using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (NCarController))]
    public class NCarAudio : MonoBehaviour
    {
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

       
        public AudioClip highAccelClip;                                          // Audio clip for high acceleration
        public AudioClip skidClip;                                                 // Audio clip for high deceleration
        public float pitchMultiplier = 1f;                                       // Used for altering the pitch of audio clips
        public float lowPitchMin = 1f;                                          // The lowest possible pitch for the low sounds
        public float lowPitchMax = 6f;                                         // The highest possible pitch for the low sounds
        public float highPitchMultiplier = 0.25f;                             // Used for altering the pitch of high sounds
		public float nitroPitchBoost = 0.5f;
		public float inputAccModifier = 0.7f;	
        public float maxRolloffDistance = 500;                             // The maximum distance where rollof starts to take place
      
		private AudioSource m_HighAccel; 								  	 // Source for the high acceleration sounds
   		private AudioSource skidSource;                                     // Source for the low acceleration sounds
        private bool m_StartedSound;										 // flag for knowing if we have started sounds
		private int boostStatus = 0;
        private NCarController m_CarController; 							 // Reference to car we are controlling

		private bool isSoundEnabled = false;

		public NCarController thisCarController 
		{
			get 
			{
				if(!m_CarController)
					m_CarController = GetComponent<NCarController>();
				return m_CarController;
			}
		}

		void Start()
		{
			isSoundEnabled = true;
		}

		public void SetSoundStatus(bool status)
		{
			isSoundEnabled = status;
			if(!status)
			{
				StopSound();
			}
		}

        public void StartSound ()
        {
			if(!isSoundEnabled)
				return;

			// setup the simple audio source
			m_HighAccel = SetUpEngineAudioSource(highAccelClip);
			// setup the skid sound source
			skidSource = SetUpEngineAudioSource(skidClip);

			// flag that we have started the sounds playing
			m_StartedSound = true;
        }

        public void StopSound()
        {
            //Destroy all audio sources on this object:
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }

            m_StartedSound = false;
        }

        // Update is called once per frame
        private void Update()
        {
			if(!thisCarController.isCarEnabled)
				return;
            // get the distance to main camera
            float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

            // stop sound if the object is beyond the maximum roll off distance
            if (m_StartedSound && camDist > maxRolloffDistance*maxRolloffDistance)
            {
                StopSound();
            }

            // start the sound if not playing and it is nearer than the maximum distance
            if (!m_StartedSound && camDist < maxRolloffDistance*maxRolloffDistance)
            {
                StartSound();
            }

            if (m_StartedSound)
            {

	//			Debug.Log(thisCarController.AccelInput);
				float accInput = Mathf.Abs(thisCarController.AccelInput);

				pitchMultiplier = (1 - inputAccModifier) * accInput + inputAccModifier;
				pitchMultiplier += boostStatus * nitroPitchBoost;


		        // The pitch is interpolated between the min and max values, according to the car's revs.
				float pitch = ULerp(lowPitchMin, lowPitchMax, thisCarController.Revs);

		        // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
		        pitch = Mathf.Min(lowPitchMax, pitch);


	            // for 1 channel engine sound, it's oh so simple:
	            m_HighAccel.pitch = pitch*pitchMultiplier*highPitchMultiplier;
//	            m_HighAccel.dopplerLevel =  0;
	            m_HighAccel.volume = 1;
	           
	 			// adjust the skid source based on the cars current skidding state
				skidSource.volume = Mathf.Clamp01(thisCarController.AvgSkid * 3 - 1);
				skidSource.pitch = Mathf.Lerp (0.8f, 1.3f, thisCarController.SpeedFactor);
//				skidSource.dopplerLevel = useDoppler ? dopplerLevel :	 0;
            }
        }

        // sets up and adds new audio source to the gane object
        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            // create the new audio source component on the game object and set up its properties
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
		source.spatialBlend = 1f;
            source.volume = 0;
            source.loop = true;

            // start the clip from a random point
            source.time = Random.Range(0f, clip.length);
            source.Play();
			
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.dopplerLevel = 0;
            return source;
        }

	public void OnBoostEnable(int status)
	{
		boostStatus = status;
	}

	public void OnNitroDisable()
	{
		boostStatus = 0;
	}
	
        // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
        float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }
    }
}
