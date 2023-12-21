using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using EVP;

public enum Drive
{
    Front,
    Rear,
    All
}

public class CarBase : VehicleBase
{

    [HideInInspector]
    public bool isEscortCar = false;

    [HideInInspector]
    public bool isDelayedStart = false;
    protected VehicleController vehicleController;
    //  protected CarController thisVehicleController;

    private ParticleSystem carFirePS;

    public Renderer[] bikeSkinRenderer;

    protected bool isNitroActivated = false;
    [HideInInspector]
    public bool isNitroSet = false;
    [HideInInspector]
    public float nitroUpdateRate;
    public float nitroMeter = 0;
    private int incSpeed=0;

    #region Car Physics Variables

    protected bool isCarBlinking = false;
    int blinkFlipTimer;
    int currentBoostStatus = 0;
    bool isInvisible = false;
    [HideInInspector]
    public int blinkInterval = 2;
    public bool isResetting = false;
    public GameObject bodyRender;
    public Renderer[] otherRender;


    public ParticleSystem[] nitroParticles;
    private ParticleSystem smokeParticle;
    public GameObject[] thisCollider;

    #endregion

    #region engine Audio

//	protected VehicleAudio vehicleAudio;
    protected ToyCarAudio toyCarAudio;

    #endregion


    protected override void Awake()
    {
        trans = transform;
        body = GetComponent<Rigidbody>();
        initCarLife = carLife;

        vehicleController = GetComponent<VehicleController>();
//        vehicleAudio = GetComponent<VehicleAudio>();
        toyCarAudio = GetComponent<ToyCarAudio>();


        thisCollider = new GameObject[1];
        thisCollider[0] = trans.Find("Collider").GetChild(0).gameObject;


        carFirePS = trans.Find("Particles/CarFire").GetComponent<ParticleSystem>();
        smokeParticle = trans.Find("Particles/Smoke Small").GetComponent<ParticleSystem>();

        Transform _nitroParent = trans.Find("Particles/_nitro");
        nitroParticles = new ParticleSystem[_nitroParent.childCount];
        for (int i = 0; i < _nitroParent.childCount; i++)
        {
            //          countDownCamPos[i] = new Transform();
            nitroParticles[i] = _nitroParent.GetChild(i).GetComponent<ParticleSystem>();
        }

        foreach (ParticleSystem _nitro in nitroParticles)
            _nitro.Stop();
        smokeParticle.Stop();
        smokeParticle.Clear();

        otherRender = bodyRender.GetComponentsInChildren<Renderer>();
        if (isPlayer)
        {
            if (GetComponent<PlayerNavigation>() == null)
                navBase = gameObject.AddComponent<PlayerNavigation>();
            else
                navBase = GetComponent<PlayerNavigation>();
        }
        else
        {
            if (GetComponent<AiNavigation>() == null)
                navBase = gameObject.AddComponent<AiNavigation>();
            else
                navBase = GetComponent<AiNavigation>();
        }
    }

    void Start()
    {
        initCarLife = carLife;
    }

    public override void PositionVehicleAndWayPoint(PosWP _posWP, DistWP _trackEndWP)
    {
        trans.position = _posWP.objTransform.position;
        trans.eulerAngles = _posWP.objTransform.eulerAngles;

        navBase.InitWayPoints(_posWP.playerStartWP, _trackEndWP); //  _startWP, _endWP, _startDistWP
        InitCar();
        ApplyHandBrake();

        body.isKinematic = false;
        vehicleController.enabled = true;
        AiCarPerform();
        base.PositionVehicleAndWayPoint(_posWP, _trackEndWP);
    }
    public void AiCarPerform()
    {
//        Debug.Log("Call here");
        switch (GlobalClass.currentCarIndex)
        {
            case 0:
                incSpeed = 1;
                break;
            case 1:
                incSpeed = 4;
                break;
            case 2:
                incSpeed = 8;
                break;
            case 3:
                incSpeed = 12;
                break;
            case 4:
                incSpeed = 16;
                break;
            case 5:
                incSpeed = 20;
                break;
        }
        vehicleController.SetSpeed(incSpeed);

    }
    public override void InitCar()
    {
        base.InitCar();
        navBase.totalTime = 0;
        navBase.lapTime = 0;

        if (isActiveVehicle && isDamage)
            GamePlayScreen.instance.PlayerCarHealthBarLerp(carLife / initCarLife);
        otherRender = bodyRender.GetComponentsInChildren<Renderer>();

        foreach (Renderer wheelRen in otherRender)
            wheelRen.enabled = true;
        carLife = initCarLife;
    }

