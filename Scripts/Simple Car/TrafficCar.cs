using UnityEngine;
using System.Collections;

public class TrafficCar : MonoBehaviour 
{

	[HideInInspector]
	public Transform trans;
	[HideInInspector]
	public Vector3 trafficWPtarget;
//    [HideInInspector]
//    public Vector3 distWPTarget;
	[HideInInspector]
	public Rigidbody body;

	public float carLife = 100;
	public TrafficDirection thisDirection;
	public bool isCarStopped = false;
	//	public bool isMoveEnabled = false;
	public bool isCarEnabled = false;
	public float oADistance = 8;
	
	public float carSpeed = 5;
	public Renderer mainBody;
	public Texture damagedTextures;
	public Transform[] wheelModels;
	
	public LayerMask visibleLayers = -1;
	public Transform center;
	public Transform left;
	public Transform leftEnd;
	public Transform right;
	public Transform rightEnd;
	
//	public DistWP currentDistWaypoint;
    public TrafficWP currTrafficWP;


	private int targetFrame;
	private int lowSpeedTimeout = 500;
	private float topSpeed = 50;
	private float spinAngle;
	private float spinAngleMultiplier = 100; 
	private float offset;

	private float initLife;
	private float initSpeed;
	private Texture initTexture;
	[SerializeField]
	private float speed;
	private float magnitude;
	private Quaternion rotation;
	private SimpleCarAudio thisCarAudio;
	private GameObjectPool _pool;

	void Awake()
	{
		trans = transform;
		initLife = carLife;
		initTexture = mainBody.material.mainTexture;
		body = GetComponent<Rigidbody>();
//		thisCarController = GetComponent<SimpleCarController>();
		thisCarAudio = GetComponent<SimpleCarAudio>();
		leftEnd.transform.position = left.transform.position + left.transform.TransformDirection(Vector3.forward * oADistance);
		rightEnd.transform.position = right.transform.position + right.transform.TransformDirection(Vector3.forward * oADistance);
//		objHeight = GetComponentInChildren<BoxCollider>().size.y/2;
	}

	void Start()
	{
//		InitWayPoints(currentWaypoint, TrafficDirection.RHS);
//		EnableCar();
	}

	void OnPoolCreate(GameObjectPool pool)
	{
		_pool = pool;
	}

    public bool InitWayPoints(TrafficWP startWp, TrafficDirection _currDirection)
	{
        currTrafficWP = startWp;
		thisDirection = _currDirection;
//        currTrafficWP = TrackManager.instance.GetCloseTrafficWP(startWp);

		if(thisDirection == TrafficDirection.LHS)
			offset = -3.0f;
		else
			offset = 3.0f;

//		Debug.Log(testTrans.position + (testTrans.right * 3));
        Transform wpTrans = currTrafficWP.thisTransform;
		Vector3 pos = wpTrans.position + (wpTrans.right * offset) + (wpTrans.forward * -offset);

		pos.y += 2.5f; 

		trans.position = pos;

		if(thisDirection == TrafficDirection.LHS)
		{
//			currentDistWaypoint = currentDistWaypoint.prevDistWaypoint;
            currTrafficWP = currTrafficWP.prevWaypoint;
		}
		else
		{
//			currentDistWaypoint = currentDistWaypoint.nextDistWaypoint;
            currTrafficWP = currTrafficWP.nextWaypoint;
		}		

        if(currTrafficWP == null)
			return false;

        pos = currTrafficWP.thisTransform.position + (currTrafficWP.thisTransform.right * offset);
		pos.y = trans.position.y;
		trans.LookAt(pos);

        wpTrans = currTrafficWP.thisTransform;
		trafficWPtarget = new Vector3(wpTrans.position.x, pos.y, wpTrans.position.z);
		trafficWPtarget += wpTrans.right * offset;

//        distWPTarget = currentDistWaypoint.thisTransform.position;

		rotation = Quaternion.LookRotation(trafficWPtarget - trans.position);
		initSpeed = carSpeed;
		body.isKinematic = false;

		return true;
	}

