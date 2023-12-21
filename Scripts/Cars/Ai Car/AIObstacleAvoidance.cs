using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class ObstacleAntennaTrans
{
    public Transform center;
    public Transform centerEnd;
    public Transform left;
    public Transform leftEnd;
    public Transform leftEnd_A;
    public Transform right;
    public Transform rightEnd;
    public Transform rightEnd_A;
}

public class SensorData
{
    public bool isBreaking = false;
    public float targetAngle = 0;
    public int smashDirection = -2;
}

public class AIObstacleAvoidance : MonoBehaviour
{

    //	public Transform center;
    //	public Transform centerEnd;
    //	public Transform left;
    //	public Transform leftEnd;
    //	public Transform leftEnd_A;
    //	public Transform right;
    //	public Transform rightEnd;
    //	public Transform rightEnd_A;

    public ObstacleAntennaTrans antennaTrans = new ObstacleAntennaTrans();

    //	bool isBreaking;
    public float sensorLength = 5;
    public float frontSensorStartPoint = 5;
    public float frontSensorSideDist = 5;
    public float frontSensorsAngle = 30;
    public float sidewaySensorLength = 5;
    //	float avoidSpeed = 10;

    public LayerMask visibleLayers = -1;
    public Transform thisTrans;

    private float oADist;
    private float oAWidth;
    private int trafficLayer = 1 << 21;
    protected AICar aiCar;
    protected AiBike aiBike;

    protected virtual void Start()
    {
        thisTrans = transform;

        if (aiCar == null && aiBike == null)
            FindAIVehicle();
        float obstacleAvoidanceWidth = antennaTrans.right.localPosition.x + oAWidth; 
        Vector3 leftDirection = antennaTrans.center.position + antennaTrans.center.TransformDirection((Vector3.left * obstacleAvoidanceWidth) + (Vector3.forward * oADist));
        Vector3 rightDirection = antennaTrans.center.position + antennaTrans.center.TransformDirection((Vector3.right * obstacleAvoidanceWidth) + (Vector3.forward * oADist));
		
        antennaTrans.leftEnd_A.position = leftDirection;
        antennaTrans.rightEnd_A.position = rightDirection;
		
        antennaTrans.centerEnd.position = antennaTrans.center.position + antennaTrans.center.TransformDirection(Vector3.forward * oADist * 3);
		
        antennaTrans.leftEnd.transform.position = antennaTrans.left.transform.position + antennaTrans.left.transform.TransformDirection(Vector3.forward * oADist * 0.6f);
        antennaTrans.rightEnd.transform.position = antennaTrans.right.transform.position + antennaTrans.right.transform.TransformDirection(Vector3.forward * oADist * 0.6f);
		
//		leftDirection = antennaTrans.center.position + antennaTrans.center.TransformDirection((Vector3.left * obstacleAvoidanceWidth * 2) + (Vector3.forward *aiCar.oADistance * 2));
//		rightDirection = antennaTrans.center.position + antennaTrans.center.TransformDirection((Vector3.right * obstacleAvoidanceWidth * 2) + (Vector3.forward * aiCar.oADistance * 2));
		
    }

    public void InitAiModule()
    {
        Quaternion quat = thisTrans.rotation;
        Vector3 pos = quat.eulerAngles;
        pos.x = 0;
        quat.eulerAngles = pos;
        thisTrans.rotation = quat;
    }

    void FindAIVehicle()
    {
        bool isAIFound = false;
        Transform parentTrans = transform;
        while (!isAIFound)
        {
            parentTrans = parentTrans.parent;
			
            if (parentTrans == null)
            {
                Debug.LogError("ai car not found in parent");
                isAIFound = true;
                break;
            }
            if (parentTrans.GetComponent<AICar>())
            {
                aiCar = parentTrans.GetComponent<AICar>();
                isAIFound = true;
                oADist = aiCar.oADistance;
                oAWidth = aiCar.oAWidth;
                break;
            }
            else if (parentTrans.GetComponent<AiBike>())
            {
                aiBike = parentTrans.GetComponent<AiBike>();
                isAIFound = true;
                oADist = aiBike.oADistance;
                oAWidth = aiBike.oAWidth;
                break;
            }
        }
    }



