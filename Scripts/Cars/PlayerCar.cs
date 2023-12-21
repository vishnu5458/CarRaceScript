using EVP;
using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class EngineUpgrade
{
    public int carSpeed;
    public float forceCurve;
}

public class PlayerCar : CarBase
{

    private Camera smoothFollowCamera;
    [SerializeField]
    private Camera hoodCamera;
    private AudioListener smoothFollowListener;
    private AudioListener hoodListener;

    public Transform camFollowTarget;
    //  public Transform weaponPivot;

    //  public GameObject[] GuardRenderer;
    public int[] healthArray;
    public EngineUpgrade[] engineUpgradeArray;

    //  public AnimationCurve steeringCurve;
    public Transform[] countDownCamPos;

    public float steeringLerpRate;

    public float camDist = 10;
    public float camHeight = 0.3f;
    public float nitroCamDist = 2;
    //  private WeaponBase

    //  Vector3 prevVelocity;


    protected override void Awake()
    {

        isPlayer = true;
        isActiveFrame = true;
        camFollowTarget = transform.Find("Target");
        Transform _countDownPosParent = transform.Find("CountDownCamPos");
        countDownCamPos = new Transform[_countDownPosParent.childCount];
        for (int i = 0; i < _countDownPosParent.childCount; i++)
        {
            //          countDownCamPos[i] = new Transform();
            countDownCamPos[i] = _countDownPosParent.GetChild(i);
        }
        
        base.Awake();
        thisVehicleInput = GetComponent<VehicleInput>();
        if (hoodCamera == null)
            hoodCamera = GetComponentInChildren<Camera>();
        hoodListener = hoodCamera.GetComponent<AudioListener>();
    }

    public VehicleController GetVehicleConroller()
    {
        return vehicleController;
    }

    public override void InitCar()
    {
        base.InitCar();
        smoothFollowCamera = SmoothFollow.instance.GetSmoothFollowCam();        
        smoothFollowListener = smoothFollowCamera.GetComponent<AudioListener>();
        smoothFollowCamera.enabled = true;
        smoothFollowListener.enabled = true;
    }

    public override void OnVehicleReset()
    {
        base.OnVehicleReset();
        //      carCameraTrans.transform.localPosition = new Vector3(0, 3.6f, 10);
    }

    protected override void Update()
    {
        if (!isCarEnabled)
            return;

        base.Update();

        #if ( UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_FLASH || UNITY_WEBGL)        

        acce = thisVehicleInput.Accell;
        steer = thisVehicleInput.Steer;
        handBrakeInput = thisVehicleInput.Handbrake;//Mathf.Clamp01(Input.GetAxis("Jump"));

        //        if(Input.GetKeyDown(KeyCode.R))
        //        {
        //            OnVehicleReset();
        //        }

        #endif

        if (isActiveVehicle)
            SpeedoMeter.instance.UpdateRPMSpeedo(speed * 3.6f, toyCarAudio.simulatedEngineRpm);

    }

    protected override void FixedUpdate()
    {
        if (!isCarEnabled)
            return;

        base.FixedUpdate();
    }

    protected override void UpdatePowerTrain()
    {
        if (!isCarEnabled)
            return;

        base.UpdatePowerTrain();
    }

    public void SetInput(float _steer, float _acce, float _handBrake)
    {
        steer = _steer;
        acce = _acce;
        handBrakeInput = _handBrake;
    }

    protected override void UpdateSteering()
    {
        steer = Mathf.Clamp(steer, -1.0f, 1.0f);

        base.UpdateSteering();

        //      Debug.Log(steerAngle);
    }

    public override void SwitchCamera()
    {

        if (!hoodCamera.enabled)
        {
            smoothFollowCamera.enabled = false;
            smoothFollowListener.enabled = false;
            hoodCamera.enabled = true;
            hoodListener.enabled = true;
        }
        else if (!smoothFollowCamera.enabled)
        {
            smoothFollowCamera.enabled = true;
            smoothFollowListener.enabled = true;
            hoodCamera.enabled = false;
            hoodListener.enabled = false;
        }


    }

    #region Input

    public void OnAcceInput()
    {
        if (!isCarEnabled)
            return;

        acce = 1;
    }

    public void OnBrakePress()
    {
        if (!isCarEnabled)
            return;

        acce = -1;
    }

    public void OnLeftKey()
    {
        steer -= steeringLerpRate;
    }

    public void OnRightKey()
    {
        steer += steeringLerpRate;
    }

    public void OnMobileSteering(float angle)
    {
        steer = angle;
    }

    public void OnReleaseInput()
    {
        acce = 0;
    }

    public void OnReleaseSteering()
    {
        steer = Mathf.Lerp(steer, 0, steeringLerpRate);
    }

    #endregion

    public override void DisableCar()
    {
        smoothFollowCamera.enabled = true;
        smoothFollowListener.enabled = true;
        hoodCamera.enabled = false;
        hoodListener.enabled = false;
        base.DisableCar();
    }

    public override void OnGameEnd()
    {
        //      speedometerText.Text = "0";
        base.OnGameEnd();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
    }
}
