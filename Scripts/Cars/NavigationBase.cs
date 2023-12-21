using UnityEngine;
using System.Collections;

public class NavigationBase : MonoBehaviour
{
    public Transform carPosDot;
    [HideInInspector]
    public bool isRaceCompleted;
    //  public Waypoint currentWaypoint;
    //  [HideInInspector]
    public DistWP currDistWP;
    public int positionWeight;
    [HideInInspector]
    public int totalTime;
    [HideInInspector]
    public int lapTime;
    [HideInInspector]
    public int currentLap;

    //  [HideInInspector]
    public bool isSuperCheatEnabled = false;
    int cheatSpeed = 300;

    protected Vector3 distTarget;
    protected int Total_waypoint;
    protected float distInc;

    private bool isGameOver;
    private int waypointOffsetTimer;
    //  private float waypointOffset;
    private float magnitude;
    private DistWP endWaypoint;
    private DistWP startDistWp;
    private VehicleBase thisCar;

    //traffic variables
//    private int trafficSpread;
//    private int trafficOffset;
//    private int trafficCount;


    private bool isDebug = false;


    // Use this for initialization
    void Awake()
    {
        //      trans = transform;
        thisCar = GetComponent<VehicleBase>();
        isSuperCheatEnabled = false;
        //      if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName("GameManager").isLoaded)
        //          isDebug = true;
    }

    void FixedUpdate()
    {
        if (isSuperCheatEnabled)
        {
            Vector3 targetPos = currDistWP.thisTransform.position;
            targetPos.y = thisCar.trans.position.y;
            transform.LookAt(targetPos);
            transform.Translate(Vector3.forward * cheatSpeed * Time.deltaTime);
            //          transform.forward += Vector3.forward * Time.deltaTime;
        }
    }

    public void InitWayPoints(DistWP _startDistWP, DistWP _endWP)
    {
        //      startWpId = _startWp.id;
        //      currentWaypoint = _startWp;
        endWaypoint = _endWP;

        currDistWP = startDistWp = _startDistWP;
        Transform distWPtrans = startDistWp.thisTransform;
        distTarget = new Vector3(distWPtrans.position.x, 0, distWPtrans.position.z);

        currentLap = 1;
        Transform waypoint = currDistWP.nextDistWaypoint.thisTransform;
        thisCar.target = new Vector3(waypoint.position.x, thisCar.trans.position.y, waypoint.position.z);
        SetWaypointOffset();
              float z = -0.25f;
              if (thisCar.isActiveVehicle)
                  z = -0.5f;
              carPosDot.localPosition = new Vector3(16, -160f, z);
        distInc = 320.0f / (GlobalClass.totalLaps*GlobalClass.totalWP);
//        Debug.Log("distInc::"+distInc);   

        positionWeight = 0;

        int carId = _startDistWP.id;
              if (carId > GlobalClass.playerStartWPiD)
              {
                  Vector3 dotPos = carPosDot.localPosition;
                  while (GlobalClass.playerStartWPiD != carId)
                  {
                      carId -= 1;
                      dotPos.y += distInc;
                      dotPos.y = Mathf.Clamp(dotPos.y, -160.0f, 160.0f);
                      carPosDot.localPosition = dotPos;
                  }
              }

        carId = _startDistWP.id;
        if (carId > GlobalClass.playerStartWPiD)
        {
            while (GlobalClass.playerStartWPiD != carId)
            {
                carId -= 1;
                positionWeight += 1;
            }
        }

//        trafficSpread = MainDirector.GetCurrentGamePlay()._trafficClass.trafficSpread;
//        trafficOffset = MainDirector.GetCurrentGamePlay()._trafficClass.trafficOffset;
//        trafficCount = MainDirector.GetCurrentGamePlay()._trafficClass.iterationsPerSpawnCall;
    }

    public void ShowCarDot(bool status)
    {
                carPosDot.GetComponent<UnityEngine.UI.Image>().enabled = status;
    }

    public void SetWaypointOffset()
    {
        //      waypointOffset = Random.Range(-0.5f, 0.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!thisCar.isCarEnabled)
            return;

        if (other.GetComponent<DistWP>())
        {
            DistWP distWp = other.GetComponent<DistWP>();
            //          if (distWp.hasWaypoint)
            //              OnWpTriggerEnter(distWp.currentWaypoint);
            OnDistWpTriggerEnter(distWp);
        }
    }

