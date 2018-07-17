using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZO
{
    public abstract class scSingleToneForMB<T> : MonoBehaviour where T : scSingleToneForMB<T>
    {
        public static bool IsExistInstance
        {
            get
            {
                return Instance != null;
            }
        }

        public static T Instance;
        protected void Awake()
        {
            Instance = this as T;
        }

        protected void OnDestroy()
        {
            if (Instance == this as T)
                Instance = null;
        }
    }

}