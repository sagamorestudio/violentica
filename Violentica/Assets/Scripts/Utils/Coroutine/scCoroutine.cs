//////////////////////////////////////////////////////////////////////////
//scCoroutine 1.0.0
//Author : WONZO (yuryueng@gmail.com)
//Lisence : LGPL
//
//1. Major Feature
//Non-Delay Coroutine : 
//  MonoBehaviour.StartCoroutine has 1 frame delay after call that method.
//  'scCoroutine' is not have that delay. so you can use chain of coroutine [yield return 'Coroutine'] without frame delay.
//
//2. How to install
//just paste .cs files in your Unity Project
//
//3. How to use
//3-1. Call:
//  IEnumerator AnyCoroutineMethod() { ... }
//  var cc = scCoroutine.Instance.Begin(AnyCoroutineMethod()); //just same as normal coroutine.
//  var cc = scCoroutine.Play(AnyCoroutineMethod());
//3-2. Stop:
//  cc.Stop()
//  cc.SetFinishFunction({ /*void no-parameter method*/});    //FinishFunction is called after finish. The FinishFunction is called even coroutine is finished by 'Stop()' method. FinishFunction is called just once if you call 'Stop' many times.
//  cc.StopIfNotNull();         //if cc is null, it prevent 'NullReferenceException'.
//3-3. yield :
//  'scCoroutine' is not share yield with 'MonoBehaviour.StartCoroutine'. 
//      example) you can't use 'yield return new WaitForSeconds()' or 'yield return new yield return WaitForEndOfFrame()'
//  yield return null;  //1 frame wait.
//  yield return scCoroutine.Play(AnyCoroutineMethod());
//  yield return cc;    //var cc = scCoroutine.Instance.Begin(AnyCoroutineMethod());
//  yield return CWait.ForSeconds(0.5f);    //wait for 0.5 seconds
//
//4. Usage of CWait
//  ...inside of IEnumerator method
//  CWait w = new CWait(5);
//  var old = transform.localPosition;
//  while(w.NotYet) 
//  { 
//      transform.localPostion = old + Vector3.left * w.FlowedRatioSin * Time.deltaTime;
//      yield return null;
//  }
//  //this is simple moving code 
//////////////////////////////////////////////////////////////////////////


//this is used for debug.
//#define LOG_ALL

//this is only unity mode (MonoBehaviour is Exist)
#define UNITY_MODE


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZO
{
    public class scCoroutine : scAbstractCoroutine
    {
        public static CoroutineClass Play(IEnumerator ie)
        {
            return Instance.Begin(ie);
        }


#if LOG_ALL
    List<CoroutineClassNew> RunThisTick = new List<CoroutineClassNew>();
#endif

        public CoroutineClass NULL
        {
            get
            {
                return new CoroutineClass(this, nopCoroutine());
            }
        }

        IEnumerator nopCoroutine()
        {
            yield break;
        }

        static scCoroutine _instance = null;
        public static scCoroutine Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject g = new GameObject();
                    _instance = g.AddComponent<scCoroutine>();
                    g.name = "Coroutine";
                    DontDestroyOnLoad(g);
                }
                return _instance;
            }
        }

        /// <summary>
        /// cc가 기다리고 있는 모든 코루틴을 연쇄적으로 중단
        /// cc를 기다리고 있는 모든 코루틴을 연쇄적으로 중단
        /// </summary>
        /// <param name="cc"></param>
        public void StopRecusively(CoroutineClass cc)
        {
            if (cc != null)
            {
                var ls = GetAllStuckedBy(cc);
                foreach (var v in ls)
                    v.Stop();
                cc.StopIfNotNull();
            }
        }

        /// <summary>
        /// cc에 의해 중지중인 모든 코루틴을 찾아낸다
        /// post-ordering 임
        /// seed 자기자신 미포함임
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="usethis"></param>
        /// <returns></returns>
        public List<CoroutineClass> GetAllStuckedBy(CoroutineClass cc, List<CoroutineClass> usethis = null)
        {
            if (usethis == null)
                usethis = new List<CoroutineClass>();

            object key = null;
            if (cc == null)
                key = CoroutineClass.null_key;
            else
                key = cc.coroutine.Current;

            var lsCC = Get(key);

            foreach (var v in lsCC)
                GetAllStuckedBy(v, usethis);
            usethis.Add(Get(cc));

            return usethis;
        }

        public static void StopAll()
        {
            if (_instance == null)
                return;

            foreach (List<CoroutineClass> lscc in _instance.dicTotalCoroutines.Values)
            {
                foreach (CoroutineClass cc in lscc)
                    cc.br = false;
            }
        }

        public static void StopAll(int tag)
        {
            if (_instance == null)
                return;

            foreach (List<CoroutineClass> lscc in _instance.dicTotalCoroutines.Values)
            {
                foreach (CoroutineClass cc in lscc)
                {
                    if (cc.tag == tag)
                        cc.br = false;
                }
            }
        }

        public override string ToString()
        {
            string ostr = "scCoroutineNew[\n";
            foreach (List<CoroutineClass> lscc in _instance.dicTotalCoroutines.Values)
            {
                foreach (CoroutineClass cc in lscc)
                {
                    ostr += cc.ToString() + "\n-----------------------------------------\n";
                }
            }
            ostr += "\n]--------------------------------------";
            return ostr;
        }

        public string ToString(int tag)
        {
            string ostr = "scCoroutineNew[\n";
            foreach (List<CoroutineClass> lscc in _instance.dicTotalCoroutines.Values)
            {
                foreach (CoroutineClass cc in lscc)
                {
                    if (tag == cc.tag)
                        ostr += cc.ToString() + "\n-----------------------------------------\n";
                }
            }
            ostr += "\n]--------------------------------------";
            return ostr;
        }


        void Update()
        {
            RunCoroutinesInList_A_Tick(null);
            ++CurrentTick;
        }


