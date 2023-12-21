using UnityEngine;
using System.Collections;

public class VehicleBase : MonoBehaviour
{
	[HideInInspector]
	public Transform trans;
	[HideInInspector]
	public Rigidbody body;


	public string driverName = "";
	public bool isLogEnabled = false;
	public bool isCarEnabled = false;

//	[HideInInspector]
	public bool isActiveFrame = false;
	[HideInInspector]
	public bool isPlayer = false;
	[HideInInspector]
	public bool didFinish = false;
	public bool isActiveVehicle = false;
	[HideInInspector]
	public bool isMasterClient = false;
	public int finalPosition = 0;

//	protected VehicleController vehicleController;
	private PlayerControl thisPlayerController;

    #region VehicleInputs

    protected float steer;
    protected float acce;
    protected float handBrakeInput;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public VehicleInput thisVehicleInput;

    #endregion

    #region damage
    //  public Texture fullLife;
    public bool isDamage = false;
    [HideInInspector]
    public FloatingHealth floatingHealthBar;
    public Texture damagedTextures;
    public float carLife = 100;
    [HideInInspector]
    public float initCarLife;
    #endregion

	#region Navigation Variables

//	[HideInInspector]
	public Vector3 target;

	protected NavigationBase navBase;

	#endregion

	#region Get Region

	public NavigationBase GetNavBase ()
	{
		if (navBase == null) {
			navBase = GetComponent<NavigationBase> ();
		}
		return navBase;
	}

	public PlayerControl GetPlayerControl ()
	{
		if (thisPlayerController == null)
		{
			thisPlayerController = GetComponent<PlayerControl> ();
		}
		return thisPlayerController;
	}

    public VehicleInput GetVehicleInput ()
    {
        if(thisVehicleInput == null)
        {
            thisVehicleInput = GetComponent<VehicleInput>();
        }
        return thisVehicleInput;
    }

	public int GetCurrentDistWPID()
	{
		return navBase.currDistWP.id;
	}

	public DistWP GetCurrentDistWP()
	{
		return navBase.currDistWP;
	}

	public TrafficWP GetCloseTrafficWP()
	{
		return TrackManager.instance.GetCloseTrafficWP(navBase.currDistWP);
	}

	#endregion

	protected virtual void Awake ()
	{
	}

	#region Init Region

	public virtual void  PositionVehicleAndWayPoint(PosWP _posWP, DistWP _trackEndWP)
    {
        
    }

	public virtual void InitCar ()
	{
		if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
		{
			isActiveVehicle = isPlayer;
            isActiveFrame = true;
		}

	}

	public virtual void EnableVehicle ()
	{
        
	}

	#endregion

	public virtual void DisableCar ()
	{
        
	}

	public virtual void SetSound(bool status)
	{
		
	}

	public virtual void OnSecondsTick()
	{
		//add code for time count here
		navBase.totalTime += 1;
		navBase.lapTime += 1;
	}

	public virtual void ApplyHandBrake ()
	{
	}

	public virtual void ApplySuperCheat()
	{
		
	}

	public virtual void ReleaseHandBrake ()
	{
	}

	public virtual void OnVehicleReset ()
	{
	}

	protected virtual void  Update ()
	{
        
	}

	protected virtual void FixedUpdate ()
	{
		UpdatePowerTrain ();
		UpdateSteering ();
	}

	protected virtual void UpdatePowerTrain ()
	{

	}

	protected virtual void UpdateSteering ()
	{

	}

	public virtual void OnGameEnd ()
	{
        
	}

	protected virtual void OnTriggerEnter (Collider other)
	{
        
	}

	protected virtual void OnCollisionEnter (Collision other)
	{
        
	}

    #region Misc Functions
    public virtual void SwitchCamera()
    {
        
    }
    #endregion
}
