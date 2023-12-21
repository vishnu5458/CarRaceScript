using UnityEngine;
using System.Collections;
using EVP;

public class WheelSkid : MonoBehaviour
{
    public Transform skidSmokePrefab;
    public Transform skidTrailPrefab;
    public float maxSlip;

    public float AvgSkid { get; private set; }

    public ParticleSystem skidParticles;

    private Rigidbody thisBody;
    private VehicleController target;
    private CarBase carBase;
    public static Transform skidTrailsDetachedParent;

    // Use this for initialization
    void Start()
    {
        if (target == null)
            target = GetComponent<VehicleController>();

        carBase = GetComponent<CarBase>();
        Transform skidParticletrans = Instantiate(skidSmokePrefab) as Transform;
        skidParticletrans.parent = transform.Find("Particles");
//        skidParticletrans.localPosition = Vector3.zero;
        skidParticles = skidParticletrans.GetComponent<ParticleSystem>();
//        skidParticles.Stop();

        if (skidTrailsDetachedParent == null)
        {
            skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
//         AvgSkid = 0;
        if (!carBase.isActiveFrame)
            return;

        float targetAvgSkid = 0; 
        foreach (WheelData wd in target.wheelData)
        {
            float SkidFactor = 0;

            int colliderLayer = 17;
            if (wd.grounded)
                colliderLayer = wd.hit.collider.gameObject.layer;
                       
            
            //                   float skidFactorTarget;
            // overall skid factor, for drawing skid particles
            //                 skidFactorTarget = Mathf.Max(burnoutFactor * 2, sideSlideFactor * thisBody.velocity.magnitude * .05f);
            //                 skidFactorTarget = Mathf.Max(skidFactorTarget, spinoutFactor * thisBody.velocity.magnitude * .05f);
            //                 skidFactorTarget = Mathf.Clamp01(-.1f + skidFactorTarget * 1.1f);
            //                 SkidFactor = Mathf.MoveTowards(SkidFactor, skidFactorTarget, Time.deltaTime * 2);
            SkidFactor = Mathf.Max(Mathf.Abs(wd.tireSlip.x), Mathf.Abs(wd.tireSlip.y));
            targetAvgSkid += SkidFactor / (2 * maxSlip); // downsizing it due to high skid factor value
           
            if (colliderLayer == 18)
            {
                SkidFactor += 1.5f;   
                target.DirtSpeedReduction = -30;
                wd.isOutOfTrack = true;
            }
            else
            {
                target.DirtSpeedReduction = 0;
                wd.isOutOfTrack = false;
            }

            if (skidTrailPrefab != null)//&& target.speed > 50)
            {
                if (SkidFactor > maxSlip && wd.grounded)
                {
                    
                    
                    //                         if(wd.wheel.isLog)
//                    Debug.Log("SkidFactor::"+SkidFactor);
                    if (colliderLayer == 17)
                    {
                        skidParticles.transform.position = wd.wheel.wheelTransform.position - Vector3.up * (wd.collider.radius);
                        skidParticles.Emit(1);

                    }

                    if (!wd.leavingSkidTrail)
                    {
//                        skidParticles.transform.position = wd.wheel.wheelTransform.position - Vector3.up * (wd.collider.radius);
//                        skidParticles.Emit(1);

                        wd.skidTrail = Instantiate(skidTrailPrefab) as Transform;
                        if (wd.skidTrail != null)
                        {
                            wd.skidTrail.parent = wd.wheel.wheelCollider.transform;
                            wd.skidTrail.localPosition = -Vector3.up * (wd.collider.radius - 0.04f);
                        }
                        wd.leavingSkidTrail = true;
                    }
                }
                else
                {
                    if (wd.leavingSkidTrail)
                    {
                        wd.skidTrail.parent = skidTrailsDetachedParent;
                        Destroy(wd.skidTrail.gameObject, 10);
                        wd.leavingSkidTrail = false;
                    }
                    skidParticles.Stop();
                }
            }
        }
        targetAvgSkid /= target.wheelData.Length;
        AvgSkid = Mathf.MoveTowards(AvgSkid, targetAvgSkid, Time.deltaTime);
        

    }
}
