using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LineRenderer))]
public class LaserScript : MonoBehaviour 
{
	public Transform startPoint;
//	public float dist;
	public Vector3 endPoint;

	private bool isActive = false;
	[SerializeField] private float laserTime = 15;
	private float initLaserTime;
	private LineRenderer laserLine;

	// Use this for initialization
	void Start ()
	{
		initLaserTime = laserTime;
		laserLine = GetComponentInChildren<LineRenderer> ();
		laserLine.SetWidth (.2f, .2f);
	}

	// Update is called once per frame
	void Update () 
	{
		if(!isActive)
			return;

		laserTime --;
		if(laserTime < 0)
		{
			isActive = false;
			laserLine.enabled = false;
		}
		
		laserLine.SetPosition (0, startPoint.position);
		laserLine.SetPosition (1, endPoint);

	}

	public void ShowLaserEffect(Transform _startTrans, Vector3 _endPoint)
	{
		isActive = true;
		laserTime = initLaserTime;
		laserLine.enabled = true;

		startPoint = _startTrans;
		endPoint = _endPoint;
//		dist = _dist;
	}
}
