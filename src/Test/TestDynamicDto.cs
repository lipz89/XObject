using System;
using System.Collections.Generic;

using DynamicObject;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class TestDynamicDto
    {
        [Test]
        public void TestDynamicDic()
        {
            var model = new Model() { Name = "model", ID = 4 };
            dynamic obj = ObjectX.From(new Dictionary<string, object> { { "ID3", 3 } });
            var id3 = obj.ID3;
            obj.ID = "1";
            obj.Null = null;

            var name = obj.Name;

            var obj2 = ObjectX.From(new Dictionary<string, object> { { "ID3", 3 }, { "M", model } });
            id3 = obj2.ID3;
            obj2.ID = "1";
            obj2.Model = model;
            obj2.X = obj;

            name = obj2.Name;

            id3 = obj2.X.ID3;

            obj2.Model = new object[] { 1, "s2", true, new Model() { Name = "model2", ID = 44 }, obj };

            var ooo = obj2.Model;
            Console.WriteLine(ooo.GetType());

            Console.WriteLine(obj2);

            var str = obj2.ToString();
            var oo = ObjectX.From(str);

            ooo = oo.Model;
            Console.WriteLine(ooo.GetType());
            Console.WriteLine(oo);
        }

        [Test]
        public void TestDynamicJSON()
        {
            object objnull = null;
            var jnull = JsonConvert.SerializeObject(objnull);
            var onull = ObjectX.From(jnull);

            var s = string.Empty;
            onull = ObjectX.From(s);

            var jonull = new JObject();
            onull = ObjectX.From(jonull);
            JToken jt = new JArray();
            onull = ObjectX.From(jt);
            jt = new JProperty("a", 3, 5);
            onull = ObjectX.From(jt);
            jt = new JConstructor("a", 2, 4);
            onull = ObjectX.From(jt);
            jt = new JValue(5);
            onull = ObjectX.From(jt);
            jt = new JRaw(5);
            onull = ObjectX.From(jt);
            var dic = new Dictionary<string, object> { { "ID", 3 }, { "Name", "Name" }, { "OO", new Dictionary<string, object> { { "ID3", 3 } } } };
            ObjectX obj = ObjectX.From(dic);

            var json = obj.Serialize();
            Console.WriteLine(json);

            var o = ObjectX.From(json);

            Console.WriteLine(o);
        }

        [Test]
        public void TestDynamicObject()
        {
            var dic = new { ID = 3, Name = "Test", M = new Model() { Name = "M" }, L = new object[] { 1, "s2", true, new Model() { Name = "model2", ID = 44 }, null } };
            ObjectX obj = ObjectX.From(dic);

            var json = obj.Serialize();
            Console.WriteLine(json);

            var sett = new JsonSerializerSettings();
            sett.Converters.Add(ObjectXJsonConverter.Instance);
            var json2 = JsonConvert.SerializeObject(obj, sett);

            var o = ObjectX.From(json);

            Console.WriteLine(o);
        }

        [Test]
        public void TestDelegate()
        {
            Action a = () => { };
            var f = a is Delegate;
            Console.WriteLine(f);
        }
    }
}