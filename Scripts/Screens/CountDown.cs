using UnityEngine;
using System.Collections;

public class CountDown : MonoBehaviour
{
    //	[HideInInspector]
    //	public TextMesh countDownTB;

    public GameObject[] countDownGO;
    //  public GameObject GoGameObject;

    public AudioClip shortBeep;
    public AudioClip longBeep;
    public Transform countDownCamTrans;
    public Transform[] countDownCamPosArray;

    //  private Transform lookAtTrans;
    private iTweenEvent thisTweenEvent;
    private Transform trans;
    private int state;
    private bool _isTimerRunning;

    public static CountDown countDownTimer;

    public GameObject countBase1;
    public GameObject countBase2;
    public GameObject[] countBase;
    // private bool isRotate=false;
    private float yRot=0;

    public bool IsTimerRunning
    {
        get
        {
            return _isTimerRunning;
        }
    }

    // Use this for initialization
    void Awake()
    {
        countDownTimer = this;
        //      countDownTB = GetComponent<TextMesh>();
        thisTweenEvent = GetComponent<iTweenEvent>();
        trans = transform;
    }
    public void InitializeCountDown()
    {
        _isTimerRunning = true;

        foreach (GameObject _go in countBase)
            _go.SetActive(false);
        if (MainDirector.GetCurrentGamePlay().currentMode == GameMode.Race)
        {
            state = 1;
            //      countDownTB.text = "3";
            foreach (GameObject _go in countDownGO)
                _go.SetActive(false);
        }
        else
        {
            state = 3;
            foreach (GameObject _go in countDownGO)
                _go.SetActive(false);
        }
        //      GoGameObject.SetActive(false);
        AudioSource.PlayClipAtPoint(shortBeep, Vector3.zero, GlobalClass.SoundVoulme);

        trans.localScale = Vector3.zero;
        //  countDownGO[3].SetActive(true);
        thisTweenEvent.Values["oncomplete"] = "CountDownTweenUpdate";
        thisTweenEvent.Play();

        countDownCamPosArray = MainDirector.GetCurrentPlayer().GetComponent<PlayerCar>().countDownCamPos;
        //      lookAtTrans = PlayerCar.playerCar.trans;

        //        countDownCamTrans.position = SmoothFollow.instance.trans.position;
        //        countDownCamTrans.rotation = SmoothFollow.instance.trans.rotation;

        countDownCamTrans.position = countDownCamPosArray[0].position;
        countDownCamTrans.rotation = countDownCamPosArray[0].rotation;

        countDownCamTrans.GetComponent<Camera>().enabled = true;

    }

    void CountDownTweenUpdate()
    {
        trans.localScale = Vector3.zero;
        //      foreach(GameObject _go in countDownGO)
        //          _go.SetActive(false);
        foreach (GameObject _go in countBase)
            _go.SetActive(true);
//        Debug.Log("yRot::"+yRot);
        countDownGO[state - 1].SetActive(false);
        if (state == 1)
        {
            //          countDownTB.text = "2";
            //countDownGO[3].SetActive(true);
            for (int i = 0; i < countDownGO.Length; i++)
                countDownGO[i].SetActive(false);
            countDownGO[0].SetActive(true);
            AudioSource.PlayClipAtPoint(shortBeep, Vector3.zero, GlobalClass.SoundVoulme);
            countDownCamTrans.position = countDownCamPosArray[0].position;
            countDownCamTrans.rotation = countDownCamPosArray[0].rotation;
            //            AudioManager.instance.PlayCarStartSound();
            //          countDownCamTrans.LookAt(lookAtTrans);
        }
        else if (state == 2)
        {
            //          countDownTB.text = "1";
            // countDownGO[3].SetActive(true);
            for (int i = 0; i < countDownGO.Length; i++)
                countDownGO[i].SetActive(false);
            countDownGO[1].SetActive(true);
            AudioSource.PlayClipAtPoint(shortBeep, Vector3.zero, GlobalClass.SoundVoulme);
            countDownCamTrans.position = countDownCamPosArray[1].position;
            countDownCamTrans.rotation = countDownCamPosArray[1].rotation;
            ;
            //          countDownCamTrans.LookAt(lookAtTrans);
        }
        else if (state == 3)
        {
            //          countDownTB.text = "GO";
            //countDownGO[3].SetActive(true);
            for (int i = 0; i < countDownGO.Length; i++)
                countDownGO[i].SetActive(false);
            countDownGO[2].SetActive(true);

            //          GoGameObject.SetActive(true);
            AudioSource.PlayClipAtPoint(longBeep, Vector3.zero, GlobalClass.SoundVoulme);

            AudioSource.PlayClipAtPoint(shortBeep, Vector3.zero, GlobalClass.SoundVoulme);
            countDownCamTrans.position = countDownCamPosArray[2].position;
            countDownCamTrans.rotation = countDownCamPosArray[2].rotation;
        }
        else if (state == 4)
        {
            //          countDownTB.text = "GO";
            //countDownGO[3].SetActive(true);
            for (int i = 0; i < countDownGO.Length; i++)
                countDownGO[i].SetActive(false);
            countDownGO[3].SetActive(true);

            //          GoGameObject.SetActive(true);
            AudioSource.PlayClipAtPoint(longBeep, Vector3.zero, GlobalClass.SoundVoulme);

            thisTweenEvent.Values["oncomplete"] = "CountDownOver";
            countDownCamTrans.position = SmoothFollow.instance.trans.position;
            countDownCamTrans.rotation = SmoothFollow.instance.trans.rotation;
        }
        state++;

        thisTweenEvent.Play();
    }

    public void CancelCountDown()
    {
        if (!_isTimerRunning)
            return;

        countDownCamTrans.GetComponent<Camera>().enabled = false;
        _isTimerRunning = false;
        //      trans.localScale = Vector3.zero;
        iTween.Stop(gameObject);
        foreach (GameObject _go in countDownGO)
            _go.SetActive(false);
        //      GoGameObject.SetActive(false);
    }

    void CountDownOver()
    {
        if (!IsTimerRunning)
            return;
        
        yRot = 0;
        foreach (GameObject _go in countBase)
            _go.SetActive(false);
        countDownCamTrans.GetComponent<Camera>().enabled = false;
        _isTimerRunning = false;
        //      trans.localScale = Vector3.zero;
        MainDirector.instance.CountDownOver();
        foreach (GameObject _go in countDownGO)
            _go.SetActive(false);
        //      GoGameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(_isTimerRunning)
        {
                yRot=yRot+1;
                countBase1.transform.rotation = Quaternion.Euler(0, 0, yRot);
                countBase2.transform.rotation= Quaternion.Euler(0, 0, -yRot);
        }
    }
}
