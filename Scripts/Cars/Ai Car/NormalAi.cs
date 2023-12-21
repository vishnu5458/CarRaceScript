using UnityEngine;
using System.Collections;

public class NormalAi : AICar
{
    private AIObstacleAvoidance thisObstaceAvoidance;
    private ParticleSystem electricShockParticle;
    private SensorData prevSensorData = new SensorData();

    //    int onCurrentBoostStatus = 0;


    protected override void Awake()
    {
        thisObstaceAvoidance = gameObject.GetComponentInChildren<AIObstacleAvoidance>();
        base.Awake();
    }

    public override void InitCar()
    {
        base.InitCar();
    }

    protected override void Update()
    {
        if (!isCarEnabled)
            return;	
		
        Vector3 RelativePosition = trans.InverseTransformPoint(target);
        magnitude = RelativePosition.z;  // + .01f instead of magnitude
        steer = RelativePosition.x / magnitude;
		
        acce = RelativePosition.z / magnitude;// - Mathf.Abs( steer );
        if(Mathf.Abs(steer) > 0.5f && !isCarBlinking)
            acce = -0.7f;
        else if(Mathf.Abs(steer) > 0.2f && !isCarBlinking)
            acce = -0.1f;
        else
        acce = Mathf.Abs(acce);
		
        DebugLog("rel pos : " + RelativePosition.z + " magnitude : " + magnitude + " Acce : " + acce + " Steer : " + steer);
        if ( speed < 2.0f)
        {
            carStopTimeOut--;
            if (carStopTimeOut < 0)
            {
                carStopTimeOut = 150;
                Debug.Log("reset on slow speed");
                OnVehicleReset();
            }
        }
        else if (RelativePosition.z < 0) // is facing the wrong direction
        {
            carStopTimeOut--;
            if (carStopTimeOut < 0)
            {
                Debug.Log("reset on relative");
                OnVehicleReset();
            }
        }
        else
        {
            carStopTimeOut = 150;
        }
		
        if (isReverse)
        {
            acce *= -0.4f;
            steer *= -1f;
			
            reverseTimeOut--;
			
            if (reverseTimeOut < 0)
                isReverse = false;
        }
        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (!isCarEnabled)
            return;	
        base.FixedUpdate();
    }

    protected override void UpdatePowerTrain()
    {
        vehicleController.AiCatchUpSpeed = 0;
        if (isCatchUpEnabled)
        {
            float distDiff = MainDirector.GetCurrentPlayer().GetNavBase().positionWeight - GetNavBase().positionWeight;
			
            if (Mathf.Abs(distDiff) > 5)
            {
                float catchUpSpeed = distDiff;
                catchUpSpeed = Mathf.Clamp(catchUpSpeed, -20, 20);
                vehicleController.AiCatchUpSpeed = catchUpSpeed;
            }
            else
                vehicleController.AiCatchUpSpeed = 0;
        }

        base.UpdatePowerTrain();
    }

    protected override void UpdateSteering()
    {
        SensorData _sensorData;
        if (isActiveFrame)
        {
            _sensorData = thisObstaceAvoidance.Sensors(speed);
            prevSensorData = _sensorData;
        }
        else
            _sensorData = prevSensorData;

        if (_sensorData.targetAngle != 0)
        {
            if (steer < _sensorData.targetAngle)
            {
                steer = steer + (Time.deltaTime * 50); //200
                if (steer > _sensorData.targetAngle)
                {
                    steer = _sensorData.targetAngle;
                }
            }
            else if (steer > _sensorData.targetAngle)
            {
                steer = steer - (Time.deltaTime * 50); //200
                if (steer < _sensorData.targetAngle)
                {
                    steer = _sensorData.targetAngle;
                }
            }
        }
        if (_sensorData.isBreaking)
        {
            acce = -0.6f;
            //            Debug.Log("Breaking");
        }
        base.UpdateSteering();
    }

    public override void OnVehicleReset()
    {
        base.OnVehicleReset();
    }

    public override void OnGameEnd()
    {
        base.OnGameEnd();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
    }
}
