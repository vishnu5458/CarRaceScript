using UnityEngine;
using System.Collections;

public class Twister : MonoBehaviour 
{
	public int initDelay = 20;
	public float twisterSpeed = 2;
	public AudioClip thunderSound;

	private Rigidbody sourceBody;
    private DistWP startWp;
    private DistWP currentWaypoint;
	private Transform trans;
	private Animation thisAnimation;
	private Vector3 target;
	private bool isInitialiseComplete = false;
	private float delay;

	// Use this for initialization
	void Start ()
	{
//		isReferenceObject = true;
//		initialise();
//		thisAudioSource = GetComponent<AudioSource>();
	}
	
    public void Initialise(float _timeOut, DistWP _startWP, Rigidbody _sourceBody)
	{
		startWp = _startWP;
		sourceBody = _sourceBody;

		Debug.Log("twister initialised");
//		thisAnimation = GetComponentInChildren<Animation>();
//		thisAnimation.Play("Twister");
		currentWaypoint = startWp;
		trans = transform;
		target = startWp.nextDistWaypoint.thisTransform.position;
		
		if(GlobalClass.SoundVoulme == 1)
		{
			GetComponent<AudioSource>().Play();
//			thisAudioSource.Play();
			AudioSource.PlayClipAtPoint(thunderSound, trans.position, GlobalClass.SoundVoulme);
		}
		
		trans.LookAt(target);	
		Invoke("KillTwister", _timeOut);
		isInitialiseComplete = true;
		delay = 0;
	}
	
	
	public void KillTwister()
	{
		sourceBody = null;
		Debug.Log("twister killed");
		CancelInvoke("KillTwister");
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!isInitialiseComplete)
			return;

		delay--;
		if(delay < 0)
		{
			delay = initDelay;
			int layer = 1 << 12;
			Vector3 explosionPos = trans.position;
			int radius = 15;
			
			Collider[] colliders = Physics.OverlapSphere(explosionPos, radius, layer);
			foreach (Collider hit in colliders) 
			{
				Transform carBody = hit.transform.parent.parent;
//				Debug.Log(carBody.name);
				if (!carBody.GetComponent<CarBase>().isResetting && carBody.GetComponent<CarBase>() && carBody.GetComponent<Rigidbody>() != sourceBody)
				{
					Debug.Log(carBody.name);
					explosionPos = carBody.position;
					explosionPos -= carBody.forward;
					carBody.GetComponent<Rigidbody>().AddExplosionForce(10, explosionPos, radius, 0.2f, ForceMode.VelocityChange );
				}     
			}

		}
		Vector3 RelativePosition = trans.InverseTransformPoint(target);
		float magnitude = RelativePosition.magnitude + .1f;
		
		
		if (magnitude < 20) 
		{
			
            DistWP closeWaypoint = currentWaypoint;
			
			currentWaypoint = closeWaypoint.nextDistWaypoint;	
		
			
			Transform waypointTrans = currentWaypoint.thisTransform;
			target = new Vector3( waypointTrans.position.x, 0, waypointTrans.position.z);
			trans.LookAt(target);
			
		}
		
		trans.position += trans.forward * twisterSpeed;

	}
}
