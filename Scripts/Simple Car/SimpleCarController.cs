using UnityEngine;
using System.Collections;

public class SimpleCarController : MonoBehaviour {


	public bool isCarEnabled = false;
	[SerializeField] private float maxSteerAngle = 28;                              // The maximum angle the car can steer

	[SerializeField] private float maxSpeed = 60;                                   // the maximum speed (in meters per second!)
	[SerializeField] private float maxTorque = 35;                                  // the maximum torque of the engine
	[SerializeField] private float minTorque = 10;                                  // the minimum torque of the engine
//	[SerializeField] private float brakePower = 40;                                 // how powerful the brakes are at stopping the car
	[SerializeField] private float adjustCentreOfMass = 0.25f;                      // vertical offset for the centre of mass
	[SerializeField] private Advanced advanced;                                     // container for the advanced setting which will expose as a foldout in the inspector

	[System.Serializable]
	public class Advanced                                                           // the advanced settings for the car controller
	{
		public int numGears = 5;                                                    // the number of gears
		[Range(0, 1)] public float gearDistributionBias = 0.2f;                     // Controls whether the gears are bunched together towards the lower or higher end of the car's range of speed.
		public float reversingSpeedFactor = 0.3f;                                   // The car's maximum reverse speed, as a proportion of its max forward speed.
		[Range(0,1)]public float revRangeBoundary = 0.8f;                           // The amount of the full rev range used in each gear.
	}
	
	
	private float[] gearDistribution;                                               // Stores the caluclated change point for each gear (0-1 as a normalised amount relative to car's max speed)
	public SimpleWheel[] wheels;                                                   // Stores a reference to each wheel attached to this car.
	private float accelBrake;                                                       // The acceleration or braking input (1 to -1 range)
	private float smallSpeed;                                                       // A small proportion of max speed, used to decide when to start accelerating/braking when transitioning between fwd and reverse motion
	private float maxReversingSpeed;                                                // The maximum reversing speed
	private bool immobilized;                                                       // Whether the car is accepting inputs.
	
	
	// publicly read-only props, useful for GUI, Sound effects, etc.
	public int GearNum; //{ get; private set; }                                        // the current gear we're in.
	public float CurrentSpeed { get; private set; }                                 // the current speed of the car
	public float CurrentSteerAngle{ get; private set; }                             // The current steering angle for steerable wheels.
	public float AccelInput { get; private set; }                                   // the current acceleration input
	public float BrakeInput { get; private set; }                                   // the current brake input
	public float RevsFactor { get; private set; }                                   // value between 0-1 indicating where the current revs fall between 0 and max revs
	public float SpeedFactor { get;  private set; }                                 // value between 0-1 of the car's current speed relative to max speed
	
	public int NumGears {					// the number of gears set up on the car
		get { return advanced.numGears; }
	}						
	
	
	// the following values are provided as read-only properties,
	// and are required by the Wheel script to compute grip, burnout, skidding, etc
	public float MaxSpeed
	{
		get { return maxSpeed; }
	}

	public float MaxTorque
	{
		get { return maxTorque; }
		set { maxTorque = value;}
	}

	public bool Immobilized {
		get {
			return immobilized;
		}
	}

	public float MaxSteerAngle
	{
		get { return maxSteerAngle; }
	}
	
	
	// variables added due to separating out things into functions!
	bool anyOnGround;
	float curvedSpeedFactor;
	bool reversing;
	float targetAccelInput; // target accel input is our desired acceleration input. We smooth towards it later
	
	
	
	
	void Awake ()
	{
		// get a reference to all wheel attached to the car.
		wheels = GetComponentsInChildren<SimpleWheel>();
		
		SetUpGears();
		
		// deactivate and reactivate the gameobject - this is a workaround
		// to a bug where changes to wheelcolliders at runtime are not 'taken'
		// by the rigidbody unless this step is performed :(
		gameObject.SetActive(false);
		gameObject.SetActive(true);
		
		// a few useful speeds are calculated for use later:
		smallSpeed = maxSpeed*0.05f;
		maxReversingSpeed = maxSpeed * advanced.reversingSpeedFactor;
	}
	
	
	void OnEnable()
	{
		// set adjusted centre of mass.
		GetComponent<Rigidbody>().centerOfMass = Vector3.up * adjustCentreOfMass;
	}
	
	public void EnableCar()
	{
		isCarEnabled = true;
	}
	
	public void DisableCar()
	{
		isCarEnabled = false;
	}
	
	public void Move (float steerInput, float accelBrakeInput)
	{
		
		// lose control of engine if immobilized
		if (immobilized) accelBrakeInput = 0;
		
		ConvertInputToAccelerationAndBraking (accelBrakeInput);
		CalculateSpeedValues ();
		HandleGearChanging ();
//		CalculateGearFactor ();
		ProcessWheels (steerInput);
		CalculateRevs();

	}
	
