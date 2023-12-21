using UnityEngine;
using System.Collections;

public class Coins : PropBase
{

	public override void InitProp ()
	{
		if(!isActivated)
			return;

		base.InitProp ();
		thisTransform.localScale = Vector3.one;
	}
	
	
	public override void ActivateCollectable (Vector3 targetPos)
	{
		if(isActivated)
			return;

		thisTransform.localScale = Vector3.zero;
//		iTween.MoveTo(gameObject,targetPos,0.5f); //from 0.5
//		iTween.ScaleTo(gameObject,Vector3.zero,0.5f); //from 0.5

		//add score here

		base.ActivateCollectable (targetPos);
	}
}
