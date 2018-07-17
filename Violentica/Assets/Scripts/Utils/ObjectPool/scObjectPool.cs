using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZO
{
    class scObjectPool : MonoBehaviour
    {
        public bool Activated = true;

        public string TraceState
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                int count_total = 0;
                sb.AppendLine("ObjectPool State (Total Disabled Object : " + transform.GetChild(0).childCount);
                foreach (KeyValuePair<GameObject, List<scObjectPoolNode>> kv in dictionary)
                {
                    sb.AppendLine(string.Format("{0} : {1}", kv.Key.name, kv.Value.Count));
                    count_total += kv.Value.Count;
                }

                sb.AppendLine("Total In Dictionary : " + count_total);

                return sb.ToString();
            }
        }

        static scObjectPool _instance;
        public static scObjectPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject g = new GameObject();
                    g.name = "ObjectPool";
                    GameObject disposed = new GameObject();
                    disposed.SetActive(false);
                    _instance = g.AddComponent<scObjectPool>();
                    _instance.parent_of_destroyd = disposed.transform;
                    _instance.parent_of_destroyd.parent = g.transform;
                    DontDestroyOnLoad(g);
                }
                return _instance;
            }
        }
        private Transform parent_of_destroyd;

        static Dictionary<GameObject, List<scObjectPoolNode>> dictionary = new Dictionary<GameObject, List<scObjectPoolNode>>();

        public static void TrimAll()
        {
            foreach (KeyValuePair<GameObject, List<scObjectPoolNode>> kv in dictionary)
            {
                kv.Value.RemoveAll((scObjectPoolNode g) => g == null);
            }
        }

        public static bool IsAlive(Component c)
        {
            if (c == null) return false;
            return IsAlive(c.gameObject);
        }

        public static bool IsAlive(GameObject g)
        {
            if (g == null) return false;
            var n = g.GetComponent<scObjectPoolNode>();
            if (n == null) return true;
            return n.IsAlive;
        }

        /// <summary>
        /// RecycleCount 번째 리사이클에서 생성했던 객체가 재활용된 기록없이 살아있나 체크한다.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="RecycleCount"></param>
        /// <returns></returns>
        public static bool IsAlive(Component c, int RecycleCount)
        {
            if (c == null) return false;
            return IsAlive(c.gameObject, RecycleCount);
        }

        public static bool IsAlive(GameObject g, int RecycleCount)
        {
            if (g == null) return false;
            var n = g.GetComponent<scObjectPoolNode>();
            if (n == null)
            {
                if (RecycleCount == 0) return true;
                else return false;
            }

            if (n.RecycledCount != RecycleCount)
                return false;

            return n.IsAlive;
        }

        public List<scObjectPoolNode> GetList(GameObject t)
        {
            List<scObjectPoolNode> rtv = null;
            if (!dictionary.TryGetValue(t, out rtv))
            {
                rtv = new List<scObjectPoolNode>();
                dictionary.Add(t, rtv);
            }
            return rtv;
        }

        public void DestroyPooled(GameObject g)
        {
            var n = g.GetComponent<scObjectPoolNode>();
            if (n == null || !Activated)
                Destroy(g);
            else
                DestroyPooled(n);
        }

        /// <summary>
        /// 풀링 오브젝트는 풀링 파괴
        /// 아닌 오브젝트는 걍 파괴
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        public void DestroyPooled<T>(T p) where T : Component
        {
            if (p == null) return;
            else if (!Activated) Destroy(p.gameObject);

            //if (p != null)
            {
                var nd = p.GetComponent<scObjectPoolNode>();
                if (nd == null || nd.OriginalType == null)
                {
                    Destroy(p.gameObject);
                }
                else
                {
                    if (!nd.IsAlive)
                    {
                        Util.LogError("ObjectPool Error Double Destroy : " + p);
                    }
                    else
                    {
                        nd.gameObject.SetActive(false);
                        //disable 의 자식으로 넣는다.
                        nd.transform.parent = parent_of_destroyd;
                        //여기에 위치하는 이유는OnDisable 뒤에 OnDestory가 호출되게 할라고
                        nd.Run("OnDestroy");
                        //풀에 넎는다.
                        GetList(nd.OriginalObject).Add(nd);
                        //파괴
                        nd.IsAlive = false;
                        //파괴 호출 후 클리어
                        if (nd.WhenPoolDeath != null)
                        {
                            nd.WhenPoolDeath();
                            nd.WhenPoolDeath = null;
                        }
                    }
                }
            }
        }

        static T TryRecycleFromPool<T>(GameObject reference = null, bool RemoveFromPool = true) where T : Component
        {
            List<scObjectPoolNode> ls;
            if (dictionary.TryGetValue(reference, out ls))
            {
                if (ls.Count == 0)
                    return null;
                else
                {
                    for (int i = 0; i < ls.Count; ++i)
                    {
                        if (ls[i] != null)
                        {
                            var node = ls[i].GetComponent<scObjectPoolNode>();
                            if (reference == null || node.OriginalObject == reference)
                            {
                                var rtv = ls[i];
                                if (RemoveFromPool)
                                {
                                    ls.RemoveAt(i);
                                    rtv.IsAlive = true;
                                }
                                ++rtv.RecycledCount;
                                rtv.gameObject.SetActive(true);
                                return rtv.GetComponent<T>();
                            }
                        }
                    }
                    //모두 null 이라면
                    ls.Clear();
                    return null;
                }
            }
            else
                return null;
        }

        public void ClearAllPooledObjects()
        {
            foreach (var kv in dictionary)
            {
                foreach(var v in kv.Value)
                {
                    Destroy(v.gameObject);
                }
                kv.Value.Clear();
            }            
        }

        /// <summary>
        /// 리사이클에서 하나 가져온다.
        /// </summary>
        /// <param name="RemoveFromPool">true시 풀에서 빼준다. false면 그냥 찾아서 포인터만 리턴</param>
        /// <returns></returns>
        static GameObject TryRecycleFromPool(GameObject reference = null, bool RemoveFromPool = true)
        {
            List<scObjectPoolNode> ls;
            if (dictionary.TryGetValue(reference, out ls))
            {
                if (ls.Count == 0)
                    return null;
                else
                {
                    for (int i = 0; i < ls.Count; ++i)
                    {
                        if (ls[i] != null)
                        {
                            var node = ls[i].GetComponent<scObjectPoolNode>();
                            if (reference == null || node.OriginalObject == reference)
                            {
                                var rtv = ls[i];
                                if (RemoveFromPool)
                                {
                                    ls.RemoveAt(i);
                                    rtv.IsAlive = true;
                                }
                                ++rtv.RecycledCount;
                                rtv.gameObject.SetActive(true);
                                return rtv.gameObject;
                            }
                        }
                    }
                    //모두 null 이라면
                    ls.Clear();
                    return null;
                }
            }
            else
                return null;
        }


        public T CreatedPooled<T>(GameObject parent, T p) where T : Component
        {
            //재활용
            var created = TryRecycleFromPool<T>(p.gameObject);
            if (!Activated)
                return scPrefabs.CreateChild(parent, p);

            if (created == null)
            {
                //없으면 새로 만든다.
                created = scPrefabs.CreateChild(parent, p);

                //관리되는 노드로 만든다.
                var node = created.gameObject.AddComponent<scObjectPoolNode>();
                node.OriginalType = typeof(T);
                node.OriginalObject = p.gameObject;
                node.Bake();
                node.IsAlive = true;
            }
            else
            {
                //함수호출
                var node = created.GetComponent<scObjectPoolNode>();
                //Awake는 안한다.
                //node.Run("Awake");

                //start는 1프레임 뒤에 : 이러면 Update를 막을수가 없다... ...일단 Script Execution Order순위를 올려서 해결
                waitForStart.Add(node);
                //scCoroutine.Instance.Begin(startFunctionCoroutine(node));

                //그래서 그냥 스타트해봄 : 이러면 start에서 시작하는 IEnumerator 등이 먼저 scMover를 깔아버린다.
                //node.Run("Start");
            }
            //부모자식 관계 만들고 트랜스폼 초기화
            Transform t = created.transform;
            if (parent != null) t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            return created;
        }

        List<scObjectPoolNode> waitForStart = new List<scObjectPoolNode>();

        void Update()
        {
            if (waitForStart.Count > 0)
            {
                var wr = waitForStart.ToArray();
                waitForStart.Clear();
                foreach (var v in wr) if (v.IsAlivePool()) v.Run("Start");
            }
        }

        IEnumerator startFunctionCoroutine(scObjectPoolNode node)
        {
            yield return null;
            if (node != null && node.gameObject.activeInHierarchy)
                node.Run("Start");
            yield break;
        }

        public GameObject CreatedPooled(GameObject parent, GameObject p)
        {
            //재활용
            var created = TryRecycleFromPool(p);
            if (!Activated)
                return scPrefabs.CreateChild(parent, p);

            if (created == null)
            {
                //없으면 새로 만든다.
                created = scPrefabs.CreateChild(parent, p);

                //관리되는 노드로 만든다.
                var node = created.gameObject.AddComponent<scObjectPoolNode>();
                node.OriginalType = typeof(GameObject);
                node.OriginalObject = p;
                node.Bake();
                node.IsAlive = true;
            }
            else
            {
                //함수호출
                var node = created.GetComponent<scObjectPoolNode>();
                //start 는 1프레임 뒤에
                waitForStart.Add(node);
            }

            //부모자식 관계 만들고 트랜스폼 초기화
            Transform t = created.transform;
            if (parent != null) t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            return created;
        }

        public T CreatedPooled<T>(Component parent, T t) where T : Component
        {
            return CreatedPooled(parent.gameObject, t);
        }

        public T CreatedPooled<T>(T t) where T : Component
        {
            return CreatedPooled((GameObject)null, t);
        }

        public GameObject CreatedPooled(GameObject g)
        {
            return CreatedPooled((GameObject)null, g);
        }

        public GameObject CreatedPooled(Component parent, GameObject g)
        {
            return CreatedPooled(parent.gameObject, g);
        }

    }

    public static class scObjectPoolExtensionMothods
    {
        public static TSource WhenRecycled<TSource>(this TSource from, Util.DelegateVoid1Param<TSource> action) where TSource : MonoBehaviour
        {
            from.VRunIfNotNull(f => f.GetComponent<scObjectPoolNode>().VRunIfNotNull(
                node => node.WhenPoolDeath = () => action(node.GetComponent<TSource>())));
            return from;
        }

        public static void DestroyPooledIfNotNull<T>(this T o) where T : Component
        {
            if (scObjectPool.IsAlive(o))
                scObjectPool.Instance.DestroyPooled(o);
        }

        public static void DestroyPooledIfNotNull(this GameObject o)
        {
            if (scObjectPool.IsAlive(o))
                scObjectPool.Instance.DestroyPooled(o);
        }

        public static bool IsAlivePool(this Component c)
        {
            return scObjectPool.IsAlive(c);
        }

        public static bool IsAlivePool(this GameObject c)
        {
            return scObjectPool.IsAlive(c);
        }
    }
}