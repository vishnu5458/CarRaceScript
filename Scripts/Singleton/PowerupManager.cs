using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour 
{
	[System.Serializable]
	public class PowerUpDef
	{
		public Mesh powMesh;
		public Material particleMaterial;
	}
	
	public PowerUpDef[] powerUpDef;
	public Twister twisterObject;
	#region Singleton
	private static PowerupManager _instance;
	public static PowerupManager instance {
		get {
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<PowerupManager> ();
			return _instance;
		}
	}
	#endregion

	void Awake () 
	{
		_instance = this;	
	}

	public PowerUpDef GetPowDefForType(int type)
	{
		return powerUpDef[type];
	}

	public void ResetPowerUp()
	{
		Twister[] twisterArray;
		twisterArray = gameObject.GetComponentsInChildren<Twister>();
		foreach(Twister twis in twisterArray)
			twis.KillTwister();
	}

    public void ActivateTwister(DistWP startwp, Vector3 pos, float timeOut, Rigidbody sourceBody)
	{
//		GameObject twisterGO = Instantiate(twisterObject, pos, startwp.thisTransform.rotation) as GameObject;
		Twister twis = Instantiate(twisterObject, pos, startwp.thisTransform.rotation) as Twister;// twisterGO.GetComponent<Twister>();
		twis.transform.parent = gameObject.transform;
//		twis.startWp = startwp;
//		twis.sourceBody = sourceBody;
		twis.Initialise(timeOut, startwp, sourceBody);
	}
}
