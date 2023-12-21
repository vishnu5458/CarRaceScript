using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassScreenObjects : MonoBehaviour
{
    public bool isClassUnlocked = false;
    public GameObject lockedObject;
    public GameObject unlockText;


    // Use this for initialization
    void Start()
    {
	
    }

    public void Init(bool isUnlocked)
    {
        isClassUnlocked = isUnlocked;
        if(isUnlocked)
            lockedObject.SetActive(false);
        
        unlockText.SetActive(false);
    }

    public void OnMouseEnter()
    {
        if (isClassUnlocked)
            return;

        unlockText.SetActive(true);
    }

    public void OnMouseExit()
    {
        if (isClassUnlocked)
            return;

        unlockText.SetActive(false);
    }
}