    public override void EnableVehicle()
    {
        ReleaseHandBrake();
        navBase.ShowCarDot(true);
        isCarEnabled = true;
        vehicleController.isCarEnabled = true;

        if (isDelayedStart && isDamage)
        {
            floatingHealthBar.SetVisibility(true);
        }
    }

    public void OnArrested()
    {
        carLife = 0;
        GetComponent<AICar>().isBotArrested = true;
        MainDirector.GetCurrentGamePlay().AiCarKilled += 1;
        if (floatingHealthBar != null)
            floatingHealthBar.SetVisibility(false);
        DisableCar();
        GamePlayScreen.instance.UpdateArrestBar(0);
    }

    public override void DisableCar()
    {
        SetSound(false);
        navBase.ShowCarDot(false);
        ImmobilizeCar();

        isCarEnabled = false;
        vehicleController.isCarEnabled = false;
    }

    public override void SetSound(bool status)
    {
        if (status)
        {
            toyCarAudio.enabled = true;
//            vehicleAudio.enabled = true;
        }
        else
        {
            toyCarAudio.StopAudio();
            toyCarAudio.enabled = false;
//            vehicleAudio.enabled = false;
        }
        if (GetComponentInChildren<CopLights>())
        {
            GetComponentInChildren<CopLights>().SetSound(status);
        }

        base.SetSound(status);
    }

    public override void OnSecondsTick()
    {
        base.OnSecondsTick();
        //      //add code for time count here
        //      navBase.totalTime += 1;
        //      navBase.lapTime += 1;
    }

    public void DebugLog(object logString)
    {
        if (isLogEnabled)
            Debug.Log(gameObject.name + " : " + logString.ToString());
    }

    #region Power Train

    protected override void  Update()
    {
        handBrakeInput = 0;

        if (trans.position.y < -55)
        {
            Debug.Log("reseting on low position");
            OnVehicleReset();
        }

        if (speed > 30)
        {
            if (!isNitroActivated && nitroMeter <= 100 && !isNitroSet)
            {
                nitroMeter += nitroUpdateRate * Time.deltaTime;
                if (isActiveVehicle)
                {
                    GamePlayScreen.instance.UpdateNitroMeter(nitroMeter / 100.0f);

                }
            }
        }

//        if (nitroMeter >= 100)
//        {
//            isNitroSet = true;
//            if (!isActiveVehicle)
//            {
//                if(GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
//                  BoostCollected(1);
//            }
//            else
//            {
//                GamePlayScreen.instance.SetNitro(true);
//                if ((LocalDataManager.instance.GetTutorialCount() < 1) && GlobalClass.gameMode==GlobalClass.GameMode.SinglePlayer)
//                {
//                    GamePlayScreen.instance.InitTutorial(1);
//                }
//            }
//        }

        if (isCarBlinking)
        {
            blinkFlipTimer--;
            if (blinkFlipTimer < 0)
            {
                if (isInvisible)
                {
                                        bodyRender.SetActive(true);
//                    foreach (Renderer wheelRen in otherRender)
//                        wheelRen.enabled = true;
                    isInvisible = false;
                }
                else
                {
                                        bodyRender.SetActive(false);
//                    foreach (Renderer wheelRen in otherRender)
//                        wheelRen.enabled = false;
                    isInvisible = true;
                }

                blinkFlipTimer = blinkInterval;
            }
        }
    }