	void ConvertInputToAccelerationAndBraking (float accelBrakeInput)
	{
		// move.Z is the user's fwd/back input. We need to convert it into acceleration and braking.
		// this differs based on if the car is currently moving forward or backward.
		// change is based slightly away from the zero value (by "smallspeed") so that for example when
		// the car transitions from reversing to moving forwards, the car does not need to come to a complete
		// rest before starting to accelerate.
		
		reversing = false;
		if (accelBrakeInput > 0) {
			if (CurrentSpeed > -smallSpeed) {
				// pressing forward while moving forward : accelerate!
				targetAccelInput = accelBrakeInput;
				BrakeInput = 0;
			}
			else {
				// pressing forward while movnig backward : brake!
				BrakeInput = accelBrakeInput;
				targetAccelInput = 0;
			}
		}
		else {
			if (CurrentSpeed > smallSpeed) {
				// pressing backward while moving forward : brake!
				BrakeInput = -accelBrakeInput;
				targetAccelInput = 0;
			}
			else {
				// pressing backward while moving backward : accelerate (in reverse direction)
				BrakeInput = 0;
				targetAccelInput = accelBrakeInput;
				reversing = true;
			}
		}
		// smoothly move the current accel towards the target accel value.
		AccelInput = Mathf.MoveTowards (AccelInput, targetAccelInput, Time.deltaTime);
	}
	
	void CalculateSpeedValues ()
	{
		// current speed is measured in the forward direction of the car (sliding sideways doesn't count!)
		CurrentSpeed = transform.InverseTransformDirection (GetComponent<Rigidbody>().velocity).z;
		// speedfactor is a normalized representation of speed in relation to max speed:
		SpeedFactor = Mathf.InverseLerp (0, reversing ? maxReversingSpeed : maxSpeed, Mathf.Abs (CurrentSpeed));
		curvedSpeedFactor = reversing ? 0 : CurveFactor (SpeedFactor);
	}
	
	void HandleGearChanging ()
	{
		// change gear, when appropriate (if speed has risen above or below the current gear's range, as stored in the gearDistribution array)
		if (!reversing) {
			if (SpeedFactor < gearDistribution [GearNum] && GearNum > 0)
				GearNum--;
			if (SpeedFactor > gearDistribution [GearNum + 1] && GearNum < advanced.numGears - 1)
				GearNum++;
		}
	}

	void ProcessWheels (float steerInput)
	{

		foreach (SimpleWheel wheel in wheels) {
			WheelCollider wheelCollider = wheel.wheelCollider;
			if (wheel.steerable) 
			{
				wheelCollider.steerAngle = steerInput * maxSteerAngle;
			}

			if (wheel.powered) 
			{
				float currentMaxTorque = Mathf.Lerp (maxTorque, (SpeedFactor < 1) ? minTorque : 0, reversing ? SpeedFactor : curvedSpeedFactor);
				wheelCollider.motorTorque = AccelInput * currentMaxTorque;

			}
			// apply curent brake torque to wheel
//			wheelCollider.brakeTorque = BrakeInput * brakePower;

		}

	}
	

	void CalculateRevs ()
	{
		// calculate engine revs (for display / sound)
		// (this is done in retrospect - revs are not used in force/power calculations)
		float gearNumFactor = GearNum / (float)NumGears;
		float revsRangeMin = ULerp (0f, advanced.revRangeBoundary, CurveFactor (gearNumFactor));
		float revsRangeMax = ULerp (advanced.revRangeBoundary, 1f, gearNumFactor);
		RevsFactor = ULerp (revsRangeMin, revsRangeMax, 0.8f);
	}
	

	
	// simple function to add a curved bias towards 1 for a value in the 0-1 range
	float CurveFactor (float factor)
	{
		return 1 - (1 - factor)*(1 - factor);
	}
	
	
	// unclamped version of Lerp, to allow value to exceed the from-to range
	float ULerp (float from, float to, float value)
	{
		return (1.0f - value)*from + value*to;
	}
	
	
	void SetUpGears()
	{
		// the gear distribution is a range of normalized values marking out where the gear changes should occur
		// over the normalized range of speeds for the car.
		// eg, if the bias is centred, 5 gears would be evenly distributed as 0-0.2, 0.2-0.4, 0.4-0.6, 0.6-0.8, 0.8-1
		// with a low bias, the gears are clumped towards the lower end of the speed range, and vice-versa for high bias.
		
		gearDistribution = new float[advanced.numGears + 1];
		for (int g = 0; g <= advanced.numGears; ++g)
		{
			float gearPos = g / (float)advanced.numGears;
			
			float lowBias = gearPos*gearPos*gearPos;
			float highBias = 1 - (1 - gearPos) * (1 - gearPos) * (1 - gearPos);
			
			if (advanced.gearDistributionBias < 0.5f)
			{
				gearPos = Mathf.Lerp(gearPos, lowBias, 1 - (advanced.gearDistributionBias * 2));
			} else {
				gearPos = Mathf.Lerp(gearPos, highBias, (advanced.gearDistributionBias - 0.5f) * 2);
			}
			
			gearDistribution[g] = gearPos;
		}
	}
	
	
	public void ApplyHandBrake()
	{
		foreach (SimpleWheel _wheel in wheels) 
		{
			_wheel.wheelCollider.brakeTorque = 200;
		}
	}
	
	public void ReleaseHandBrake()
	{
		foreach (SimpleWheel _wheel in wheels) 
		{
			_wheel.wheelCollider.brakeTorque = 0;
		}
	}
	
	// Immobilize can be called from other objects, if the car needs to be made uncontrollable
	// (eg, from asplosion!)
	public void Immobilize ()
	{
		immobilized = true;
	}
	
	// Reset is called via the ObjectResetter script, if present.
	public void Reset()
	{
		immobilized = false;
	}
}
