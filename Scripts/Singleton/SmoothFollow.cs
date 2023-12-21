using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
	/*
	This camera smoothes out rotation around the y-axis and height.
	Horizontal Distance to the target is always fixed.
	
	There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.
	
	For every of those smoothed values we calculate the wanted value and the current value.
	Then we smooth it using the Lerp function.
	Then we apply the smoothed values to the transform's position.
	*/
	//	private bool isNitroEnable = false;
	private float initCamDist;
	[SerializeField]private float targetDist;

	private Camera thisCam;
//    private VehicleInput currentBike;
	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 15.0f;
	// Nitro dist Inc
	public float nitroDist;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	public float turnFromInput = 0.0f;
	
	//	public float horisObl;
	//	public float verticalObl;
	[HideInInspector]
	public Rigidbody body;
	[HideInInspector]
	public Transform trans;

	public Transform particleHolder;
	public float particleMinY = 3;
	public float particleMinZ = 25;
	public float particleZVeloMul = 1.12f;
	public float particleYVeloMul = 0.1f;

	//	public iTweenEvent cameraShake;
	//	public iTweenEvent nitroShake;

	#region Singleton

	private static SmoothFollow _instance;

	public static SmoothFollow instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<SmoothFollow>();
			}
			return _instance;
		}
	}

	#endregion

	public Camera GetSmoothFollowCam()
	{
		if (thisCam == null)
			thisCam = GetComponent<Camera>();
		return thisCam;
	}

	void Awake()
	{
		_instance = this;
		trans = transform;
		if (target != null)
		{
			body = target.GetComponent<Rigidbody>();
			if (body == null)
			{
				body = target.parent.GetComponent<Rigidbody>();
			}
		}
//		nitroShake = GetComponent<iTweenEvent>();
//		SetObliqueness(0,-0.4f);
		targetDist = distance;

	}

	void SetObliqueness(float horizObl, float vertObl)
	{
		Matrix4x4 mat = GetComponent<Camera>().projectionMatrix;
		mat[0, 2] = horizObl;
		mat[1, 2] = vertObl;
		GetComponent<Camera>().projectionMatrix = mat;
	}

	void LateUpdate()
	{
		// Early out if we don't have a target
		if (!target)
			return;

		float mag = Mathf.Abs(body.velocity.magnitude);
		Vector3 particlePos = Vector3.zero;

		particlePos.y = particleMinY + mag * particleYVeloMul;
		particlePos.z = particleMinZ + mag * particleZVeloMul;

//		particleHolder.localPosition = particlePos;
//        Debug.Log("Mathf.Abs(distance - targetDist)::"+Mathf.Abs(distance - targetDist));
		if (Mathf.Abs(distance - targetDist) > 0.1f)
		{
            
			distance = Mathf.Lerp(distance, targetDist, 3f * Time.deltaTime);
		}

		float steer;
		#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WEBGL	
		steer = Input.GetAxis("Horizontal");
		#else
		steer = Input.acceleration.x;
		if(Screen.orientation == ScreenOrientation.LandscapeRight)
		{
			steer = -steer;
		}
		#endif

//        thisCam.fieldOfView = thisCam.fieldOfView + currentBike.Accell * 20f * Time.deltaTime;
		if (thisCam.fieldOfView > 85)
		{
			thisCam.fieldOfView = 85;
		}
		if (thisCam.fieldOfView < 50)
		{
			thisCam.fieldOfView = 50;
		}
		if (thisCam.fieldOfView < 60)
		{
			thisCam.fieldOfView = thisCam.fieldOfView += 10f * Time.deltaTime;
		}
		if (thisCam.fieldOfView > 60)
		{
			thisCam.fieldOfView = thisCam.fieldOfView -= 10f * Time.deltaTime;
		}

		// Calculate the current rotation angles
		float wantedRotationAngle;
		if (body.velocity.magnitude < 0.01)
		{
			wantedRotationAngle = trans.eulerAngles.y;
		}
		else
		{
			wantedRotationAngle = Quaternion.LookRotation(body.velocity + trans.forward).eulerAngles.y;

			//turn from input
			wantedRotationAngle += steer * turnFromInput;
		}
		
			
		float currentRotationAngle = trans.eulerAngles.y;
		float currentHeight = trans.position.y;
	
		float wantedHeight = target.position.y + height;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
	
		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
	
		// Convert the angle into a rotation
		// The quaternion interface uses radians not degrees so we need to convert from degrees to radians
		Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		Vector3 pos = target.position;
		pos -= currentRotation * Vector3.forward * distance;
		pos.y = currentHeight;
		trans.position = pos;

		// Always look at the target
		trans.LookAt(target.position);
#if UNITY_ANDROID || UNITY_IPHONE
		Vector3 lea = trans.localEulerAngles; 
		lea.z = steer * 10;
		trans.localEulerAngles = lea;
#endif
	}

	public void InitCamera(float _distance, float _height, float _nitroDist, Rigidbody _body)
	{
		distance = targetDist = initCamDist = _distance;
		height = _height;
		nitroDist = _nitroDist;

		body = _body;
//        currentBike = MainDirector.GetCurrentPlayer().GetComponent<VehicleInput>();
	}

	public void OnEnableNitro(int status)
	{
//		StartCoroutine(LerpCamera(nitroDist));
        targetDist = initCamDist + (status * nitroDist);
	}

	public void OnDisableNitro()
	{
//		StartCoroutine(LerpCamera(initCamDist));
		targetDist = initCamDist;
	}
}