    protected override void FixedUpdate()
    {
        UpdatePowerTrain();
        UpdateSteering();
        speed = vehicleController.speed;
        vehicleController.Move(steer, acce, handBrakeInput);

        Vector3 velo = body.velocity;
        if (Mathf.Abs(velo.y) > 10)
        {
            velo.y = Mathf.Clamp(velo.y, -10, 10);
            body.velocity = velo;
        }

        if (MainDirector.instance == null)
            return;

        if (isActiveVehicle)
            WeatherManager.instance.UpdateRainAngle(speed);
    }

    protected override void UpdatePowerTrain()
    {

    }

    protected override void UpdateSteering()
    {

    }

    #endregion

    public override void OnVehicleReset()
    {
        if (!isCarEnabled)
            return;




        Transform prevWPTrans;
        prevWPTrans = navBase.currDistWP.prevDistWaypoint.thisTransform;

        Vector3 eulerAngles = prevWPTrans.eulerAngles;
        body.MoveRotation(Quaternion.Euler(0, eulerAngles.y, 0));
        body.MovePosition(prevWPTrans.position + Vector3.up * 1.0f);

        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        foreach (GameObject go in thisCollider)
            go.layer = 13;
        isResetting = true;
        DebugLog("resetting car ");

        CancelInvoke("DisableColliderTigger");
        Invoke("DisableColliderTigger", 3);

        isCarBlinking = true;
    }

    public void DisableColliderTigger()
    {
        if (IsInvoking("DisableColliderTigger"))
        {
            CancelInvoke("DisableColliderTigger");
        }

        isCarBlinking = false;
        isResetting = false;
                bodyRender.SetActive(true);
//        foreach (Renderer wheelRen in otherRender)
//            wheelRen.enabled = true;

        foreach (GameObject go in thisCollider)
            go.layer = isPlayer ? 11 : 12;

    }

    public void ImmobilizeCar()
    {
        vehicleController.Immobilize();
    }
    public override void ApplySuperCheat()
    {
        //      navBase.isSuperCheatEnabled = !navBase.isSuperCheatEnabled;
    }

    public override void ApplyHandBrake()
    {
        vehicleController.ApplyHandBrake();
    }

    public override void ReleaseHandBrake()
    {
        vehicleController.ReleaseHandBrake();
    }

    #region Boost, Damage, oildrum

    public void BoostCollected(int status)
    {
        if (currentBoostStatus != status)
        {
            DisableBoost();
            currentBoostStatus = status;
        }

        if (!IsInvoking("DisableBoost"))
        {
            if (status == 1)
            {
//                foreach (ParticleSystem _nitro in nitroParticles)
//                    _nitro.Play();
//                ParticleManager.instance.PlayNitroAtPoint();
            }
            else if (status == -1)
            {
                smokeParticle.Play();
            }
            isNitroActivated = true;
            nitroMeter = 0;
            isNitroSet = false;
            vehicleController.OnBoostEnable(status);
//            vehicleAudio.OnBoostEnable(status);
            toyCarAudio.OnBoostEnable(status);

            if (isActiveVehicle)
            {
                SmoothFollow.instance.OnEnableNitro(status);
            }
        }
        else
        {
            CancelInvoke("DisableBoost");
        }

        Invoke("DisableBoost", 5);
    }

