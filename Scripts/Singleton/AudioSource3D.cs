using UnityEngine;
using System.Collections;

public class AudioSource3D : MonoBehaviour {

	public static AudioSource PlayClip3DAtPoint(AudioClip soundClip,Vector3 emitPos,float volume) 
	{
		GameObject nObj = new GameObject("OneShotAudio");
		nObj.transform.position = emitPos;
		AudioSource audio = nObj.AddComponent<AudioSource>();
		audio.spatialBlend = 1f;
		audio.volume = volume;
		audio.clip = soundClip;
		audio.Play();
		Destroy(nObj, soundClip.length);
		return audio;
	}

    public static AudioSource PlayClip2D(AudioClip soundClip,float volume) 
    {
        GameObject nObj = new GameObject("OneShotAudio");
        nObj.transform.position = Vector3.zero;
        AudioSource audio = nObj.AddComponent<AudioSource>();
        audio.spatialBlend = 0.0f;
        audio.volume = volume;
        audio.clip = soundClip;
        audio.Play();
        Destroy(nObj, soundClip.length);
        return audio;
    }
}
