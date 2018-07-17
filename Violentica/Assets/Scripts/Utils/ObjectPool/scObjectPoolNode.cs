using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZO
{
    public class scObjectPoolNode : MonoBehaviour
    {

        static int id_generator = 0;
        public int UniqueID;

        public int RecycledCount = 0;

        public bool IsAlive = true;

        public System.Type OriginalType;

        public GameObject OriginalObject;

        public Util.DelegateVoid0Param WhenPoolDeath;

        void Awake()
        {
            UniqueID = ++id_generator;
        }

        public List<KeyValuePair<System.Reflection.MethodInfo, object>> baked_runs = new List<KeyValuePair<System.Reflection.MethodInfo, object>>();

        public void Run(string functionaname)
        {
            foreach (var v in baked_runs)
            {
                if (v.Key.Name == functionaname)
                    v.Key.Invoke(v.Value, null);
            }
        }

        public void Bake()
        {
            FindFunctionRecusively(gameObject, "Awake", baked_runs);
            FindFunctionRecusively(gameObject, "Start", baked_runs);
            FindFunctionRecusively(gameObject, "OnDestroy", baked_runs);
        }

        /// <summary>
        /// 찾아서 리커시브하게 호출
        /// </summary>
        /// <param name="g"></param>
        /// <param name="fname"></param>
        /// <param name="bake"></param>
        static void FindFunctionRecusively(GameObject g, string fname, List<KeyValuePair<System.Reflection.MethodInfo, object>> bake, List<scObjectPoolNodeNotBake> notBake_propagated = null)
        {
            if (notBake_propagated == null) notBake_propagated = new List<scObjectPoolNodeNotBake>();
            var ls = g.GetComponents<MonoBehaviour>();

            //notbake 오브젝트중 관련된애를 전부 가져온다
            var my_notbakes = g.GetComponents<scObjectPoolNodeNotBake>();
            var using_notbakes = my_notbakes.ToList();
            using_notbakes.Add(notBake_propagated);

            //ignore할지
            bool ignored = using_notbakes.Find(nb => nb.NotBakeName.VFind(nbstr => nbstr == fname) != null) != null;

            if (ls.Length > 0)
            {
                foreach (var v in ls)
                {
                    var ti = v.GetType();
                    if (ti == typeof(scObjectPoolNodeNotBake)) continue;
                    if (ti == typeof(scObjectPoolNode)) continue;

                    if (!ignored || ti.IsSubclassOf(typeof(scAbsObjectPoolIgnoreNotBake)))
                    {
                        while (ti.Name != "MonoBehaviour")
                        {
                            var m = ti.GetMethod(fname, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);

                            if (m != null && m.GetParameters().Length == 0)
                            {
                                bake.Add(new KeyValuePair<System.Reflection.MethodInfo, object>(m, v));
                                break;
                            }
                            else
                                ti = ti.BaseType;
                        }
                    }
                }
            }

            //전파시킨다
            foreach (var v in my_notbakes)
            {
                if (v.AffectRecusively)
                {
                    if (!notBake_propagated.Contains(v))
                        notBake_propagated.Add(v);
                }
            }

            var tt = g.transform;
            foreach (Transform t in tt)
            {
                FindFunctionRecusively(t.gameObject, fname, bake, notBake_propagated);
            }
        }

        /// <summary>
        /// 해당 객체의 모든 MonoBehaviour를 돌며 funcname(instance함수)를 실행. 다만 void() 형태만 실행가능하다.
        /// 옵션용으로 남겨놓음. 지금은 안씀...
        /// </summary>
        /// <param name="g"></param>
        /// <param name="funcname"></param>
        public static void CallFuncRecusivelyFor(GameObject g, string funcname)
        {
            var ls = g.GetComponents<MonoBehaviour>();
            if (ls.Length > 0)
            {
                foreach (var v in ls)
                {
                    var m = v.GetType().GetMethod(funcname, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    try
                    {
                        m.Invoke(v, null);
                    }
                    catch (System.Exception e)
                    {
                        Util.LogError(e);
                    }
                }
            }

            var tt = g.transform;
            foreach (Transform t in tt)
            {
                CallFuncRecusivelyFor(t.gameObject, funcname);
            }
        }

    }

}