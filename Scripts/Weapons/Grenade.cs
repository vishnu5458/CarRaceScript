using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour 
{
	public float speed = 80;
	public float lifeTime = 0.5f;
	public float damage = .1f;
//	public float dist = 20;
//	public float pubVar1 = 2;
//	public float pubVar2 = 2;

//	public float firingAngle = 20;

	private float life = 0.0f;
	[HideInInspector]
	public Transform trans;

	private bool isPlayerCar = false;
	private int layerMask;
//	private float Vx, Vy;
//	private float elapse_time = 0;
	private Rigidbody body;
	private GameObjectPool _pool;
//	private GrenadeLauncher launcher; 

	void Awake()
	{
		trans = transform;
		body = GetComponent<Rigidbody>();
	}

	void OnPoolCreate(GameObjectPool pool)
	{
		_pool = pool;
	}

	public void Fire(float _carSpeedVector, int _layerMask, bool _isPlayer, float _damage)
	{
		isPlayerCar = _isPlayer;
		layerMask = _layerMask;
		damage = _damage;
		life = 0;
		body.velocity = (trans.forward * (_carSpeedVector + speed));
//		launcher = _launcher;

	}

	void ProjectileMotion(float _carSpeedVector)
	{
//		Vector3 targetPos = trans.position + (trans.forward * dist);
//
//		// Calculate distance to target
//		float target_Distance = Vector3.Distance(trans.position, targetPos);
//
//		// Calculate the velocity needed to throw the object to the target at specified angle.
//		float targetTime = Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / (pubVar2 * 9.8f);
//
//		float projectile_Velocity = target_Distance / targetTime;
//
//		// Extract the X  Y componenent of the velocity
//		Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad) + (0.5f * _carSpeedVector);
//		Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
//
//		elapse_time = 0;
	}

//	IEnumerator SimulateProjectile()
//	{
//
//		Vector3 targetPos = launcher.transform.position + (launcher.transform.forward * 30);
//
//		// Calculate distance to target
//		float target_Distance = Vector3.Distance(trans.position, targetPos);
//		Debug.Log ("project velocity " + target_Distance / Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / 9.8f);
//		
//		// Calculate the velocity needed to throw the object to the target at specified angle.
//		float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / 9.8f);
//
//		// Extract the X  Y componenent of the velocity
//		float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
//		float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
//
//		// Calculate flight time.
//		float flightDuration = target_Distance / Vx;
//		float elapse_time = 0;
//
//		while (elapse_time < flightDuration)
//		{
//			trans.Translate(0, (Vy - (9.8f * elapse_time)) * Time.deltaTime, (Vx + launcher.linkedCar.speed) * Time.deltaTime);
//
//			elapse_time += Time.deltaTime;
//
//			yield return null;
//		}
//	}  

	void FixedUpdate () 
	{
		//		return;

		float dt = Time.deltaTime;
		life += dt;

		if (life > lifeTime) 
		{
			Explode(trans.position);
			_pool.ReleaseInstance(trans);
		}

//		body.AddForce(0, Mathf.Lerp(-0.5f, -2.5f, speed * Time.deltaTime), 0);
		body.AddForce(0, -9.8f * life, 0);
//		trans.Translate(0, (-9.8f * life), 0);

//		trans.Translate(0, (Vy - (9.8f * elapse_time)) * pubVar1 * dt, (Vx) * pubVar1 * dt);
//		elapse_time += dt;

//		lerpPos = Mathf.Clamp(lerpPos + Time.deltaTime * speed, 0.0, 1.0); //lerp values need to be between 0 and 1
//		transform.position = Vector3.Lerp(startPos, target.position, lerpPos);
//		transform.position.y = 10 * Mathf.Sin(lerpPos * Mathf.Pi); //multiplying by pi gives it a cycle of 2 - we'll only use half a cycle
	}

	void OnTriggerEnter(Collider other)
	{
		LayerMask _layerMask = new LayerMask();
		_layerMask.value = layerMask;
//		if(other.gameObject.layer !)
//		Debug.Log(layerMask);
		if (_layerMask != (_layerMask | (1 << other.gameObject.layer))) 
			return;


		Vector3 position = trans.position;
		Explode(position);

		Collider[] colliders = Physics.OverlapSphere(position, 15, layerMask);
		for(int i = 0; i < colliders.Length; ++i)
		{
			float proximity = (position - colliders[i].transform.position).magnitude;
			float effect = 1 - (proximity/15);
			GameObject obj = colliders[i].gameObject;
			Debug.Log("hit :  "+ obj.transform.name);
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
		_pool.ReleaseInstance(trans);
	}

	void Explode(Vector3 position)
	{
		ParticleManager.instance.PlaySmallExplosionAtPoint(position);
		AudioManager.instance.PlayBlastSoundAtPos(position);
	}
}
