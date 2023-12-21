using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour 
{
	public float buildingLife = 100;
	public Renderer activeBuilding;
	public Renderer destroyedBuilding;
	public Transform explosivePosBase;

	private float initBuildingLife;

	// Use this for initialization
	void Start () 
	{
		initBuildingLife = buildingLife;	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void InitBuilding(int startWpid)
	{
		Transform buildDotPos =  GamePlayScreen.instance.gamePlayObjects.buildingPosDot;
		buildDotPos.localPosition  = new Vector3(0.32f, -6.0f, -0.25f);
		buildDotPos.GetComponent<Renderer>().enabled = true;
		float distInc = 12.0f/(GlobalClass.totalLaps * GlobalClass.totalWP);

		int carId = startWpid;
		if(carId > GlobalClass.playerStartWPiD)
		{
			Vector3 dotPos = buildDotPos.localPosition;
			while(GlobalClass.playerStartWPiD != carId)
			{
				carId -= 1;
				dotPos.y += distInc;
				dotPos.y = Mathf.Clamp(dotPos.y, -6.0f, 6.0f);
				buildDotPos.localPosition = dotPos;
			}
		}
	}
	 
	public void ApplyDamage(float damage)
	{
//		Debug.Log(damage);
		buildingLife -= damage;
		GamePlayScreen.instance.UpdateBigHealthBar(buildingLife/initBuildingLife);
		if(buildingLife < 0)
		{
			Debug.Log("Building " + gameObject.name + " destroyed");
			// renderer changes
		}
	}

	public void ExplodeBuilding()
	{
		StartCoroutine(Explosion());
	}

	IEnumerator Explosion()
	{
		Transform[] expPosArray = explosivePosBase.GetComponentsInChildren<Transform>();

		foreach(Transform expTrans in expPosArray)
		{
			Vector3 pos = expTrans.position;
			ParticleManager.instance.PlayBigExplosionAtPoint(pos);
			AudioManager.instance.PlayBlastSoundAtPos(pos);

			yield return new WaitForSeconds(1.0f);
		}

		activeBuilding.enabled = false;
		destroyedBuilding.enabled = true;

		yield return new WaitForSeconds(1.5f);
		MainDirector.GetCurrentGamePlay().GameOver();
	}

	public void OnGameEndSession()
	{
		GamePlayScreen.instance.gamePlayObjects.buildingPosDot.GetComponent<Renderer>().enabled = false;
	}
}
