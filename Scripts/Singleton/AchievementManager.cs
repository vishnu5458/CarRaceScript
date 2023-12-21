using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class AchievementDefnition
{
	public string achievementId;
	public string achievementName;
	public string unlockKey;
}

public class AchievementManager : MonoBehaviour
{
	#region Singleton
	private static AchievementManager _instance;
	public static AchievementManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<AchievementManager> ();
			}
			return _instance;
		}
	}
	#endregion

    private int nitroCollected;
    private int coinCollected;
    private int speedReducerCollected;
    private int playerCrashCount;

	public AchievementDefnition[] _achievementObj;

	// Use this for initialization
	void Awake () 
	{
		_instance = this;

        nitroCollected = PlayerPrefs.GetInt("nitroCollected",0);
        coinCollected = PlayerPrefs.GetInt("coinCollected",0);
        speedReducerCollected = PlayerPrefs.GetInt("speedReducerCollected",0);
        playerCrashCount = PlayerPrefs.GetInt("playerCrashCount",0);
	}

    public void OnNitroCollect()
    {
//        Debug.Log("OnNitroCollected"+nitroCollected);
        if(nitroCollected < 10)
        {
            nitroCollected++;
//            Debug.Log("nitroCollected:::"+nitroCollected);
            PlayerPrefs.SetInt("nitroCollected",nitroCollected);

        }
    }

    public void OnCoinCollect()
    {
        if(coinCollected < 15)
        {
            coinCollected++;
            PlayerPrefs.SetInt("coinCollected",coinCollected);
        }
    }

    public void OnSpeedReducerCollect()
    {
        if(coinCollected < 15)
        {
            speedReducerCollected++;
            PlayerPrefs.SetInt("speedReducerCollected",coinCollected);
        }
    }

    public void OnPlayerCrash()
    {
        if(playerCrashCount < 10)
        {
            playerCrashCount++;
            PlayerPrefs.SetInt("playerCrashCount",playerCrashCount);
        }
    }


	public AchievementDefnition GetAchievementFromName(string _achievementId)
	{
		AchievementDefnition achieveObj = new AchievementDefnition();
		foreach(AchievementDefnition _def in _achievementObj)
		{
			if(string.Compare(_def.achievementId, _achievementId) == 0)
				achieveObj = _def;
		}
		return achieveObj;
	}
}
