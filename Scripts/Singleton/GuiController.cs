using UnityEngine;
using System.Collections;

public class GuiController : MonoBehaviour
{
	#region Singleton
	private static GuiController _instance;
	public static GuiController instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<GuiController> ();
			}
			return _instance;
		}
	}
	#endregion

	public Camera mainCamera;

	private static Camera guiCam;

	void Awake () 
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		_instance = this;
		guiCam = GetComponent<Camera>();
		float initAspect;
	
#if !UNITY_WEBGL && !UNITY_WEBPLAYER
		initAspect = guiCam.aspect;
#else
		initAspect = 1.333333f;
//		guiCam.aspect = initAspect;
//		mainCamera.aspect = initAspect;
#endif
		GlobalClass.aspectDiff = initAspect - GlobalClass.DESIGN_ASPECT;
//		Debug.Log("init aspect : " + initAspect + " aspect diff : " + GlobalClass.aspectDiff);
		GlobalClass.vertiExtent = GuiController.guiCam.orthographicSize;
		GlobalClass.horiExtent = -1 * GuiController.guiCam.orthographicSize * initAspect;
		Debug.Log("Vertical Extenet : " + GlobalClass.vertiExtent + " Horizontal Extenet : " + GlobalClass.horiExtent);
	}

	public static Camera GetGuiCam()
	{
		return guiCam;
	}
}
