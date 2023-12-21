using UnityEngine;
using System.Collections;

public class Explosives : MonoBehaviour 
{
	public float life = 30;
	public float damage = 50;
	public float explosiveRadius = 10;
	public LayerMask targetLayer;

	// Use this for initialization
	void Start () {
	
	}



	public void ApplyDamage(float _hitDamage)
	{
		life -= _hitDamage;
		if(life < 0)
		{

			Vector3 position = transform.position;
			Explode(position);
			bool isBuildingHit = false;
			Collider[] colliders = Physics.OverlapSphere(position, explosiveRadius, targetLayer);
			for(int i = 0; i < colliders.Length; ++i)
			{
				float proximity = (position - colliders[i].transform.position).magnitude;
				float effect = 1 - (proximity/explosiveRadius);
				GameObject obj = colliders[i].gameObject;
				Debug.Log("hit :  "+ obj.transform.name);
				if(obj.layer == 11 || obj.layer == 12)
				{
					obj.GetComponent<CarCollider>().GetCarBase().ApplyDamage(damage * effect);
				}
				else if(obj.layer == 10)
				{
					if(isBuildingHit)
						continue;
					isBuildingHit = true;

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
								Debug.LogError("traffic car not found in parent");
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
			}

			// renderer changes
		}
	}

	void Explode(Vector3 position)
	{
		ParticleManager.instance.PlaySmallExplosionAtPoint(position);
		AudioManager.instance.PlayBlastSoundAtPos(position);

		gameObject.SetActive(false);
	}
}
