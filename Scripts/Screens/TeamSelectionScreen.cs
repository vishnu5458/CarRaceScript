using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TEAM
{
    HAVOC,
    VIPER,
    LIGHTENING,
    BLADE
}

[System.Serializable]
public class ChampionshipTeams
{
    public string teamID;
    public string[] teamPlayers;
}

public class TeamSelectionScreen : ScreenBase
{


    #region Singleton

    private static TeamSelectionScreen _instance;

    public static TeamSelectionScreen instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TeamSelectionScreen>();
            }
            return _instance;
        }
    }

    #endregion


    //garage
    public Transform platformTrans;
    public Transform carParent;

    public Camera garageCamera;
    public GarageCars[] carsGO;
    //public GameObject playerAvatars;
    public GameObject[] lockTexts;
    public GameObject lockIcon;
    public Light[] garageLights;

    public Text carName;
    public GameObject inputText;
    public GameObject customTeamButton;
    public GameObject customTeamPopup;
    private int modelIndex = 0;

    // public TextMesh carName;

    //    private int noCars;
    [SerializeField]
    private int currentBikeIndex;
    private int currentTeamIndex;
    public GameObject avatar;

    public BikeBase[] bikeBase;
    //garage
//    public Transform[] carPos;

    public ChampionshipTeams[] TeamsArray;

    //    public TEAM team;
    public string[] teamNames;
    public Text teamNameTextBox;
//    bool iscam=false;
    public Transform[] CarImage;
    public GameObject rightArrow;
    public GameObject leftArrow;
    public GameObject[] arrows;
    private int index;
    private string[] CarText={"RENEGADE","SCORPION","RAGER","BULLET","THUNDER","SPIRIT"};

//    public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
//    public float panSpeed = 4.0f;       // Speed of the camera when being panned
//    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

