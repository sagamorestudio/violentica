using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZO;
//editor상, 어떤 씬에서 실행해도 정상적인 connect 과정을 전부 거치도록 해준다.    
public class GameInitializer : MonoBehaviour
{

    void Awake()
    {
        scCoroutine.Instance.Begin(InitialzeTotalOnce());
    }

    static bool loaded = false;

    public static bool NowLoading = true;

    IEnumerator InitialzeTotalOnce()
    {
        if (!loaded)
        {
            loaded = true;
            Application.runInBackground = true;
            //화면꺼짐 방지
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.fullScreen = true;

            //지체없이 pre_start 씬으로 전환
            UnityEngine.SceneManagement.SceneManager.LoadScene("pre_start");
        }

        //not using
        //DB Update from google
        //scCoroutine.Instance.Begin(ZO.RefTable_google.GetDBFromWWW(null));            
        // 
        yield break;
    }

}