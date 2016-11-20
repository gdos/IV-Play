﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Machine
    {
        [XmlAttribute]
        [BsonIndex(unique: true)]
        [BsonId]
        public string name { get; set; }
        [XmlAttribute]
        public string sourcefile { get; set; }
        [XmlAttribute]
        public string isbios { get; set; } = "no";
        [XmlAttribute]
        public string isdevice { get; set; } = "no";
        [XmlAttribute]
        public string ismechanical { get; set; } = "no";
        [XmlAttribute]
        public string runnable { get; set; } = "yes";
        [XmlAttribute]
        public string cloneof { get; set; }
        [XmlAttribute]
        public string romof { get; set; }
        [XmlAttribute]
        public string sampleof { get; set; }

        public string year { get; set; }
        public string manufacturer { get; set; }
        public Input input { get; set; }
        public Driver driver { get; set; }
        public Sound sound { get; set; }

        [XmlElement]
        public Rom[] rom { get; set; }
        [XmlElement]
        public Disk[] disk { get; set; }
        [XmlElement]
        public Chip[] chip { get; set; }
        [XmlElement]
        public Display[] display { get; set; }

        private string _description;
        public string description
        {
            get
            {
                var descriptionMatch = Regex.Match(_description, @"^(?<opening>(?:the|a|an))\s(?<content>[^\(]*)\s(?<info>\(.*)$", RegexOptions.IgnoreCase);

                if (!descriptionMatch.Success)
                    return _description.TrimStart('\'');

                return string.Format("{0}, {1} {2}", descriptionMatch.Groups[2], descriptionMatch.Groups[1], descriptionMatch.Groups[3]).TrimStart('\'');
            }
            set
            {
                _description = value;
            }
        }

        public string cpuinfo()
        {
            if (chip == null) return string.Empty;
            return GetStringFromArray(chip.Where(x => x.type == "cpu").ToArray());
        }

        public string rominfo()
        {
            var roms = GetStringFromArray(rom);
            var disks = GetStringFromArray(disk);

            return roms + "\r\n" + disks;
        }

        public string displayinfo()
        {
            return GetStringFromArray(display);
        }

        public string soundinfo()
        {
            if (chip == null || sound == null) return string.Empty;
            var soundString = GetStringFromArray(chip.Where(x => x.type == "audio").ToArray());
            return string.Format("{0} Channel(s)\r\n{1}", sound.channels, soundString);
        }
        private string GetStringFromArray(object[] array)
        {
            if (array == null) return string.Empty;

            var sb = new StringBuilder();

            foreach (var obj in array)
            {
                sb.AppendLine(obj.ToString());
            }

            return sb.ToString();
        }

    }
}

