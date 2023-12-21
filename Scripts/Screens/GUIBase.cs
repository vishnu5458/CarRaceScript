using UnityEngine;
using System.Collections;

public enum Corner
{
	None,
	TopRight,
	TopLeft,
	BottomRight,
	BottomLeft
}

public enum TopAnchor
{
	None,
	Top,
	Bottom
}

public enum SideAnchor
{
	None,
	Left,
	Right
}

public class GUIBase : MonoBehaviour 
{
	public bool isScale = false;
//	public Corner thisCorner;
	public TopAnchor _topAnchor;
	public SideAnchor _sideAnchor;
	public Vector2 bounds;
	public float cornerX = 0.0f;
	public float cornerY = 0.0f;

	[HideInInspector]
	public Transform trans;

    protected virtual void Awake()
    {        
        trans = transform;
    }

    protected virtual void Start () 
    {

		if(isScale)
		{
			Vector3 newScale = transform.localScale;
//			if(Screen.orientation == ScreenOrientation.Landscape)
				newScale.x -= 0.45f * GlobalClass.aspectDiff;
//			else if(Screen.orientation == ScreenOrientation.Portrait)
//				newScale.y += 1.5f * GlobalClass.aspectDiff;

			trans.localScale = newScale;
		}

		if(_topAnchor != TopAnchor.None || _sideAnchor != SideAnchor.None)
		{
			float xpos = trans.localPosition.x;
			float ypos = trans.localPosition.y;
			Vector3 bodyExtent;
			if(GetComponent<Renderer>())
				bodyExtent = GetComponent<Renderer>().bounds.size;
			else
				bodyExtent = bounds;
			

			cornerX += bodyExtent.x/2;// + xAspectMod;  //1.5f * GlobalClass.aspectDiff;
			cornerY += bodyExtent.y/2;// - yAspectMod;  //1.5f * GlobalClass.aspectDiff;

			switch(_topAnchor)
			{
			case TopAnchor.Top : 
				ypos = GlobalClass.vertiExtent - cornerY;
				break;

			case TopAnchor.Bottom :
				ypos = (-1 * GlobalClass.vertiExtent) + cornerY;
				break;
			}

			switch(_sideAnchor)
			{
			case SideAnchor.Left : 
				xpos = GlobalClass.horiExtent + cornerX;
				break;

			case SideAnchor.Right : 
				xpos = (-1 * GlobalClass.horiExtent) - cornerX;
				break;

			}

			trans.localPosition = new Vector3(xpos, ypos, trans.localPosition.z);
		}

//		if(thisCorner != Corner.None)
//		{
//			float xpos = 0;
//			float ypos = 0;
//			Vector3 bodyExtent;
//
//			if(renderer)
//				bodyExtent = renderer.bounds.size;
//			else
//				bodyExtent = bounds;
//
//			float xAspectMod = 0 * GlobalClass.aspectDiff;
////			float yAspectMod = 0.5f * GlobalClass.aspectDiff;
////			Debug.Log(GlobalClass.horiExtent + "   " + GlobalClass.vertiExtent + "  " + bodyExtent);
//			cornerX += bodyExtent.x/2;// + xAspectMod;  //1.5f * GlobalClass.aspectDiff;
//			cornerY += bodyExtent.y/2;// - yAspectMod;  //1.5f * GlobalClass.aspectDiff;
//
//			switch(thisCorner)
//			{
//			case Corner.TopLeft : 
//				xpos = GlobalClass.horiExtent + cornerX;
//				xpos -= xAspectMod;
//				ypos = GlobalClass.vertiExtent - cornerY;
//				break;
//
//			case Corner.TopRight : 
//				xpos = (-1 * GlobalClass.horiExtent) - cornerX;
//				xpos += xAspectMod;
//				ypos = GlobalClass.vertiExtent - cornerY;
//				break;
//
//			case Corner.BottomLeft : 
//				xpos = GlobalClass.horiExtent + cornerX;
//				xpos -= xAspectMod;
//				ypos = (-1 * GlobalClass.vertiExtent) + cornerY;
//				break;
//
//			case Corner.BottomRight : 
//				xpos = (-1 * GlobalClass.horiExtent) - cornerX;
//				Debug.Log(xpos);
//				xpos += xAspectMod;
//				ypos = (-1 * GlobalClass.vertiExtent) + cornerY;
//				break;
//			}
//
//			trans.localPosition = new Vector3(xpos, ypos, trans.localPosition.z);
//
////			Debug.Log(GuiController.GetInstance().GetGuiCam().orthographicSize * Screen.width / Screen.height);
//			Debug.Log(xpos + "   " + ypos);
////			Debug.Log(GuiController.GetInstance().GetGuiCam().ScreenToWorldPoint(new Vector3(Screen.width, 400)));
//		}
	}
}