	public void EnableCar()
	{
		if(isCarEnabled)
			return;
		
		carLife = initLife;
		targetFrame = Random.Range(0, 8);
		isCarEnabled = true;
		
		if(GlobalClass.SoundVoulme == 1)
			SetSound(true);
		else
			SetSound(false);

		bool frontContact = false;
		RaycastHit hitFrontLeft;
		RaycastHit hitFrontRight;
		
		
		if (Physics.Linecast(left.position, leftEnd.position, out hitFrontLeft, visibleLayers))
		{
			frontContact = true;
		}
		
		if (Physics.Linecast(right.position, rightEnd.position, out hitFrontRight, visibleLayers))
		{
			frontContact = true;
		}
		
		if(frontContact)
		{
			if(!mainBody.isVisible)
				OnCarReset();
		}

//		body.isKinematic = false;
//		if(gameObject.activeSelf)
//			StartCoroutine(CheckForResetCar());
//		else
//			OnCarReset();
	}

	public void SetSound(bool status)
	{
		if(status)
			thisCarAudio.StartSound();
		else
			thisCarAudio.StopSound();
	}

	void FixedUpdate() 
	{
		if(!isCarEnabled || Time.timeScale == 0)
			return;	
		
//        Vector3 RelativePosition = trans.InverseTransformPoint(distWPTarget);
//		magnitude = RelativePosition.z;  // + .01f instead of magnitude
//
//		if (magnitude < 5)  //40
//		{
//            DistWP closeDistWP = currentDistWaypoint;
//			if(thisDirection == TrafficDirection.LHS)
//				currentDistWaypoint = closeDistWP.prevDistWaypoint;	
//			else
//				currentDistWaypoint = closeDistWP.nextDistWaypoint;	
//
//			if(currentDistWaypoint == null)
//			{
//				OnCarReset();
//				return;
//			}
//
//            distWPTarget = currentDistWaypoint.thisTransform.position;
//		}

        Vector3 RelativePosition = trans.InverseTransformPoint(trafficWPtarget);
        magnitude = RelativePosition.z;  // + .01f instead of magnitude

        if (magnitude < 5)  //40
        {
            TrafficWP closeWaypoint = currTrafficWP;
            if(thisDirection == TrafficDirection.LHS)
                currTrafficWP = closeWaypoint.prevWaypoint;   
            else
                currTrafficWP = closeWaypoint.nextWaypoint;   

            if(currTrafficWP == null)
            {
                OnCarReset();
                return;
            }
            Transform wpTrans = currTrafficWP.thisTransform;
            trafficWPtarget = new Vector3( wpTrans.position.x, trans.position.y, wpTrans.position.z);
            trafficWPtarget += wpTrans.right * offset;
            rotation = Quaternion.LookRotation(trafficWPtarget - trans.position);

        }

		if(speed < topSpeed)
		{
			if(speed < -0.05f)
			{
				isCarStopped = true;
				body.AddForce(Vector3.zero, ForceMode.VelocityChange);
			}
			else
			{
				if(!isCarStopped)
				{
					Vector3 forceDir = trans.TransformDirection(Vector3.forward);
					body.AddForce(forceDir * carSpeed, ForceMode.Impulse);
				}
			}
		}
		else
		{
//			Debug.Log("speed limit");
		}
		trans.rotation = Quaternion.Slerp(trans.rotation, rotation, Time.deltaTime * (speed / 2));
//		trans.forward = Vector3.Lerp(trans.forward, currentWaypoint.thisTransform.forward, Time.deltaTime);

		if(RelativePosition.z < 0) // is facing the wrong direction   !thisCarController.Immobilized && 
		{
			Debug.Log("reset on relative : " + trans.position);
//			Debug.Break();
			if(!mainBody.isVisible)
				OnCarReset();
		}

		if(speed < 2 )
		{
			lowSpeedTimeout--;
			if(lowSpeedTimeout < 0)
			{
//				Debug.Log("low speed time out");
				if(!mainBody.isVisible)
					OnCarReset();
			}
		}
		else
		{
			lowSpeedTimeout = 500;
		}

		if(trans.position.y < -30)
			OnCarReset();

		if(targetFrame % 90 == 0)
		{
            // (currTrafficWP.id * 2) this is assuming there are twice as many distWP as there are trafficWP
			if((currTrafficWP.id * 2) + 10 < MainDirector.GetCurrentPlayer().GetCurrentDistWPID())
			{
				if(!mainBody.isVisible)
					OnCarReset();
			}
		}
		speed = trans.InverseTransformDirection (body.velocity).z;
		UpdateWheels();
		UpdateSteering();
		
//		thisCarController.Move(steer, acce);
	}
	
