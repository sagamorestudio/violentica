using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZO
{

    public class RefTable_google
    {
        static System.DateTime LastDownloadedTime
        {
            get
            {
                long ld = 0;
                if (long.TryParse(PlayerPrefs.GetString("LastReftableDownloadedTime"), out ld))
                {
                    return System.DateTime.FromFileTimeUtc(ld);
                }
                else
                    return System.DateTime.FromFileTimeUtc(0);
            }
            set
            {
                PlayerPrefs.SetString("LastReftableDownloadedTime", value.ToFileTimeUtc().ToString());
            }
        }

        public static System.TimeSpan HowMuchTimeFlowedLastDownloaded
        {
            get
            {
                return System.DateTime.UtcNow - LastDownloadedTime;
            }
        }


        static string url = "https://script.google.com/macros/s/AKfycbxKXze3MHHVLXyIeRqEiWWRyXZZmQ_K67-cLyfdN2PX_l0A4Q8/exec";
        
        public static IEnumerator GetDBFromWWW(Boxer<TotalSpreadSheet> bout)
        {
            WWW w = new WWW(url);
            while (!w.isDone) yield return null;

            if (string.IsNullOrEmpty(w.error))
            {
                string s = w.text;
                var dic = MiniJSON.Json.Deserialize(s) as Dictionary<string, object>;

                TotalSpreadSheet ts = new TotalSpreadSheet(dic);
                if (bout != null) bout.value = ts;

                LastDownloadedTime = System.DateTime.UtcNow;
            }

            yield break;
        }
        
    }

    public class TotalSpreadSheet
    {
        Dictionary<string, object> _rawdata;

        public override string ToString()
        {
            return MiniJSON.Json.Serialize(_rawdata);
        }


        public TotalSpreadSheet(Dictionary<string, object> rawdata)
        {
            _rawdata = rawdata;
            Pages = new Dictionary<string, SpreadSheetPage>();
            foreach (var kv in rawdata)
            {
                SpreadSheetPage p = new SpreadSheetPage(kv.Key, kv.Value as List<object>);
                Pages.Add(kv.Key, p);
            }
        }

        public Dictionary<string, SpreadSheetPage> Pages;

        /// <summary>
        /// 해당 spreadsheet_name 에서
        /// reference_key가 reference_value와 일치하는
        /// key 의 값을 찾는다.
        /// 없으면 null
        /// </summary>
        /// <param name="spreadsheet_name"></param>
        /// <param name="key"></param>
        /// <param name="reference_key"></param>
        /// <param name="reference_value"></param>
        /// <returns></returns>
        public Variable Find(string spreadsheet_name, string key, string reference_key, Variable reference_value)
        {
            SpreadSheetPage p = null;
            if(Pages.TryGetValue(spreadsheet_name, out p))
            {
                return p.Find(key, reference_key, reference_value);
            }
            return null;
        }

        public Variable Find(string spreadsheet_name, string key, string reference_key, string reference_value)
        {
            return Find(spreadsheet_name, key, reference_key, new Variable(reference_value));
        }

        public Variable Find(string spreadsheet_name, string key, string reference_key, int reference_value)
        {
            return Find(spreadsheet_name, key, reference_key, new Variable(reference_value));
        }

        public Variable Find(string spreadsheet_name, string key, string reference_key, float reference_value)
        {
            return Find(spreadsheet_name, key, reference_key, new Variable(reference_value));
        }
    }


    public class SpreadSheetPage
    {
        public string SpreadSheetName;
        List<string> headers;
        List<SpreadSheetRow> raws;
        public SpreadSheetPage(string _name, List<object> input)
        {
            SpreadSheetName = _name;
            headers = new List<string>();
            raws = new List<SpreadSheetRow>();

            if(input.Count > 0)
            {
                (input[0] as List<object>).ForEach(v => headers.Add(v.ToString()));
                for(int i = 1; i < input.Count; ++i)
                {
                    raws.Add(new SpreadSheetRow(headers, input[i] as List<object>));
                }
            }
        }

        public Variable Find(string key, string reference_key, Variable reference_value)
        {
            var k = headers.Find(v => v == key);
            if (!string.IsNullOrEmpty(k))
            {
                var vv = raws.Find(v => v.Get(reference_key).isEqual(reference_value));
                if(vv != null)
                    return vv.Get(k);
            }
            return null;
        }

        public Variable Find(string key, string reference_key, string reference_value)
        {
            return Find(key, reference_key, new Variable(reference_value));
        }

        public Variable Find(string spreadsheet_name, string key, string reference_key, int reference_value)
        {
            return Find(key, reference_key, new Variable(reference_value));
        }

        public Variable Find(string spreadsheet_name, string key, string reference_key, float reference_value)
        {
            return Find(key, reference_key, new Variable(reference_value));
        }
    }

    public class SpreadSheetRow
    {
        Dictionary<string, Variable> Values;
        public Variable Get(string key)
        {
            Variable ov = null;
            if (Values.TryGetValue(key, out ov))
                return ov;
            else
                return null;
        }

        public SpreadSheetRow(List<string> keys, List<object> values)
        {
            Values = new Dictionary<string, Variable>();
            for(int i = 0; i < keys.Count; ++i)
            {
                Values.Add(keys[i], new Variable(values[i]));
            }
        }
    }
}