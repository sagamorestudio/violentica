using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ZO
{
    /// <summary>
    /// 오브젝트 트윈용 오브젝트
    /// 특정 오브젝트를 따라가는데 능하다.
    /// </summary>
    public class scMover : MonoBehaviour
    {

        static scMover instance;
        public static scMover Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (new GameObject()).AddComponent<scMover>();
                    instance.name = "Mover";
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
        void Awake()
        {
            gameObjectPool = transform;
        }

        /// <summary>
        /// move 를 위한 utility class
        /// 직선이동에 대한 모든 정보를 담고있다.
        /// </summary>
        [System.Serializable]
        public class Mover
        {
            public bool DontStopIfPaused
            {
                get
                {
                    return waiter.IgnorePause;
                }
                set
                {
                    waiter.IgnorePause = value;
                }
            }
            static int id_generator = 0;
            public int unique_id;
            public int curve_0normal_1sin_2oneminuscos_3sin0toquaterpi;
            public bool NotYet { get { return !MoveFinished; } }
            public bool MoveFinished;
            public Transform Body;
            public Transform Target;
            public CWait waiter;
            public Vector3 beginpos;
            public bool surpressStopPrev;
            scMover manager;
            public bool IsPaused;

            Util.DelegateVoid0Param FinishFunc;

            public Mover RemoveFinishFunction()
            {
                FinishFunc = null;
                return this;
            }
            public Mover SetFinishFunction(Util.DelegateVoid0Param vf)
            {
                FinishFunc = vf;
                return this;
            }

            public Util.DelegateVoid0Param GetFinishFunction()
            {
                return FinishFunc;
            }

            public Mover(Transform target, Transform body, float time, int curve, bool surpress)
            {
                waiter = new CWait(time);
                Body = body;
                Target = target;
                MoveFinished = false;
                beginpos = body.position;
                manager = scMover.Instance;
                surpressStopPrev = surpress;
                curve_0normal_1sin_2oneminuscos_3sin0toquaterpi = curve;
                unique_id = id_generator++;
                IsPaused = false;
            }
            public void Update()
            {
                if (MoveFinished) return;

                if (IsPaused) return;

                if (Target == null || !Target.gameObject.activeSelf)
                {
                    //Util.LogError("Mover Finished Target Lost");
                    Stop();
                    return;
                }
                else if (Body == null || !Body.gameObject.activeSelf)
                {
                    //Util.LogError("Mover Finished Body Lost");
                    Stop();
                    return;
                }

                if (DontStopIfPaused || scPause.IsPlaying)
                {
                    if (waiter.NotYet)
                    {
                        Vector3 distTotal = Target.position - beginpos;
                        distTotal.z = 0;
                        switch (curve_0normal_1sin_2oneminuscos_3sin0toquaterpi)
                        {
                            case 1:
                                Body.position = beginpos + distTotal * waiter.FlowedRatioSin;
                                break;
                            case 2:
                                Body.position = beginpos + distTotal * waiter.FlowedRatio1MinusCos;
                                break;
                            case 3:
                                Body.position = beginpos + distTotal * (Mathf.Sin(waiter.FlowedRatio * Mathf.PI/4) / Mathf.Sin(Mathf.PI/4));
                                break;
                            case 0:
                            default:
                                Body.position = beginpos + distTotal * waiter.FlowedRatio;
                                break;
                        }
                    }
                    else
                    {
                        //마감처리
                        Stop(true);
                    }
                }
            }
            bool stopcalled = false;
            public void Stop(bool PutToFinish = false)
            {
                if (stopcalled) return;
                stopcalled = true;

                if (!MoveFinished && FinishFunc != null)
                    FinishFunc();

                MoveFinished = true;

                if (PutToFinish)
                {
                    if (Target != null && Body != null)
                        Body.position = new Vector3(Target.position.x, Target.position.y, Body.position.z);
                }
                if (Target != null) manager.removeTargetObjectPooled(Target);
            }

            public override string ToString()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("MoverInfo(" + unique_id + ")");
                if (Body != null)
                    sb.AppendLine("BODY(" + Util.ToString(Body.transform.position) + ") : " + Body.GetHierarchyName());
                else
                    sb.Append("BODY(Destroyed)");
                sb.AppendLine("Target(" + Util.ToString(Target.transform.position) + ") : " + Body.GetHierarchyName());
                sb.AppendLine("Time(" + waiter.Flowed + "(" + (waiter.FlowedRatio * 100.0f) + "%))");
                sb.AppendLine("IsFinished(" + MoveFinished + ")");
                return sb.ToString();
            }

            public void Pause()
            {
                
            }
        }

        //target object 를 담아놓는 pool
        [SerializeField]
        List<GameObject> targetObjectPool = new List<GameObject>();
        [SerializeField]
        List<GameObject> targetObjectUsing = new List<GameObject>();

        [SerializeField]
        //target object pool 의 parent object
        private Transform gameObjectPool;

        private const string objprefix = "_scMover.TempTarget";

        //target object pool 로부터 받아온다.
        Transform createTargetObjectPooled(Vector3 localPosition, Transform parent = null)
        {
            GameObject obj = null;
            if (targetObjectPool.Count > 0)
            {
                obj = targetObjectPool[targetObjectPool.Count - 1];
                targetObjectPool.RemoveAt(targetObjectPool.Count - 1);
                targetObjectUsing.Add(obj);
            }
            else
            {
                obj = new GameObject();
                obj.name = objprefix;
                targetObjectUsing.Add(obj);
            }
            var t = obj.transform;
            t.parent = parent;
            t.localPosition = localPosition;
            return t;
        }

        //target object pool 로 집어넣는다. (만약 기본제공임시타겟오브젝이 아니면 냄겨놈)
        void removeTargetObjectPooled(Transform obj)
        {
            if (obj != null && targetObjectUsing.Remove(obj.gameObject))
            {
                targetObjectPool.Add(obj.gameObject);
                obj.parent = gameObjectPool;
            }
        }

        Dictionary<Transform, Mover> TotalMoveset = new Dictionary<Transform, Mover>();
        List<Mover> ReservedMoveSet = new List<Mover>();

        //     public Mover MoveLocal(GameObject target, Vector3 targetLocalPos, float time, int curve_0normal_1sin_2oneminuscos = 1)
        //     {
        //         return new Mover(target, );
        //     }



        /// <summary>
        /// 직선이동을 개시한다. 단, 한번에 하나만 이동한다.
        /// </summary>
        /// <param name="moveObject">이동할 오브젝</param>
        /// <param name="targetObject">타겟 오브젝</param>
        /// <param name="time">이동 시간</param>
        /// <param name="curve_0normal_1sin_2oneminuscos">속도 커브</param>
        /// <param name="surpress">강제이동. true라면 기존에 하던 이동이 있을 경우 꺼버리고 새 이동을 추구한다.</param>
        /// <returns></returns>
        public Mover MoveToTarget(GameObject moveObject, GameObject targetObject, float time, int curve_0normal_1sin_2oneminuscos = 1, bool surpress = false)
        {
            if (surpress) StopAndCancelAllReserved(moveObject);

            //Util.LogError("MoveToTarget : " + Util.GetHierarchyName<scCard>(moveObject));
            var obj = moveObject.transform;
            var mov = new Mover(targetObject.transform, obj, time, curve_0normal_1sin_2oneminuscos, surpress);
            ReservedMoveSet.Add(mov);
            return mov;
        }

        /// <summary>
        /// 직선이동을 개시한다. 단, 한번에 하나만 이동한다.
        /// </summary>
        /// <param name="moveObject">이동할 오브젝</param>
        /// <param name="target">목표점(글로벌 포지션)</param>
        /// <param name="time">이동 시간</param>
        /// <param name="curve_0normal_1sin_2oneminuscos">속도 커브</param>
        /// <param name="surpress">강제이동. true라면 기존에 하던 이동이 있을 경우 꺼버리고 새 이동을 추구한다.</param>
        /// <returns></returns>
        public Mover MoveToTargetGlobal(GameObject moveObject, Vector3 target, float time, int curve_0normal_1sin_2oneminuscos = 1, bool surpress = false)
        {
            if (surpress) StopAndCancelAllReserved(moveObject);

            //Util.LogError("MoveToTargetGlobal : " + Util.GetHierarchyName<scCard>(moveObject));
            var obj = moveObject.transform;
            var targetObject = createTargetObjectPooled(target, null);
            targetObject.parent = gameObjectPool;
            var mov = new Mover(targetObject.transform, obj, time, curve_0normal_1sin_2oneminuscos, surpress);
            ReservedMoveSet.Add(mov);
            return mov;
        }

        /// <summary>
        /// 직선이동을 개시한다. 단, 한번에 하나만 이동한다.
        /// </summary>
        /// <param name="moveObject">이동할 오브젝</param>
        /// <param name="target">목표점(로컬 포지션)</param>
        /// <param name="time">이동 시간</param>
        /// <param name="curve_0normal_1sin_2oneminuscos">속도 커브</param>
        /// <param name="surpress">강제이동. true라면 기존에 하던 이동이 있을 경우 꺼버리고 새 이동을 추구한다.</param>
        /// <returns></returns>
        public Mover MoveToTargetLocal(GameObject moveObject, Vector3 target, float time, int curve_0normal_1sin_2oneminuscos = 1, bool surpress = false)
        {
            if (surpress) StopAndCancelAllReserved(moveObject);

            //Util.LogError("MoveToTargetLocal : " + Util.GetHierarchyName<scCard>(moveObject));
            var obj = moveObject.transform;
            var targetObject = createTargetObjectPooled(target, moveObject.transform.parent);
            targetObject.parent = gameObjectPool;
            var mov = new Mover(targetObject.transform, obj, time, curve_0normal_1sin_2oneminuscos, surpress);
            ReservedMoveSet.Add(mov);
            return mov;
        }

        public Mover Find(GameObject moveObject)
        {
            Mover mov = null;
            if (TotalMoveset.TryGetValue(moveObject.transform, out mov))
                return mov;
            else
                return null;
        }

        public void Pause(GameObject moveObject)
        {
            var mov = Find(moveObject);
            if (mov != null)
            {
                mov.Pause();
                TotalMoveset.Remove(moveObject.transform);
            }
        }

        public void Stop(GameObject moveObject)
        {
            var mov = Find(moveObject);
            if (mov != null)
            {
                mov.Stop();
                TotalMoveset.Remove(moveObject.transform);
            }
        }

        public void CancelAllReserved(GameObject moveObject)
        {
            ReservedMoveSet.RemoveAll(delegate (Mover m)
            {
                if (m.Body == moveObject.transform) return true;
                return false;
            });
        }

        public void StopAndCancelAllReserved(GameObject moveObject)
        {
            Stop(moveObject);
            CancelAllReserved(moveObject);
        }

        List<Transform> temp_keylist = new List<Transform>();
        // Update is called once per frame
        void LateUpdate()
        {
            //모든 애들을 돌리면서 update 로 진행하고 끝낸애들은 리스트에서제거하고, 예약된놈들은 들여보낸다.

            temp_keylist.Clear();
            foreach (var key in TotalMoveset.Keys)
                temp_keylist.Add(key);

            foreach (var key in temp_keylist)
            {
                Mover mov = null;
                if (TotalMoveset.TryGetValue(key, out mov))
                {
                    mov.Update();
                    if (mov.MoveFinished) TotalMoveset.Remove(key);
                }
            }

            var ls = ReservedMoveSet.ToArray();
            foreach (var mov in ls)
            {
                Mover current = null;
                if (TotalMoveset.TryGetValue(mov.Body, out current))
                {
                    if (mov.surpressStopPrev)
                    {
                        current.Stop();
                        TotalMoveset.Remove(current.Body);
                        TotalMoveset.Add(mov.Body, mov);
                        ReservedMoveSet.Remove(mov);
                    }
                    //else do nothing
                }
                else
                {
                    TotalMoveset.Add(mov.Body, mov);
                    ReservedMoveSet.Remove(mov);
                }
            }
        }
    }

    public static class scMoverExtensions
    {
        public static CoroutineClass UntilFinish(this scMover.Mover m)
        {
            return CWait.ForSeconds(m.waiter.Left);
        }

        public static void StopAndCancelAllReserved_scMover(this GameObject g)
        {
            scMover.Instance.StopAndCancelAllReserved(g);
        }
    }


}