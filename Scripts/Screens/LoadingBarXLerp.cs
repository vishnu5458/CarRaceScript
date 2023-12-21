using UnityEngine;
using System.Collections;

public class LoadingBarXLerp : MonoBehaviour {

	// Use this for initialization
	public float lerpRate = 0.1f;
	
	Material thisMaterial;
	
	void Start () 
	{
		thisMaterial = gameObject.GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameObject.activeSelf)
		{
			float offset = Time.time * lerpRate;
			thisMaterial.mainTextureOffset = new Vector2 (offset, 0);
		}
	}
}
