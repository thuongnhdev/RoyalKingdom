using System;
using System.Collections.Generic;
using System.Reflection;

public class CollectionUtils
{
    public static int ObjectCompare(object o1, object o2, string compareField, bool collectionType = false)
    {
        FieldInfo field = o1.GetType().GetField(compareField);
        if (field == null)
        {
            return 0;
        }
        object value1 = field.GetValue(o1);
        object value2 = field.GetValue(o2);
        if (collectionType)
        {
            string count1 = value1.GetType().GetProperty("Count").GetValue(value1).ToString();
            string count2 = value2.GetType().GetProperty("Count").GetValue(value2).ToString();
            return string.Compare(count1, count2);
        }

        return string.Compare(value1.ToString(), value2.ToString());
    }

    public static int ObjectCompareProperty(object o1, object o2, string compareProperty, bool collectionType = false)
    {
        PropertyInfo field = o1.GetType().GetProperty(compareProperty);
        if (field == null)
        {
            return 0;
        }
        object value1 = field.GetValue(o1);
        object value2 = field.GetValue(o2);
        if (collectionType)
        {
            string count1 = value1.GetType().GetProperty("Count").GetValue(value1).ToString();
            string count2 = value2.GetType().GetProperty("Count").GetValue(value2).ToString();
            return string.Compare(count1, count2);
        }

        return string.Compare(value1.ToString(), value2.ToString());
    }

    public static Dictionary<Key, Value> CreateDictionaryFromList<Key, Value>(List<Value> list, string keyField)
    {
        Dictionary<Key, Value> dict = new();
        if (list == null)
        {
            return dict;
        }

        Type valueType = typeof(Value);
        for (int i = 0; i < list.Count; i++)
        {
            var ele = list[i];
            Key key = (Key)valueType.GetField(keyField).GetValue(ele);
            dict.Add(key, ele);
        }

        return dict;
    }
}
