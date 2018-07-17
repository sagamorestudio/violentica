using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZO
{

    /// <summary>
    /// target_pivot 은 shaking 이후 0,0,0으로 돌아오니 하나를 준비한다.
    /// </summary>
    public class scCameraShaker : MonoBehaviour
    {
        public const float CameraShakeBasicPeriod = 0.1f;
        public const float CameraShakeBasicAmplitude = 0.1f;
        public const float CameraShakeBasicTime = 0.3f;


        public static scCameraShaker Instance;

        void Awake()
        {
            Instance = this;
            foreach (var v in target_pivot)
            {
                var cam = v.GetComponent<Camera>();
                if (cam != null)
                {
                    CameraOriginalOrthographicSize.Add(cam.orthographicSize);
                }
                else
                {
                    CameraOriginalOrthographicSize.Add(0);
                }
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }



        CoroutineClass shaker = null;

        [SerializeField]
        Transform[] target_pivot;

        public CoroutineClass ShakeVertical(float BeginAmplitude = CameraShakeBasicAmplitude, float time = CameraShakeBasicTime, float period = CameraShakeBasicPeriod)
        {
            if (lefttime < time)
            {
                Stop();
                shaker = scCoroutine.Instance.Begin(shake(Vector2.up, BeginAmplitude, time, period));
            }
            return shaker;
        }

        public CoroutineClass ShakeHorizontal(float BeginAmplitude = CameraShakeBasicAmplitude, float time = CameraShakeBasicTime, float period = CameraShakeBasicPeriod)
        {
            if (lefttime < time)
            {
                Stop();
                shaker = scCoroutine.Instance.Begin(shake(Vector2.right, BeginAmplitude, time, period));
            }
            return shaker;
        }

        public CoroutineClass Shake(Vector2 axis, float BeginAmplitude = CameraShakeBasicAmplitude, float time = CameraShakeBasicTime, float period = CameraShakeBasicPeriod)
        {
            if (lefttime < time)
            {
                Stop();
                shaker = scCoroutine.Instance.Begin(shake(axis, BeginAmplitude, time, period));
            }
            return shaker;
        }

        public CoroutineClass Scale(float MaximumScaleAdder = 0.2f, float time = 0.2f, float period = 0.1f)
        {
            if (lefttime < time)
            {
                Stop();
                shaker = scCoroutine.Instance.Begin(scaler(MaximumScaleAdder, time, period));
            }
            return shaker;
        }

        List<float> CameraOriginalOrthographicSize = new List<float>();

        IEnumerator scaler(float MaximumScaleAdder = 0.2f, float time = 0.2f, float period = 0.1f)
        {
            CWait wait = new CWait(time);

            var targetCamera = target_pivot;
            float scaleaddr = MaximumScaleAdder;


            while (wait.NotYet)
            {
                lefttime = wait.Left;
                //if (scPause.IsPlaying)
                {
                    //1 period = 민-맥스-민 의 1사이클 = 1pi 만큼의 시간
                    //즉 점점 줄어드는 period 를 가져야한다.
                    //MaximumScaleAddr 은 time동안 지속적으로 줄어든다.
                    scaleaddr = MaximumScaleAdder * wait.LeftRatioSin;

                    //targetCamera.VRun(v => v.transform.localScale = Vector3.one + Vector3.one * scaleaddr * Mathf.Sin(wait.FlowedRatio * (time / period) * Mathf.PI));

                    targetCamera.VRunEach(v => v.GetComponent<Camera>().orthographicSize = 1 + scaleaddr * Mathf.Sin(wait.FlowedRatio * (time / period) * Mathf.PI));

                    //             float flowedSineX = ((wait.FlowedRatio * Mathf.PI * 2)); //time동안 1주기를 도는 싸인의 x값.
                    //             float SineXPeriodPerSec = flowedSineX * time; //time 만큼의 주기를 돈다. 즉 time2 면 time동안 2주기 = 1초ㅓ에 1주기
                    //             float move = Mathf.Sin(SineXPeriodPerSec / period); //period 에 1주기를 돈다.
                    // 
                    //             float amplitude = BeginScale * wait.LeftRatio;
                    //             float finalScaler = move * amplitude;
                    //             targetCamera.transform.localScale = Vector3.one * finalScaler;
                }
                yield return null;
            }
            Stop();
        }

        float lefttime;
        /// <summary>
        /// coroutineouter 를 쓰기위해 한겹 더 쌈
        /// </summary>
        /// <param name="target"></param>
        /// <param name="axis"></param>
        /// <param name="time"></param>
        /// <param name="period"></param>
        /// <param name="BeginAmplitude"></param>
        /// <returns></returns>
        IEnumerator shake(Vector2 axis, float BeginAmplitude = CameraShakeBasicAmplitude, float time = CameraShakeBasicTime, float period = CameraShakeBasicPeriod)
        {

            CWait wait = new CWait(time);

            var targetCamera = target_pivot;
            axis.Normalize();
            wait.IgnorePause = true;
            while (wait.NotYet)
            {
                lefttime = wait.Left;
                float flowedSineX = ((wait.FlowedRatio * Mathf.PI * 2)); //time동안 1주기를 도는 싸인의 x값.
                float SineXPeriodPerSec = flowedSineX * time; //time 만큼의 주기를 돈다. 즉 time2 면 time동안 2주기 = 1초ㅓ에 1주기
                float move = Mathf.Sin(SineXPeriodPerSec / period); //period 에 1주기를 돈다.

                float amplitude = BeginAmplitude * wait.LeftRatio;
                Vector3 finalMover = (axis * move * amplitude);
                targetCamera.VRunEach(v => Util.Put2DOnly(v.transform, finalMover + v.transform.position));
                yield return null;
            }
            Stop();
        }

        public void Stop()
        {
            var v = shaker;
            if (v != null)
            {
                v.Stop();
            }
            target_pivot.VRunEach(t => t.transform.localPosition = Vector3.zero);

            lefttime = 0;

            //orthosize 를 원래대로 되돌린다.
            if (CameraOriginalOrthographicSize != null)
            {
                for (int i = 0; i < CameraOriginalOrthographicSize.Count; ++i)
                {
                    if (CameraOriginalOrthographicSize.Count > i && target_pivot.Length > 0)
                    {
                        var sz = CameraOriginalOrthographicSize[i];
                        var cam = target_pivot[i];
                        if (cam != null && cam.GetComponent<Camera>())
                        {
                            cam.GetComponent<Camera>().orthographicSize = sz;
                        }
                    }
                }
            }
        }
    }


}