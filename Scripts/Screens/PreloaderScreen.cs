using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PreloaderScreen : ScreenBase
{

	#region Singleton
	private static PreloaderScreen _instance;
	public static PreloaderScreen instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<PreloaderScreen>();
			return _instance;
		}
	}
	#endregion

	public Renderer[] loadingIndicator;
    public GameObject loadingIndicator1;
    public GameObject blackListObject;

	private int lightIndex = 0;
	private bool isPreloaderTimeOut = false;
	private bool isAutoLoginComplete = false;

    public Image load;
    bool fillImage=false;
    float timer=0;

	private bool IsHostedFromY8Net()
	{
		string absoluteURL = Application.absoluteURL;
	
		return false;
	}

	protected override void Awake ()
	{

        fillImage = true;
		Invoke("loadMainGame",2.5f);


		base.Awake ();

		InitScreen();

		foreach(Renderer ren in loadingIndicator)
			ren.enabled = false;
		//		StartCoroutine(LoadingLerp());
	}

	IEnumerator LoadingLerp()
	{
		int i = 0;
		foreach(Renderer ren in loadingIndicator)
			ren.enabled = false;
		while(true)
		{
			yield return new WaitForSeconds(0.15f);

			loadingIndicator[i].enabled = false;
			i++;
			if(i > loadingIndicator.Length -1)
				i = 0;
			loadingIndicator[i].enabled = true;
		}
	}

	public override void InitScreen ()
	{
		base.InitScreen ();

		OnScreenLoadComplete();
	}

	public override void OnScreenLoadComplete ()
	{
		base.OnScreenLoadComplete ();
	}

	public override void ExitScreen ()
	{
		base.ExitScreen ();
	}

	/*protected override void UpdateInput ()
	{
		base.UpdateInput ();

		if(Time.frameCount % 15 == 0)
		{
			loadingIndicator[lightIndex].enabled = false;
			lightIndex++;
			if(lightIndex > loadingIndicator.Length -1)
				lightIndex = 0;
			loadingIndicator[lightIndex].enabled = true;
		}
		//			loadingIndicator.Rotate(Vector3.forward, -45);

		if(isPreloaderTimeOut && isAutoLoginComplete)
		{
			ExitScreen();
			MainDirector.instance.WaitBeforeLoadingData();
		}

		if(selectButton)
		{
			if(selectButton == _uiButton)
			{
				if(_uiButton.name == "Sponsor Logo" )
				{
					if(!GlobalClass.isHostedFromY8)
						Application.OpenURL(GlobalClass.SponsorLinkPreloader);
				}
				else if(_uiButton.name == "IdNet")
				{
					Application.OpenURL(GlobalClass.IDNet);
				}

				selectButton = null;
			}
		}
	}*/

	public override void ButtonClick(Button _button)
	{
		base.ButtonClick(_button);
		if(Time.frameCount % 15 == 0)
		{
			loadingIndicator[lightIndex].enabled = false;
			lightIndex++;
			if(lightIndex > loadingIndicator.Length -1)
				lightIndex = 0;
			loadingIndicator[lightIndex].enabled = true;
		}
		//			loadingIndicator.Rotate(Vector3.forward, -45);

		if(isPreloaderTimeOut && isAutoLoginComplete)
		{
			//Debug.Log ("preloader timeout");
			ExitScreen();
			MainDirector.instance.WaitBeforeLoadingData();
		}

		if(_button.name == "Sponsor Logo" )
		{
			if(!GlobalClass.isHostedFromY8)
				Application.OpenURL(GlobalClass.SponsorLinkPreloader);
		}
		else if(_button.name == "IdNet")
		{
			Application.OpenURL(GlobalClass.IDNet);
		}



	}

	void Update()
	{
        if (fillImage)
        {
            timer += Time.deltaTime;
            if (timer <= 2)
            {
                load.fillAmount = timer / 2;
            }
            else
            {
                timer = 0;
                fillImage = false;
            }
        }
        if (Time.frameCount % 15 == 0)
        {
            
//            RectTransform rotate = loadingIndicator1.GetComponent<RectTransform>();
//            rotate.Rotate(Vector3.forward, -45);
        }
		//Debug.Log ("update");
        if(isPreloaderTimeOut)// && isAutoLoginComplete)
		{
			//Debug.Log ("preloader timeout");
            MainDirector.instance.WaitBeforeLoadingData();
            ExitScreen();
		}
	}

	void loadMainGame()
	{
		isPreloaderTimeOut = true;
	}

	public void AutoLoginComplete()
	{
		isAutoLoginComplete = true;
	}
}
