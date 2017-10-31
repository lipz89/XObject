using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicObject
{
    public sealed class ObjectX : IDynamicMetaObjectProvider
    {
        internal IDictionary<string, object> Values { get; }

        private ObjectX(IDictionary<string, object> values = null)
        {
            this.Values = new Dictionary<string, object>();
            if (values == null)
            {
                return;
            }
            foreach (var kvp in values)
            {
                var val = Convert(kvp.Value);
                this.Values.Add(kvp.Key, val);
            }
        }

        public object SetValue(string name, object value)
        {
            //value = Convert(value);
            if (Values.ContainsKey(name))
            {
                Values[name] = value;
            }
            else
            {
                Values.Add(name, value);
            }
            return value;
        }

        public object GetValue(string name)
        {
            if (Values.ContainsKey(name))
            {
                return Values[name];
            }

            return null;
        }

        public T GetValueOrDefault<T>(string name)
        {
            if (Values.ContainsKey(name))
            {
                return (T)Values[name];
            }

            return default(T);
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            try
            {
                if (Values.ContainsKey(name))
                {
                    value = (T)Values[name];
                    return true;
                }
                else
                {
                    value = default(T);
                    return false;
                }
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new ObjectXMeta(parameter, this);
        }

        public IEnumerable<string> GetNames()
        {
            return Values.Keys;
        }

        public override string ToString()
        {
            return this.Serialize();
        }

        public string Serialize(Formatting formatting = Formatting.None)
        {
            return this.Serialize(new JsonSerializerSettings(), formatting);
        }

        public string Serialize(JsonSerializerSettings settings, Formatting formatting = Formatting.None)
        {
            settings.Converters.Add(ObjectXJsonConverter.Instance);
            return JsonConvert.SerializeObject(this, formatting, settings);
        }

        public static dynamic From(IDictionary<string, object> values = null)
        {
            if (values == null)
            {
                return null;
            }
            return new ObjectX(values);
        }

        public static dynamic From(JObject jObject)
        {
            if (jObject == null || jObject.Count == 0)
            {
                return null;
            }
            return ObjectX.From(jObject.ToString());
        }

        public static dynamic From(object obj)
        {
            return Convert(obj);
        }

        public static dynamic From(string json, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            settings = settings ?? new JsonSerializerSettings();
            settings.Converters.Add(ObjectXJsonConverter.Instance);
            return JsonConvert.DeserializeObject<ObjectX>(json, settings);
        }

        private static object Convert(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var type = obj.GetType();
            if (type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(ObjectX))
            {
                return obj;
            }

            if (obj is JValue)
            {
                return (obj as JValue).Value;
            }

            if (obj is JObject)
            {
                return ObjectX.From(obj as JObject);
            }
            if (obj is JProperty)
            {
                return ObjectX.From("{" + obj + "}");
            }
            if (obj is JConstructor)
            {
                return ObjectX.From("{}");
            }

            if (obj is JToken && !(obj is JArray))
            {
                return ObjectX.From(obj.ToString());
            }

            if (obj is IDictionary<string, object>)
            {
                return ObjectX.From(obj as IDictionary<string, object>);
            }

            if (obj is IEnumerable)
            {
                var enumer = obj as IEnumerable;
                var list = new List<object>();
                foreach (var item in enumer)
                {
                    list.Add(Convert(item));
                }
                return list.ToArray();
            }

            if (type.IsClass)
            {
                return ConvertToObjectX(obj);
            }

            return obj;
        }
        private static object ConvertToObjectX(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var type = obj.GetType();

            List<MemberInfo> members = new List<MemberInfo>();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            members.AddRange(props);
            members.AddRange(fields);
            if (members.Count == 0)
            {
                return new ObjectX();
            }

            var vals = new Dictionary<string, object>();
            foreach (var member in members)
            {
                var name = member.Name;
                if (member is PropertyInfo)
                {
                    var prop = member as PropertyInfo;
                    if (prop.CanRead && prop.GetMethod.GetParameters().Length == 0)
                    {
                        var val = prop.GetValue(obj);
                        vals.Add(name, val);
                    }
                }
                else if (member is FieldInfo)
                {
                    var val = (member as FieldInfo).GetValue(obj);
                    vals.Add(name, val);
                }
            }
            return new ObjectX(vals);
        }
    }
}
