using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public float speed = 80;
	public float lifeTime = 0.5f;
	public float damage = .1f;
	
	private float life = 0.0f;
	[HideInInspector]
	public Transform trans;

	private bool isPlayerCar = false;
	private int layerMask;
	private int radius;
	private Rigidbody body;
	private GameObjectPool _pool;

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
		damage = _damage;
		isPlayerCar = _isPlayer;
		radius = isPlayerCar ? 3 : 2;
		layerMask = _layerMask;
		life = 0;
		body.velocity = (trans.forward * (_carSpeedVector + speed));
	}
	
	void Update () 
	{
//		return;

		float dt = Time.deltaTime;
//		trans.position += trans.forward * speed * dt;
		
		life += dt;

		//		RaycastHit hit;
		//		if(Physics.SphereCast(currFireTrans.position, 3, currFireTrans.forward, out hit, 100, targetLayer))
		//		{
		//			Debug.Log("hit :  "+ hit.transform.name);
		//			RaycastHitTarget(hit.transform);
		//		}
		
		if (life > lifeTime) 
		{
			_pool.ReleaseInstance(trans);
		}
		else
		{
//			Ray ray = new Ray(trans.position, trans.forward);
//			RaycastHit hit;

            Collider[] colliders;
            if((colliders = Physics.OverlapSphere(trans.position, radius, layerMask)).Length > 0)
            {
                foreach(Collider _collider in colliders)
                {
                    GameObject obj = _collider.gameObject;
                    if(obj.layer == 11 || obj.layer == 12)
                    {
                        obj.GetComponent<CarCollider>().GetCarBase().ApplyDamage(damage);
                    }
                    else if(obj.layer == 21)
                    {
                        obj.GetComponent<TrafficCollider>().GetTrafficCar().ApplyDamage(damage, isPlayerCar);
                        AudioManager.instance.PlayBulletHitSound(trans.position);
                    }
                    else if(obj.layer == 8)
                    {
                        if(obj.GetComponent<Explosives>() != null)
                            obj.GetComponent<Explosives>().ApplyDamage(damage);
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
                    AudioManager.instance.PlayBulletHitSound(trans.position);
                    ParticleManager.instance.PlaySmallCollisionSpark(trans.position);
                    _pool.ReleaseInstance(trans);
                }
            }
		

//            if(Physics.SphereCast(ray, radius, out hit, 2, layerMask))//
//			{
//				GameObject obj = hit.collider.gameObject;
////				Debug.Log("hit :  "+ hit.transform.name);
//				if(obj.layer == 11 || obj.layer == 12)
//				{
//					obj.GetComponent<CarCollider>().GetCarBase().ApplyDamage(damage);
//				}
//				else if(obj.layer == 21)
//				{
//					obj.GetComponent<TrafficCollider>().GetTrafficCar().ApplyDamage(damage, isPlayerCar);
//					AudioManager.instance.PlayBulletHitSound(trans.position);
//				}
//				else if(obj.layer == 8)
//				{
//					if(obj.GetComponent<Explosives>() != null)
//						obj.GetComponent<Explosives>().ApplyDamage(damage);
//				}
//				else if(obj.layer == 10)
//				{
//					if(obj.GetComponent<Building>())
//					{
//						obj.GetComponent<Building>().ApplyDamage(damage);
//					}
//					else
//					{
//						bool isClassFound = false;
//						Transform parentTrans = obj.transform;
//						while(!isClassFound)
//						{
//							parentTrans  = parentTrans.parent;
//
//							if(parentTrans == null)
//							{
//								Debug.LogError("building class not found in parent");
//								isClassFound = true;
//								break;
//							}
//							else if(parentTrans.GetComponent<Building>())
//							{
//								parentTrans.GetComponent<Building>().ApplyDamage(damage);
//								isClassFound = true;
//								break;
//							}
//						}
//					}
//				}
//				AudioManager.instance.PlayBulletHitSound(trans.position);
//				ParticleManager.instance.PlaySmallCollisionSpark(trans.position);
//				_pool.ReleaseInstance(trans);
//			}
		}
	}
}
