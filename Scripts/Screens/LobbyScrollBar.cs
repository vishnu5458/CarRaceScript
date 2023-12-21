using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class LobbyScrollBar : MonoBehaviour
{

    #region Singleton

    private static LobbyScrollBar _instance;

    public static LobbyScrollBar instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LobbyScrollBar>();
            }
            return _instance;
        }
    }

    #endregion

    public RectTransform contentsRectTrans;
    public GameObject roomsPrefab;

    private List<RoomDetails> roomList = new List<RoomDetails>();

//    private RectTransform lobbyRect;
    //	[SerializeField]
    //	private float width;
    [SerializeField]
    private float height;

    void Awake()
    {
        _instance = this;   
        HideRooms();
    }
    // Use this for initialization

    void Start()
    {
//        lobbyRect = gameObject.GetComponent<RectTransform>();


//		code to move the lobby off stage at start

        //calculate the width and height of each child item.
//		width = contentsRectTrans.rect.width;
//		ratio = width / rowRectTransform.rect.width;
//		height = rowRectTransform.rect.height * ratio;
//		int itemCount = tempitemCount;
//		int rowCount = itemCount;
//		if (itemCount % rowCount > 0)
//			rowCount++;
//
//		//adjust the height of the container so that it will just barely fit all its children
//		float scrollHeight = height * rowCount;
//		Debug.Log(scrollHeight);
//		contentsRectTrans.sizeDelta = new Vector2(0, scrollHeight);
//
//		int i = 0;
//		int j = 0;
//		for(i = 0; i < tempitemCount;)
//		{
//			//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
//			if (i % 1 == 0)
//				j++;
//
//			//create a new item, name it, and set the parent
//			GameObject newItem = Instantiate(roomsPrefab) as GameObject;
//			newItem.name = i.ToString();
//			newItem.transform.SetParent(contentsRectTrans, false);
//			string info = i.ToString();
//			newItem.GetComponentInChildren<Text>().text = info;
//
//
//			//move and size the new item
//			RectTransform rectTransform = newItem.GetComponent<RectTransform>();
//
//			rectTransform.anchoredPosition = new Vector2(0, ((i + 0) * height) - scrollHeight/2 + height/2);
////			Debug.Log(((i + 0) * height) - scrollHeight/2  + height/2);
//
//			i++;
//		}
    }

    public void ShowRooms()
    {
//        Vector3 pos = lobbyRect.anchoredPosition;
//        pos.y = -27.0f;
//        lobbyRect.anchoredPosition = pos;
        GetComponent<ScrollRect>().enabled = true;
    }

    public void HideRooms()
    {
        ClearRooms();
        GetComponent<ScrollRect>().enabled = false;
//        Vector3 pos = lobbyRect.anchoredPosition;
//        pos.y = -800;
//        lobbyRect.anchoredPosition = pos;
    }

    public void UpdateRooms()
    {
//		Debug.Log("OnReceivedRoomListUpdate");

        ClearRooms();

        int itemCount = PhotonNetwork.GetRoomList().Length;

        if (itemCount < 1)
            return;

        int rowCount = itemCount;
        if (itemCount % rowCount > 0)
            rowCount++;

        //adjust the height of the container so that it will just barely fit all its children
        float scrollHeight = height * rowCount;
        contentsRectTrans.sizeDelta = new Vector2(0, scrollHeight);
//		contentsRectTrans.offsetMin = new Vector2(contentsRectTrans.offsetMin.x, -scrollHeight / 2);
//		contentsRectTrans.offsetMax = new Vector2(contentsRectTrans.offsetMax.x, scrollHeight / 2);


        int i = 0;
        int j = 0;
	
        List<RoomInfo> openRooms = new List<RoomInfo>();

        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            if (room.IsOpen)
                openRooms.Add(room);
			
        }

        foreach (RoomInfo room in openRooms)
        {
            //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
            if (i % 1 == 0)
                j++;

            //create a new item, name it, and set the parent
            GameObject newItem = Instantiate(roomsPrefab) as GameObject;
            newItem.name = gameObject.name + " item at (" + i + "," + j + ")";
            newItem.transform.SetParent(contentsRectTrans, false);
            RoomDetails _newRoom = newItem.GetComponent<RoomDetails>();
            _newRoom.roomName = room.Name;
            _newRoom.roomNameText.text = room.Name;
//          Debug.Log(room.customProperties.Count);
            if (room.CustomProperties.ContainsKey("trackId"))
                _newRoom.trackNameText.text = "T " + ((int)room.CustomProperties["trackId"] + 1).ToString();
            if (room.CustomProperties.ContainsKey("bikeClass"))
            {
                int bikeClass = (int)room.CustomProperties["bikeClass"];
                if(bikeClass == 0)
                    _newRoom.bikeClassText.text = "GT3";
                else if(bikeClass == 1)
                    _newRoom.bikeClassText.text = "GT2";
                else
                    _newRoom.bikeClassText.text = "GT1";
            }
            if (room.CustomProperties.ContainsKey("lapCount"))
                _newRoom.lapCountText.text = ((int)room.CustomProperties["lapCount"]).ToString();
            _newRoom.playerCountText.text = room.PlayerCount + "/" + room.MaxPlayers;


            roomList.Add(_newRoom);

            //move and size the new item
            RectTransform rectTransform = newItem.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, (((i + 0) * height) - scrollHeight / 2 + height / 2) + -120);

	
            i++;
        }

    }

    void ClearRooms()
    {
        foreach (RoomDetails _rooms in roomList)
        {
            Destroy(_rooms.gameObject);
        }
        roomList.Clear();

    }
}
