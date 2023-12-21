using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[Serializable]
public class PlayerDetailsMenu
{
	//	public Vector3 initPos;
	public Transform carTrans;
	public Text playerName;
}

[Serializable]
public class NetworkClientCars
{
	public Transform clientCarParent;
	public Transform[] carsArray;
}

[Serializable]
public class TrackPieceClass
{
	public Vector3 stagePos;
	public Vector3 offStagePos;
	public Transform[] trackTrans;
}

public class MultiplayerScreen : ScreenBase
{
	#region Singleton

	private static MultiplayerScreen _instance;

	public static MultiplayerScreen instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<MultiplayerScreen>();
			}
			return _instance;
		}
	}

	#endregion

    [SerializeField]
    private int lapCount;
    private int noTracks;
    public Text lapTxt;

	public Text statusTxt;
	//	public PlayerDetailsMenu[] playerDetailsArray;
	//	public NetworkClientCars[] networkClientCarArray;
	public TrackPieceClass trackPieces;
	public GameObject roomTextGO;
	public InputField roomNameText;
	//	public Transform carPrefabSpawnPos;

	//	public Transform localPlayerCarTrans;
	public Transform carParent;

	//	public Camera[] mulScreenCamera;
	//	public GarageCars[] carsGO;

	public GameObject startGameBtn;
	public GameObject[] trackArrows;

	public GameObject lobbyPanelGO;
	public GameObject roomCreationPanelGO;

	int noCars;
	public int currentTrackIndex;

	//	public string newRoomName;

	private bool isRoomLoaded = false;

	protected override void Awake()
	{
		base.Awake();
		_instance = this;

//		noCars = carsGO.Length;
	
//		int i = 0;
//		foreach (NetworkClientCars _networkCars in networkClientCarArray)
//		{
//			for (i = 0; i < _networkCars.carsArray.Length; i++)
//			{
//				_networkCars.carsArray [i] = _networkCars.clientCarParent.GetChild (i);
//			}
//		}

	}

	protected override void Start()
	{
		base.Start();
		roomTextGO.SetActive(false);
	}

	public override void InitScreen()
	{
		base.InitScreen();

		lobbyPanelGO.SetActive(false);
		roomCreationPanelGO.SetActive(false);

		OnScreenLoadComplete();
		isRoomLoaded = false;
	}

	public override void OnScreenLoadComplete()
	{
		base.OnScreenLoadComplete();
	}

	public void ShowLobbyPanel()
	{
		lobbyPanelGO.SetActive(true);
		roomCreationPanelGO.SetActive(false);
		LobbyScrollBar.instance.ShowRooms();
	}

	public void ShowRoomCreationPanel(bool isMasterClient)
	{
		isRoomLoaded = true;
        lapCount = 1;
        lapTxt.text = lapCount.ToString();

		lobbyPanelGO.SetActive(false);
		roomCreationPanelGO.SetActive(true);
		roomTextGO.SetActive(true);
//		mulScreenCamera [0].enabled = true;
//		mulScreenCamera [1].enabled = true;

		roomNameText.text = "IDnet Room " + UnityEngine.Random.Range(100, 999);

		//show room creation panel

//		ToStageCar (currentCarIndex);

		foreach (GameObject go in  trackArrows)
			go.SetActive(isMasterClient);
	
		for (int i = 0; i < trackPieces.trackTrans.Length; i++)
			OffStageTrack(i);
		
		startGameBtn.SetActive(isMasterClient);

		currentTrackIndex = UnityEngine.Random.Range(0, 5);

		if (isMasterClient)
			ToStageTrack(currentTrackIndex);
		

	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		roomTextGO.SetActive(false);
	}

	protected override void UpdateInput()
	{
		base.UpdateInput();
	}

	public override void ButtonClick(Button _button)
	{
		base.ButtonClick(_button);
       
		if (!isUpdateInput)
			return;
//		localPlayerCarTrans.Rotate (Vector3.up * Time.deltaTime * 10);

		if (_button.name == "Create Room")
		{
			lobbyPanelGO.SetActive(false);
//            Debug.Log("Call Here1");
			PhotonMenu.instance.OnCreateRoomClick();
		}
		else if (_button.name == "Race")
		{
            PhotonMenu.instance.OnRaceClick(roomNameText.text, currentTrackIndex, lapCount);

//					PhotonMenu.instance.StartRace ();
		}
		else if (_button.name == "Quick Play")
		{
			PhotonMenu.instance.OnQuickPlayClick();
		}
        else if (_button.name == "Lap Arrow L")
        {
            DecreaseLap();
        }
        else if (_button.name == "Lap Arrow R")
        {
            IncreaseLap();
        }
		else if (_button.name == "Track Arr L")
		{
			PrevTrack();
		}
		else if (_button.name == "Track Arr R")
		{
			NextTrack();
		}
		else if (_button.name == "Back")
		{
			if (!isRoomLoaded)
			{
				ExitScreen();
				LobbyScrollBar.instance.HideRooms();
				MenuScreen.instance.InitScreen();
				PhotonMenu.instance.DisconnectFromPhoton();
			}
			else
			{
				isRoomLoaded = false;
//						mulScreenCamera [0].enabled = false;
//						mulScreenCamera [1].enabled = false;
				PhotonMenu.instance.LeaveRoom();
				roomTextGO.SetActive(false);
			}
		}
		else if (_button.name == "More Games" || _button.name == "Sponsor Logo")
		{
			if (!GlobalClass.isHostedFromY8)
				Application.OpenURL(GlobalClass.SponsorLinkMenu);

		}
			
	}

	public void UpdateStatusText(string _statusTxt)
	{
		statusTxt.text = _statusTxt;
	}

	//	public void EnterRoomName(string roomName)
	//	{
	//		newRoomName = roomName;
	//	}

	void PrevTrack()
	{
		OffStageTrack(currentTrackIndex);
		currentTrackIndex--;

		if (currentTrackIndex < 0)
			currentTrackIndex = trackPieces.trackTrans.Length - 1;

		ToStageTrack(currentTrackIndex);
	}

	void NextTrack()
	{
		OffStageTrack(currentTrackIndex);
		currentTrackIndex++;

		if (currentTrackIndex > trackPieces.trackTrans.Length - 1)
			currentTrackIndex = 0;

		ToStageTrack(currentTrackIndex);
	}

    void IncreaseLap()
    {
        lapCount++;
        if (lapCount > 4)
            lapCount = 4;
        lapTxt.text = lapCount.ToString();
    }

    void DecreaseLap()
    {
        lapCount--;
        if (lapCount < 1)
            lapCount = 1;
        lapTxt.text = lapCount.ToString();
    }

	void OffStageTrack(int trackIndex)
	{
		trackPieces.trackTrans[trackIndex].localPosition = trackPieces.offStagePos;
	}

	void ToStageTrack(int trackIndex)
	{
		trackPieces.trackTrans[trackIndex].localPosition = trackPieces.stagePos;
//		PhotonMenu.instance.SetTrack (trackIndex);
	}

	//	void PrevCar ()
	//	{
	//		OffStageCar (currentCarIndex);
	//		currentCarIndex--;
	//
	//		if (currentCarIndex < 0)
	//			currentCarIndex = noCars - 1;
	//
	//		ToStageCar (currentCarIndex);
	//	}
	//
	//  void NextCar ()
	//	{
	//		OffStageCar (currentCarIndex);
	//		currentCarIndex++;
	//
	//		if (currentCarIndex > noCars - 1)
	//			currentCarIndex = 0;
	//
	//		ToStageCar (currentCarIndex);
	//	}
	//
	//	void OffStageCar (int carIndex)
	//	{
	//		carsGO [carIndex].trans.parent = carParent;
	//		carsGO [carIndex].trans.position = carsGO [carIndex].initPos;
	//
	//	}
	//
	//	void ToStageCar (int carIndex)
	//	{
	//		carsGO [carIndex].trans.parent = localPlayerCarTrans;
	//		carsGO [carIndex].trans.localPosition = Vector3.zero;
	//		carsGO [carIndex].trans.localEulerAngles = Vector3.zero;
	////		PhotonMenu.instance.SetCar (carIndex);
	//	}
}
