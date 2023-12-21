using UnityEngine;
using System.Collections;
[RequireComponent (typeof (BoxCollider))]
public class GUIButton : GUIBase 
{
	public bool isRollOverButton = true;
//	public Sprite rollOverSprite;

//	private Sprite initSprite;
//	private SpriteRenderer spriteRenderer;
	private Vector3 initButtonScale;

	protected override void Start ()
	{
		base.Start ();
		initButtonScale = trans.localScale;
//		if(isRollOverButton)
//		{
//			if(spriteRenderer == null)
//				spriteRenderer = GetComponent<SpriteRenderer>();
//			initSprite = spriteRenderer.sprite;
//		}
	} 

	public virtual void OnRollOver()
	{
		//		Debug.Log("on rollover");
		if(isRollOverButton)
		{
			trans.localScale = initButtonScale * 1.1f;
			AudioManager.instance.PlayRollOverSound();
//			spriteRenderer.sprite = rollOverSprite;
		}
	}
	
	public virtual void OnRollOff()
	{
		//		Debug.Log("on rolloff");
		if(isRollOverButton)
		{
			trans.localScale = initButtonScale;
		}
	}

	public void OnTouchDown()
	{
		trans.localScale = initButtonScale * .95f;
	}

	public void OnTouchRelease()
	{
		AudioManager.instance.PlayMouseClickSound();
		trans.localScale = initButtonScale;
		if(isRollOverButton)
		{
			OnRollOff();
		}
	}

}
