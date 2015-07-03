using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


namespace BetterUpgrade
{
    public class Configuration
    {
        [XmlArray(ElementName = "Buildings")]
        [XmlArrayItem(ElementName = "Building")]
        public List<Building> buildings = new List<Building>();

        public Building GetBuilding(string name)
        {
            foreach (Building building in buildings)
            {
                if (building.name == name) return building;
            }
            return null;
        }

        public class Building
        {
            [XmlIgnoreAttribute]
            public bool isBuiltIn = false;

            [XmlAttribute("name")]
            public string name;

            [XmlArray(ElementName = "Variations")]
            [XmlArrayItem(ElementName = "Variation")]
            public List<Variation> variations;

            public Building(string name)
            {
                this.name = name;
            }

            public Building() { }

            public Variation GetVariation(string name)
            {
                foreach (Variation variation in variations)
                {
                    if (variation.name == name) return variation;
                }
                return null;
            }
        }

        public class Variation
        {
            [XmlIgnoreAttribute]
            public bool isBuiltIn = false;

            [XmlAttribute("name"), DefaultValue(null)]
            public string name;

            [XmlAttribute("level"), DefaultValue(-1)]
            public int level;

            public ItemClass getItemClass(ItemClass baseClass)
            {
                var itemClass = new ItemClass();

                itemClass.m_service = baseClass.m_service;
                itemClass.m_subService = baseClass.m_subService;
                itemClass.m_layer = baseClass.m_layer;

                switch (level)
                {
                    case 1:
                        itemClass.m_level = ItemClass.Level.Level1;
                        break;
                    case 2:
                        itemClass.m_level = ItemClass.Level.Level2;
                        break;
                    case 3:
                        itemClass.m_level = ItemClass.Level.Level3;
                        break;
                    case 4:
                        itemClass.m_level = ItemClass.Level.Level4;
                        break;
                    case 5:
                        itemClass.m_level = ItemClass.Level.Level5;
                        break;
                    default:
                        itemClass.m_level = baseClass.m_level;
                        break;
                }

                if (itemClass.m_service != ItemClass.Service.Residential && itemClass.m_level > ItemClass.Level.Level3)
                {
                    itemClass.m_level = ItemClass.Level.Level3;
                }

                if (itemClass.m_service == ItemClass.Service.Industrial && itemClass.m_subService != ItemClass.SubService.IndustrialGeneric)
                {
                    itemClass.m_level = ItemClass.Level.Level1;
                }

                return itemClass;
            }
        }

        public static Configuration Deserialize(string filename)
        {
            if (!File.Exists(filename)) return null;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            try
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(filename))
                {
                    return (Configuration)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't load configuration (XML malformed?)");
                throw e;
            }
        }

        public static void Serialize(string filename, Configuration config)
        {
            var xmlSerializer = new XmlSerializer(typeof(Configuration));
            try
            {
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(filename))
                {
                    var configCopy = new Configuration();
                    foreach (var building in config.buildings)
                    {
                        var newBuilding = new Building(building.name);
                        foreach (var variation in building.variations.Where(variation => !building.isBuiltIn || !variation.isBuiltIn))
                        {
                            newBuilding.variations.Add(variation);
                        }
                        if (!building.isBuiltIn || newBuilding.variations.Count > 0)
                        {
                            configCopy.buildings.Add(newBuilding);
                        }
                    }
                    xmlSerializer.Serialize(streamWriter, configCopy);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't create configuration file at \"" + Directory.GetCurrentDirectory() + "\"");
                throw e;
            }
        }
    }
}