	void UpdateWheels ()
	{
		spinAngle += speed * spinAngleMultiplier * Time.deltaTime;
		Quaternion wheelRotation = Quaternion.AngleAxis(spinAngle, Vector3.right);
		foreach(Transform wheelTrans in wheelModels)
		{
			wheelTrans.localRotation = wheelRotation;
//			wheelTrans.localRotation = Quaternion.Euler(spinAngle, 0, 0);
		}
	} 
	
	void UpdateSteering ()
	{
		targetFrame ++;
		if(targetFrame % 8 == 0)
		{

			RaycastHit hitFrontLeft;
			RaycastHit hitFrontRight;
		
			bool leftContact = Physics.Linecast(left.position, leftEnd.position, out hitFrontLeft, visibleLayers);
			bool rightContact = Physics.Linecast(right.position, rightEnd.position, out hitFrontRight, visibleLayers);

//			if (Physics.Linecast(left.position, leftEnd.position, out hitFrontLeft, visibleLayers))
//			{
//				leftContact = true;
//			}
//			if (Physics.Linecast(right.position, rightEnd.position, out hitFrontRight, visibleLayers))
//			{
//				rightContact = true;
//			}

			if(leftContact && rightContact)
			{
				carSpeed = -initSpeed;
			}
			else if(leftContact && !rightContact)
			{
				trans.Rotate(Vector3.up, 1);
			}
			else if(rightContact && !leftContact)
			{
				trans.Rotate(Vector3.up, -1);
			}
			else
			{
				isCarStopped = false;
				carSpeed = initSpeed;
			}

		}

	}

	public void DisableCar()
	{
		isCarEnabled = false;
//		thisCarController.isCarEnabled = false;
	}

	public void OnCarReset()
	{
		mainBody.material.mainTexture = initTexture;
		foreach(Transform childTrans in mainBody.transform)
			childTrans.gameObject.SetActive(true);

		isCarEnabled = false;
//		StopAllCoroutines();
		TrafficManager.instance.ReleaseTrafficCar(this);
		_pool.ReleaseInstance(trans);
	}

	public void ApplyHandBrake()
	{
//		thisCarController.ApplyHandBrake();
//		thisCarController.Immobilize();
	}

	public void ApplyDamage(float damage, bool isPlayer)
	{
		carLife -= damage;
		if(carLife < 0)
		{
			ParticleManager.instance.PlaySmallExplosionAtPoint(trans.position);
			mainBody.material.mainTexture = damagedTextures;
			foreach(Transform childTrans in mainBody.transform)
				childTrans.gameObject.SetActive(false);
			if(isPlayer)
			{
				Debug.Log("player destroyed traffic car ");
				MainDirector.GetCurrentGamePlay().TrafficCarKilled += 1;
			}
			isCarEnabled = false;
		}
	}

	void OnCollisionEnter(Collision other)
	{
//		if(other.gameObject.name.CompareTo("Track") != 0 && other.gameObject.name.CompareTo("Barricade") != 0)
//			Debug.Log(other.gameObject.name + "  " + trans.position);
	}
	
	public void ReleaseHandBrake()
	{
//		thisCarController.ReleaseHandBrake();
//		thisCarController.Reset();
	}
	
	public void OnDrawGizmos()
    {
	
		Vector3 forwardDirection = center.TransformDirection(Vector3.forward * oADistance);
        Debug.DrawRay(left.position, forwardDirection, Color.green);
        Debug.DrawRay(right.position, forwardDirection, Color.green);
	}
	
}
