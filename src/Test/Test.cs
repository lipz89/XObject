using System;
using System.Collections.Generic;
using System.Linq;

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
        public void TestMethod()
        {
            dynamic  obj=new ObjectX();
            obj.SetValue("Name","TestNmae");
            var v = obj.GetValue("Name");
            Console.WriteLine(v);
            var v2 = obj.Name;
            Console.WriteLine(v2);
            var names = obj.GetNames();
            Console.WriteLine(string.Join(",",names));
        }

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
        public void TestArrayAndObject()
        {
            var dic = new Dictionary<string, object> { { "ID", 3 }, { "Name", "Name" }, { "OO", new Dictionary<string, object> { { "ID3", 3 } } } };
            ObjectX obj = ObjectX.From(dic);

            var jss = new JsonSerializer();
            jss.Converters.Add(new ObjectXJsonConverter());
            var jo = JObject.FromObject(obj, jss);
            var jo2 = JObject.FromObject(obj, jss);
            Console.WriteLine(jo);
            Console.WriteLine(jo2);
            Console.WriteLine(obj);

            var ox = ObjectX.From(jo);
            Console.WriteLine(ox);
            Console.WriteLine(obj.Equals(ox));


            var a1 = new object[] { obj, ox };
            var ja = JArray.FromObject(a1, jss);
            var oa = ObjectX.From(ja);
            Console.WriteLine(ja);
            Console.WriteLine(oa);
        }

        [Test]
        public void TestEqual()
        {
            dynamic o0 = new ObjectX();
            var h0 = o0.GetHashCode();
            dynamic o01 = new ObjectX();
            var h01 = o01.GetHashCode();
            Console.WriteLine(h0);
            Console.WriteLine(h01);
            Console.WriteLine(o0.Equals(o01));
            Console.WriteLine(object.Equals(o0, o01));


            var dic = new Dictionary<string, object> { { "ID", 3 }, { "Name", "Name" }, { "AOO", new Dictionary<string, object> { { "ID3", 3 } } } };
            dynamic obj = ObjectX.From(dic);

            var dic2 = new Dictionary<string, object> { { "Name", "Name" }, { "ID", 3 }, { "AOO", new Dictionary<string, object> { { "ID3", 3 } } } };
            dynamic obj2 = ObjectX.From(dic2);

            Console.WriteLine("dic:" + dic.OrderBy(x => x.Key).SequenceEqual(dic2.OrderBy(x => x.Key)));

            var h1 = obj.GetHashCode();
            var h2 = obj2.GetHashCode();
            obj2.NN2 = obj;
            obj2.Self = obj2;
            obj.Self = obj;
            obj.NN2 = obj2;
            Console.WriteLine(obj.GetHashCode());

            Console.WriteLine(h1);
            Console.WriteLine(h2);

            Console.WriteLine(obj.Equals(obj2));
            Console.WriteLine(object.Equals(obj, obj2));
        }
    }

    public class Model
    {
        public string Name { get; set; }

        public int ID { get; set; }
    }
}