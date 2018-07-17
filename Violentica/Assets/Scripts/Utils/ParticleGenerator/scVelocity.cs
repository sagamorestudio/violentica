using UnityEngine;
using System.Collections;

namespace ZO
{
    public class scVelocity : scAbsObjectPoolIgnoreNotBake
    {

        public Vector3 velocity;

        public Vector3 accel;

        Vector3 fv;
        Vector3 fa;
        float fl;
        bool ig;

        void Awake()
        {
            fa = accel;
            fv = velocity;
            ig = IgnorePause;
            fl = SpeedLimit;
        }

        public void ResetTo0()
        {
            accel = velocity = Vector3.zero;
        }

        void Update()
        {
            if (IgnorePause || scPause.IsPlaying)
            {
//                 if (!IgnorePause && scTimeBullet.IsOn)
//                 {
//                     transform.localPosition += velocity * Time.deltaTime * scTimeBullet.TimeRatioWhenBullet;
//                     velocity += accel * Time.deltaTime * scTimeBullet.TimeRatioWhenBullet;
//                 }
//                 else
                {
                    transform.localPosition += velocity * Time.deltaTime;
                    velocity += accel * Time.deltaTime;
                }
                if (!Mathf.Approximately(0, SpeedLimit))
                    if (velocity.magnitude > SpeedLimit)
                        velocity = velocity.normalized * SpeedLimit;
            }
        }

        public float SpeedLimit = 0;

        void OnDestroy()
        {
            accel = fa;
            velocity = fv;
            IgnorePause = ig;
            SpeedLimit = fl;
        }

        public bool IgnorePause;

    }

}