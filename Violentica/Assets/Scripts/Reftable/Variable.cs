using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZO
{
    /// <summary>
    /// TODO : 추후 lazy-load 등의 최적화를 고려할 것
    /// </summary>
    public class Variable : IComparer<Variable>, IEqualityComparer<Variable>
    {
        public string str;
        public int i;
        public float f;

        public bool isNumeric = false;

        public Variable(object raw_value)
        {
            str = raw_value.ToString();
            if (int.TryParse(str, out i) && float.TryParse(str, out f))
                isNumeric = true;
            else
            {
                isNumeric = false;
                i = 0;
                f = 0;
            }
        }

        public override string ToString()
        {
            return str;
        }

        public int Compare(Variable x, Variable y)
        {
            return x.str.CompareTo(y.str);
        }

        bool IEqualityComparer<Variable>.Equals(Variable x, Variable y)
        {
            return x.str == y.str;
        }

        int IEqualityComparer<Variable>.GetHashCode(Variable obj)
        {
            return str.CompareTo(obj.str);
        }
    }

    public static class VariableExtensionMethod
    {
        public static bool isEqual(this Variable v1, Variable v2)
        {
            if (v1 == null)
            {
                if (v2 == null) return true;
                else return false;
            }
            else if (v2 == null) return false;
            else
            {
                return v1.str == v2.str;
            }
        }

        public static bool isEqual(this Variable v1, string v2)
        {
            if (v1 == null)
            {
                if (v2 == null) return true;
                else return false;
            }
            else if (v2 == null) return false;
            else
            {   
                return v1.str == v2;
            }
        }

        public static bool isEqual(this Variable v1, int v2)
        {
            if (v1 == null) return false;
            return v1.i == v2;            
        }

        public static bool isEqual(this Variable v1, float v2)
        {
            if (v1 == null) return false;
            return Mathf.Approximately(v1.f, v2);
        }
    }
}

