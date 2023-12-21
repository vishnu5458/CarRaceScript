using UnityEngine;
using System.Collections;

public class SeekerMissile : MonoBehaviour 
{
	public float lifeTime = 0.5f;
	public Transform trans;
	public float radius = 10;
	public float damage = .2f;

	private bool isPlayerCar = false;
	private int layerMask;
//	private Rigidbody body;
//	private AudioSource thisAudio;

	private bool isTargetAcquired;
	private bool isActive = false;
	private Vector3 target;
	private Transform targetTransform;
	private float damping = 5.0f;
	private float drivespeed = 25f;
	private float carSpeed;
	private float missileSpeed;
	private float rotationBuffer;
	[SerializeField]
	private AnimationCurve speedCurve;
	private float timeSpent;
	private GameObjectPool _pool;
	
	void Awake()
	{
		trans = transform;
//		thisAudio = GetComponent<AudioSource>();
	}

	void OnPoolCreate(GameObjectPool pool)
	{
		_pool = pool;
	}

	public void Fire(float _carSpeedVector, int _layerMask, bool _isPlayer, float _damage)
	{
		isActive = true;
		carSpeed = _carSpeedVector;
		isPlayerCar = _isPlayer;
		layerMask = _layerMask;
		damage = _damage;

		drivespeed += carSpeed;

//		Invoke("SetTarget",0.5f);
		SetTarget();
		rotationBuffer = .25f;
		timeSpent = 0;

//		thisAudio.Play();
	}
		
	void SetTarget()
	{
		Vector3 fwd = trans.forward;
		fwd.y = 0;
		Vector3 targetPosition = trans.position + fwd * drivespeed * 2;

		RaycastHit hit;
		int _layerMask;
		if(isPlayerCar)
		{
			_layerMask = 1 << 12;
		}
		else
		{
			_layerMask = 1 << 11;
		}
		if(Physics.SphereCast(trans.position, 6, fwd, out hit, 500, _layerMask))
		{
			targetTransform = hit.transform;
		
			target = targetTransform.position;
			target.y += 0.75f;
			isTargetAcquired=true;
			Debug.Log("Target locked : "+ hit.transform.name);
		}
		else
		{
			target = targetPosition;
			target.y = 0;
			isTargetAcquired = false;
			//body.velocity = thisTransform.forward * speed * 1.6f;
		}
			

//		Collider[] colliders = Physics.OverlapSphere(targetPosition, 50, layerMask);
//		if(colliders.Length > 0)
//		{
//			foreach(Collider _coll in colliders)
//			{
//				if(_coll.gameObject.layer == 21) //Traffic Car
//					targetTransform = _coll.transform;
//			}
//
//			foreach(Collider _coll in colliders)
//			{
//				if(_coll.gameObject.layer == 12 || _coll.gameObject.layer == 11) //AI Car or player car . added here because we are giving more priority for targeting AI Car
//					targetTransform = _coll.transform;
//			}
//
//			target = targetTransform.position;
//			isTargetAcquired=true;
//			Debug.Log("Traditional lock");
//		}
//		else
//		{
//			target = targetPosition;
//			target.y = 0;
//			isTargetAcquired = false;
//			//body.velocity = thisTransform.forward * speed * 1.6f;
//		}
		
	}
	
	void FixedUpdate ()
	{
		if(!isActive)
			return;
		
		float dt = Time.deltaTime;
		timeSpent += dt;

		if (timeSpent > lifeTime) 
		{
			Explode(trans.position);
			GetComponent<Renderer>().enabled = false;
			Invoke("ResetMissile",2);
		}

		missileSpeed = Mathf.Max(drivespeed * speedCurve.Evaluate(timeSpent), carSpeed);
		
		if(isTargetAcquired)
		{
			target = targetTransform.position;
			target.y += 0.75f;
		}
		
		damping= Mathf.Lerp(damping, 10, dt * 2);
		if(rotationBuffer > 0)
		{
			rotationBuffer -= dt;
		}
		else
		{
       	 	Quaternion rotation = Quaternion.LookRotation(target - trans.position);
			if(rotation.y < -0.7f || rotation.y > 0.7f) // target missed
			{
				isTargetAcquired = false;
				target = trans.position + trans.forward * missileSpeed;
			}
        	trans.rotation = Quaternion.Slerp(trans.rotation, rotation, dt * damping);
		}
				
		trans.Translate(Vector3.forward * dt * missileSpeed);
	}

	void OnTriggerEnter(Collider other)
	{
		LayerMask _layerMask = new LayerMask();
		_layerMask.value = layerMask;

		if (_layerMask != (_layerMask | (1 << other.gameObject.layer))) 
			return;
		
		Vector3 position = trans.position;
		Explode(position);

		Collider[] colliders = Physics.OverlapSphere(position, radius, layerMask);
		for(int i = 0; i < colliders.Length; ++i)
		{
			
			float proximity = (position - colliders[i].transform.position).magnitude;
			float effect = 1 - (proximity/radius);
			GameObject obj = colliders[i].gameObject;
			if(obj.layer == 11 || obj.layer == 12)
			{
				obj.GetComponent<CarCollider>().GetCarBase().ApplyDamage(damage * effect);
			}
			else if(obj.layer == 21)
			{
				obj.GetComponent<TrafficCollider>().GetTrafficCar().ApplyDamage(damage * effect, isPlayerCar);
				//						parentTrans.GetComponent<Rigidbody>().AddExplosionForce(1000000, position, 10, -10, ForceMode.Impulse);
			}
			else if(obj.layer == 10)
			{
				if(obj.GetComponent<Building>())
				{
					obj.GetComponent<Building>().ApplyDamage(damage);
				}
				else
				{
					bool isClassFound = false;
					Transform parentTrans = obj.transform;
					while(!isClassFound)
					{
						parentTrans  = parentTrans.parent;

						if(parentTrans == null)
						{
							Debug.LogError("building class not found in parent");
							isClassFound = true;
							break;
						}
						else if(parentTrans.GetComponent<Building>())
						{
							parentTrans.GetComponent<Building>().ApplyDamage(damage);
							isClassFound = true;
							break;
						}
					}
				}
			}
			else if(obj.layer == 8)
			{
				if(obj.GetComponent<Explosives>() != null)
					obj.GetComponent<Explosives>().ApplyDamage(damage);
			}
		}
		GetComponent<Renderer>().enabled = false;
		Invoke("ResetMissile",2);
	}

	void ResetMissile()
	{
		GetComponent<Renderer>().enabled = true;
		_pool.ReleaseInstance(trans);
		
	}

	void Explode(Vector3 position)
	{
		isActive = false;
		ParticleManager.instance.PlaySmallExplosionAtPoint(position);
		AudioManager.instance.PlayBlastSoundAtPos(position);
	}
}
