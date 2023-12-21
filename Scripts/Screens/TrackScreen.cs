using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrackScreen : ScreenBase
{

    #region Singleton

    private static TrackScreen _instance;

    public static TrackScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TrackScreen>();
            }
            return _instance;
        }
    }

    #endregion

    public Text lapTxt;
    public Transform trackMap;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject[] soundUI;
    public GameObject[] mobileObjects;

    [SerializeField]
    private int lapCount;
    private int noTracks;
    private int currentIndex;
    [SerializeField]
    private TrackScreenObjects[] trackScreenObj;

    //	public TextMesh playerName;
    //	public TextMesh playerRank;



    protected override void Awake()
    {
        _instance = this;

        trackScreenObj = GetComponentsInChildren<TrackScreenObjects>();

        #if UNITY_WEBPLAYER || UNITY_WEBGL
        foreach (GameObject go in mobileObjects)
            go.SetActive(false);
        #else
        foreach(GameObject go in mobileObjects)
            go.SetActive(true);
        #endif
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

    }

    public override void InitScreen()
    {
        base.InitScreen();

//		titleGameObject.GetComponent<ObjectRandomShake>().EnableShake();
        lapCount = 2;
        lapTxt.text = lapCount.ToString();
        noTracks = trackScreenObj.Length;
		currentIndex = GlobalClass.currentTrackIndex;


//		Vector3 pos = trackMap.localPosition;
//		pos.x = 18 * -currentIndex;
//		trackMap.localPosition = pos;

	
        int i;

        for (i = 0; i < trackScreenObj.Length; i++)
        {
            trackScreenObj[i].gameObject.SetActive(true);
            trackScreenObj[i].Init(GlobalClass.trackRecordArray[i], GlobalClass.trackUnlockedArray[i]);
        }

        ClearTracks();
//        currentIndex = 0;
        ToStage(currentIndex, 30);

//		if(GlobalClass.SoundVoulme == 1)
//		{
//			soundUI[0].SetActive(true);
//			soundUI[1].SetActive(false);
//		}
//		else
//		{
//			soundUI[1].SetActive(true);
//			soundUI[0].SetActive(false);
//		}
        OnScreenLoadComplete();
    }

    public override void OnScreenLoadComplete()
    {
        base.OnScreenLoadComplete();
    }

    public override void ExitScreen()
    {
        base.ExitScreen();
        OffStage(currentIndex, 30);
    }

    public override void ButtonClick(Button _button)
    {
        base.ButtonClick(_button);

	
        if (_button.name == "Play")
        {	
            if (!GlobalClass.trackUnlockedArray[currentIndex])
                return;
            ExitScreen();
            foreach (TrackScreenObjects tso in trackScreenObj)
                tso.gameObject.SetActive(false);

            GlobalClass.currentTrackIndex = currentIndex;
            GlobalClass.lapCount = lapCount;
            MainDirector.instance.InitialiseGame(trackScreenObj[currentIndex].trackId);
        }
        else if (_button.name == "iA")
        {
            DecreaseLap();
        }
        else if (_button.name == "rA")
        {
            IncreaseLap();
        }
		else if (_button.name == "Arrow L")
        {
            PrevTrack();
        }
        else if (_button.name == "Arrow R")
        {
            NextTrack();
        }
        else if (_button.name == "Mute")
        {
            if (GlobalClass.SoundVoulme == 1)
            {
                GlobalClass.SoundVoulme = 0;
                soundUI[1].SetActive(true);
                soundUI[0].SetActive(false);
            }
            AudioManager.instance.PlayMuteBG();
					
        }
        else if (_button.name == "UnMute")
        {
            if (GlobalClass.SoundVoulme == 0)
            {
                GlobalClass.SoundVoulme = 1;
                soundUI[0].SetActive(true);
                soundUI[1].SetActive(false);
            }
            AudioManager.instance.PlayMuteBG();
        }
        else if (_button.name == "Back")
        {
            foreach (TrackScreenObjects tso in trackScreenObj)
                tso.gameObject.SetActive(false);

            GlobalClass.currentTrackIndex = currentIndex;
            TeamSelectionScreen.instance.InitScreen();
            ExitScreen();
        }
	
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.K))
        {
            GlobalClass.trackUnlockedArray[currentIndex] = true;
        }
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

    void ClearTracks()
    {
        for(int i = 0; i< noTracks; i++)
        {
            OffStage(i, 1000);
        }
    }

    void PrevTrack()
    {
//		currentIndex--;
//
//		MoveTrackMap();

        OffStage(currentIndex, 1000);
        currentIndex--;
		
//		if(currentIndex < 0)
//			currentIndex = noTracks - 1;
		
        ToStage(currentIndex, -30);
    }

    void NextTrack()
    {
//		currentIndex++;
//
//		MoveTrackMap();

        OffStage(currentIndex, -1000);
        currentIndex++;
		
//		if(currentIndex > noTracks - 1)
//			currentIndex = 0;
		
        ToStage(currentIndex, 30);
    }

    void MoveTrackMap()
    {
        if (currentIndex < 1)
            leftArrow.SetActive(false);
        else
            leftArrow.SetActive(true);
		
        if (currentIndex > noTracks - 2)
            rightArrow.SetActive(false);
        else
            rightArrow.SetActive(true);

        Vector3 pos = trackMap.localPosition;
        pos.x = (18 * -currentIndex);
		
        Hashtable ht = new Hashtable();
        ht.Add("position", pos);
        ht.Add("time", .5f);
        ht.Add("islocal", true);
        ht.Add("ignoretimescale", true);
        iTween.MoveTo(trackMap.gameObject, ht);
    }

    void OffStage(int trackIndex, float toPosition)
    {
        Hashtable ht = new Hashtable();
        ht.Add("x", toPosition);
        ht.Add("time", .5f);
        ht.Add("islocal", true);
        ht.Add("ignoretimescale", true);
        iTween.MoveTo(trackScreenObj[trackIndex].gameObject, ht);
    }

    void ToStage(int trackIndex, float fromPosition)
    {
        if (trackIndex < 1)
            leftArrow.SetActive(false);
        else
            leftArrow.SetActive(true);

        if (trackIndex > noTracks - 2)
            rightArrow.SetActive(false);
        else
            rightArrow.SetActive(true);
		
        trackScreenObj[trackIndex].trans.localPosition = new Vector3(fromPosition, 0, 0);

        Debug.Log("track index:" + trackIndex);
		
        Hashtable ht = new Hashtable();
        ht.Add("x", 0);
        ht.Add("time", .5f);
        ht.Add("islocal", true);
        ht.Add("ignoretimescale", true);
        iTween.MoveTo(trackScreenObj[trackIndex].gameObject, ht);
    }
	
}
