using UnityEngine;
using System.Collections;

public class GuiMove : MonoBehaviour 
{
	public Vector3 targetPos;
	public Vector3 exitPos;
	public bool ignoreTimeScale = true;
	public bool isLocal = true;
	public float tweenDuration = 0.5f;

	private Vector3 initPos; 
	private Transform trans;

	// Use this for initialization
	void Awake ()
	{
		trans = transform;
		if(isLocal)
			initPos = trans.localPosition;
		else
			initPos = trans.position;
	}
	
	public void InitialseCompement(bool shouldTween = false)
	{	
		if(!gameObject.activeSelf)
			return;

		if(isLocal)
			trans.localPosition = initPos;
		else
			trans.position = initPos;

		MoveToPos(shouldTween, targetPos);
	}

	public void Reset(bool shouldTween = false)
	{
		if(!gameObject.activeSelf)
			return;

		if(exitPos == Vector3.zero)
			MoveToPos(shouldTween, initPos);
		else
			MoveToPos(shouldTween, exitPos);
	}

	void MoveToPos(bool shouldTween, Vector3 pos)
	{
		if(shouldTween)
		{
			Hashtable ht = new Hashtable();
			ht.Add("islocal", isLocal);
			ht.Add("position", pos);
			//		ht.Add("y", targetPos.y);
			//		ht.Add("z", targetPos.y);
			ht.Add("time", tweenDuration);
			ht.Add("ignoretimescale", ignoreTimeScale);
			iTween.MoveTo(gameObject, ht);
		}
		else
		{
			if(isLocal)
				trans.localPosition = pos;
			else
				trans.position = pos;
		}
	}
	

}
