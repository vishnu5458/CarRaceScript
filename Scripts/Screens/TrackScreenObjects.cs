using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrackScreenObjects : GUIButton 
{
	public int trackId;
//	public GameMode missionGameMode;
//	public string trackDetails;

    public Text lapRecord;

	public GameObject lockedObject;
//	public GameObject unlockedObject;
//	public Renderer missionCompleteRenderer;
//	public TextMesh missionType;
//	public TextMesh missionDescription;
//
//	private Vector3 unlockedPos = new Vector3(-4.27f, 3.05f, -0.5f);
//	private Vector3 lockedPos = new Vector3(-0.67f, 4, -0.5f);
//	private bool isUnlocked = true;
	protected override void Awake()
    {
		base.Awake ();
        lapRecord = GetComponentInChildren<Text>();
       
    }
    // Use this for initialization
    protected override void Start () 
    {
        trans = transform;
		trackId = trans.GetSiblingIndex();
//		missionNumber.text = "Track "+ (trackId + 1).ToString();

		base.Start();
	}

	public void Init(int raceRec, bool isUnlocked)
	{
		if(raceRec != 0)
		{
			System.TimeSpan t = System.TimeSpan.FromSeconds(raceRec);
			lapRecord.text = "Best Time : "+string.Format("{0:00} : {1:00}", t.Minutes, t.Seconds); 
//			missionCompleteRenderer.enabled = true;
		}
		else
		{
			lapRecord.text = "Best Time : ---";
//			missionCompleteRenderer.enabled = false;
		}

		if(isUnlocked)
		{
			lockedObject.SetActive(false);
		
		}
		else
		{
			lockedObject.SetActive(true);
		}
	}

	public override void OnRollOver ()
	{
		base.OnRollOver ();
		lapRecord.GetComponent<Renderer>().enabled = true;
//		TrackScreenTutorial.instance.InitTutorial(trackDetails);
	}

	public override void OnRollOff ()
	{
		base.OnRollOff ();
		lapRecord.GetComponent<Renderer>().enabled = false;
//		TrackScreenTutorial.instance.DisableTutorial();
	}

}
