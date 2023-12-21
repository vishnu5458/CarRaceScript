using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour 
{

//	private float sparkTimeOut;

//	public float sparkDuration;
	public ParticleSystem spark;
	public ParticleSystem sparkSmall;
//	public ParticleEmitter continousSpark;

	public ParticleSystem explosionSmall;
	public ParticleSystem explosionBig;
	public ParticleSystem blasterParticle;
    public ParticleSystem turboParticle;

#region Singleton
	private static ParticleManager _instance;
	public static ParticleManager instance {
		get {
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<ParticleManager> ();
			return _instance;
		}
	}
#endregion

	// Use this for initialization
	void Start () 
	{
		_instance = this;
	}

	public void PlayCollisionSpark(Vector3 pos)
	{
		spark.transform.position = pos;
		spark.Play();
//		sparkTimeOut = sparkDuration;
	}

	public void PlaySmallCollisionSpark(Vector3 pos)
	{
		sparkSmall.transform.position = pos;
		sparkSmall.Play();
	}

//	public void PlayContinousSpark(Vector3 pos)
//	{
//		continousSpark.transform.position = pos;
//		continousSpark.Emit();
//	}

	public void PlaySmallExplosionAtPoint(Vector3 pos)
	{
		explosionSmall.transform.position = pos;
		explosionSmall.Play();
	}

	public void PlayBigExplosionAtPoint(Vector3 pos)
	{
		explosionBig.transform.position = pos;
		explosionBig.Play();
	}

	public void PlayPentaBlastAtPoint(Vector3 pos)
	{
		blasterParticle.transform.position = pos;
		blasterParticle.Play();
	}

    public void PlayNitroAtPoint()
    {
        turboParticle.Play();
    }
    public void StopNitroAtPoint()
    {
        turboParticle.Stop();
        turboParticle.Clear();
    }

	// Update is called once per frame
	void Update () 
	{
//		if(spark.emit)
//		{
//			sparkTimeOut --;
//			if(sparkTimeOut < 0)
//				spark.emit = false;
//		}
	}
}
