using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    #region Singleton
    private static TutorialManager _instance;
    public static TutorialManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TutorialManager>();
            }
            return _instance;
        }
    }
    #endregion

    public GameObject baseObject;
    public GameObject[] tutorialObjects;
    public GameObject nextButton;
    public GameObject skipButton;
    public GameObject closeButton;
    public int currIndex = 0;
	// Use this for initialization
	void Awake () 
    {
        _instance = this;	
	}
	
    public void InitTutorial()
    {
        baseObject.SetActive(true);
        currIndex = 0;
        tutorialObjects[currIndex].SetActive(true);

        nextButton.SetActive(true);
        skipButton.SetActive(true);
        closeButton.SetActive(false);
    }

    public void OnNextClick()
    {
        tutorialObjects[currIndex].SetActive(false);
        currIndex += 1;

        if(currIndex == tutorialObjects.Length - 1)
        {
            nextButton.SetActive(false);
            skipButton.SetActive(false);
            closeButton.SetActive(true);
        }
        if(currIndex > tutorialObjects.Length - 1)
            baseObject.SetActive(false);
        else
            tutorialObjects[currIndex].SetActive(true);
        
    }

    public void OnSkipClick()
    {
        baseObject.SetActive(false);
    }
}
