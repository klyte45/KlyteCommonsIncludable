using System.Xml.Serialization;
using UnityEngine;

namespace Klyte.Commons.Utils.UtilitiesClasses
{
    [XmlRoot("Vector2")]

    public class Vector2Xml
    {
        [XmlAttribute("x")]
        public float X { get; set; }
        [XmlAttribute("y")]
        public float Y { get; set; }


        public static implicit operator Vector2(Vector2Xml v) => new Vector2(v?.X ?? 0, v?.Y ?? 0);
        public static explicit operator Vector2Xml(Vector2 v) => new Vector2Xml { X = v.x, Y = v.y };

        public override string ToString() => $"Vector2Xml({X},{Y})";
    }

    [XmlRoot("Vector3")]

    public class Vector3Xml : Vector2Xml
    {
        [XmlAttribute("z")]
        public float Z { get; set; }


        public static implicit operator Vector3(Vector3Xml v) => new Vector3(v?.X ?? 0, v?.Y ?? 0, v?.Z ?? 0);
        public static explicit operator Vector3Xml(Vector3 v) => new Vector3Xml { X = v.x, Y = v.y, Z = v.z };
        public override string ToString() => $"Vector3Xml({X},{Y},{Z})";
    }

    [XmlRoot("Vector4")]

    public class Vector4Xml : Vector3Xml
    {
        [XmlAttribute("w")]
        public float W { get; set; }


        public static implicit operator Vector4(Vector4Xml v) => new Vector4(v?.X ?? 0, v?.Y ?? 0, v?.Z ?? 0, v?.W ?? 0);
        public static explicit operator Vector4Xml(Vector4 v) => new Vector4Xml { X = v.x, Y = v.y, Z = v.z, W = v.w };
        public override string ToString() => $"Vector4Xml({X},{Y},{Z},{W})";
    }



}
