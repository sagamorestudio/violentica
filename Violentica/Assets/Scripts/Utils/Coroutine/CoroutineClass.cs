#define UNITY_MODE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZO
{
    public class CoroutineClass : ObjectYield
    {

        public enum WhenDangledNotAcitve
        {
            GoOn,
            Pause,
            Stop
        }

        /// <summary>
        /// AffectBySuspend 를 편하게 설정하게 하기위한 유틸리티 함수
        /// </summary>
        /// <returns></returns>
        public CoroutineClass AffectByWaiterStucked()
        {
            _AffectByWaiterSuspend = true;
            return this;
        }

        /// <summary>
        /// 이 코루틴을 어떤 다른 코루틴이 기다리는 중일 때,
        /// 그 다른 코루틴이 DangledObject(Behaviour)나 Suspend등의 이유로 쉬고 있다면, 이 코루틴도 함께 쉬게된다.
        /// 주로 Util.Wait.ForSeconds 로 발생한 코루틴 등에서 씀.
        /// ※이 코루틴을 기다리는 여러 코루틴 중 1개만 쉬고 있어도 이 코루틴이 함께 멈춘다 명심 또 명심
        /// </summary>
        public bool _AffectByWaiterSuspend = false;


#if UNITY_MODE

        //기본 설정을 어떻게 할 것인가...
        //코루틴이 멈추고 대기 또는 코루틴이 아예 파괴됨

        public WhenDangledNotAcitve DangledDisabledAction = WhenDangledNotAcitve.Pause;
        //public bool KillWhenDisabled = false;

        //dangled monobehaviour
        public GameObject DangledGameobject;
        public bool is_dangled_with_gameobject = false;

        public MonoBehaviour DangledBehaviour;
        public bool is_dangled_with_monobehaviour = false;
#endif

        static int id_generator = 0;
        public int ID;

        /// <summary>
        /// mark as pause
        /// </summary>
        public void Pause()
        {
            Suspend = true;
        }

        /// <summary>
        /// mark as resume
        /// </summary>
        public void Resume()
        {
            if (Suspend)
                Suspend = false;
        }

        /// <summary>
        /// if waited by null, start a tick once
        /// </summary>
        public void ResumeAtOnce()
        {
            if (Suspend)
            {
                Suspend = false;
                if (coroutine.Current == null)
                {
                    MoveNext(true);
                }
            }
        }

        /// <summary>
        /// waiter_list 에 자기자신을 포함한, 자기가 기다리는 대기열의 모든 리스트를 내보내준다.
        /// </summary>
        /// <param name="waiter_list"></param>
        /// <returns></returns>
        public void MakeWaiterList(List<CoroutineClass> waiter_list)
        {
            waiter_list.Clear();
            object waiter = coroutine.Current;
            waiter_list.Add(this);

            while (waiter != null && waiter.GetType() == typeof(CoroutineClass))
            {
                CoroutineClass c_waiter = (CoroutineClass)waiter;
                waiter_list.Add(c_waiter);
                waiter = c_waiter.coroutine.Current;
            }
        }

        //use it like const
        public static object null_key = new object();

        //position
        public object key_of_mylist = null;
        public List<CoroutineClass> mylist = null;
        public scAbstractCoroutine myCoroutine = null;

        //tag : when clear, only tagged is cleared
        public int tag = 0;

        public override string ToString()
        {
            string str = "CoroutineClass{(ID:" + ID + " / track:" + tag + ")" + coroutine + "}, delayed by{" + (coroutine.Current == null ? "null" : coroutine.Current) + "} : finished(" + !br + ")";
            return str;
        }

        public int last_runned_tick = 0;
        public IEnumerator coroutine;

        //bool _br = false;

        public bool br = true;
        //     {
        //         get
        //         {
        //             return _br;
        //         }
        //         set
        //         {
        //             Debug.Log("br from(" + _br + ") to(" + value + ") : " + ToString());
        //             _br = value;
        //         }
        //     }

        public CoroutineClass(scAbstractCoroutine mycoroutine, IEnumerator ii)
        {
            ID = id_generator++;
            coroutine = ii;
            myCoroutine = mycoroutine;
        }

        public void Stop()
        {
            if (br)
            {
                br = false;
                RunFinishFunction();
            }
        }

        public bool Suspend = false;

        public bool LastRunFailedBecauseDangling = false;

        /// <summary>
        /// 무언가에 Dangled되어있고, 그것때문에 플레이불가일 때
        /// </summary>
        public bool IsDangledAndStuckedByIt
        {
            get
            {
                if (is_dangled_with_gameobject)
                {
                    return DangledGameobject == null
                        || (!DangledGameobject.activeInHierarchy && DangledDisabledAction != WhenDangledNotAcitve.GoOn);
                }
                else if (is_dangled_with_monobehaviour)
                {
                    return DangledBehaviour == null
                        ||
                            (
                                (!DangledBehaviour.enabled || !DangledBehaviour.gameObject.activeInHierarchy)
                                && DangledDisabledAction != WhenDangledNotAcitve.GoOn
                            );
                }
                else
                    return false;
            }
        }


        /// <summary>
        /// always call this method when move to next tick
        /// </summary>
        /// <param name="IgnoreRunPerOnceATick"> if true, play with ignoring "Only one move pre One Tick". CAN'T ignore "Dangled Behaviour Enabled State" and "Suspend"</param>
        /// <returns></returns>
        public bool MoveNext(bool IgnoreRunPerOnceATick)
        {
            //finished coroutine is absolutely stop!
            if (!br)
                return false;

            if (Suspend)    //just suspended
                return false;
            #region UNITY_DangledByMonoBehaviour
#if UNITY_MODE
            //check unity specpic

            if (is_dangled_with_monobehaviour)
            {
                if (DangledBehaviour == null)   //dangled monobehaviour is destroyed
                {
                    Stop();
                    LastRunFailedBecauseDangling = true;
                    return false;
                }
                else if (!DangledBehaviour.gameObject.activeInHierarchy || !DangledBehaviour.enabled)    //dangled monobehaviour is disabled
                {
                    switch (DangledDisabledAction)
                    {
                        case WhenDangledNotAcitve.GoOn:
                            goto GO_ON;
                        case WhenDangledNotAcitve.Pause:
                            LastRunFailedBecauseDangling = true;
                            return false;
                        case WhenDangledNotAcitve.Stop:
                            Stop();
                            return false;
                    }
                    //if (DangledDisabledAction == WhenDangledNotAcitve.Stop) Stop();
                }
            }
            else if (is_dangled_with_gameobject)
            {
                if (DangledGameobject == null)  //dangled gameobject is destroyed
                {
                    Stop();
                    LastRunFailedBecauseDangling = true;
                    return false;
                }
                else if (!DangledGameobject.activeInHierarchy)  //dangled gameobject is disabled
                {
                    switch (DangledDisabledAction)
                    {
                        case WhenDangledNotAcitve.GoOn:
                            goto GO_ON;
                        case WhenDangledNotAcitve.Pause:
                            LastRunFailedBecauseDangling = true;
                            return false;
                        case WhenDangledNotAcitve.Stop:
                            Stop();
                            return false;
                    }
                }
            }
#endif
            #endregion
            GO_ON:

            LastRunFailedBecauseDangling = false;
            if (IgnoreRunPerOnceATick)
                return moveNext_IgnoreRunPerATick();
            else
                return moveNext();
        }

        bool moveNext()
        {
            //실행은 한 틱에 한 번만!
            //Util.Log("br is" + br+"/last_runn:"+last_runned_tick+"/curr:"+current_tick);
            if (last_runned_tick < myCoroutine.CurrentTick)
            {
                last_runned_tick = myCoroutine.CurrentTick;

                //MoveNext할 수 있는지 체크
                if (coroutine.Current == null)
                {
                    bool t_br = coroutine.MoveNext();
                    if (br) br = t_br;
                    if (!br) RunFinishFunction();
                }
                else if (coroutine.Current is CoroutineClass)
                {
                    //쉰다.
                    CoroutineClass waitCoroutine = (CoroutineClass)coroutine.Current;

                    //만약 waitCoroutine이 끝나있는 상황이라면..
                    if (!waitCoroutine.br)
                    {
                        bool t_br = coroutine.MoveNext();
                        if (br) br = t_br;
                        if (!br) RunFinishFunction();
                    }
                }
                return br;
            }
            return br;
        }
        //한 틱에 한번 실행 제한 없이 강제실행한다.
        bool moveNext_IgnoreRunPerATick()
        {
            last_runned_tick = myCoroutine.CurrentTick;
            bool t_br = coroutine.MoveNext();
            if (br) br = t_br;
            if (!br) RunFinishFunction();
            return br;
        }

        public void RunFinishFunction()
        {
            if (OnFinishFunction != null)
            {
                OnFinishFunction();
                OnFinishFunction = null;
            }
            if(OnFinishFunction_param != null)
            {
                OnFinishFunction_param(this);
                OnFinishFunction_param = null;
            }
        }

        public CoroutineClass SetFinishFunction(Util.DelegateVoidNoParam func)
        {
            OnFinishFunction = func;
            if (!br)
                RunFinishFunction();
            return this;
        }

        public CoroutineClass SetFinishFunction(Util.DelegateVoid1Param<CoroutineClass> func)
        {
            OnFinishFunction_param = func;
            if (!br)
                RunFinishFunction();
            return this;
        }

        Util.DelegateVoidNoParam OnFinishFunction;
        Util.DelegateVoid1Param<CoroutineClass> OnFinishFunction_param;
    }

    public class ObjectYield
    {

    }

    public static class CCExtensionMethods
    {
        public static void StopIfNotNull(this CoroutineClass t)
        {
            if (t != null) t.Stop();
        }
    }

    public class CWait
    {
        public static CoroutineClass ForSeconds(float time)
        {
            return scCoroutine.Instance.Begin(forSeconds(time, false));
        }

        public static CoroutineClass ForSeconds_IgnorePause(float time)
        {
            return scCoroutine.Instance.Begin(forSeconds(time, true));
        }

        static IEnumerator forSeconds(float time, bool ignore_pause)
        {
            var w = new CWait(time);
            w.IgnorePause = ignore_pause;
            while (w.NotYet) yield return null;
        }

        public bool IgnorePause;
        public float FlowedRatio    //1을 못넘게한다. timelimit이 0일때 Inf 뜨는 문제 막기위함
        {
            get
            {
                float ratio = accumtime / timelimit;
                if (ratio > 1)
                    return 1;
                return ratio;
            }
        }
        public float FlowedRatioSinToPI
        {
            get
            {
                return Mathf.Sin(FlowedRatio * (Mathf.PI));
            }
        }
        public float FlowedRatioSin
        {
            get
            {
                return Mathf.Sin(FlowedRatio * (Mathf.PI / 2));
            }
        }
        public float FlowedRatio1MinusCos
        {
            get
            {
                return 1 - Mathf.Cos(FlowedRatio * (Mathf.PI / 2));
            }
        }

        public float LeftRatioSin
        {
            get
            {
                return Mathf.Sin(LeftRatio * (Mathf.PI / 2)); ;
            }
        }
        public float LeftRatio
        {
            get
            {
                return Left / timelimit;
            }
        }
        public float LeftRatio1MinusCos
        {
            get
            {
                return 1 - Mathf.Cos(LeftRatio * (Mathf.PI / 2));
            }
        }
        public float Flowed
        {
            get { return accumtime; }
            set { accumtime = value; }
        }
        public float Left
        {
            get { return timelimit - accumtime; }
        }
        float accumtime;
        float timelimit;

        public CWait()
        {
            IgnorePause = false;
            Reset(0);
        }
        public CWait(float time)
        {
            IgnorePause = false;
            Reset(time);
        }
        public bool NotYet
        {
            get
            {
                return !IsExpired();
            }
        }
        public bool IsExpired() //call every tick
        {
            if (!IgnorePause && scPause.isPause) return false;

            if (accumtime > timelimit)
                return true;

            accumtime += Time.deltaTime;

            return false;
        }
        public void Reset()
        {
            Reset(0);
        }
        public void Reset(float time)
        {
            accumtime = 0;
            timelimit = time;
        }
    }
}

