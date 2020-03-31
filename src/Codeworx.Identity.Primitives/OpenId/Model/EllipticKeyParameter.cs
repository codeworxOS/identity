using System.Runtime.Serialization;

namespace Codeworx.Identity.OpenId.Model
{
    public class EllipticKeyParameter : KeyParameter
    {
        public EllipticKeyParameter(string keyId, KeyUse keyUse, CurveType curveType, string x, string y)
            : base(keyId, KeyType.EllipticCurve, keyUse)
        {
            this.Curve = curveType;
            this.X = x;
            this.Y = y;
        }

        [DataMember(Order = 10, Name = "crv")]
        public CurveType Curve { get; }

        [DataMember(Order = 11, Name = "x")]
        public string X { get; }

        [DataMember(Order = 12, Name = "y")]
        public string Y { get; }
    }
}