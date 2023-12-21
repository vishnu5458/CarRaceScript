using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverEffect : MonoBehaviour
{
	#region Singleton
	private static GameOverEffect _instance;
	public static GameOverEffect instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<GameOverEffect> ();
			}
			return _instance;
		}
	}
	#endregion

	public SpriteRenderer[] gameEffect;
    public Image[] gameEffect2;
   
//	public TextMesh missionStatus;

    private SpriteRenderer targetRenderer;
    public Image targetRenderer2;

	void Awake()
	{
		_instance = this;
	}

	public void SetGameOverStatus(bool status)
	{
    if(status)
		{
//			missionStatus.text = "MISSION COMPLETED";
           // targetRenderer = gameEffect[0];
            targetRenderer2 = gameEffect2[0];
		}
		else
		{
//			missionStatus.text = "MISSION FAILED";
           // targetRenderer = gameEffect[1];
            targetRenderer2 = gameEffect2[1];
		}

		StartCoroutine(LerpProgressBar());
        Debug.Log("GameOver"+status);
		//Invoke("OnGameOverEffectComplete", 3);
       
	}
	
	IEnumerator LerpProgressBar()
	{
         while(targetRenderer2.color.a < 1)
		{
            Color _color = targetRenderer2.color;
			_color.a += 0.01f;
            targetRenderer2.color = _color;
//			missionStatus.color = _color;
            yield return null;
		}
		
		Invoke("OnGameOverEffectComplete",2);

	}

	void OnGameOverEffectComplete()
	{
//		contractFailed.GetComponent<Renderer>().enabled = false;
//		contractCompleted.GetComponent<Renderer>().enabled = false;
		Color _color = Color.white;
		_color.a = 0;
        targetRenderer2.color = _color;
//		missionStatus.color = _color;

		MainDirector.instance.WaitForGameOverEffect();
	}
}