#if UNITY_MODE
        //unity coroutine control utility
        public void StopAllDangledCoroutine(MonoBehaviour mb)
        {
            dicTotalCoroutines.VRun((KeyValuePair<object, List<CoroutineClass>> kv) => kv.Value.VRunEach(v => (v.DangledBehaviour == mb).IfTrue(() => v.StopIfNotNull())));
        }

        public void PauseAllDangledCoroutine(MonoBehaviour mb)
        {
            dicTotalCoroutines.VRun((KeyValuePair<object, List<CoroutineClass>> kv) => kv.Value.VRunEach(v => (v.DangledBehaviour == mb).IfTrue(() => v.Pause())));
        }

        public void ResumeAllDangledCoroutine(MonoBehaviour mb)
        {
            dicTotalCoroutines.VRun((KeyValuePair<object, List<CoroutineClass>> kv) => kv.Value.VRunEach(v => (v.DangledBehaviour == mb).IfTrue(() => v.Resume())));
        }

        public void StopAllDangledCoroutine(GameObject g)
        {
            g.GetComponents<MonoBehaviour>().VRunEach(v => scCoroutine.Instance.StopAllDangledCoroutine(v));
        }

        public void PauseAllDangledCoroutine(GameObject g)
        {
            g.GetComponents<MonoBehaviour>().VRunEach(v => scCoroutine.Instance.PauseAllDangledCoroutine(v));
        }

        public void ResumeAllDangledCoroutine(GameObject g)
        {
            g.GetComponents<MonoBehaviour>().VRunEach(v => scCoroutine.Instance.ResumeAllDangledCoroutine(v));
        }
#endif


        /// <summary>
        /// 만들어진 코루틴의 첫실행.
        /// 내부적으로만 호출
        /// </summary>
        /// <param name="cc"></param>
        void coroutineFirstRun(CoroutineClass cc)
        {
            do
            {
                cc.MoveNext(true);
            }
            while (
            //아직 안 끝났으며, 코루틴에 의해 막혔는데, 그 코루틴이 노딜레이로 끝났다면 계속함
            cc.br
            && cc.coroutine.Current is CoroutineClass
            && !((CoroutineClass)cc.coroutine.Current).br
                );

            //내가 끝났다면 노딜레이 피니시
            if (!cc.br)
            {
                cc.RunFinishFunction();
            }
            else
            {
                #region commantations
                //노 딜레이로 끝나지 않음 = 바로 안 끝났다는 이야기

                //케이스 : 
                // 0. 내가 끝났는데 나를 기다리게 하는놈이 있을 수 없음.
                // 1. 난 아직 안 끝났고, 나를 기다리게 한 게 함수이고, 그놈이 nodelayfinish라면 -> 앞에서 이미 체크됨
                // 2. 난 아직 안 끝났고, 나를 기다리게 한 게 함수이고, 그놈이 delayed-finish라면 -> ... 여기서 그럴수가 있나? 걍 리턴 (불가능한 case이므로)
                // 3. 난 아직 안 끝났고, 나를 기다리게 한 게 함수이고, 그놈이 not yet finished 라면 -> 내가 이미 실행됐으므로 return
                // 4. 난 아직 안 끝났고, 나를 기다리게 한 게 null이면 -> return

                /*
                //3 :
                if(cc.coroutine.Current is CoroutineClass)
                {
                    Add(cc.coroutine.Current, cc);
                    return cc;
                }
                else
                //4 : 아직 안 끝남
                if (cc.coroutine.Current == null)
                {
                    Add(cc.coroutine.Current, cc);
                    return cc;
                }
                */

                //3과 4는 처리방식이 동일하므로 그냥 이렇게 처리. 그러나 위쪽에 로직을 남겨놓는 이유는 legacy를 위하여
                #endregion
                Add(cc.coroutine.Current, cc);
            }
        }

        /// <summary>
        /// standalone begin
        /// </summary>
        /// <param name="wanna_run"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public CoroutineClass Begin(IEnumerator wanna_run, int tag = 0)
        {
            try
            {
                CoroutineClass cc = new CoroutineClass(this, wanna_run);
                cc.myCoroutine = this;
                cc.tag = tag;
                coroutineFirstRun(cc);
                return cc;
            }
            catch (System.Exception e)
            {
                Util.LogError(e);
                return null;
            }
        }

