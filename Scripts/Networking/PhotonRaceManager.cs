using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon;

public class PhotonRaceManager : PunBehaviour
{

	#region Singleton

	private static PhotonRaceManager _instance;

	public static PhotonRaceManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<PhotonRaceManager>();
			}
			return _instance;
		}
	}

	#endregion

	/// <summary>
	/// Manages the race gameplay, instantiating player car and sending/receiving
	/// RPCs for race start/finish, timer, etc.
	/// </summary>


	public bool waitForCountDown = false;
	public bool waitForLoadComplete = false;
	public bool waitForGameOver = false;

	public bool isGameLoaded = false;
	public int loadedPlayers = 0;

	// reference to local player car
	//	[HideInInspector]
	//	public CarRaceControl localCar;

	public VehicleBase currLocalCar;
	public float raceTime = 0;

	// list of all player´s cars (for position calculation)
	public List<NetworkCar> networkCarList = new List<NetworkCar>();


	void Start()
	{
		_instance = this;


	}

	public void OnLoadConfirm()
	{
		Debug.Log("Created car");
		CreateCar();
		waitForCountDown = false;
		waitForLoadComplete = false;
		isGameLoaded = false;
		waitForGameOver = false;
		loadedPlayers = 0;
	}

	void Update()
	{
		if (waitForCountDown && PhotonNetwork.isMasterClient)
		{
			CheckCountdown();
		}

		if (waitForLoadComplete)
		{
			bool takingTooLong = raceTime >= 5;
			bool finishedLoading = loadedPlayers == PhotonNetwork.playerList.Length;
			if (takingTooLong || finishedLoading)
			{
				FindNetworkCars();
			}
		}

		raceTime += Time.deltaTime;
//		SortCars ();

	}

