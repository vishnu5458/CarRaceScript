using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision other)
	{
		Debug.Log("colli detect "+other.gameObject.name);

	}
}
