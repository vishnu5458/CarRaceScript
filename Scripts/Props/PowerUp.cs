using UnityEngine;
using System.Collections;

public enum PowerUpType{Nitro, Stopper, AmmoSmall, AmmoLarge, Health, Coin};

public class PowerUp : PropBase
{
	public string powerUpId;
	public PowerUpType thisType;
	public MeshFilter thisMesh;
	public Renderer thisRenderer;

	protected override void Start ()
	{
//		thisMesh = GetComponent<MeshFilter>();
//		thisRenderer = GetComponent<Renderer>();

		base.Start ();
	}

	public override void InitProp ()
	{
		base.InitProp ();
		transform.localScale = Vector3.one;
	}

//	public void SetPowerUpType()
//	{
//
//		//set random power up from here
//		int powType;// = Random.Range(0, 3);
//		float _powRand = Random.value;
//		if(_powRand < 0.25f)
//		{
//			powType = 2;
//		}
//		else if(_powRand < 0.5f)
//		{
//			powType = 1;
//		}
//		else
//		{
//			powType = 0;
//		}
//
//		thisType = (PowerUpType)powType;
//
//		thisMesh.mesh = PowerupManager.instance.GetPowDefForType(powType).powMesh;
//		thisRenderer.sharedMaterial = PowerupManager.instance.GetPowDefForType(powType).particleMaterial;
//
//		InitProp();
//	}

	public override void ActivateCollectable (Vector3 targetPos)
	{
//		iTween.MoveTo(gameObject, targetPos, 0.5f);
//		iTween.ScaleTo(gameObject, Vector3.zero, 0.5f);

		base.ActivateCollectable (targetPos);
	}
}
