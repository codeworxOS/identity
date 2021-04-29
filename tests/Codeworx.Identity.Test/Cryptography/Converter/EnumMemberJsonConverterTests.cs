using System.Runtime.Serialization;
using Codeworx.Identity.Converter;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Cryptography.Converter
{
    public class EnumMemberJsonConverterTests
    {
        private const string Part1Name = "Part1Name";
        
        private enum TestEnum
        {
            [EnumMember(Value = Part1Name)]
            Part1,
            Part2
        }

        [Test]
        public void CanConvert_Enum_ReturnsTrue()
        {
            var instance = new EnumMemberJsonConverter();

            Assert.True(instance.CanConvert(typeof(TestEnum)));

        }

        [Test]
        public void CanConvert_Object_ReturnsFalse()
        {
            var instance = new EnumMemberJsonConverter();

            Assert.False(instance.CanConvert(typeof(object)));
        }

        [Test]
        public void WriteJson_EnumWithAttribute_ReturnsCorrectSerialization()
        {
            var enumValue = TestEnum.Part1;
            
            var instance = new EnumMemberJsonConverter();
            var jsonWriter = new Mock<JsonWriter>();
            
            instance.WriteJson(jsonWriter.Object, enumValue, null);

            jsonWriter.Verify(p => p.WriteValue((object)Part1Name), Times.Once);
        }

        [Test]
        public void WriteJson_EnumWithoutAttribute_ReturnsEnumValue()
        {
            var enumValue = TestEnum.Part2;

            var instance = new EnumMemberJsonConverter();
            var jsonWriter = new Mock<JsonWriter>();

            instance.WriteJson(jsonWriter.Object, enumValue, null);

            jsonWriter.Verify(p => p.WriteValue(enumValue), Times.Once);
        }
    }
}