using UnityEngine;
using System.Collections;

public class CarPreset : MonoBehaviour 
{
//	[HideInInspector]
	public Mesh carMesh;
	public Mesh[] wheelMesh;
	public Material[] skins;
	public Transform[] colliderTrans;
	public Transform[] nitroTrans;
	
	public float maxTorque = 20;
	public float minTorque = 20;
	public float sideFriction;
	public Mesh colliderMesh;
	
	public void InitialiseBehaviour()
	{
//		Mesh hell =  wheelObject[1].GetComponent<MeshFilter>().mesh;
		//meshFilter = GetComponentInChildren<MeshFilter>();
//		selectedCarTrans = transform;
		//gameObject.SetActiveRecursively(false);
	}
}
