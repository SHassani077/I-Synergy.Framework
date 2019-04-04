﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace ISynergy.Utilities
{
    public static class CompareUtility
    {
        public static List<string> CompareObject(object source, object destination)
        {
            var result = new List<string>();

            var oType = source.GetType();

            foreach (var oProperty in oType.GetProperties().EnsureNotNull())
            {
                var oOldValue = oProperty.GetValue(source, null);
                var oNewValue = oProperty.GetValue(destination, null);

                // this will handle the scenario where either value is null
                if (!object.Equals(oOldValue, oNewValue))
                {
                    // Handle the display values when the underlying value is null
                    var sOldValue = oOldValue is null ? "null" : oOldValue.ToString();
                    var sNewValue = oNewValue is null ? "null" : oNewValue.ToString();

                    result.Add("Property " + oProperty.Name + " was: " + sOldValue + "; is: " + sNewValue);
                }
            }

            return result;
        }

        public static bool Compare<T>(string operation, T value1, T value2) where T:IComparable
        {
            switch (operation)
            {
                case "==": return value1.CompareTo(value2) == 0;
                case "!=": return value1.CompareTo(value2) != 0;
                case ">": return value1.CompareTo(value2) > 0;
                case ">=": return value1.CompareTo(value2) >= 0;
                case "<": return value1.CompareTo(value2) < 0;
                case "<=": return value1.CompareTo(value2) <= 0;
                default: return false;
            }
        }

        public static bool Compare(string operation, object value1, object value2)
        {
            switch (operation)
            {
                case "==": return value1 == value2;
                case "!=": return value1 != value2;
                default: return false;
            }
        }
    }
}