//	private void SortCars()
//	{
//		networkCarList.Sort();
//		int position = 1;
//		foreach (NetworkCar c in networkCarList)
//		{
//			c.currentPosition = position;
//			position++;
//		}
//	}

	// Instantiates player car on all peers, using the appropriate spawn point (based
	// on join order), and sets local camera target.
	private void CreateCar()
	{
		// gets spawn Transform based on player join order (spawn property)
		int pos = (int)PhotonNetwork.player.CustomProperties["spawn"];
		int carNumber = (int)PhotonNetwork.player.CustomProperties["car"];
//        int teamID = (int)PhotonNetwork.player.customProperties["teamID"];

		PosWP spawn = BotPlayerPosWP.instance.botPosition[pos];

		Debug.Log("Local Car Position: " + pos);

		// instantiate car at Spawn Transform
		// car prefabs are numbered in the same order as the car sprites that the player chose from
		GameObject car = PhotonNetwork.Instantiate("Player Car " + (carNumber + 1), spawn.objTransform.position, spawn.objTransform.rotation, 0);

		currLocalCar = car.GetComponent<VehicleBase>();
		currLocalCar.isActiveVehicle = true;
		currLocalCar.isMasterClient = PhotonNetwork.isMasterClient;
		currLocalCar.GetComponent<NetworkCar>().currentPosition = pos;

		// car starting race position (for GUI) is same as spawn position + 1 (grid position)
		GamePlayScreen.instance.SetLocalPlayerPosition(pos + 1);

//		photonView.RPC("OnPhotonCarCreated",PhotonTargets.All, currLocalCar.transform);

	}

	public void LoadCompleted()
	{
		Debug.Log(PhotonNetwork.playerName + " load completed");
		NetworkingPeer.lastMethodName = "";
		waitForLoadComplete = true;
		raceTime = 0;
		photonView.RPC("ConfirmLoad", PhotonTargets.All, PhotonNetwork.playerName);
	}

	// called when a player computer finishes loading this scene...
	[PunRPC]
	public void ConfirmLoad(string playerName)
	{
		loadedPlayers++;
		Debug.Log("Player : " + playerName + " loaded");
	}

	public void FindNetworkCars()
	{
		waitForLoadComplete = false;
		networkCarList.Clear();
		VehicleBase[] vehicles = GameObject.FindObjectsOfType<VehicleBase>();
		foreach (VehicleBase go in vehicles)
		{
			NetworkCar _netCar = go.GetComponent<NetworkCar>();
			networkCarList.Add(_netCar);
			_netCar.currentPosition = (int)_netCar.photonView.owner.CustomProperties["spawn"];

            int bikeIndex = (int)_netCar.photonView.owner.CustomProperties["car"];
            int teamIndex = (int)_netCar.photonView.owner.CustomProperties["teamID"];
			//			int carPos = _netCar.currentPosition;
			go.driverName = _netCar.photonView.owner.NickName;
			if (go.driverName.Length > 9)
			{
				go.driverName = go.driverName.Substring(0, 9);
			}
			Debug.Log("Player Name : " + go.driverName + " Photon View : " + _netCar.photonView.viewID);
			//			Debug.Log("Network Name : " + PhotonNetwork.playerList[go.GetComponent<NetworkCar> ().currentPosition].name);
            MainDirector.GetCurrentGamePlay().OnNewNetworkCar(go, _netCar.currentPosition, bikeIndex, teamIndex);
			go.trans.parent = MainDirector.instance.carParent;
			
		}

		MainDirector.GetCurrentGamePlay().InitializePlayerControl();
		Invoke("WaitAfterGameLoad", 1);
	}

	void WaitAfterGameLoad()
	{
		raceTime = 0;
		waitForCountDown = true;
	}


	// master-client only: we start the countdown only when ALL players are connected
	private void CheckCountdown()
	{
		if (GlobalClass.isRaceStarted)
			return;
		bool takingTooLong = raceTime >= 5;
		bool finishedLoading = loadedPlayers == PhotonNetwork.playerList.Length;
		if (takingTooLong || finishedLoading)
		{
			photonView.RPC("HideLoadingScreen", PhotonTargets.All);
		}
	}

	[PunRPC]
	public void HideLoadingScreen()
	{
		loadedPlayers = 0;
		waitForCountDown = false;
		isGameLoaded = true;
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.IsOpen = false;
			PhotonNetwork.room.IsVisible = false;
		}
		LoadingScreen.instance.HideLoadingScreen();
	}


	//	[PunRPC]
	//	public void StartCountdown (double startTimestamp)
	//	{
	//		loadedPlayers = 0;
	//		waitForCountDown = false;
	//		Debug.Log ("Countdown");
	//		//		state = RaceState.COUNTDOWN;
	//		CountDown.countDownTimer.InitializeCountDown ();
	//		// sets local timestamp to the desired server timestamp (will be checked every frame)
	//	}

	public void LocalCarGameFinish()
	{
		photonView.RPC("OnCarGameOver", PhotonTargets.All, currLocalCar.GetComponent<NetworkCar>().photonView.viewID);

		waitForGameOver = false;
	}

	[PunRPC]
	public void OnCarGameOver(int viewID)
	{
		loadedPlayers++;

		VehicleBase currVehicle = GetCarBaseForPhotonID(viewID);
		MainDirector.GetCurrentGamePlay().OnTrackComplete(currVehicle, currVehicle.GetNavBase().totalTime);
		GameOverScreen.instance.CheckForGameOver();
	}

	//  [PunRPC]
	public void OnEndGame()
	{
//      loadedPlayers = 0;
		MainDirector.GetCurrentGamePlay().GameOver();
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer disconnetedPlayer)
	{
		//error here
		if (!isGameLoaded)
			return;
		
		Debug.Log(disconnetedPlayer.NickName + " disconnected...");
		NetworkCar toRemove = null;
		foreach (NetworkCar nc in networkCarList)
		{
			//Debug.Log (rc.photonView.owner);
			if (nc.photonView.owner == null)
			{
				toRemove = nc;
			}
		}
		MainDirector.GetCurrentGamePlay().RemoveCarFromList(toRemove.GetComponent<VehicleBase>());

		// remove car controller of disconnected player from the list
		networkCarList.Remove(toRemove);
		if (disconnetedPlayer.IsLocal)
		{
			PhotonNetwork.LeaveRoom();
			MainDirector.instance.UnloadTrack();
			MainDirector.instance.LoadMenu("MenuScreen");
			GamePlayScreen.instance.ExitScreen();
		}
	}

	// Use this to go back to the menu, without leaving the lobby
	public void ResetToMenu()
	{
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.LoadLevel("Menu");
	}

	public VehicleBase GetLocalCar()
	{
		return currLocalCar;
	}

	public VehicleBase GetCarBaseForPhotonID(int viewID)
	{
		foreach (NetworkCar nc in networkCarList)
		{
			if (nc.photonView.viewID.CompareTo(viewID) == 0)
				return nc.m_VehicleBase;
		}
		return null;
	}

}