#if UNITY_MODE
        public CoroutineClass Begin_Dangled(MonoBehaviour mb, IEnumerator wanna_run, CoroutineClass.WhenDangledNotAcitve dangledisbleAction, int tag = 0)
        {
            CoroutineClass cc = new CoroutineClass(this, wanna_run);
            cc.myCoroutine = this;
            cc.tag = tag;
            if (mb != null)
            {
                //dangling
                cc.DangledBehaviour = mb;
                cc.is_dangled_with_monobehaviour = true;
            }
            cc.DangledDisabledAction = dangledisbleAction;
            coroutineFirstRun(cc);
            return cc;
        }

        public CoroutineClass Begin_Dangled(GameObject mb, IEnumerator wanna_run, CoroutineClass.WhenDangledNotAcitve dangledisbleAction, int tag = 0)
        {
            CoroutineClass cc = new CoroutineClass(this, wanna_run);
            cc.myCoroutine = this;
            cc.tag = tag;
            if (mb != null)
            {
                //dangling
                cc.DangledGameobject = mb;
                cc.is_dangled_with_gameobject = true;
            }
            cc.DangledDisabledAction = dangledisbleAction;
            coroutineFirstRun(cc);
            return cc;
        }

#endif

        //legacy
        //     bool isRunOnceNextCoroutine(object o)
        //     {
        //         if (o == null) return false;
        //         if (o.GetType() == typeof(CoroutineClass))
        //         {
        //             var cc = o as CoroutineClass;
        //             return cc.IsRunNextOncePerATick;
        //         }
        //         return false;
        //     }

        /// <summary>
        /// 동시에 실행되도 되는 코루틴을 다 실행해버린다.
        /// </summary>
        /// <param name="cList">이 입력값만 누군가의 복사값이면 </param>
        public void RunCoroutinesInList_A_Tick(object seed_reason)
        {
            if (seed_reason == null)
                seed_reason = CoroutineClass.null_key;

            List<object> will_run_reasons = new List<object>();

            //일단 cList 에 들어오는 놈들은 누군가에 의해 막혀본 경험 있는 놈들이다
            //기본적으로 할일 : cList를 돌면서 전부 MoveNext(Surpress)
            //결과에 따른 케바케 처리
            //!br -> 끝났으므로 나를 finished list 에 추가해준다. 
            //       
            //br, by coroutine
            // -> StuckedByCoroutine 으로 보낸다.
            //br, by null
            // -> StuckedByNull 로 보낸다.

            will_run_reasons.Add(seed_reason);

#if LOG_ALL
        RunThisTick.Clear();
#endif

            do
            {
                //이 reason은 제거되었다.
                object current_reason = will_run_reasons[will_run_reasons.Count - 1];
                if (current_reason == null)
                    current_reason = CoroutineClass.null_key;

                will_run_reasons.RemoveAt(will_run_reasons.Count - 1);

                List<CoroutineClass> lscc = Get(current_reason);
                if (lscc != null)
                {
                    CoroutineClass[] ccs = lscc.ToArray();
                    //전부 돌린다.
                    foreach (CoroutineClass cc in ccs)
                    {
                        if (cc._AffectByWaiterSuspend)
                        {
                            var ls = Get(cc);
                            if (ls != null && ls.Find(c => c.Suspend || c.IsDangledAndStuckedByIt) != null)
                                continue;
                        }

#if LOG_ALL
                    RunThisTick.Add(cc);
#endif
                        if (current_reason == CoroutineClass.null_key)
                            cc.MoveNext(false);
                        else
                            cc.MoveNext(true);

                        //즉시 끝나는 코루틴에 의해 막혀있는 경우 연속적으로 실행을 시킨다.
                        while (
                                //아직 안 끝났으며, 코루틴에 의해 막혔는데, 그놈이 노딜레이로 끝났다면 계속함
                                cc.br
                                && cc.coroutine.Current is CoroutineClass
                                && !((CoroutineClass)cc.coroutine.Current).br
                                //추가: 마지막 실행이 댕글링이 diabled 된 이유라면 계속하지 않음... 안그러면 무한루프 도니까
                                && !cc.LastRunFailedBecauseDangling
                                    )
                        {
                            cc.MoveNext(true);
                        }

                        if (cc.br)
                        {
                            //만약 안 끝났다면 재등록
                            if (cc.coroutine.Current == null)
                                Add(CoroutineClass.null_key, cc);
                            else
                                Add(cc.coroutine.Current, cc);
                        }
                        else
                        {
                            //끝났다면 나에 의해 멈춰있는 애들을 실행리스트에 등록
                            int prev = will_run_reasons.Count;
                            if (has(cc))
                            {
                                will_run_reasons.Add(cc);
                            }
                            else
                                //만약, 나에 의해 멈춰있는 애가 하나도 없을때에만 나를 제거한다.
                                //왜냐하면 suspend, disabled by dangling 등의 이유로 멈춰있는 애들이 대기중일 수 있음...
                                RemoveThis(cc);
                        }
                    }
                }
            } while (will_run_reasons.Count != 0);


#if LOG_ALL
        string log = "Tick = " + CurrentTick +"\n";
        foreach (var v in RunThisTick)
        {
            log += v.ToString() + "\n";
        }
        Debug.Log(log);
#endif
        }

        Dictionary<object, List<CoroutineClass>> dicTotalCoroutines = new Dictionary<object, List<CoroutineClass>>();

        bool has(object reason)
        {
            if (reason == null)
                reason = CoroutineClass.null_key;
            return dicTotalCoroutines.ContainsKey(reason);
        }

        /// <summary>
        /// dictionary 로부터 reason을 멈춤원인으로 하는 코루틴을 전부 가져온다.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        List<CoroutineClass> Get(object reason)
        {
            List<CoroutineClass> outv = null;
            if (reason == null)
                reason = CoroutineClass.null_key;

            if (dicTotalCoroutines.TryGetValue(reason, out outv))
                return outv;
            return null;
        }

        /// <summary>
        /// dictionary에 reason을 멈춤원인으로 하는 코루틴을 등록
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="cc"></param>
        void Add(object reason, CoroutineClass cc)
        {
            if (reason == null)
                reason = CoroutineClass.null_key;

            if (cc.key_of_mylist == reason)
                return;

            else
            {
                RemoveThis(cc);
            }

            List<CoroutineClass> lsCC = Get(reason);
            if (lsCC == null)
            {
                lsCC = new List<CoroutineClass>();
                dicTotalCoroutines.Add(reason, lsCC);
            }

            lsCC.Add(cc);
            cc.key_of_mylist = reason;
            cc.mylist = lsCC;
        }

        /// <summary>
        /// dictionary에 등록되있던 코루틴 제거 : 이 코루틴에 의해 stuck 되어있던 애들의 실행은 보장하지 않는다.
        /// </summary>
        /// <param name="cc"></param>
        void RemoveThis(CoroutineClass cc)
        {
            if (cc.key_of_mylist == null)
                return;

            cc.mylist.Remove(cc);
            if (cc.mylist.Count == 0)
            {
                dicTotalCoroutines.Remove(cc.key_of_mylist);
            }

            cc.mylist = null;
            cc.key_of_mylist = null;
        }

    }

    public static class ExtensionOfscCoroutine
    {
#if UNITY_MODE
        public static CoroutineClass PlayCoroutine(this MonoBehaviour mb, IEnumerator coroutine, CoroutineClass.WhenDangledNotAcitve dangledisableAction = CoroutineClass.WhenDangledNotAcitve.Pause)
        {
            return scCoroutine.Instance.Begin_Dangled(mb, coroutine, dangledisableAction);
        }

        public static void StopAll(this MonoBehaviour mb)
        {
            scCoroutine.Instance.StopAllDangledCoroutine(mb);
        }

        public static CoroutineClass PlayCoroutine(this GameObject g, IEnumerator coroutine, CoroutineClass.WhenDangledNotAcitve dangledisableAction = CoroutineClass.WhenDangledNotAcitve.Pause)
        {
            return scCoroutine.Instance.Begin_Dangled(g, coroutine, dangledisableAction);
        }

        public static void StopAll(this GameObject g)
        {
            scCoroutine.Instance.StopAllDangledCoroutine(g);
        }
#endif

    }
}

