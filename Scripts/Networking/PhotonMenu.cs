using UnityEngine;
using System.Collections;
using Photon;

public class PhotonMenu : PunBehaviour
{
    #region Singleton

    private static PhotonMenu _instance;

    public static PhotonMenu instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PhotonMenu>();
            }
            return _instance;
        }
    }

    #endregion

    private bool isPhotonMenuLoaded = false;
    private bool shouldShowLobby = false;
    private int currTrackIndex;
    private int currLapCount;
    private string currRoomName;

    void Awake()
    {
        if (!PhotonNetwork.inRoom)
        {
            PhotonNetwork.autoCleanUpPlayerObjects = true;
            PhotonNetwork.autoJoinLobby = true;
        }
        PhotonNetwork.sendRate = 10;
        PhotonNetwork.sendRateOnSerialize = 10;

    }

    // Use this for initialization
    void Start()
    {
        _instance = this;
    }

    void Update()
    {
        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
        {
            if (shouldShowLobby)
                DisconnectFromPhoton();

            return;
        }
                
        if (shouldShowLobby && MultiplayerScreen.instance.isUpdateInput)
        {
            MultiplayerScreen.instance.UpdateStatusText("CREATE / JOIN ROOM");
            MultiplayerScreen.instance.ShowLobbyPanel();
            PhotonNetwork.player.NickName = GlobalClass.PlayerName;
            shouldShowLobby = false;
        }
    }

    public void InitPhoton()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("BurningWheelsBackyard");
            MultiplayerScreen.instance.UpdateStatusText("CONNECTING...");
        }
        else
        {
//			PhotonNetwork.JoinLobby();
        }
    }

    // When connected to Photon, enable nickname editing (too)
    public override void OnConnectedToMaster()
    {
//		Debug.Log("OnConnectedToMaster");
//		PhotonNetwork.JoinLobby();
        MultiplayerScreen.instance.UpdateStatusText("ENTERING LOBBY...");
    }

    // When connected to Photon Lobby, disable nickname editing and messages text, enables room list
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        isPhotonMenuLoaded = true;
        shouldShowLobby = true;
    }

    public override void OnReceivedRoomListUpdate()
    {
        if (LobbyScrollBar.instance != null)
            LobbyScrollBar.instance.UpdateRooms();
    }

    public void OnQuickPlayClick()
    {
        MultiplayerScreen.instance.isUpdateInput = false;
        LobbyScrollBar.instance.HideRooms();
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("Join Random Room Failed, creating new room");
        OnRaceClick("IDnet Room " + UnityEngine.Random.Range(100, 999), Random.Range(0, 5), Random.Range(1, 4));
    }

    // Called from UI
    public void OnCreateRoomClick()
    {
//        Debug.Log("Call Here");
        LobbyScrollBar.instance.HideRooms();
        MultiplayerScreen.instance.UpdateStatusText("CREATE ROOM");
        MultiplayerScreen.instance.ShowRoomCreationPanel(true);
//		RoomOptions options = new RoomOptions ();
//		options.maxPlayers = 4;
//		PhotonNetCwork.CreateRoom (PhotonNetwork.player.name, options, TypedLobby.Default);
    }

    public void OnRaceClick(string roomName, int trackID, int lapCount)
    {
        MultiplayerScreen.instance.isUpdateInput = false;
        //		LobbyScrollBar.instance.HideRooms ();
        currLapCount = lapCount;
        currTrackIndex = trackID;
        currRoomName = roomName;
        int bikeClass = (int)GlobalClass.currChampionshipMode;
        MultiplayerScreen.instance.UpdateStatusText("CREATING ROOM");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
//		options.
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {
                "trackId",
                trackID
            },
            {
                "bikeClass",
                bikeClass
            },
            {
                "lapCount",
                lapCount
            }
        };
        string[] customPropertiesForLobby = new string[3];
        customPropertiesForLobby[0] = "trackId";
        customPropertiesForLobby[1] = "bikeClass";
        customPropertiesForLobby[2] = "lapCount";
        options.CustomRoomPropertiesForLobby = customPropertiesForLobby;
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnPhotonCreateRoomFailed(object[] codeMessage)
    {
        if ((short)codeMessage[0] == ErrorCode.GameIdAlreadyExists)
        {
            currRoomName += "-2";
            OnRaceClick(currRoomName, currTrackIndex, currLapCount);
        }
    }

    // (masterClient only) enables start race button
    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
