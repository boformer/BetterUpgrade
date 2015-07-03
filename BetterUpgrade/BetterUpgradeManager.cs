using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterUpgrade
{
    public class BetterUpgradeManager : Singleton<BetterUpgradeManager>
    {
        private Configuration _configuration;
        private const string configPath = "BetterUpgrade.xml";
        private Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = Configuration.Deserialize(configPath);

                    if (_configuration == null)
                    {
                        _configuration = new Configuration();
                        Configuration.Serialize(configPath, Configuration);
                    }
                }

                return _configuration;
            }
        }

        public BuildingInfo[] getVariations(BuildingInfo prefab) 
        { 
            var prefabs = new List<BuildingInfo>();

            var building = Configuration.GetBuilding(prefab.name);

            if (building != null) 
            {
                BetterUpgradeMod.debugLog.Add("Found " + building.variations.Count + " variation(s).");
                
                foreach (var variation in building.variations) 
                {
                    var prefabCopy = BuildingInfo.Instantiate(prefab);

                    prefabCopy.name = variation.name;
                    prefabCopy.m_class = variation.getItemClass(prefab.m_class);

                    prefabs.Add(prefabCopy);
                }
            }

            return prefabs.ToArray<BuildingInfo>();
        }
    }
}