    public void DisableBoost()
    {
        if (IsInvoking("DisableBoost"))
        {
            CancelInvoke("DisableBoost");
        }

        DebugLog("disabled nitro");

        vehicleController.OnNitroDisable();
//        vehicleAudio.OnNitroDisable();
        toyCarAudio.OnNitroDisable();
        isNitroActivated = false;


        if (isActiveVehicle)
        {
            SmoothFollow.instance.OnDisableNitro();
            GamePlayScreen.instance.SetNitro(false);
        }

        if (currentBoostStatus == 1)
        {
            ParticleManager.instance.StopNitroAtPoint();
//            foreach (ParticleSystem _nitro in nitroParticles)
//            {
//                _nitro.Stop();
//                _nitro.Clear();
//            }
        }
        else if (currentBoostStatus == -1)
        {
            float lifePerCent = carLife / initCarLife;
            if (!isDamage || lifePerCent > 0.25f)
            {
                smokeParticle.Stop();
            }
        }
        currentBoostStatus = 0;
    }

    public void ResetLayer()
    {
        foreach (GameObject go in thisCollider)
            go.layer = isPlayer ? 11 : 12;
    }

    //  public void EngageTwister()
    //  {
    //      PowerupManager.instance.ActivateTwister(navBase.currentWaypoint, trans.position, 8.0f, body);
    //  }

    public void OnOilDrumHit()
    {
        if (!IsInvoking("OnOilDrumRecover"))
        {
            //          isOilDrumHit = true;
            //          maxAudioPitch -= 0.4f;
        }
        else
        {
            CancelInvoke("OnOilDrumRecover");
        }
        Invoke("OnOilDrumRecover", 4);
    }

    public void OnOilDrumRecover()
    {
        if (IsInvoking("OnOilDrumRecover"))
        {
            CancelInvoke("OnOilDrumRecover");
        }
    }

    public void ApplyDamage(float damage)
    {
        if (!isCarEnabled)
            return;

        carLife -= damage;
        carLife = Mathf.Clamp(carLife, -initCarLife, initCarLife);

        float lifePerCent = carLife / initCarLife;
        if (lifePerCent < 0.25f)
        {
            smokeParticle.Play();
        }
        else
        {
            smokeParticle.Stop();
        }

        if (isActiveVehicle)
        {
            GamePlayScreen.instance.PlayerCarHealthBarLerp(lifePerCent);
        }
        else if (isEscortCar)
        {
            GamePlayScreen.instance.UpdateBigHealthBar(lifePerCent);
        }
        else
        {
            if (floatingHealthBar != null)
                floatingHealthBar.SetHealth(lifePerCent);
        }

        if (carLife < 0)
        {
            if (bodyRender.GetComponent<Renderer>())
            {
                bodyRender.GetComponent<Renderer>().material.mainTexture = damagedTextures;
                foreach (Transform childTrans in bodyRender.transform)
                    childTrans.gameObject.SetActive(false);
            }
            carFirePS.Play();
            ParticleManager.instance.PlayBigExplosionAtPoint(trans.position);
            AudioManager.instance.PlayBlastSoundAtPos(trans.position);
            isCarEnabled = false;

            vehicleController.ApplyHandBrake();
            body.drag = 0.5f;

            body.AddForce(0, 0.05f, 0, ForceMode.Impulse);
            Invoke("OnCarDestroyed", 3);

            foreach (ParticleSystem _nitro in nitroParticles)
                _nitro.Stop();
            smokeParticle.Stop();

        }
    }

    void OnCarDestroyed()
    {
        navBase.positionWeight = 0;
        if (isActiveVehicle)
        {
            carLife = 0;
            GlobalClass.gameOverCause = "Player_Killed";
            MainDirector.GetCurrentGamePlay().GameOver();
        }
        else
        {
            if (!isEscortCar)
                MainDirector.GetCurrentGamePlay().AiCarKilled += 1;
            if (floatingHealthBar != null)
                floatingHealthBar.SetVisibility(false);
            DisableCar();
        }
    }

    public float GetLifePercent()
    {
        if (carLife == 0)
        {
            return 0;
        }
        return carLife / initCarLife;
    }

    #endregion

    public override void OnGameEnd()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.Sleep();
        body.isKinematic = true;

