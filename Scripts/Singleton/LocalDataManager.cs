using UnityEngine;
using SimpleJSON;
using System.Collections;

public class LocalDataManager : MonoBehaviour
{

	#region Singleton

	private static LocalDataManager _instance;

	public static LocalDataManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<LocalDataManager>();
			}
			return _instance;
		}
	}

	#endregion

	// Use this for initialization
	void Awake()
	{
		_instance = this;	
	}

	public int TiltControl
	{
		get
		{
			return PlayerPrefs.GetInt("TiltControl", 0);
		}
		set
		{
			PlayerPrefs.SetInt("TiltControl", value);
		}
	}

	public string PlayerName
	{
		get
		{
			if (string.IsNullOrEmpty(GlobalClass.PlayerName))
				GlobalClass.PlayerName = PlayerPrefs.GetString("PlayerName", "player " + Random.Range(0, 100));
			if (GlobalClass.PlayerName == "" || GlobalClass.PlayerName == " ")
				GlobalClass.PlayerName = "player " + Random.Range(0, 100);

			return GlobalClass.PlayerName; 
		}
		set
		{
			GlobalClass.PlayerName = value;
			PlayerPrefs.SetString("PlayerName", GlobalClass.PlayerName);
		}
	}

	public bool IsFirstTime()
	{
		if (PlayerPrefs.HasKey("isFirstTime"))
			return false;
		else
		{
			PlayerPrefs.SetInt("isFirstTime", 0);
			return true;
		}
	}

	public void InitLocalDataManager()
	{

		Load();


		MainDirector.instance.OnDataLoadComplete ();
	}

	public void Save()
	{

        //      PlayerPrefs.SetInt("PlayerCredits", (int)GlobalClass.totalCredits);
        //      PlayerPrefs.SetInt("HighScore", (int)GlobalClass.highScore);
        PlayerPrefsX.SetBoolArray("ChampUnlockedArray", GlobalClass.championshipUnlockedArray);
		PlayerPrefsX.SetBoolArray("TracksUnlockedArray", GlobalClass.trackUnlockedArray);
        PlayerPrefsX.SetBoolArray("CarUnlockedArray", GlobalClass.carUnlockedArray);
		PlayerPrefsX.SetIntArray("TrackRecordArray", GlobalClass.trackRecordArray);

        if (GlobalClass.isChampionship)
        {
            PlayerPrefs.SetInt("ChampionshipMode", (int)GlobalClass.currChampionshipMode);
            PlayerPrefsX.SetStringArray("ChampionshipNameArray", GlobalClass.teamNameArray);
            PlayerPrefsX.SetBoolArray("ChampProgressArray", GlobalClass.champProgressArray);

            JSONClass _save = new JSONClass();
            ChampionshipPointsTable[] pointsTable = GlobalClass.championshipPointTable;
            for (int i = 0; i < pointsTable.Length; i++)
            {
                _save["ChampTable"][i]["isActiveCar"] = pointsTable[i].isActiveCar.ToString();
                _save["ChampTable"][i]["name"] = pointsTable[i].name;
                _save["ChampTable"][i]["teamID"] = pointsTable[i].teamID.ToString();
                _save["ChampTable"][i]["carModel"] = pointsTable[i].carModel.ToString();
                _save["ChampTable"][i]["points"] = pointsTable[i].points.ToString();
                _save["ChampTable"][i]["speedBoost"] = pointsTable[i].speedBoost.ToString();
            }
//            Debug.Log(_save.ToString());
            PlayerPrefs.SetString("ChampionShipTable", _save.ToString());
        }
        else
        {
            PlayerPrefs.SetInt("ChampionshipMode", -1);
            PlayerPrefsX.SetBoolArray("ChampProgressArray", GlobalClass.champProgressArray);
            PlayerPrefs.SetString("ChampionShipTable", "");
        }

		if (!PlayerPrefs.HasKey("FirstTimeKey"))
			PlayerPrefs.SetInt("FirstTimeKey", 0);

		PlayerPrefs.Save();
	}

	public void Load()
	{
		GlobalClass.PlayerName = PlayerName;

//		GlobalClass.totalCredits = PlayerPrefs.GetInt("PlayerCredits", 1000);
//		GlobalClass.highScore = PlayerPrefs.GetInt("HighScore", 0);
        GlobalClass.championshipUnlockedArray = PlayerPrefsX.GetBoolArray("ChampUnlockedArray", false, 3);
		GlobalClass.trackUnlockedArray = PlayerPrefsX.GetBoolArray("TracksUnlockedArray", false, 10);
        GlobalClass.carUnlockedArray = PlayerPrefsX.GetBoolArray("CarUnlockedArray", false, 10);
		GlobalClass.trackRecordArray = PlayerPrefsX.GetIntArray("TrackRecordArray", 0, 10);

        int championShipMode = PlayerPrefs.GetInt("ChampionshipMode",-1);
        if(championShipMode == -1)
        {
            MainDirector.instance.ResetChampionship();
        }
        else
        {
            GlobalClass.newChampionship = false;
            GlobalClass.currChampionshipMode = (ChampionshipModes)championShipMode;
            GlobalClass.teamNameArray = PlayerPrefsX.GetStringArray("ChampionshipNameArray");
            GlobalClass.champProgressArray = PlayerPrefsX.GetBoolArray("ChampProgressArray");

            JSONNode _loadData = JSONData.Parse(PlayerPrefs.GetString("ChampionShipTable"))["ChampTable"];
            for(int i = 0; i < _loadData.Count; i++)
            {
                ChampionshipPointsTable _tempTable = new ChampionshipPointsTable();
                _tempTable.isActiveCar = _loadData[i]["isActiveCar"].AsBool;
                _tempTable.name = _loadData[i]["name"];
                _tempTable.teamID = _loadData[i]["teamID"].AsInt;
                _tempTable.carModel = _loadData[i]["carModel"].AsInt;
                _tempTable.points = _loadData[i]["points"].AsInt;
                _tempTable.speedBoost = _loadData[i]["speedBoost"].AsInt;
                GlobalClass.championshipPointTable[i] = _tempTable;
            }
        }

		if (!PlayerPrefs.HasKey("FirstTimeKey"))
		{
			Debug.Log("first time saving");
			Save();
		}
	}
    public int GetTutorialCount()
    {
        return PlayerPrefs.GetInt("Tutorial", 0);
    }
    public void SetTutorialCount(int id)
    {
        PlayerPrefs.SetInt("Tutorial", id);
    }

}