    //  void OnWpTriggerEnter(Waypoint closeWp)
    //  {
    //      if (closeWp == currentWaypoint)
    //      {
    //          currentWaypoint = currentWaypoint.nextWaypoint;
    //
    //            Transform waypointTrans = currentWaypoint.nextWaypoint.nextWaypoint.nextWaypoint.thisTransform;
    //          thisCar.target = new Vector3(waypointTrans.position.x, 0, waypointTrans.position.z);
    //
    ////            float x = waypointTrans.localScale.x;
    ////            SetWaypointOffset();
    ////            thisCar.target += waypointTrans.right * x * waypointOffset;
    //
    //          if (!isDebug)
    //          {
    ////                Vector3 dotPos = carPosDot.localPosition;
    ////                dotPos.y += distInc;
    ////                dotPos.y = Mathf.Clamp(dotPos.y, -6.0f, 6.0f);
    ////                carPosDot.localPosition = dotPos;
    //              //          Debug.Log(distInc);
    //              if (closeWp == endWaypoint)
    //              {
    //                  currentLap++;
    //                  MainDirector.GetCurrentGamePlay().OnTrackComplete(thisCar, totalTime);
    //              }
    //              if (thisCar.isActiveVehicle)
    //              {
    //                  GamePlayScreen.instance.SetWrongWay(false);
    //              }
    //          }
    //      }
    //      else
    //      {
    //          if (thisCar.isActiveVehicle && !isDebug)
    //          {
    //              int waypointDiff = currentWaypoint.id - closeWp.id;
    //              if (waypointDiff > 3)
    //              {
    //                  Vector3 relativePosition = thisCar.trans.InverseTransformPoint(thisCar.target);
    //                  if (relativePosition.z < 0)
    //                  {
    //                      //show wrong way
    //                      GamePlayScreen.instance.SetWrongWay(true);
    //
    //                      if (waypointDiff > 10)
    //                      {
    //                          thisCar.OnVehicleReset();
    //                      }
    //                  }
    //                  else
    //                  {
    //                      GamePlayScreen.instance.SetWrongWay(false);
    //                  }
    //              }
    //          }
    //      }
    //  }

    public void OnDistWpTriggerEnter(DistWP closeDistWp)
    {
//        if (thisCar.isActiveVehicle && trafficOffset != 0)
//        {
//            if (currDistWP.id % trafficSpread == 0)
//                TrafficManager.instance.SpawnTrafficCar(trafficOffset, trafficCount);
//        
//        }

        if (closeDistWp == currDistWP)
        {
            currDistWP = closeDistWp.nextDistWaypoint;

            if (GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer || (GlobalClass.gameMode == GlobalClass.GameMode.Multiplayer && thisCar.isActiveVehicle))
                positionWeight++;

            Transform waypointTrans = currDistWP.nextDistWaypoint.thisTransform;
            thisCar.target = new Vector3(waypointTrans.position.x, thisCar.trans.position.y, waypointTrans.position.z);

            //          float x = waypointTrans.localScale.x;
            //          SetWaypointOffset();
            //          thisCar.target += waypointTrans.right * x * waypointOffset;

            if (!isDebug)
            {
                              Vector3 dotPos = carPosDot.localPosition;
                              dotPos.y += distInc;
                              dotPos.y = Mathf.Clamp(dotPos.y, -160.0f, 160.0f);
                              carPosDot.localPosition = dotPos;
                //          Debug.Log(distInc);
                if (currDistWP == endWaypoint)
                {
                    currentLap++;
                    MainDirector.GetCurrentGamePlay().OnTrackComplete(thisCar, totalTime);
                }
                if (thisCar.isActiveVehicle)
                {
                    GamePlayScreen.instance.SetWrongWay(false);
                }
            }
        }
        else
        {
            if (thisCar.isActiveVehicle && !isDebug)
            {
                int waypointDiff = currDistWP.id - closeDistWp.id;
                if (waypointDiff > 3)
                {
                    Vector3 relativePosition = thisCar.trans.InverseTransformPoint(thisCar.target);
                    if (relativePosition.z < 0)
                    {
                        //show wrong way
                        GamePlayScreen.instance.SetWrongWay(true);

                        if (waypointDiff > 10)
                        {
                            Debug.Log("Reset on waypoint");
                            thisCar.OnVehicleReset();
                        }
                    }
                    else
                    {
                        GamePlayScreen.instance.SetWrongWay(false);
                    }
                }
            }
        }
    }
}
