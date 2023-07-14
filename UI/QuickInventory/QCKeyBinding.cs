using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

namespace QuickCast.UI.QuickInventory
{
    [XmlRoot("QCKeyBinding")]
    public class QCKeyBinding : IXmlSerializable
    {
        [XmlAttribute("KeyCode")]
        public KeyCode Key;

        [XmlAttribute("Ctrl")]
        public bool Ctrl;

        [XmlAttribute("Shift")]
        public bool Shift;

        [XmlAttribute("Alt")]
        public bool Alt;

        public QCKeyBinding() { }

        public QCKeyBinding(KeyCode key, bool ctrl, bool shift, bool alt)
        {
            this.Key = key;
            this.Ctrl = ctrl;
            this.Shift = shift;
            this.Alt = alt;
        }

        public XmlSchema GetSchema()
        {
            Main.Mod.Debug("Attempting to GetSchema");
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Main.Mod.Debug("Attempting to read");

            XmlSerializer keyCodeSerializer = new XmlSerializer(typeof(KeyCode));
            XmlSerializer boolSerializer = new XmlSerializer(typeof(bool));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            reader.ReadStartElement("QCKeyBinding");

            Key = (KeyCode)keyCodeSerializer.Deserialize(reader);
            Alt = (bool)boolSerializer.Deserialize(reader);
            Ctrl = (bool)boolSerializer.Deserialize(reader);
            Shift = (bool)boolSerializer.Deserialize(reader);

            reader.MoveToContent();

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            Main.Mod.Debug("Attempting to write");

            var keyCodeSerializer = new XmlSerializer(typeof(KeyCode));
            var boolSerializer = new XmlSerializer(typeof(bool));

            writer.WriteStartElement("QCKeyBinding");
            keyCodeSerializer.Serialize(writer, Key);
            boolSerializer.Serialize(writer, Alt);
            boolSerializer.Serialize(writer, Ctrl);
            boolSerializer.Serialize(writer, Shift);
            writer.WriteEndElement();
        }
    }
}
