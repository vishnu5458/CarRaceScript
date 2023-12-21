using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementObject : MonoBehaviour 
{
	public Text difficulty;
	public Text achTitle;
	public Text achDescription;
	public Text achStatus;
	public Image lockedIcon;
	public Image lockedShade;

	void Awake()
	{
		difficulty = transform.Find("Difficulty Status").GetComponent<Text>();
		lockedShade = transform.Find("Locked Shade").GetComponent<Image>();
	}
}
