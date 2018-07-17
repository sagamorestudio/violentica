using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZO
{

    /// <summary>
    /// 첫 시작시 분할 apk 로딩을 위해 깨끗한 씬에서 잠깐 대기한다.
    /// </summary>
    public class scPre_Start_main : MonoBehaviour
    {

        void Awake()
        {
            main().VPlay();
        }

        IEnumerator main()
        {
            GameInitializer.NowLoading = true;
            yield return CWait.ForSeconds_IgnorePause(1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("connect");
        }

    }

}
