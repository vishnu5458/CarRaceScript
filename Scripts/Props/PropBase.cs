using UnityEngine;
using System.Collections;

public class PropBase : MonoBehaviour
{
		
	[HideInInspector]
	public Transform thisTransform;

	Vector3 initialPosition;
	Vector3 initialRotation;

	public bool isActivated = false;

	// place particles according to presets
	// add glow emitter
	// on trigger enter do the functionality
	void Awake () 
	{
		thisTransform = transform;
		initialPosition = thisTransform.position;
		initialRotation = thisTransform.localEulerAngles;
	}
	
	protected virtual void Start()
	{
	}
	
	public virtual void InitProp()
	{
		if(!isActivated)
			return;

		isActivated = false;
		thisTransform.position = initialPosition;
		thisTransform.localEulerAngles = initialRotation;

	}

	public virtual void ActivateCollectable(Vector3 targetPos)
	{
		isActivated = true;
	}

	public virtual void ActivateCollectable()
	{
		isActivated = true;
	}

	public virtual void SleepRigidbody()
	{

	}

}

