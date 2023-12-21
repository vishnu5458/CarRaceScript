using UnityEngine;
using System.Collections;

public class VehicleSetup : MonoBehaviour 
{
//	[HideInInspector]
//	public AICar thisAICar;
	
	public MeshFilter bodyFilter;

	public MeshFilter[] wheelMeshFilter;
	
	public Transform[] wheelColliders;
	
	public MeshCollider thisCollider;
	
	public Transform[] nitroTransform;
	
	public void InitialiseCar(int texId, int rimId)
	{
//		CarPreset preset = CarManager.carManager.carPreset[id];
//		
//		bodyFilter.mesh = preset.carMesh;
//		thisCollider.sharedMesh = preset.colliderMesh;
//		
//		if(!isPlayerCar)
//			bodyFilter.gameObject.GetComponent<MeshRenderer>().material = preset.skins[0];
//		else
//			bodyFilter.gameObject.GetComponent<MeshRenderer>().material = preset.skins[0];
//		
////		for(int i = 0; i < 4; i++)
////		{
////			wheelMeshFilter[i].mesh = preset.wheelMesh[i];
////			
////			wheelMeshFilter[i].gameObject.GetComponent<MeshRenderer>().material = preset.skins[0];
////			
////			
////			wheelMeshFilter[i].gameObject.transform.localPosition = wheelColliders[i].localPosition = preset.colliderTrans[i].localPosition;  // aligning both wheel mesh and wheel collider
////			wheelMeshFilter[i].gameObject.transform.localEulerAngles = wheelColliders[i].localEulerAngles = preset.colliderTrans[i].localEulerAngles;
////		}
//		
//		for(int i = 0; i < 2; i++)
//		{
//			nitroTransform[i].localPosition = preset.nitroTrans[i].localPosition;
//			nitroTransform[i].localRotation = preset.nitroTrans[i].localRotation;
//		}
	}
	
	
	
}
