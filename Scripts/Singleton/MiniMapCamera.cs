using UnityEngine;
using System.Collections;

public class MiniMapCamera : MonoBehaviour 
{
	bool isCamEnabled = false;
	Transform trans;
	Camera miniMapCam;
//	Rigidbody body;

	public float minSize = 30;
	public float maxSize = 110;
	public Transform target;

	public static MiniMapCamera miniMapCamera;
	
	void Start () 
	{
		miniMapCamera = this;

		trans = transform;
		miniMapCam = GetComponent<Camera>();
	}

	public void Init(Transform _target)
	{
		target = _target;
//		body = _body;
	}

	public void EnableCamera()
	{
		miniMapCam.enabled = true;
		isCamEnabled = true;
	}

	public void DisableCamera()
	{
		miniMapCam.enabled = false;
		isCamEnabled = false;
	}

	public void Dealloc()
	{
		target = null;
//		body = null;
	}

	void Update () 
	{
		if(!target || !isCamEnabled)
			return;

		Vector3 playerPos = target.position;
		playerPos.y = 50;
		trans.position = playerPos;//Vector3.Lerp(trans.position, playerPos, lerpRate * Time.fixedDeltaTime);
		trans.eulerAngles = new Vector3(90, target.eulerAngles.y, 0);

//		if(!body)
//			return;
//
//		float velo = Mathf.Clamp(body.velocity.magnitude * 0.8f, minSize, maxSize);
//		miniMapCam.orthographicSize = velo;

	}

	
}
