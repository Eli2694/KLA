using System.Xml.Serialization;

namespace Model.XmlModels
{
    [XmlRoot("ktgem")]
    public class Ktgem
    {
        [XmlElement("header")]
        public Header? Header { get; set; }

        [XmlArray("equipmentconstants")]
        [XmlArrayItem("equipmentconstant")]
        public List<EquipmentConstant>? EquipmentConstants { get; set; }

        [XmlArray("statusvariables")]
        [XmlArrayItem("statusvariable")]
        public List<StatusVariable>? StatusVariables { get; set; }

        [XmlArray("datavariables")]
        [XmlArrayItem("datavariable")]
        public List<DataVariable>? DataVariables { get; set; }

        [XmlArray("dynamicvariables")]
        [XmlArrayItem("dynamicvariable")]
        public List<DynamicVariable>? DynamicVariables { get; set; }

        [XmlArray("events")]
        [XmlArrayItem("event")]
        public List<Event>? Events { get; set; }

        [XmlArray("alarms")]
        [XmlArrayItem("alarm")]
        public List<Alarm>? Alarms { get; set; }
    }

    public class Header
    {
        [XmlElement("division")]
        public string Division { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("modeltype")]
        public string ModelType { get; set; }

        [XmlElement("revcode")]
        public string RevCode { get; set; }
    }

    public class KTGemVariable
    {
        [XmlElement("id")]
        public uint Id { get; set; }

        [XmlElement("internalname")]
        public string InternalName { get; set; }

        [XmlElement("externalname")]
        public string ExternalName { get; set; }

        [XmlElement("units")]
        public string Units { get; set; }

        [XmlElement("datatype")]
        public string DataType { get; set; }

        [XmlElement("value")]
        public string Value { get; set; }

        [XmlAttribute("is-gem-specific")]
        public string IsGem { get; set; }

        [XmlElement("only-equipment-access")]
        public string Access { get; set; }
    }

    public class EquipmentConstant : KTGemVariable
    {

        [XmlElement("min")]
        public string Min { get; set; }

        [XmlElement("max")]
        public string Max { get; set; }

        [XmlElement("default")]
        public string Default { get; set; }

        public override string ToString()
        {
            return "EC";
        }

    }
    public class StatusVariable : KTGemVariable
    {

        [XmlElement("limitsmin")]
        public string LimitsMin { get; set; }

        [XmlElement("limitsmax")]
        public string LimitsMax { get; set; }

        [XmlElement("eventname")]
        public string EventName { get; set; }

        public override string ToString()
        {
            return "SVID";
        }
    }

    public class DataVariable : KTGemVariable
    {
        public override string ToString()
        {
            return "datavariable";
        }
    }

    public class DynamicVariable : KTGemVariable
    {
        public override string ToString()
        {
            return "VID";
        }
    }

    public class IdName
    {
        [XmlElement("id")]
        public uint Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }
    }

    public class Event : IdName
    {
        public override string ToString()
        {
            return "Event";
        }
    }

    public class Alarm : IdName
    {
        [XmlElement("eventon")]
        public IdName Eventon { get; set; }

        [XmlElement("eventoff")]
        public IdName Eventoff { get; set; }

        public override string ToString()
        {
            return "Alarm";
        }
    }

    [XmlRoot("XMLElements")]
    public class DDMappingRoot
    {
        [XmlElement("XMLElement")]
        public List<XmlElement>? XmlElements { get; set; }
    }

    public class XmlElement
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("ID")]
        public uint Id { get; set; }
    }
}
