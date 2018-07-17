using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZO
{
    public class SingleTone<T> where T : SingleTone<T>, new()
    {
        static T _instance = null;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }

}