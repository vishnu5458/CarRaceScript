using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievementScreen : ScreenBase, IPointerUpHandler, IPointerDownHandler, IDragHandler 
{
	#region Singleton
	private static AchievementScreen _instance;
	public static AchievementScreen instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<AchievementScreen> ();
			}
			return _instance;
		}
	}
	#endregion

	public Text statusText;
	public GameObject achievementContent;

	private AchievementObject[] achArray;

	protected override void Awake()
	{
		base.Awake();
		_instance = this;

	}
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
//		float pos = 750;
//		achArray = achievementContent.GetComponentsInChildren<AchievementObject>();
//		for(int i = 1; i < achArray.Length; i++)
//		{
//			pos -= 100;
//			Vector3 transPos = achArray[i].GetComponent<RectTransform>().anchoredPosition;
//			transPos.y = pos;
//			achArray[i].GetComponent<RectTransform>().anchoredPosition = transPos;
//		}
	}
	
	public override void InitScreen()
	{
		base.InitScreen();

		OnScreenLoadComplete();
//		IdnetManager.instance.ShowAchievementsUI();
	}
	
	public override void OnScreenLoadComplete()
	{
		Invoke("WaitTimeComplete", 1.0f);

		//statusText.GetComponent<Renderer>().enabled = false;
        statusText.text = "";
        //statusText.
		achArray = achievementContent.GetComponentsInChildren<AchievementObject>();

	}

	void WaitTimeComplete()
	{
//		Vector3 pos = achievementContent.GetComponent<RectTransform>().anchoredPosition;
//		pos.y = 4.0f;
//		achievementContent.GetComponent<RectTransform>().anchoredPosition = pos;
		//statusText.GetComponent<Renderer>().enabled = true;
		achievementContent.GetComponent<ScrollRect>().enabled = true;
        base.OnScreenLoadComplete();
	}

	public override void ExitScreen()
	{
//		achievementContent.SetActive(false);
		achievementContent.GetComponent<ScrollRect>().enabled = false;
//		Vector3 pos = achievementContent.GetComponent<RectTransform>().anchoredPosition;
//		pos.y = -800;
//		achievementContent.GetComponent<RectTransform>().anchoredPosition = pos;
		base.ExitScreen();
	}
	
	public override void ButtonClick(Button _button)
	{
		base.ButtonClick(_button);
		

			
				if(_button.name == "Back")
				{
					ExitScreen();
					MenuScreen.instance.InitScreen();
				}
				else if(_button.name == "More Games" || _button.name == "Sponsor Logo")
				{
					if(!GlobalClass.isHostedFromY8)
						Application.OpenURL(GlobalClass.SponsorLinkMenu);
				}
				else if(_button.name == "Studd Logo")
				{
					Application.OpenURL("http://www.studdgames.com/");
				}

		
	}

//	protected override void ButtonTouchDown (Button _button)
//	{
//		base.ButtonTouchDown (_button);
//		if(_uiButton.name == "More Games" || _uiButton.name == "Sponsor Logo")
//		{
//			if(!GlobalClass.isHostedFromY8)
//				Application.OpenURL(GlobalClass.SponsorLinkMenu);
//		}
//	}
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown was called for object " + gameObject.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp was called for object " + gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging " + gameObject.name);
    }
}