using UnityEngine;
using System.Collections;

public class MissileBase : MonoBehaviour 
{
	[HideInInspector]
	public Transform trans;
	
	public float radius = 10;
  	protected Rigidbody body;
	protected float damage = .2f;
	protected AudioSource thisAudio;
	const int layerMask = 1 << 10 | 1 << 15;
	
	void Awake()
	{
		trans = transform;
		body = GetComponent<Rigidbody>();
	}
	
	public virtual void Fire(float speed, float missileOffset)
	{
		
	}
	
	protected virtual void OnTriggerEnter(Collider other)
	{
		Vector3 position = trans.position;
		
//		ParticleEmitter explosion = GameManager.gameManager.missileExplosion;
//		explosion.transform.position = position;
//		explosion.minSize = 2;
//		explosion.maxSize = 3;
//		explosion.Emit();
		
		Collider[] colliders = Physics.OverlapSphere(position, radius, layerMask);
		for(int i = 0; i < colliders.Length; ++i)
		{
			GameObject obj = colliders[i].gameObject;
			if(obj.layer==10)
			{
//				ZombieBase zombieBase = (ZombieBase)colliders[i].GetComponent(typeof(ZombieBase));
//				
//				Vector3 velocity = zombieBase.trans.position - position;
//				velocity = velocity.normalized * 15;
//				velocity.y = 10;
//				
//				zombieBase.Damage(.2f, zombieBase.trans.position, velocity, WeaponType.Missile);
			}
			else if(obj.layer==15)
			{
//				oilDrumBase oildrum = (oilDrumBase)colliders[i].GetComponent(typeof(oilDrumBase));
//				oildrum.damage(0.5f,obj.transform.position);
			}

		}
		
//		thisAudio.Stop();
//		AudioController.audioController.Play_MissileExplosion(position);
		
	}
}