        DisableCar();
        SetSound(false);
        DisableBoost();
    }

    #region Collision

    protected override void OnTriggerEnter(Collider other)
    {
        if (!isCarEnabled)
            return;

        if (other.GetComponent<PowerUp>())
        {
            PowerUp pow = other.GetComponent<PowerUp>();
            if (pow)
            {
                if (isActiveVehicle)
                {
                    if (pow.thisType == PowerUpType.AmmoSmall || pow.thisType == PowerUpType.AmmoLarge)
                    {
                        if (pow.isActivated)
                            return;
                        PlayerControl.GetInstance().OnAmmoCollect(pow.thisType == PowerUpType.AmmoSmall ? 1 : 2);
                        //                      pow.ActivateCollectable(trans.position);
                        pow.gameObject.SetActive(false);
                    }
                    AudioManager.instance.PlayNitroCollectSound();
                }

                if (pow.thisType == PowerUpType.Nitro)
                {
                    if (isActiveVehicle)
                    {
                        ParticleManager.instance.PlayNitroAtPoint();
                        AchievementManager.instance.OnNitroCollect();
                    }
                    BoostCollected(1);
                }
                else if (pow.thisType == PowerUpType.Stopper)
                {
                    if(isActiveVehicle || GlobalClass.gameMode == GlobalClass.GameMode.SinglePlayer)
                        BoostCollected(-1);
                }
                else if (pow.thisType == PowerUpType.Coin)
                {
                    if (isActiveVehicle)
                    {
                        AudioManager.instance.PlayCoinSound();
                        AchievementManager.instance.OnCoinCollect();
                        MainDirector.GetCurrentGamePlay().CoinsCollected++;
                        pow.gameObject.SetActive(false);
                    }
                }
                else if (pow.thisType == PowerUpType.Health)
                {
                    float damage;
                    if (isActiveVehicle || isEscortCar)
                    {
                        damage = 0.3f * initCarLife;
                    }
                    else
                    {
                        damage = 0.1f * initCarLife;
                    }
                    ApplyDamage(-damage);
                }
            }
        }
        //      else if(other.GetComponent<Health>())
        //      {
        //          if(!other.GetComponent<Health>().isActivated)
        //          {
        //              if(isActiveVehicle)
        //              {
        //                  AudioManager.audioManager.PlayNitroCollectSound();
        //              }
        //              ApplyDamage(-other.GetComponent<Health>().healthValue);
        //          }
        //      }

    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (!isCarEnabled || didFinish)
            return;

        float magnitude = other.relativeVelocity.magnitude;
//        float damage = 0;

//        if (!SceneManager.GetSceneByName("GameManager").isLoaded)
//            return;

//        Debug.Log("OnCollisionEnter");
//        if (other.gameObject.GetComponent<Explosives>())
//        {
//            Explosives obs = other.gameObject.GetComponent<Explosives>();
//            obs.ApplyDamage(magnitude / 10);
//
//        }
        if ((magnitude > 10 || (magnitude > 5 && speed > 15)) && other.gameObject.layer != 17) //layer 17 is track base
        {
//            Debug.Log("call here");
            Vector3 pos = other.contacts[0].point;
//            ParticleManager.instance.PlayCollisionSpark(pos);
//            float vol = Mathf.Clamp(magnitude / 50, 0, 0.6f);
//            AudioManager.instance.PlayCrashSoundAtPos(pos, vol);

            //          if( other.gameObject.layer == 19)
            //          {
            //              damage = magnitude/8;
            //          }
//            if (other.gameObject.GetComponent<TrafficCar>())
//            {
//                if (!isEscortCar)
//                    damage = magnitude / 5;
//            }
//            else if (other.gameObject.GetComponent<CarBase>())
//            {
//                damage = magnitude / 5;
//            }
            //play sound

            //          Debug.Log("collided with : " + other.gameObject.name + " damage : " + damage);
        }
//        if (isDamage && damage != 0)
//            ApplyDamage(damage);
    }

    #endregion

}
