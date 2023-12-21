using UnityEngine;
using System.Collections;

public class SmoothLookAtC : MonoBehaviour {

	public Transform target;
	public float damping = 6.0f;
	public bool smooth = true;

	public bool isEnabled = false;

	Transform trans;
	GameObject go;

	public void EnablelookAt()
	{
		GetComponent<Camera>().enabled = true;
		isEnabled = true;
	}

	public void DisablelookAt()
	{
		GetComponent<Camera>().enabled = false;
		isEnabled = false;
	}

	public void LookAt(Transform _target, Vector3 targetPos)
	{
		target = _target;
		iTween.MoveTo(go, targetPos, 0.5f);
	}

	// Use this for initialization
	void Start () 
	{
		if(GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;

		trans = transform;
		go = gameObject;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(!target || !isEnabled)
			return;
		if(smooth)
		{
			Quaternion rotation = Quaternion.LookRotation(target.position - trans.position);
			trans.rotation = Quaternion.Slerp(trans.rotation, rotation, Time.deltaTime * damping);
		}
		else
		{
			trans.LookAt(target);
		}
	}
}