//		MultiplayerScreen.instance.UpdateStatusText (PhotonNetwork.room.name + " Joined");
//		SetCustomProperties (PhotonNetwork.player, GlobalClass.currentCarIndex, PhotonNetwork.playerList.Length - 1);

    }

    public void OnJoinRoomClicked(RoomDetails _roomDetail)
    {
        MultiplayerScreen.instance.isUpdateInput = false;
        LobbyScrollBar.instance.HideRooms();
        PhotonNetwork.JoinRoom(_roomDetail.roomName);
    }

    // if we join (or create) a room, no need for the create button anymore;
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        MultiplayerScreen.instance.UpdateStatusText("ROOM : " + PhotonNetwork.room.Name);
        if (checkSameNameOnPlayersList(PhotonNetwork.playerName))
        {
            string newName = PhotonNetwork.playerName;
            do
            {
                if (newName.Contains("-"))
                {
                    string[] nameArr = newName.Split('-');

                    int similarNameCount;
                    if (int.TryParse(nameArr[1], out similarNameCount))
                    {
                        similarNameCount++;
                        newName = PhotonNetwork.playerName + "-" + similarNameCount;
                    }
                    else
                    {
                        newName += "1";
                    }
                }
                else
                    newName += "-2";
                Debug.Log(newName);
            } while (checkSameNameOnPlayersList(newName));

            PhotonNetwork.playerName = newName;
        }

        GlobalClass.lapCount = ((int)PhotonNetwork.room.CustomProperties["lapCount"]);
        Debug.Log("multiplayer lapcount : " + GlobalClass.lapCount);
//        int teamIndex = GlobalClass.currentTeamIndex;// = GlobalClass.currentAvatar == AvatarType.Girl ? 0 : 1;
        SetCustomProperties(PhotonNetwork.player, GlobalClass.currentCarIndex, PhotonNetwork.playerList.Length - 1, GlobalClass.currentTeamIndex);
        Debug.Log("Show Loading with Player List");
        LoadingScreen.instance.InitLoadingScreen("Multiplayer");
        MultiplayerScreen.instance.ExitScreen();

//		MultiplayerScreen.instance.ShowRoomCreationPanel (PhotonNetwork.isMasterClient);
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
//		base.OnPhotonJoinRoomFailed(codeAndMsg);
        LobbyScrollBar.instance.ShowRooms();
        MultiplayerScreen.instance.isUpdateInput = true;
        StartCoroutine(ShowRoomJoinFailedMsg());
    }

    IEnumerator ShowRoomJoinFailedMsg()
    {
        MultiplayerScreen.instance.UpdateStatusText("Disconnected From Room");
        yield return new WaitForSeconds(2);
        MultiplayerScreen.instance.UpdateStatusText("Create / Join Room");
    }

    private bool checkSameNameOnPlayersList(string name)
    {
        foreach (PhotonPlayer pp in PhotonNetwork.otherPlayers)
        {
            if (pp.NickName.Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    // If master client, for every newly connected player, sets the custom properties for him
    // car = 0, position = last (size of player list)
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("OnPhotonPlayerConnected : " + newPlayer.NickName);
//		LoadingScreen.GetInstance().UpdatePlayerList();
//		if (PhotonNetwork.isMasterClient)
//		{
//			SetCustomProperties (newPlayer, (int)newPlayer.customProperties ["car"], PhotonNetwork.playerList.Length - 1);
//		}
    }

    // when a player disconnects from the room, update the spawn/position order for all
    public override void OnPhotonPlayerDisconnected(PhotonPlayer disconnetedPlayer)
    {
        if (!isPhotonMenuLoaded)
            return;
        if (PhotonNetwork.isMasterClient)
        {
            int playerIndex = 0;
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
                SetCustomProperties(p, (int)p.CustomProperties["car"], playerIndex++, (int)p.CustomProperties["teamID"]);
            }
        }
//		if (disconnetedPlayer.isMasterClient)
//		{
//			Debug.Log ("Leave Room");
//			LeaveRoom ();
//		}
    }

    // Called from Set Custom property on each client
    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
//		MultiplayerScreen.instance.UpdatePlayerList ();
        LoadingScreen.instance.UpdatePlayerList();
    }

    // all Clients can start race only. Calls an RPC to start the race on all clients. Called from GUI
    public void OnStartRaceClicked()
    {
        photonView.RPC("LoadTrack", PhotonTargets.AllViaServer);
    }

    // Loads race level (called once from masterClient)
    // Use LoadLevel from Photon, otherwise it messes up the GOs created in
    // between level changes
    // The level loaded is related to the track chosen by the Master Client (updated via RPC).
    [PunRPC]
    public void LoadTrack()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;
        }
        isPhotonMenuLoaded = false;
        MainDirector.instance.InitialiseGame((int)PhotonNetwork.room.CustomProperties["trackId"]);
    }

    //	// sets and syncs custom properties on a network player (including masterClient)
    private void SetCustomProperties(PhotonPlayer player, int car, int position, int teamID)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {
                "spawn",
                position
            },
            {
                "car",
                car
            },
            {
				"teamID",
				teamID
			}
			
		};
		player.SetCustomProperties(customProperties);
	}

	// Use this to go back to the menu, without leaving the lobby
	public void LeaveRoom()
	{
		if (PhotonNetwork.room != null)
			PhotonNetwork.LeaveRoom();
		MultiplayerScreen.instance.ShowLobbyPanel();
	}


	public void DisconnectFromPhoton()
	{
		if (PhotonNetwork.connected)
			PhotonNetwork.Disconnect();

		shouldShowLobby = false;
		isPhotonMenuLoaded = false;
	}

}
