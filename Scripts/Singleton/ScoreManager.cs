using UnityEngine;
using System.Collections;

//public enum ChampionshipProgress
//{
//    none,
//    InProgress,
//    Completed,
//    Failed
//}

public class ScoreObject
{
	public GameMode gameMode;
	public bool isTrackWon = false;
    public int championshipinProgress = -1; // -1 no Championship || 1: in progress || 2: Champ win || 3: Champ fail
	public int totalTime = 0;
	public int trackRecord = 0;

	public int levelScore = 0;
	public int timeBonus = 0;
    public int positionBonus = 0;
	public int healthBonus = 0;
	public int totalCredits = 0;
    public int totalScore = 0;

	public int trafficCarsKilledBonus;
	public int aiCarsKilledBonus;
    public int coinCollectedBonus;
	//race
	public int playerPos = -1;
//	public int bestLapTime;

	//chase timeTrial
	public float playerHealth = 0.0f;

}

public class ScoreManager : MonoBehaviour 
{

	#region Singleton
	private static ScoreManager _instance;
	public static ScoreManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<ScoreManager> ();
			}
			return _instance;
		}
	}
	#endregion

//	public int finalPos;
	public int bestLapTime = 0;
	public int totalLapTime = 0;

	public int coins = 0;
	public int nitroCollected = 0;

	// Use this for initialization
	void Start () 
	{
		_instance = this;
	}
	
	public void OnPlayerLapUp(int _currentLap)
	{
		if(bestLapTime == 0 || _currentLap < bestLapTime)
		{
			bestLapTime = _currentLap;
		}

		totalLapTime += _currentLap;
	}

	public void Reset()
	{
		bestLapTime = 0;
		totalLapTime = 0;
	}
}
