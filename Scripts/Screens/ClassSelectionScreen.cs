using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClassSelectionScreen : ScreenBase
{
    
    #region Singleton

    private static ClassSelectionScreen _instance;

    public static ClassSelectionScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ClassSelectionScreen>();
            }
            return _instance;
        }
    }

    #endregion

    private ClassScreenObjects[] classScreenObj;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
        classScreenObj = GetComponentsInChildren<ClassScreenObjects>();
    }

    protected override void Start()
    {
        base.Start();

    }

    public override void InitScreen()
    {
        base.InitScreen();
        OnScreenLoadComplete();

        for (int i = 0; i < classScreenObj.Length; i++)
        {
            if (GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer)
                classScreenObj[i].Init(true);
            else
                classScreenObj[i].Init(GlobalClass.championshipUnlockedArray[i]);
        }
    }

    public override void OnScreenLoadComplete()
    {
        base.OnScreenLoadComplete();
    }

    public override void ExitScreen()
    {
        base.ExitScreen();
    }

    void ChampionshipSelection()
    {
        TeamSelectionScreen.instance.InitScreen();
        ExitScreen();

//        if (GlobalClass.isChampionship)
//        {
//            GlobalClass.newChampionship = false;
//        }
//        else
//        {
//            ChampionshipProgressScreen.instance.InitScreen();
//            ChampionshipProgressScreen.instance.FillPointsTable(champMode);
//            ExitScreen();
//        }
    }

    public override void ButtonTouchDown(Button _button)
    {
        base.ButtonTouchDown(_button);
        if (_button.name.Contains("class"))
        {
            _button.GetComponent<ClassScreenObjects>().OnMouseEnter();
        }
    }

    public override void ButtonTouchUP(Button _button)
    {
        base.ButtonTouchUP(_button);
        if (_button.name.Contains("class"))
        {
            _button.GetComponent<ClassScreenObjects>().OnMouseExit();
        }
    }

    public override void ButtonClick(Button _button)
    {
        base.ButtonClick(_button);

        if (_button.name.Contains("class"))
        {
            if (!_button.GetComponent<ClassScreenObjects>().isClassUnlocked)
                return;
        }

        if (_button.name == "class250")//GT3
        {	
            GlobalClass.currentCarIndex = 0;
            GlobalClass.currChampionshipMode = ChampionshipModes.GT1;
            ChampionshipSelection();
        }
        else if (_button.name == "class600")//GT2
        {
            GlobalClass.currentCarIndex = 2;
            GlobalClass.currChampionshipMode = ChampionshipModes.GT2;
            ChampionshipSelection();
        }
        else if (_button.name == "class1000")//GT1
        {
            GlobalClass.currentCarIndex = 4;
            GlobalClass.currChampionshipMode = ChampionshipModes.GT3;
            ChampionshipSelection();
        }
        else if (_button.name == "Back")
        {
            MenuScreen.instance.InitScreen();
            ExitScreen();
        }

    }

}
