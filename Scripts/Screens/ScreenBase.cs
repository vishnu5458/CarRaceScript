using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenBase : MonoBehaviour
{

    [SerializeField]
    public bool isUpdateInput = false;
    //	[SerializeField]
    //	private bool isRollOverEnabled = false;
    [SerializeField]
    private bool disableScreenOnAwake = true;

    public bool isLogEnabled = false;

    public GuiMove[] screenObjects;

    protected string callSource;
    //	protected Vector3 onScreen = new Vector3(0, 0, -5);
    //	protected Vector3 offScreen = new Vector3(0, -100, 50);

    //    protected UIButton rollOverButton = null;
    //    protected UIButton selectButton = null;
    //    protected UIButton _uiButton = null;
    protected RectTransform trans;
    protected GameObject screenBaseObj;

    protected int layer = 1 << 5;
    protected Camera thisCamera;

    protected virtual void Awake()
    {
        trans = GetComponent<RectTransform>();
        screenBaseObj = transform.GetChild(0).gameObject;
    }

    protected virtual void Start()
    {
        if (disableScreenOnAwake)
            screenBaseObj.SetActive(false);
        thisCamera = GuiController.GetGuiCam();
    }

    public virtual void InitScreen()
    {
        InitScreen(false);

        if (GlobalClass.isHostedFromY8)
        {
            foreach (Button _idButton in GetComponentsInChildren<Button>())
            {
                if (_idButton.name == "More Games")
                {
                    _idButton.GetComponent<ButtonBaseLinker>().buttonBase.SetActive(false);
                    _idButton.gameObject.SetActive(false);
                }
                if (_idButton.name == "Sponsor Logo"||_idButton.name=="Id Net")
                {
                    _idButton.enabled = false;
                }
            }
        }
    }

    public virtual void InitScreen(bool isGUITween)
    {
        screenBaseObj.SetActive(true);
        foreach (GuiMove guiMov in screenObjects)
            guiMov.InitialseCompement(!isGUITween);

    }

    public virtual void InitScreen(string source)
    {
        callSource = source;
        screenBaseObj.SetActive(true);
    }

    public virtual void OnScreenLoadComplete()
    {
        isUpdateInput = true;
    }

    public virtual void ExitScreen()
    {
        isUpdateInput = false;
        if (disableScreenOnAwake)
            screenBaseObj.SetActive(false);

        foreach (GuiMove guiMov in screenObjects)
            guiMov.Reset(true);
    }

    void Update()
    {
        if (isUpdateInput)
        {
            UpdateInput();
        }
    }

    protected virtual void UpdateInput()
    {
        //        if (isRollOverEnabled)
        //        {
        //            Ray ray;
        //            RaycastHit hit;
        //            Vector3 position = Input.mousePosition;
        //            ray = thisCamera.ScreenPointToRay(position);
        //			
        //            if (Physics.Raycast(ray, out hit, 100, layer))
        //            {
        //                Button currButton = hit.transform.GetComponent<Button>();
        //				
        //                if (currButton)
        //                {
        //                    if (rollOverButton != currButton)
        //                    {
        //                        if (rollOverButton != null)
        //                        {
        //                            rollOverButton.OnRollOff();  //disable rollover
        //                        }
        //                        rollOverButton = currButton;
        //                        rollOverButton.OnRollOver();
        //                    }
        //                }
        //                else //special condition when using transparent BG with same layer and doesnt have Button class
        //                {
        //                    if (rollOverButton)
        //                    {
        //                        rollOverButton.OnRollOff();
        //                        rollOverButton = null;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (rollOverButton != null)
        //                {
        //                    rollOverButton.OnRollOff();  //disable rollover
        //                    rollOverButton = null;
        //                }
        //            }
        //        }
        //
        //        if (Input.GetMouseButton(0))
        //        {
        //            Ray ray;
        //            RaycastHit hit;
        //            Vector3 position = Input.mousePosition;
        //            ray = thisCamera.ScreenPointToRay(position);
        //			
        //            if (Physics.Raycast(ray, out hit, 100, layer))
        //            {
        //                _uiButton = hit.transform.GetComponent<Button>();
        //
        //                if (_uiButton)
        //                {
        //                    if (Input.GetMouseButtonDown(0))
        //                    {
        //                        if (isLogEnabled)
        //                            Debug.Log("Update Input Running in " + gameObject.name + " : " + _uiButton.name);
        //						
        //                        selectButton = _uiButton;
        //                        _uiButton.OnTouchDown();
        //                        ButtonTouchDown(_uiButton);
        //                    }
        //                    else
        //                    {
        //                        if (selectButton && selectButton != _uiButton)
        //                        {
        //                            selectButton.OnTouchRelease();
        //                            selectButton = null;
        //                        }
        //                    }
        //                }
        //                _uiButton = null;
        //            }
        //            else
        //            {
        //                if (selectButton)
        //                {
        //                    selectButton.OnTouchRelease();
        //                    selectButton = null;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (Input.GetMouseButtonUp(0) && selectButton)
        //            {
        //                Ray ray;
        //                RaycastHit hit;
        //				
        //                Vector3 position = Input.mousePosition;
        //                ray = thisCamera.ScreenPointToRay(position);
        //				
        //                _uiButton = null;
        //                if (Physics.Raycast(ray, out hit, 100, layer))
        //                {
        //                    _uiButton = hit.transform.GetComponent<Button>();
        //                }
        //
        //                selectButton.OnTouchRelease();
        //            }
        //        }
    }

    public virtual void ButtonTouchDown(Button _button)
    {
        if (!isUpdateInput)
        {
            Debug.Log(_button.name + " is not in active screen");
            return;
        }
    }

    public virtual void ButtonTouchUP(Button _button)
    {
        if (!isUpdateInput)
        {
            Debug.Log(_button.name + " is not in active screen");
            return;
        }
    }

    public virtual void ButtonClick(Button _button)
    {
        if (!isUpdateInput)
        {
            Debug.Log(_button.name + " is not in active screen");
            return;
        }
        if (isLogEnabled)
            Debug.Log("On Button Click: " + _button.name);
    }
}