//    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
//    private bool isPanning;     // Is the camera being panned?
//    private bool isRotating;    // Is the camera being rotated?
//    private bool isZooming; 

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
//        noCars = carsGO.Length;
    }

    protected override void Start()
    {
        base.Start();
        //team = TEAM.HAVOC;
//        teamNameTextBox.text = teamNames[GlobalClass.currentTeamIndex];
    }

    public override void InitScreen()
    {
//        modelIndex = 0;
        base.InitScreen();
        teamNames = new string[]{ "Rage Racing", "Speed Motorsports", "Team Tacho", "NXT Racing","Infinity Racing","Range Motorsports" };

        //playerAvatars.SetActive(true);
        garageCamera.enabled = true;
//        iscam = true;
        for (int i = 0; i < carsGO.Length; i++)
        {
            OffStage(i);
        }
        //team = TEAM.HAVOC;
        OnScreenLoadComplete();

    }

    public override void OnScreenLoadComplete()
    {
        base.OnScreenLoadComplete();
//        if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer && GlobalClass.isChampionship)
//            customTeamButton.SetActive(true);
//        else
//            customTeamButton.SetActive(false);
        
        if (GlobalClass.currChampionshipMode == ChampionshipModes.GT1)
        {
//            GlobalClass.currentCarIndex = 0;
        }
        else if (GlobalClass.currChampionshipMode == ChampionshipModes.GT2)
        {
            GlobalClass.currentCarIndex = 2;
        }
        else if (GlobalClass.currChampionshipMode == ChampionshipModes.GT3)
        {
            GlobalClass.currentCarIndex = 4;
        }
        ClearStage();
        foreach (GameObject _go in lockTexts)
            _go.SetActive(false);
        lockIcon.SetActive(false);

        ToStage(GlobalClass.currentCarIndex, 20);
        currentTeamIndex = UnityEngine.Random.Range(0, 4);
//        ChangeMaterial(currentTeamIndex);
//        carName.text = carsGO[GlobalClass.currentCarIndex].carName;   
//        carName.text = carsGO[GlobalClass.currentCarIndex].carName;  
    }

    public override void ExitScreen()
    {
        base.ExitScreen();
        garageCamera.enabled = false;
        ClearStage();
    }
    void Update()
    {
        if (!garageCamera.enabled)
            return;

/*        if(Input.GetMouseButtonDown(0))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }
        if(Input.GetMouseButtonDown(1))
        {
//             Get mouse origin
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        if (!Input.GetMouseButton(0)) isRotating=false;
        if (!Input.GetMouseButton(1)) isPanning=false;
        if (!Input.GetMouseButton(2)) isZooming=false;

        if (isRotating)
        {
            Vector3 pos = garageCamera.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            garageCamera.transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
            garageCamera.transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
        }

        // Move the camera on it's XY plane
        if (isPanning)
        {
            Vector3 pos = garageCamera.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            garageCamera.transform.Translate(move, Space.Self);
        }*/
/*       if (Input.GetMouseButtonDown(0)&& iscam==true)
        {
            Debug.Log("Mouse is down");

            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out hitInfo,100)) 
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (hitInfo.transform.gameObject.tag == "car1")
                {
                    Debug.Log ("It's working!");
                }
                else if (hitInfo.transform.gameObject.tag == "car2")
                {
                    Debug.Log ("It's working!");
                }
                else if (hitInfo.transform.gameObject.tag == "car3")
                {
                    Debug.Log ("It's working!");
                }
                else if (hitInfo.transform.gameObject.tag == "car4")
                {
                    Debug.Log ("It's working!");
                }
                else if (hitInfo.transform.gameObject.tag == "car5")
                {
                    Debug.Log ("It's working!");
                }
                else if (hitInfo.transform.gameObject.tag == "car6")
                {
                    Debug.Log ("It's working!");
                }
                else {
                    Debug.Log ("nopz");
                }
            } else {
                Debug.Log("No hit");
            }
            Debug.Log("Mouse is down");
        } */
    }

    public override void ButtonClick(Button _button)
    {
        base.ButtonClick(_button);

//        Debug.Log("team#" + (int)team);

        if (_button.name == "NextTeam")
        {
            NextTeam();
        }
        else if (_button.name == "PrevTeam")
        {
            PrevTeam();
        }
        else if (_button.name == "Back")
        {
//            ClassSelectionScreen.instance.InitScreen();
            MenuScreen.instance.InitScreen();
            ExitScreen();
        }
        else if (_button.name == "Race")
        {
            GlobalClass.currentTeamIndex = currentTeamIndex;
            if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
            {
                if (!GlobalClass.carUnlockedArray[GlobalClass.currentCarIndex])
                    return;
                SetGlobalPointsTable(currentTeamIndex);
                if (GlobalClass.isChampionship)
                {
                    GlobalClass.newChampionship = false;
                    GlobalClass.teamNameArray = teamNames;
                    ChampionshipProgressScreen.instance.InitScreen();
                }
                else
                {
                    TrackScreen.instance.InitScreen();
                }
            }
            else
            {
                MultiplayerScreen.instance.InitScreen();
            }
//            customTeamPopup.SetActive(false);
            ExitScreen();
        }
        else if (_button.name == "NextCar")
        {
            ChangeModelNex();
        }
        else if (_button.name == "PrevCar")
        {
            ChangeModelPrev();
        }
        else if (_button.name == "CustomTeam")
        {
//            customTeamPopup.SetActive(true);
        }
        else if (_button.name == "CustomTeamNext")
        {
            string teamName = inputText.GetComponent<InputField>().text;
            teamNames[currentTeamIndex] = teamName;
            teamNameTextBox.text = teamNames[currentTeamIndex];
//            customTeamPopup.SetActive(false);
        }
        else if (_button.name == "Sponsor Logo")
        {
            if (!GlobalClass.isHostedFromY8)
                Application.OpenURL(GlobalClass.SponsorLinkMenu);

        }


    }

    void ChangeModelNex()
    {
//        if (modelIndex == 0)
//        {
//            modelIndex = 1;
//            carName.text = "Car 1";
//        }
//        else
//        {
//            modelIndex = 0;
//            carName.text = "Car 2";
//        }
        GlobalClass.currentCarIndex++;
        if (GlobalClass.currentCarIndex > 5)
        {
            GlobalClass.currentCarIndex = 0;
        }
      
        ClearStage();
        ToStage(GlobalClass.currentCarIndex , 20);
//        ChangeMaterial(currentTeamIndex);
    }
    void ChangeModelPrev()
    {
        GlobalClass.currentCarIndex--;
        if (GlobalClass.currentCarIndex < 0)
        {
            GlobalClass.currentCarIndex = 5;
        }

        ClearStage();
        ToStage(GlobalClass.currentCarIndex, 20);
//        ChangeMaterial(currentTeamIndex);
    }
    public void SetGlobalPointsTable(int playerTeamID)
    {
     
        int i = 0;
        int[] intTeamArray = new int[6] { 0, 0, 0, 0,0,0 };
        while (i < 5)
        {
            int index = i;//UnityEngine.Random.Range(0, 4);
            if (intTeamArray[index] < 2)
            {
                if (index == playerTeamID)
                {                    
                    if (intTeamArray[index] < 1)
                    {
                        intTeamArray[index] = modelIndex == 0 ? 1 : 0;
                    }
                    else
                        continue;
                }
				
                MainDirector.instance.pointsTable[i].isActiveCar = false;
                MainDirector.instance.pointsTable[i].name = TeamsArray[index].teamPlayers[intTeamArray[index]];
                MainDirector.instance.pointsTable[i].teamID = index;
                MainDirector.instance.pointsTable[i].carModel = intTeamArray[index];
                MainDirector.instance.pointsTable[i].points = 0;
                MainDirector.instance.pointsTable[i].rank = i;
                MainDirector.instance.pointsTable[i].speedBoost = UnityEngine.Random.Range(-5, 5);

//                intTeamArray[index]++;
            }
            else
                continue;
            i++;
        }

        MainDirector.instance.pointsTable[i].isActiveCar = true;
        MainDirector.instance.pointsTable[i].name = GlobalClass.PlayerName;
        MainDirector.instance.pointsTable[i].teamID = playerTeamID;
        MainDirector.instance.pointsTable[i].carModel = modelIndex;
        MainDirector.instance.pointsTable[i].points = 0;
        MainDirector.instance.pointsTable[i].rank = i;
        MainDirector.instance.pointsTable[i].speedBoost = 0;
              
    }

    void PrevTeam()
    {
        currentTeamIndex--;
        if (currentTeamIndex < 0)
            currentTeamIndex = 5;
//        OffStage(currentBikeIndex);
       
//        ChangeMaterial(currentTeamIndex);
      
//        SetBikeClass(currentBikeIndex, -20);
    }

    void NextTeam()
    {
        currentTeamIndex++;
        if (currentTeamIndex > 5)
            currentTeamIndex = 0;
//        OffStage(currentBikeIndex);
//        ChangeMaterial(currentTeamIndex);
//        Debug.Log("Call here");
//        SetBikeClass(currentBikeIndex, 20);
    }

    void ChangeMaterial(int teamIndex)
    {
//        Debug.Log(teamIndex);
        teamNameTextBox.text = teamNames[teamIndex];
        BikeSkinRenderer skinRenderer = carsGO[GlobalClass.currentCarIndex + modelIndex].GetComponentInChildren<BikeSkinRenderer>();

        foreach (Renderer ren in skinRenderer.skinRenderers)
            ren.material = TeamMaterialManager.instance.teamTextureArray[GlobalClass.currentCarIndex].modelClass[0].carTexture[teamIndex];
//        avatar.GetComponent<Renderer>().material = TeamMaterialManager.instance.teamTextureArray[teamIndex].charecTexture;
    }

    void ClearStage()
    {
        foreach(GarageCars _garageCars in carsGO)
        {
            _garageCars.trans.parent = carParent;
            _garageCars.trans.position = _garageCars.initPos;
        }
    }

    void OffStage(int carIndex)
    {
        carsGO[carIndex].trans.parent = carParent;
        carsGO[carIndex].trans.position = carsGO[carIndex].initPos;
         lockTexts[carIndex].SetActive(false);
    }


    void ToStage(int carIndex, float fromPosition)
    {
        carName.text = carsGO[carIndex].carName;    
        carsGO[carIndex].trans.parent = platformTrans;
        carsGO[carIndex].trans.localPosition = Vector3.zero;
        carsGO[carIndex].trans.localEulerAngles = Vector3.zero;
        if (!GlobalClass.carUnlockedArray[carIndex] && (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer))
        {
            foreach (GameObject _lock in lockTexts)
                _lock.SetActive(false);
            lockTexts[carIndex].SetActive(true);
            lockIcon.SetActive(true);
        }
        else
        {
            
            foreach (GameObject _lock in lockTexts)
                _lock.SetActive(false);
            lockIcon.SetActive(false);
        }
    }

    void ToScreen(int index,float toPos)
    {
//        Debug.Log("Next");
//        foreach (GameObject arraow in arrows)
//            arraow.SetActive(true);
        if (GlobalClass.currentCarIndex > 4)
            rightArrow.SetActive(false);
        if (GlobalClass.currentCarIndex < 1)
        {
            leftArrow.SetActive(false);
        }
        carName.text=CarText[GlobalClass.currentCarIndex];
//        foreach (GameObject Dot in Dots)
//        {
//            Dot.SetActive(false);
//        }
//
//        if (index == 0)
//        {
//            LeftArrow.SetActive(false);
//            GlobalClass.currentCarIndex = 0;
//            GlobalClass.currChampionshipMode = ChampionshipModes.GT1;
//        }
//        else
//        { 
//            LeftArrow.SetActive(true);
//        }
//        if (index == 4)
//        {
//            RightArrow.SetActive(false);
//            GlobalClass.currentCarIndex = 8;
//            GlobalClass.currChampionshipMode = ChampionshipModes.GT5;
//        }
//        else{
//            RightArrow.SetActive(true);
//        }
//        if (index == 1)
//        {
//            GlobalClass.currentCarIndex = 2;
//            GlobalClass.currChampionshipMode = ChampionshipModes.GT2;
//        }else if(index==2)
//        {
//            GlobalClass.currentCarIndex = 4;
//            GlobalClass.currChampionshipMode = ChampionshipModes.GT3;
//        }else if(index==3)
//        {
//            GlobalClass.currentCarIndex = 6;
//            GlobalClass.currChampionshipMode = ChampionshipModes.GT4;
//        }
//        Dots[index].SetActive(true);
        Hashtable ht = new Hashtable();
        ht.Add("x", toPos);
        ht.Add("time", .5f);
        ht.Add("islocal", true);
        ht.Add("ignoretimescale", true);
        iTween.MoveTo(CarImage[index].gameObject, ht);
    }

    void NextCar()
    {
        foreach (GameObject arraow in arrows)
            arraow.SetActive(true);
        index++;
        Debug.Log("index::"+index);
        if (index > 4)
        {
            Debug.Log("Call here");
            rightArrow.SetActive(false);
        }
        ClearStage();
        ToStage(GlobalClass.currentCarIndex , 20);
    }
    void PrevCar()
    {
        foreach (GameObject arraow in arrows)
            arraow.SetActive(true);
        index--;
        Debug.Log("index::"+index);
        if (index < 1)
        {
            Debug.Log("Call here");
            leftArrow.SetActive(false);
        }
        ClearStage();
        ToStage(GlobalClass.currentCarIndex, 20);
    }
    void ClearScreen(int index,float exitPos)
    {
        Hashtable ht = new Hashtable();
        ht.Add("x", exitPos);
        ht.Add("time", .5f);
        ht.Add("islocal", true);
        ht.Add("ignoretimescale", true);
        iTween.MoveTo(CarImage[index].gameObject, ht);

    }
}
