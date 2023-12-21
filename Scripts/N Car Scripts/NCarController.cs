using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{

	[Serializable]
	public class NWheel
	{
		public WheelCollider wheelCollider;
		public Transform wheelTransform;
		public bool steer = false;
		public bool drive = false;
		public bool brake = true;
		public bool handbrake = false;

		[HideInInspector]
		public bool leavingSkidTrail = false;
		[HideInInspector]
		public Transform skidTrail;
		public bool isLog = false;
	}

    public class NCarController : MonoBehaviour
    {
		public NWheel[] m_wheels = new NWheel[0];
		public Transform skidTrailPrefab;

        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
//        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

//        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
		private int m_PoweredWheels;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
		private float m_MaxHandbrakeTorque;
		private Transform m_Transform;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
		public float speed{ get; private set;}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
		public float AccelInput { get; private set; }
		public float HandbrakeInput { get; private set; }
		public float SpeedFactor { get;  private set; }                                 // value between 0-1 of the car's current speed relative to max speed
		public float AvgSkid { get; private set; }                                         // the average skid factor from all wheels

#region Studd Variable
		[Header("Custom Variables")]

		public bool isCarEnabled = false;
		public float nitroPowerBoost = 20;

		private bool immobilized;          	                                                // Whether the car is accepting inputs.
		private bool isHandBrakeEnabled = false;
		private int boostStatus = 0;
		private int dirtSpeedReduction;													// speed reduction when Driving offroad
		private float aiCatchUpSpeed = 0;												// speed buffer for ai to catch-up or slow down (contolled from AI Car)
		private float initTopSpeed;

		public static Transform skidTrailsDetachedParent;

		public float AiCatchUpSpeed 
		{
			set {aiCatchUpSpeed = value;}
		} 
		
		public int DirtSpeedReduction 
		{
			set {dirtSpeedReduction = value;}
		}

		public void EnableCar()
		{
			isCarEnabled = true;
		}
		
		public void DisableCar()
		{
			isCarEnabled = false;
		}

		public void OnBoostEnable(int status)
		{
			boostStatus = status;
		}
		
		public void OnNitroDisable()
		{
			boostStatus = 0;
		}

		public void ApplyHandBrake()
		{
			isHandBrakeEnabled = true;
		}
		
		public void ReleaseHandBrake()
		{
			isHandBrakeEnabled = false;
		}
		public void Immobilize ()
		{
			immobilized = true;
		}
		
		// Reset is called via the ObjectResetter script, if present.
		public void Reset()
		{
			immobilized = false;
		}
		#endregion

        // Use this for initialization
		private void Awake()
		{
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Transform = transform;
			foreach(NWheel _wheel in m_wheels)
			{
				if(_wheel.drive)
					m_PoweredWheels++;
			}

			if (skidTrailsDetachedParent == null)
			{
				skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
			}
		}

        private void OnEnable()
        {
//            m_WheelMeshLocalRotations = new Quaternion[4];
//            for (int i = 0; i < 4; i++)
//            {
//                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
//            }
			m_wheels[0].wheelCollider.attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

         
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
			initTopSpeed = m_Topspeed;
        }

        public void Move(float steering, float accel, float handbrake)
        {
	        //clamp input values
			float footbrake;
            steering = Mathf.Clamp(steering, -1, 1);
			BrakeInput = footbrake = -1 * Mathf.Clamp(accel, -1, 0);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            HandbrakeInput = handbrake = Mathf.Clamp(handbrake, 0, 1);
			if (immobilized) AccelInput = 0;

			m_Topspeed = initTopSpeed;
//			Debug.Log(footbrake);
			//y = mx + c
//			float acceAdj = 1 - (0.25f * Mathf.Abs(steerInput));
//			
//			maxSpeedForward = maxSpeedForward * acceAdj;
			
			m_Topspeed += aiCatchUpSpeed;
			m_Topspeed += dirtSpeedReduction;
			
			m_Topspeed += boostStatus * nitroPowerBoost;


            //Set the steer on the front wheels.
            m_SteerAngle = steering*m_MaximumSteerAngle;

//            SteerHelper();
          
			speed =  m_Rigidbody.velocity.magnitude;//*2.23693629f 
			SpeedFactor = Mathf.InverseLerp (0, m_Topspeed, Mathf.Abs (speed));
           
			//Set the handbrake.
			if(isHandBrakeEnabled) handbrake = 1;
			float hbTorque = handbrake*m_MaxHandbrakeTorque;
            
			foreach(NWheel _wheel in m_wheels)
			{
				Quaternion quat;
				Vector3 position;
				_wheel.wheelCollider.GetWorldPose(out position, out quat);
				_wheel.wheelTransform.position = position;
				_wheel.wheelTransform.rotation = quat;
				if(_wheel.steer)
					_wheel.wheelCollider.steerAngle = m_SteerAngle;
				if(_wheel.handbrake)
					_wheel.wheelCollider.brakeTorque = hbTorque;
			}

			ApplyDrive(accel, footbrake);
			CapSpeed();

            CalculateRevs();
            GearChanging();

            AddDownForce();
//            CheckForWheelSpin();
            TractionControl();
        }

		private void GearChanging()
		{
			float f = Mathf.Abs(speed/MaxSpeed);
			float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
			float downgearlimit = (1/(float) NoOfGears)*m_GearNum;
			
			if (m_GearNum > 0 && f < downgearlimit)
			{
				m_GearNum--;
			}
			
			if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
			{
				m_GearNum++;
			}
		}
		
		// simple function to add a curved bias towards 1 for a value in the 0-1 range
		private static float CurveFactor(float factor)
		{
			return 1 - (1 - factor)*(1 - factor);
		}
		
		// unclamped version of Lerp, to allow value to exceed the from-to range
		private static float ULerp(float from, float to, float value)
		{
			return (1.0f - value)*from + value*to;
		}
		
		private void CalculateGearFactor()
		{
			float f = (1/(float) NoOfGears);
			// gear factor is a normalised representation of the current speed within the current gear's range of speeds.
			// We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
			var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(speed/MaxSpeed));
			m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
		}
		
		private void CalculateRevs()
		{
			// calculate engine revs (for display / sound)
			// (this is done in retrospect - revs are not used in force/power calculations)
			CalculateGearFactor();
			var gearNumFactor = m_GearNum/(float) NoOfGears;
			var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
			var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
			Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
		}
		 
        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
//			speed *= 2.23693629f;
			if (speed > m_Topspeed)
				m_Rigidbody.velocity = (m_Topspeed) * m_Rigidbody.velocity.normalized; //2.23693629f
        }

        private void ApplyDrive(float accel, float footbrake)
        {

            float thrustTorque;
			thrustTorque = accel * (m_CurrentTorque / m_PoweredWheels);

			foreach(NWheel _wheel in m_wheels)
			{
				if(_wheel.drive)
					_wheel.wheelCollider.motorTorque = thrustTorque;
				if(footbrake > 0)
				{
					if (speed > 2.25f && Vector3.Angle(m_Transform.forward, m_Rigidbody.velocity) < 50f)
					{
//						if(_wheel.isLog)
//							Debug.Log(Vector3.Angle(transform.forward, m_Rigidbody.velocity));
						if(_wheel.brake)
						 _wheel.wheelCollider.brakeTorque = m_BrakeTorque*footbrake;
					}
					else //if (footbrake > 0)
					{
						if(_wheel.brake)
							_wheel.wheelCollider.brakeTorque = 0f;
						if(_wheel.drive)
							_wheel.wheelCollider.motorTorque = - 0.2f * m_FullTorqueOverAllWheels*footbrake;
					}
				}
			}
         }


        private void SteerHelper()
        {
			WheelHit wheelhit;
			foreach(NWheel _wheel in m_wheels)
			{
				_wheel.wheelCollider.GetGroundHit(out wheelhit);
				if (wheelhit.normal == Vector3.zero)
					return; // wheels arent on the ground so dont realign the rigidbody velocity
			}
            
            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - m_Transform.eulerAngles.y) < 10f)
            {
				var turnadjust = (m_Transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
				if(Mathf.Abs(velRotation.w - 1.0f) > 0.1f)
					Debug.Log(velRotation.w);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
			else
			{
				Debug.Log("skip frame");
			}
			m_OldRotation = m_Transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
			m_wheels[0].wheelCollider.attachedRigidbody.AddForce(-m_Transform.up*m_Downforce*m_wheels[0].wheelCollider.attachedRigidbody.velocity.magnitude);
        }
		 

        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
			float targetAvgSkid = 0; 
			WheelHit wheelHit;
			foreach(NWheel _wheel in m_wheels)
			{
				_wheel.wheelCollider.GetGroundHit(out wheelHit);
				float SkidFactor = 0;

				SkidFactor = Mathf.Max(Mathf.Abs(wheelHit.sidewaysSlip), Mathf.Abs(wheelHit.forwardSlip));
				targetAvgSkid += SkidFactor/(2 * m_SlipLimit); // downsizing it due to high skid factor value
				
				if (skidTrailPrefab != null)
				{
					if (SkidFactor > m_SlipLimit && wheelHit.normal != Vector3.zero)
					{
						//                         if(wd.wheel.isLog)
						//                             Debug.Log(SkidFactor);
						if (!_wheel.leavingSkidTrail)
						{
							_wheel.skidTrail = Instantiate(skidTrailPrefab) as Transform;
							if (_wheel.skidTrail != null)
							{
								_wheel.skidTrail.parent = _wheel.wheelCollider.transform;
								_wheel.skidTrail.localPosition = -Vector3.up * (_wheel.wheelCollider.radius - 0.04f);
							}
							_wheel.leavingSkidTrail = true;
						}
						
					}
					else
					{
						if (_wheel.leavingSkidTrail)
						{
							_wheel.skidTrail.parent = skidTrailsDetachedParent;
							Destroy(_wheel.skidTrail.gameObject, 10);
							_wheel.leavingSkidTrail = false;
						}
					}
				}
			}
			targetAvgSkid /= m_wheels.Length;
			AvgSkid = Mathf.MoveTowards(AvgSkid, targetAvgSkid, Time.deltaTime);

//            // loop through all wheels
//            for (int i = 0; i < 4; i++)
//            {
//                WheelHit wheelHit;
//                m_WheelColliders[i].GetGroundHit(out wheelHit);
//
//                // is the tire slipping above the given threshhold
//                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
//                {
//                    m_WheelEffects[i].EmitTyreSmoke();
//
//                    // avoiding all four tires screeching at the same time
//                    // if they do it can lead to some strange audio artefacts
//                    if (!AnySkidSoundPlaying())
//                    {
//                        m_WheelEffects[i].PlayAudio();
//                    }
//                    continue;
//                }
//
//                // if it wasnt slipping stop all the audio
//                if (m_WheelEffects[i].PlayingAudio)
//                {
//                    m_WheelEffects[i].StopAudio();
//                }
//                // end the trail generation
//                m_WheelEffects[i].EndSkidTrail();
//            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
			foreach(NWheel _wheel in m_wheels)
			{
				_wheel.wheelCollider.GetGroundHit(out wheelHit);
				AdjustTorque(wheelHit.forwardSlip);
			}
         }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }

		[ContextMenu("Adjust WheelColliders to their meshes")]
		void AdjustWheelColliders ()
		{
			foreach (NWheel wheel in m_wheels)
			{
				if (wheel.wheelCollider != null)
					AdjustColliderToWheelMesh(wheel.wheelCollider, wheel.wheelTransform);
			}
		}

		[ContextMenu("Find wheel collider and meshes")]
		void FindWheelAndMesh()
		{
#if UNITY_EDITOR
			UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
			transform.Find("WheelColliders").localPosition = Vector3.zero;
//			while(transform.Find("colider").childCount > 0)
//			{
//				DestroyImmediate(transform.Find("colider").GetChild(0).gameObject);
//			}
//			GameObject nitroGO = new GameObject("_nitro");
//
//			nitroGO.transform.parent = transform.Find("Particles");
//			nitroGO.transform.localPosition = Vector3.zero;
//
//			for(int i = 0; i < transform.Find("Particles").childCount; i++)
//			{
//				if(transform.Find("Particles").GetChild(i).name == "Nitro Small")
//					transform.Find("Particles").GetChild(i).parent = nitroGO.transform;
//			}

			GetComponent<CarBase>().bodyRender = transform.Find("Carbody").gameObject;

			if(transform.Find("Carbody/Brakelight").GetComponent<NBrakeLight>() == null)
			{
				transform.Find("Carbody/Brakelight").gameObject.AddComponent<NBrakeLight>().car = this;
			}

			if(transform.Find("CoM"))
				DestroyImmediate(transform.Find("CoM").gameObject);

			if(transform.Find("Audio"))
				DestroyImmediate(transform.Find("Audio").gameObject);


			m_wheels[0].wheelCollider =  transform.Find("WheelColliders/WheelFL").GetComponent<WheelCollider>();
			m_wheels[1].wheelCollider =  transform.Find("WheelColliders/WheelFR").GetComponent<WheelCollider>();
			m_wheels[2].wheelCollider =  transform.Find("WheelColliders/WheelRL").GetComponent<WheelCollider>();
			m_wheels[3].wheelCollider =  transform.Find("WheelColliders/WheelRR").GetComponent<WheelCollider>();

			m_wheels[0].wheelTransform =  transform.Find("Carbody/WheelFL");
			m_wheels[1].wheelTransform =  transform.Find("Carbody/WheelFR");
			m_wheels[2].wheelTransform =  transform.Find("Carbody/WheelRL");
			m_wheels[3].wheelTransform =  transform.Find("Carbody/WheelRR");

//			m_wheels[0].wheelTransform =  transform.Find("muscle car_01/muscle_car_2_body/muscle_b_l_wheel");
//			m_wheels[1].wheelTransform =  transform.Find("muscle car_01/muscle_car_2_body/muscle_b_r_wheel");
//			m_wheels[2].wheelTransform =  transform.Find("muscle car_01/muscle_car_2_body/muscle_f_l_wheel");
//			m_wheels[3].wheelTransform =  transform.Find("muscle car_01/muscle_car_2_body/muscle_f_r_wheel");

//			m_wheels[0].wheelTransform =  transform.Find("jeep_01/jeep_01 body/jeep_car_b_l_wheel");
//			m_wheels[1].wheelTransform =  transform.Find("jeep_01/jeep_01 body/jeep_car_b_r_wheel");
//			m_wheels[2].wheelTransform =  transform.Find("jeep_01/jeep_01 body/jeep_car_f_l_wheel");
//			m_wheels[3].wheelTransform =  transform.Find("jeep_01/jeep_01 body/jeep_car_f_r_wheel");

//			m_wheels[0].wheelTransform =  transform.Find("rally car_01/rally_car_body/rally_car_b_l_wheel");
//			m_wheels[1].wheelTransform =  transform.Find("rally car_01/rally_car_body/rally_car_b_r_wheel");
//			m_wheels[2].wheelTransform =  transform.Find("rally car_01/rally_car_body/rally_car_f_l_wheel");
//			m_wheels[3].wheelTransform =  transform.Find("rally car_01/rally_car_body/rally_car_f_r_wheel");

//			m_wheels[0].wheelTransform =  transform.Find("range rover_01/range_rover_2_body/range_rover_b_l_wheel1");
//			m_wheels[1].wheelTransform =  transform.Find("range rover_01/range_rover_2_body/range_rover_b_r_wheel");
//			m_wheels[2].wheelTransform =  transform.Find("range rover_01/range_rover_2_body/range_rover_f__l_wheel");
//			m_wheels[3].wheelTransform =  transform.Find("range rover_01/range_rover_2_body/range_rover_f_r_wheel");

//			m_wheels[0].wheelTransform =  transform.Find("minicooper_body/mini_b_l_wheel");
//			m_wheels[1].wheelTransform =  transform.Find("minicooper_body/mini_b_r_wheel");
//			m_wheels[2].wheelTransform =  transform.Find("minicooper_body/mini_f_l_wheel");
//			m_wheels[3].wheelTransform =  transform.Find("minicooper_body/mini_f_r_wheel");

//			Debug.Log("call here");
#endif
		}
		
		static void AdjustColliderToWheelMesh (WheelCollider wheelCollider, Transform wheelTransform)
		{
			// Adjust position and rotation
			
			if (wheelTransform == null)
			{
				Debug.LogError(wheelCollider.gameObject.name + ": A Wheel transform is required");
				return;
			}
			
			wheelCollider.transform.position = wheelTransform.position + wheelTransform.up * wheelCollider.suspensionDistance * 0.5f;
			wheelCollider.transform.rotation = wheelTransform.rotation;
			
			// Adjust radius
			
			MeshFilter[] meshFilters = wheelTransform.GetComponentsInChildren<MeshFilter>();
			if (meshFilters == null || meshFilters.Length == 0)
			{
				Debug.LogWarning(wheelTransform.gameObject.name + ": Couldn't calculate radius. There are no meshes in the Wheel transform or its children");
				return;
			}
			
			// Calculate the bounds of the meshes contained in the Wheel transform
			
			Bounds bounds = GetScaledBounds(meshFilters[0]);
			
			for (int i=1, c=meshFilters.Length; i<c; i++)
			{
				Bounds meshBounds = GetScaledBounds(meshFilters[i]);
				bounds.Encapsulate(meshBounds.min);
				bounds.Encapsulate(meshBounds.max);
			}
			
			// If this is a correct round wheel then extents for y and z should be approximately the same.
			
			if (Mathf.Abs(bounds.extents.y-bounds.extents.z) > 0.01f)
				Debug.LogWarning(wheelTransform.gameObject.name + ": The Wheel mesh might not be a correct wheel. The calculated radius is different along forward and vertical axis.");
			
			wheelCollider.radius = bounds.extents.y;
		}
		
		
		static Bounds GetScaledBounds (MeshFilter meshFilter)
		{
			Bounds bounds = meshFilter.sharedMesh.bounds;
			Vector3 scale = meshFilter.transform.lossyScale;
			bounds.max = Vector3.Scale(bounds.max, scale);
			bounds.min = Vector3.Scale(bounds.min, scale);
			return bounds;
		}
    }
}
