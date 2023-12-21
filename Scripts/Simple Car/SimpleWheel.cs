using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WheelCollider))]
public class SimpleWheel : MonoBehaviour 
{

	public Transform wheelModel;
	public WheelCollider wheelCollider { get; private set; }
	public SimpleCarController car { get; private set; }
	public bool steerable = false;
	public bool powered = false;

	
	
	public float suspensionSpringPos { get; private set; }
	
	private float spinAngle;
	private Vector3 originalWheelModelPosition;
	
	void Start()
	{
		car = transform.parent.GetComponent<SimpleCarController>();
		wheelCollider = GetComponent<WheelCollider>();
		
		if (wheelModel != null)
		{
			originalWheelModelPosition = wheelModel.localPosition;
			transform.position = wheelModel.position;// - wheelCollider.suspensionDistance*0.5f*transform.up;
		}
}
	
	
	// called in sync with the physics system
	void Update()
	{
		if(!car.isCarEnabled)
			return;

		// *6 converts RPM to Degrees per second (i.e. *360 and /60 )
		spinAngle += wheelCollider.rpm * 6 * Time.deltaTime;

		// update wheel model position and rotation
		if (wheelModel != null)
		{
			wheelModel.localPosition = originalWheelModelPosition + Vector3.up * suspensionSpringPos;
			wheelModel.localRotation = Quaternion.AngleAxis(wheelCollider.steerAngle, Vector3.up) * Quaternion.Euler(spinAngle, 0, 0);
		}
	}
}
