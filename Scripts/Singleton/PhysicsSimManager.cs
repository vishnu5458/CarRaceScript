using UnityEngine;
using System.Collections;

public class PhysicsSimManager : MonoBehaviour 
{

	#region Singleton
	private static PhysicsSimManager _instance;
	public static PhysicsSimManager instance {
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<PhysicsSimManager> ();
			return _instance;
		}
	}
	#endregion

	public bool isEnabled = false;
	public SmoothFollow carCamera;
	public CarBase[] testCars;
	public Vector3 initPos;

	public GameObject wheelColliderPrefab;

	private int currentIndex;
	private CarBase currentCar;

	// Use this for initialization
	void Awake () 
	{
		_instance = this;
		currentIndex = 0;
		ResetCurrentCar();
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetKeyDown(KeyCode.R))
		{
			DestroyImmediate(currentCar.gameObject);
			ResetCurrentCar();
		}

		if(Input.GetKeyDown(KeyCode.PageUp))
		{
			currentIndex ++;

			if(currentIndex > testCars.Length - 1)
				currentIndex = 0;

			DestroyImmediate(currentCar.gameObject);
			ResetCurrentCar();
		}
	}


	void ResetCurrentCar()
	{
		if(!isEnabled)
			return;

//		testCars[currentIndex].useEdyPhysics = useEdyPhysic;
		currentCar = Instantiate(testCars[currentIndex]);
		currentCar.transform.position = initPos;
		carCamera.target = currentCar.GetComponent<PlayerCar>().camFollowTarget;
		currentCar.body.isKinematic = false;
		currentCar.GetComponent<EVP.VehicleController>().enabled = true;
		currentCar.InitCar();
//		currentCar.EnableCar();

		carCamera.InitCamera(currentCar.GetComponent<PlayerCar>().camDist,currentCar.GetComponent<PlayerCar>().camHeight, currentCar.GetComponent<PlayerCar>().nitroCamDist, currentCar.GetComponent<Rigidbody>());
	}
}
