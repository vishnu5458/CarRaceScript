using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using Photon;

/// <summary>
/// Synchronizes car position/rotation between peers and performs dead
/// reckoning.
/// 
/// If car is controlled by local player, reads position/rotation/input
/// data from Transform and CarInput and sends to peers.
/// 
/// If it's a remote car, receives data from network to cache correct
/// position/rotation, which are smoothly interpolated, and set input
/// values on CarInput, which performs dead reckoning between synchonization
/// frames.
/// </summary>
public class NetworkCar : PunBehaviour
{
	[HideInInspector]
    public VehicleBase m_VehicleBase;
		
//	public bool isDebug = false;
	// the CarInput to read/write input data from/to
    private VehicleInput m_VehicleInput;
	private NavigationBase m_NavBase;
	private Rigidbody rb;
	private Transform trans;

	// cached values for correct position/rotation (which are then interpolated)
	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;
	private Vector3 currentVelocity;
	private int posWeight;
	private float updateTime = 0;

	public int currentPosition;

	private void Awake ()
	{
		m_VehicleInput = GetComponent<VehicleInput> ();
        m_VehicleBase = GetComponent<VehicleBase> ();
		rb = GetComponent<Rigidbody> ();
		trans = transform;
	}

	private void Start ()
	{
		m_NavBase = m_VehicleBase.GetNavBase ();
	}

	/// <summary>
	/// If it is a remote car, interpolates position and rotation
	/// received from network. 
	/// </summary>
	public void FixedUpdate ()
	{
		if (!photonView.isMine)
		{
			Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);
			trans.position = Vector3.Lerp (trans.position, projectedPosition, Time.deltaTime * 4);
			trans.rotation = Quaternion.Lerp (trans.rotation, this.correctPlayerRot, Time.deltaTime * 4);
		}
	}

	/// <summary>
	/// At each synchronization frame, sends/receives player input, position
	/// and rotation data to/from peers/owner.
	/// </summary>
	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this car: send the others our input and transform data
			stream.SendNext ((float)m_VehicleInput.Steer);
			stream.SendNext ((float)m_VehicleInput.Accell);
			stream.SendNext ((float)m_VehicleInput.Handbrake);
			stream.SendNext (trans.position);
			stream.SendNext (trans.rotation);
			stream.SendNext (rb.velocity);
//			if(isDebug)
//				Debug.Log("sending stream: "+ photonView.viewID);
//			if (m_CarBase.isActiveCar)
			if (m_NavBase != null)
				stream.SendNext ((int)m_NavBase.positionWeight);
		}
		else
		{
			//Remote car, receive data
			m_VehicleInput.Steer = (float)stream.ReceiveNext ();
			m_VehicleInput.Accell = (float)stream.ReceiveNext ();
			m_VehicleInput.Handbrake = (float)stream.ReceiveNext ();
			correctPlayerPos = (Vector3)stream.ReceiveNext ();
			correctPlayerRot = (Quaternion)stream.ReceiveNext ();
			currentVelocity = (Vector3)stream.ReceiveNext ();
//			if(isDebug)
//				Debug.Log("receiving stream: "+ photonView.viewID);
			if (m_NavBase != null)
				m_NavBase.positionWeight = (int)stream.ReceiveNext ();
			updateTime = Time.time;
		} 
	}
}

