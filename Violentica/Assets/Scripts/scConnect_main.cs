using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZO
{
    public class scConnect_main : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            var l = scNetworkLoading.Instance.LoadingForWhile(main().VPlay());
            l.LoadingLogo = false;
            l.Background = false;
        }

        /// <summary>
        /// 접속 stub
        /// 향후 이곳에서 실제 접속을 처리하게 될 것
        /// </summary>
        /// <returns></returns>
        IEnumerator main()
        {


#if UNITY_EDITOR
            //cursor visible
            Cursor.visible = true;

            //load pre_configuration
            //             System.IO.StreamReader sr = new System.IO.StreamReader("configuration_dev.txt");
            //             string data = sr.ReadToEnd();
            //             var dic = MiniJSON.Json.Deserialize(data) as Dictionary<string, object>;            
            //             sr.Close();
#endif

            //load reftable
            yield return Reftable.Instance.Initialize().VPlay();

            //connection stub
            yield return CWait.ForSeconds_IgnorePause(1);

            
            //wait for additional some more frames
            yield return null;
            yield return null;

            //scScreenLog.Instance.Log(3);



            //finished
            UnityEngine.SceneManagement.SceneManager.LoadScene("maingame");

            //scScreenLog.Instance.Log(19);

        }
    }

}
