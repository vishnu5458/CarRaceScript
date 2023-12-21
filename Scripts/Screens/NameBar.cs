using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NameBar : MonoBehaviour
{
	public static NameBar nameBar;
    public Text playerNameText;


	void Awake()
	{
		nameBar = this;
	}

	public void InitNameBar()
	{
        playerNameText.text = "player " + Random.Range(0, 100);
	}

	public string GetName()
	{
        if (playerNameText.text == "" || playerNameText.text == " ")
            playerNameText.text = "player " + Random.Range(0, 100);
        return playerNameText.text;
	}
    	
}
