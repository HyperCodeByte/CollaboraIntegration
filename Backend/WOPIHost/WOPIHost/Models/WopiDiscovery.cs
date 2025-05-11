using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WOPIHost.Models
{
    [XmlRoot("wopi-discovery")]
    public class WopiDiscovery
    {
        [XmlElement("net-zone")]
        public NetZone NetZone { get; set; }
    }

    public class NetZone
    {
        [XmlElement("app")]
        public List<App> Apps { get; set; }
    }

    public class App
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("favIconUrl")]
        public string FavIconUrl { get; set; }

        [XmlElement("action")]
        public List<Action> Actions { get; set; }
    }

    public class Action
    {
        [XmlAttribute("ext")]
        public string Extension { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("urlsrc")]
        public string Urlsrc { get; set; }

        [XmlAttribute("default")]
        public bool Default { get; set; }
    }
}
