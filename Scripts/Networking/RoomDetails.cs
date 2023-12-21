using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomDetails : MonoBehaviour 
{
	public string roomName;

	public Text roomNameText;
	public Text trackNameText;
    public Text bikeClassText;
    public Text lapCountText;
	public Text playerCountText;


	public void OnRoomClicked()
	{
		PhotonMenu.instance.OnJoinRoomClicked(this);
	}
}
