using UnityEngine;
using System.Collections;


namespace ZO
{
    /// <summary>
    /// PlayerPrefbs 로 연결하는 곳
    /// </summary>
    public class WrapPlayerPrefs
    {
        string cachedvalue;
        string key;
        public WrapPlayerPrefs(string keystr, string default_value)
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString(keystr)))
                PlayerPrefs.SetString(key = keystr, cachedvalue = default_value);
            cachedvalue = PlayerPrefs.GetString(key = keystr);
        }

        public WrapPlayerPrefs(string keystr)
        {
            cachedvalue = PlayerPrefs.GetString(key = keystr);
        }

        public string Value
        {
            get
            {
                return cachedvalue;
            }
            set
            {
                PlayerPrefs.SetString(key, cachedvalue = value);
            }
        }

        public int ValueToIntDefaultIs0
        {
            get
            {
                int ii = 0;
                if (int.TryParse(cachedvalue, out ii))
                {
                    return ii;
                }
                else
                    return 0;
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}