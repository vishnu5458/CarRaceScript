using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour 
{
	public float lifeTime = 5.0f;
	[HideInInspector]
	public Transform trans;

	private bool isPlayerCar = false;
	private int layerMask;
	private float damage = 20.0f;
	private float life = 0.0f;
	private Renderer litUp;
	private Rigidbody body;
	private GameObjectPool _pool;
	//	private GrenadeLauncher launcher; 

	void Awake()
	{
		trans = transform;
		body = GetComponent<Rigidbody>();
		litUp = trans.GetChild(0).GetComponent<Renderer>();
	}

	void OnPoolCreate(GameObjectPool pool)
	{
		_pool = pool;
	}

	void OnPoolRelease()
	{
		body.isKinematic = true;
	}

	void FixedUpdate () 
	{
		float dt = Time.deltaTime;
		life += dt;
		if(Time.frameCount % 45 == 0)
		{
			litUp.enabled = !litUp.enabled;
		}

		if (life > lifeTime) 
		{
			Explode(trans.position);
			_pool.ReleaseInstance(trans);
		}
	}

	public void Fire(int _layerMask, bool _isPlayer, float _damage)
	{
		isPlayerCar = _isPlayer;
		layerMask = _layerMask;
		damage = _damage;
		life = 0;
		body.isKinematic = false;
		litUp.enabled = false;
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
