using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour
{
	public float flashInterval = 0.2f;
	public Vector2 scaleRange = new Vector2(0.75f, 1.5f);

	private float flashIntervalDt;
	private Transform trans;

	// Use this for initialization
	void Start () 
	{
		trans = transform;
		GetComponent<Light>().enabled = true;
	}

	public void ShowFlash()
	{
		gameObject.SetActive(true);
		flashIntervalDt = flashInterval;
		trans.localScale = Vector3.one * Random.Range(scaleRange.x, scaleRange.y);
		trans.Rotate(Vector3.forward, Random.Range(0, 90.0f));
	}

	// Update is called once per frame
	void Update () 
	{
		flashIntervalDt -= Time.deltaTime;

		if(flashIntervalDt < 0)
			gameObject.SetActive(false);
	}
}
