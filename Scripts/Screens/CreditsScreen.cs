using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsScreen : ScreenBase
{
    #region Singleton

    private static CreditsScreen _instance;

    public static CreditsScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CreditsScreen>();
            }
            return _instance;
        }
    }

    #endregion

    //	public GameObject[] email;
    //	public GameObject names;
	
    iTweenEvent initTween;
    //	int clickCounter = 0;
	


    protected override void Awake()
    {
        base.Awake();
        _instance = this;
//		initTween = GetComponent<iTweenEvent>();
//		initPosition = trans.localPosition;
    }
	
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public override void InitScreen()
    {
        base.InitScreen();
//		clickCounter = 0;
//		email[0].SetActive(true);
//		email[1].SetActive(true);
//		names.SetActive(false);
//		trans.localPosition = initPosition;
//		initTween.Play();
        OnScreenLoadComplete();
    }

    public override void OnScreenLoadComplete()
    {
        base.OnScreenLoadComplete();
    }

    public override void ExitScreen()
    {
        base.ExitScreen();
    }

    protected override void UpdateInput()
    {
        base.UpdateInput();

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.L))
        {
            Debug.Log("unlock all");
            for (int i = 0; i < GlobalClass.trackUnlockedArray.Length; i++)
            {
                GlobalClass.trackUnlockedArray[i] = true;
                //              GlobalClass.timedUnlockedArray[i] = true;
            }
            for (int i = 0; i < GlobalClass.carUnlockedArray.Length; i++)
            {
                GlobalClass.carUnlockedArray[i] = true;
                //              GlobalClass.timedUnlockedArray[i] = true;
            }

        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.K))
        {
            GlobalClass.totalCredits += 10000;
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Delete player prefs");
            PlayerPrefs.DeleteAll();
        }
    }

    public override void ButtonClick(Button _button)
    {
        base.ButtonClick(_button);

      if (_button.name == "Back")
        {
//					Hashtable ht = new Hashtable();
//					ht.Add("time", .5f);
//					ht.Add("islocal",true);
//					ht.Add("y",15);
//					iTween.MoveTo(gameObject, ht);	
            ExitScreen();
            MenuScreen.instance.InitScreen();
        }
        else if (_button.name == "More Games" || _button.name == "Sponsor Logo")
        {
            if (!GlobalClass.isHostedFromY8)
                Application.OpenURL(GlobalClass.SponsorLinkMenu);
        }
        else if (_button.name == "Studd Logo")
        {
            Application.OpenURL("http://www.studdgames.com/");
               
        }
    }


}
