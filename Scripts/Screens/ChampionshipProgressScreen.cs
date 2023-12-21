using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ChampionshipProgressScreen : ScreenBase
{

    #region Singleton

    private static ChampionshipProgressScreen _instance;

    public static ChampionshipProgressScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ChampionshipProgressScreen>();
            }
            return _instance;
        }
    }

    #endregion


    int currentTrack;
    public GameObject[] tracks;
    public TableDetails[] tableValues;
	public Text championshipTitle;

    public Text currentTrackText;
    public Text totalTracks;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
    }

    protected override void Start()
    {
        base.Start();

    }

    public override void InitScreen()
    {
        base.InitScreen();
        OnScreenLoadComplete();
        MainDirector.instance.SortChampionshipTable();
        GlobalClass.championshipPointTable = MainDirector.instance.pointsTable;

        FillPointsTable();
        SetCurrentTrack();
	
    }

    public void FillPointsTable()
    {
        for (int i = 0; i < 8; i++)
        {
            tableValues[i].rank.text = (MainDirector.instance.pointsTable[i].rank + 1).ToString();
            tableValues[i].riderName.text = MainDirector.instance.pointsTable[i].name.ToString();
            tableValues[i].teamName.text = TeamSelectionScreen.instance.teamNames[MainDirector.instance.pointsTable[i].teamID];
            tableValues[i].points.text = MainDirector.instance.pointsTable[i].points.ToString();
        }
    }

    public void SetCurrentTrack()
    {
        //int trackID;
//        Debug.Log(IdnetManager.instance.GetStringFromBoolArray(GlobalClass.champProgressArray));
//        Debug.Log(IdnetManager.instance.GetStringFromIntArray(GlobalClass.class250Tracks));
        int i;
        for(i = 0 ; i < GlobalClass.champProgressArray.Length; i++)
        {
            if (!GlobalClass.champProgressArray[i])
                break;
        }

        totalTracks.text = "/"+GlobalClass.champProgressArray.Length;
        currentTrackText.text = (i + 1).ToString();

        if (GlobalClass.currChampionshipMode == ChampionshipModes.GT1)
        {
            currentTrack = GlobalClass.class250Tracks[i];
			championshipTitle.text = "GT3 CHAMPIONSHIP";
        }
        else if (GlobalClass.currChampionshipMode == ChampionshipModes.GT2)
        {
            currentTrack = GlobalClass.class600Tracks[i];
			championshipTitle.text = "GT2 CHAMPIONSHIP";
        }
        else if (GlobalClass.currChampionshipMode == ChampionshipModes.GT3)
        {
            currentTrack = GlobalClass.class1000Tracks[i];
			championshipTitle.text = "GT1 CHAMPIONSHIP";
        }


        foreach (GameObject track in tracks)
        {
            track.SetActive(false);
        }
        tracks[currentTrack].SetActive(true);
    }

    public override void OnScreenLoadComplete()
    {
        base.OnScreenLoadComplete();
    }

    public override void ExitScreen()
    {
        base.ExitScreen();
    }
       
   
    public override void ButtonClick(Button _button)
    {
        base.ButtonClick(_button);


        if (_button.name == "Back")
        {
            MainDirector.instance.SaveData();
            MenuScreen.instance.InitScreen();
            ExitScreen();
        }
        else if (_button.name == "Play")
        {
			
            GlobalClass.currentTrackIndex = currentTrack;
            MainDirector.instance.InitialiseGame(currentTrack);
            ExitScreen();
        }
        else if (_button.name == "Quit")
        {
            MainDirector.instance.ResetChampionship();
            MenuScreen.instance.InitScreen();
            ExitScreen();
        }

    }

}
