using UnityEngine;
using System.Collections;

public enum ObstacleType{
	OilDrum,
	DynamicBody,
	StaticBody
}

public class Obstacles : PropBase
{
	Rigidbody body;

	public float damage;
	public ObstacleType obsTyp;

	protected override void Start ()
	{
		if(obsTyp == ObstacleType.DynamicBody || obsTyp == ObstacleType.OilDrum)
			body = GetComponent<Rigidbody>();

		base.Start ();
	}

	public override void InitProp ()
	{
		if(!isActivated)
			return;

		base.InitProp ();

		if(body)
		{
			body.Sleep();
		}
	}

	public override void ActivateCollectable ()
	{
		if(isActivated)
			return;

		base.ActivateCollectable ();
	}

	public override void SleepRigidbody ()
	{
		if(body)
			body.Sleep();
		base.SleepRigidbody ();
	}
}
