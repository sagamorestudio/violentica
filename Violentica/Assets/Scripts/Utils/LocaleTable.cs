using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZO
{
    public class LocaleTable : SingleTone<LocaleTable>
    {
        public LocaleTable()
        {
            
        }

        public string Get(string key)
        {
            return I2.Loc.LocalizationManager.GetTranslation(key);
        }

        public string Get(string key, params object[] p)
        {
            return string.Format(I2.Loc.LocalizationManager.GetTranslation(key), p);
        }
    }

}