    public virtual SensorData Sensors(float _speed)
    {
        Quaternion quat = thisTrans.rotation;
        Vector3 pos = quat.eulerAngles;
        pos.z = 0;
        quat.eulerAngles = pos;
        thisTrans.rotation = quat;
//        Debug.Log(thisTrans.rotation.eulerAngles);
        bool isLeftHit = false;
        bool isRightHit = false;


        SensorData _sensorData = new SensorData();
//		float avoidSenstivity = 0;
//		Vector3 pos;
        RaycastHit hit;
//		Vector3 rightAngle = Quaternion.AngleAxis(frontSensorsAngle,transform.up) * transform.forward;
//		Vector3 leftAngle = Quaternion.AngleAxis(-frontSensorsAngle,transform.up) * transform.forward;

//		if (Physics.Raycast(center.position,centerEnd.position,out hit, sensorLength, visibleLayers))
//		{
//
//		}
//			
//			pos = transform.position;
//		pos += transform.forward*frontSensorStartPoint;
//		
//
//		//Front Straight Right Sensor
//		pos += transform.right*frontSensorSideDist;
		
        if (Physics.Linecast(antennaTrans.right.position, antennaTrans.rightEnd.position, out hit, visibleLayers))
        {
            isRightHit = true;
            _sensorData.targetAngle -= 1; 
            Debug.DrawLine(antennaTrans.right.position, hit.point, Color.white);
        }
        else if (Physics.Linecast(antennaTrans.right.position, antennaTrans.rightEnd_A.position, out hit, visibleLayers))
        {
            _sensorData.targetAngle -= 0.5f; 
            Debug.DrawLine(antennaTrans.right.position, hit.point, Color.white);
        }

//		//Front Straight left Sensor
//		pos = transform.position;
//		pos += transform.forward*frontSensorStartPoint;
//		pos -= transform.right*frontSensorSideDist;
		
        if (Physics.Linecast(antennaTrans.left.position, antennaTrans.leftEnd.position, out hit, visibleLayers))
        {
            isLeftHit = true;
            _sensorData.targetAngle += 1; 
            Debug.DrawLine(antennaTrans.left.position, hit.point, Color.white);
        }
        else if (Physics.Linecast(antennaTrans.left.position, antennaTrans.leftEnd_A.position, out hit, visibleLayers))
        {
            _sensorData.targetAngle += 0.5f;
            Debug.DrawLine(antennaTrans.left.position, hit.point, Color.white);
        }
		
//		//Right SideWay Sensor
//		if (Physics.Raycast(transform.position,transform.right,hit,sidewaySensorLength)){
//			if (hit.transform.tag != "Terrain"){
//				flag++;
//				avoidSenstivity -= 0.5;
//				Debug.DrawLine(transform.position,hit.point,Color.white);
//			}
//		}
//		
//		//Left SideWay Sensor
//		if (Physics.Raycast(transform.position,-transform.right,hit,sidewaySensorLength)){
//			if (hit.transform.tag != "Terrain"){
//				flag++;
//				avoidSenstivity += 0.5;
//				Debug.DrawLine(transform.position,hit.point,Color.white);
//			}
//		}

        //BRAKING SENSOR & Front Mid sensor
        float frontDist = 0.3f * _speed; 
        antennaTrans.centerEnd.position = antennaTrans.center.position + antennaTrans.center.TransformDirection(Vector3.forward * frontDist);
        if (Physics.Linecast(antennaTrans.center.position, antennaTrans.centerEnd.position, out hit, trafficLayer))
        {

            _sensorData.isBreaking = true;

            if (isLeftHit && isRightHit)
            {
                if (hit.normal.x < 0)
                    _sensorData.targetAngle = 1;
                else
                    _sensorData.targetAngle = -1;
//                Debug.Log(_sensorData.targetAngle);
//                Debug.Break();
            }
            Debug.DrawLine(antennaTrans.center.position, hit.point, Color.red);

        }
        else
        {
//			_sensorData.isBreaking = false;
        }

        return _sensorData;// * avoidSpeed;

//		if (flag != 0)
//			AvoidSteer (avoidSenstivity);
		
		
    }

    public virtual void OnDrawGizmos()
    {
        if (aiCar == null && aiBike == null)
            FindAIVehicle();
		
        Vector3 fwdDist;
        Vector3 centerDist;
        ;
        float obstacleAvoidanceWidth;

        if (aiCar != null)
        {
            fwdDist = Vector3.forward * aiCar.oADistance;
            obstacleAvoidanceWidth = aiCar.oAWidth;
            centerDist = Vector3.forward * 0.3f * aiCar.speed;
        }
        else
        {
            fwdDist = Vector3.forward * aiBike.oADistance;
            obstacleAvoidanceWidth = aiBike.oAWidth;
            centerDist = Vector3.forward * 0.3f * aiBike.speed;
        }

        Vector3 forwardDirection = antennaTrans.center.TransformDirection(fwdDist * 0.6f);


        Vector3 leftDirection = antennaTrans.center.TransformDirection((Vector3.left * obstacleAvoidanceWidth) + (fwdDist));
        Vector3 rightDirection = antennaTrans.center.TransformDirection((Vector3.right * obstacleAvoidanceWidth) + (fwdDist));
				
        Debug.DrawRay(antennaTrans.left.position, leftDirection, Color.cyan);
        Debug.DrawRay(antennaTrans.right.position, rightDirection, Color.cyan);
        Debug.DrawRay(antennaTrans.center.position, antennaTrans.center.TransformDirection(centerDist), Color.green);
        Debug.DrawRay(antennaTrans.left.position, forwardDirection, Color.green);
        Debug.DrawRay(antennaTrans.right.position, forwardDirection, Color.green);
		
    }
	
}
