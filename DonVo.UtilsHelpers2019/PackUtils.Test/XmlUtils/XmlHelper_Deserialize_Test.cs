using PackUtils.XmlUtils;
using Newtonsoft.Json;
using NUnit.Framework;
using PackUtils.Test.LinqUtils;

namespace PackUtils.Test.XmlUtils
{
    [TestFixture]
    public class XmlHelper_Deserialize_Test
    {
        SampleObject GetObj(bool useNull = false)
        {
            return new SampleObject(1, useNull ? null : "hello");
        }

        [Test]
        public void Deserialize_can_deserialize_from_clean_mode()
        {
            var obj = GetObj();
            var xml = XmlHelper.Serialize(obj);
            var newObj = XmlHelper.Deserialize<SampleObject>(xml);
            Assert.AreEqual(JsonConvert.SerializeObject(obj), JsonConvert.SerializeObject(newObj));
        }

        [Test]
        public void Deserialize_can_deserialize_from_unclean_mode()
        {
            var obj = GetObj();
            var xml = XmlHelper.Serialize(obj, clean: false);
            var newObj = XmlHelper.Deserialize<SampleObject>(xml);
            Assert.AreEqual(JsonConvert.SerializeObject(obj), JsonConvert.SerializeObject(newObj));
        }

        [Test]
        public void Deserialize_can_deserialize_with_explicit_name()
        {
            var obj = GetObj();
            var name = "FooBar";
            var xml = XmlHelper.Serialize(obj, rootName: name);
            var newObj = XmlHelper.Deserialize<SampleObject>(xml, rootName: name);
            Assert.AreEqual(JsonConvert.SerializeObject(obj), JsonConvert.SerializeObject(newObj));
        }
    }
}
