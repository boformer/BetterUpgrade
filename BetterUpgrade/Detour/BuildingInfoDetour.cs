using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BetterUpgrade.Detour
{
    public class BuildingInfoDetour : BuildingInfo
    {
        private static bool deployed = false;
        
        private static RedirectCallsState _InitializePrefab_state;
        private static MethodInfo _InitializePrefab_original;
        private static MethodInfo _InitializePrefab_detour;

        public static void Deploy() 
        {
            if (!deployed)
            {
                _InitializePrefab_original = typeof(BuildingInfo).GetMethod("InitializePrefab", BindingFlags.Instance | BindingFlags.Public);
                _InitializePrefab_detour = typeof(BuildingInfoDetour).GetMethod("InitializePrefab", BindingFlags.Instance | BindingFlags.Public);
                _InitializePrefab_state = RedirectionHelper.RedirectCalls(_InitializePrefab_original, _InitializePrefab_detour);

                deployed = true;

                BetterUpgradeMod.debugLog.Add("Better Upgrade: BuildingInfo Methods detoured!");
            }
        }

        public static void Revert() 
        {
            if (deployed)
            {
                RedirectionHelper.RevertRedirect(_InitializePrefab_original, _InitializePrefab_state);
                _InitializePrefab_original = null;
                _InitializePrefab_detour = null;

                deployed = false;

                BetterUpgradeMod.debugLog.Add("Better Upgrade: BuildingInfo Methods restored!");
            }
        }

        public virtual void InitializePrefab()
        {
            bool growable = this.m_class.GetZone() != ItemClass.Zone.None;

            if (growable)
            {
                BetterUpgradeMod.debugLog.Add("InitializePrefab called: " + this.name);
            }

            RedirectionHelper.RevertRedirect(_InitializePrefab_original, _InitializePrefab_state);
            base.InitializePrefab();
            RedirectionHelper.RedirectCalls(_InitializePrefab_original, _InitializePrefab_detour);

            if (growable) 
            {
                var prefabVariations = Singleton<BetterUpgradeManager>.instance.getVariations(this);

                if (prefabVariations.Length > 0) 
                { 
                    PrefabCollection<BuildingInfo>.InitializePrefabs("BetterUpgrade", prefabVariations, null);
                }
                BetterUpgradeMod.debugLog.Add("InitializePrefab done:   " + this.name);
            }
        }
    }
}
