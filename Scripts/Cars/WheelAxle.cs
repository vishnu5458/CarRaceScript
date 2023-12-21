using UnityEngine;
using System.Collections;

public class WheelAxle : MonoBehaviour 
{
	private Transform thisTrans;
	public Transform wheelTransform;
	// Use this for initialization
	void Start () 
	{
		thisTrans = transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		thisTrans.LookAt(wheelTransform);
	}
}
