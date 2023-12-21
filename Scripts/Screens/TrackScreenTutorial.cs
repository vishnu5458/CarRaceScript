using UnityEngine;
using System.Collections;

public class TrackScreenTutorial : MonoBehaviour
{
	#region Singleton
	private static TrackScreenTutorial _instance;
	public static TrackScreenTutorial instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<TrackScreenTutorial> ();
			}
			return _instance;
		}
	}
	#endregion

	Renderer[] tutorialRenderers;
	TextMesh tutText;

	void Awake()
	{
		_instance = this;
		tutorialRenderers = GetComponentsInChildren<Renderer>();
		tutText = GetComponentInChildren<TextMesh>();

		DisableTutorial();
	}

	public void InitTutorial(string tutorialText)
	{
		foreach(Renderer ren in tutorialRenderers)
			ren.enabled = true;

		tutText.text = tutorialText;
	}

	public void DisableTutorial()
	{
		foreach(Renderer ren in tutorialRenderers)
			ren.enabled = false;
	}
}
