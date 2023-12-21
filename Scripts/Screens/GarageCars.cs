using UnityEngine;
using System.Collections;

public class GarageCars : MonoBehaviour 
{
	[HideInInspector]
	public Transform trans;
	[HideInInspector]
	public Vector3 initPos;

//	public Material carMaterial;
//	public Material rimMaterial;

	public string carName;

	public Transform weaponTrans;
	public int[] engineUpgradeCost;
	public int[] armorUpgradeCost;

	public GameObject[] upgradeKit;

//	Light carSpotlight;

	public bool isUnlocked = false;

	void Start()
	{
		trans = transform;
		initPos = trans.position;
//		weaponTrans = trans.FindChild("Body/Weapon Pivot");
//		carSpotlight = GetComponentInChildren<Light>();
	}

	public void SetUpgradeKit(int index)
	{
		for(int i = 0; i < index; i++)
			upgradeKit[i].SetActive(true);
	}

//	public void SetCarAndRimTexture(CarPanelObject selectedObj)
//	{
//		if(selectedObj.thisType == PanelObjectType.CarPaint)
//		{
//			carMaterial.mainTexture = selectedObj.corresTexture;
//		}
//		else
//		{
//			rimMaterial.mainTexture = selectedObj.corresTexture;
//		}
//	}

	public void LockCar()
	{
		isUnlocked = false;
//		carSpotlight.enabled = false;
	}

	public void UnlockCar()
	{
		isUnlocked = true;
//		carSpotlight.enabled = true;
	}
}
