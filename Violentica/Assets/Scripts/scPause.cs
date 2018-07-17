using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZO
{
    public class scPause : MonoBehaviour
    {
        public static scPause Instance;        

        /// <summary>
        /// return false -> Pause is true
        /// return true -> Pause is false
        /// </summary>
        public static bool IsPlaying
        {
            get
            {
                if (Instance == null)
                    return true;
                return !Instance.PAUSED;
            }
        }

        public static bool isPause
        {
            get
            {
                if (Instance == null)
                    return false;
                return Instance.PAUSED;
            }
        }

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public GameObject content;

        private int pause_request_count = 0;

        public bool PAUSED
        {
            get
            {
                return pause_request_count > 0;
            }
            set
            {
                pause_request_count += (value ? 1 : -1);
                if (pause_request_count < 0)
                    Util.LogError("Pause Count is Minus(-) !!: " + pause_request_count);
                content.SetActive(pause_request_count > 0);
                if (pause_request_count == 1)
                {
                    if (WhenPause != null) WhenPause();
                }
                else if (pause_request_count == 0)
                {
                    if (WhenResume != null) WhenResume();
                }
            }
        }
        

        public Util.DelegateVoid0Param WhenPause;
        public Util.DelegateVoid0Param WhenResume;
    }

}
