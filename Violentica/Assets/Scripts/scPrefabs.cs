using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZO
{
    public class scPrefabs : MonoBehaviour
    {
        //아틀라스
        public UIAtlas AtlasNormal1;

        //스파인 일단 잠금
        public static Spine.Unity.SkeletonAnimation GetSpineAsset(string prefabname)
        {
            string spinepath = "Prefabs/Spine/";

            string pathout = "";
            object r = null;
            //pathout = SpineLinker.GetSpinePath(prefabname);
            pathout = "need path here";
            r = Resources.Load(spinepath + pathout);
            if(r == null)
            {
                return null;
            }
            else
                return (r as GameObject).GetComponent<Spine.Unity.SkeletonAnimation>();
        }

        public static Spine.Unity.SkeletonAnimation CreateSpine(Component parent, string prefabname)
        {
            return CreateSpine(parent.gameObject, prefabname);
        }

        public static Spine.Unity.SkeletonAnimation CreateSpine(GameObject parent, string prefabname)
        {
            var r = GetSpineAsset(prefabname);
            if (r == null)
            {
                Util.LogError("Spine Create Failed : Prefab " + prefabname + " is not Exist");
                return null;
            }
            else
            {
                var instantiated = Instantiate(r.gameObject) as GameObject;
                var final = instantiated.GetComponent<Spine.Unity.SkeletonAnimation>();
                var tr = final.transform;
                tr.parent = parent.transform;
                tr.localPosition = Vector3.zero;
                tr.localScale = Vector3.one;
                tr.localEulerAngles = Vector3.zero;
                return final;
            }
        }

        //이하 프리팹들
        public scSpineRenderStage PrefabSpineRenderStage;
        public scOutCameraStage PrefabOutCameraStage;
        public scNetworkLoading PrefabNetworkLoading;

        public static scPrefabs Instance
        {
            get
            {
                if (instance == null)
                {
                    if (gameIsFinished) return null;
                    var g = Instantiate(Resources.Load("Prefabs")) as GameObject;
                    instance = g.GetComponent<scPrefabs>();
                    DontDestroyOnLoad(g);
                }
                return instance;
            }
        }

        

        void OnDestroy()
        {
            gameIsFinished = true;
        }

        static bool gameIsFinished = false;
        static scPrefabs instance;

        /// <summary>
        /// 프리팹으로부터 게임오브젝트를 제작한다.
        /// </summary>
        /// <param name="c">scPrefabManager안에 들어있는 프리팹</param>
        /// <returns></returns>
        public static GameObject Create(GameObject g)
        {
            return Instantiate(g) as GameObject;
        }

        public static T Create<T>(T c) where T : Component
        {
            return Instantiate(c) as T;
        }

        public static T CreateChild<T>(Component t, T c) where T : Component
        {
            return CreateChild(t.gameObject, c);
        }

        public static T CreateChild<T>(GameObject g, T c) where T : Component
        {
            GameObject gg = NGUITools.AddChild(g, c.gameObject);
            gg.transform.localScale = c.transform.localScale;
            return gg.GetComponent<T>();
        }

        public static GameObject CreateChild<T>(T g, GameObject c) where T : Component
        {
            return CreateChild(g.gameObject, c);
        }

        public static GameObject CreateChild(GameObject g, GameObject c)
        {
            GameObject gg = NGUITools.AddChild(g, c);
            gg.transform.localScale = c.transform.localScale;
            return gg;
        }


    }

}