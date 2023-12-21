using UnityEngine;
using System.Collections;

public class ObjectRandomShake : MonoBehaviour 
{
	[SerializeField]
	private bool isShakeEnabled = false;
	private Transform trans;
//	private Vector3 initPos;

	public int repeatTimeOut = 100;
//	public float magnitude = 0.5f;

	// Use this for initialization
	void Start () 
	{
		trans = transform;
//		initPos = trans.localPosition;
	}

	public void EnableShake()
	{
//		isShakeEnabled = true;
	}

	public void DisableShake()
	{
		isShakeEnabled = false;
	}

	// Update is called once per frame
	void Update () 
	{
	 	if(!isShakeEnabled)
			return;

		if(Time.frameCount % repeatTimeOut == 0)
		{
			Vector3 scaleVal = new Vector3(0.9f, 0.9f, 1);
			trans.localScale = scaleVal;

			Hashtable ht =  new Hashtable();
			ht.Add("x",1);
			ht.Add("y",1);
			ht.Add("time", .5f);
			ht.Add("easetype", "spring");
			ht.Add("islocal",true);
			ht.Add("ignoretimescale",true);
			iTween.ScaleTo(gameObject, ht);

//			Vector2 randVal = Random.insideUnitCircle;
//			Vector3 pos = trans.localPosition;
//			pos.x += randVal.x * magnitude;
////			pos.y += randVal.y * magnitude;
//			pos.x = Mathf.Clamp(pos.x, initPos.x - 0.5f, initPos.x + 0.5f );
////			pos.y = Mathf.Clamp(pos.y, initPos.y - 1, initPos.y + 1 );
//			trans.localPosition = pos;
		}
	}
}
