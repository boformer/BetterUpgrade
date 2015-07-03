
using ICities;
using System.Collections.Generic;
using UnityEngine;
namespace BetterUpgrade
{
    public class BetterUpgradeMod : LoadingExtensionBase, IUserMod
    {
        public static List<string> debugLog = new List<string>();
        
        public string Name
        {
            get { return "Better Upgrade"; }
        }
        public string Description
        {
            get { return "Allows growable assets to exist on multiple levels."; }
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            debugLog.Clear();

            debugLog.Add("Better Upgrade: Initializing Mod...");

            Detour.PrivateBuildingAIDetour.Deploy();
            Detour.BuildingInfoDetour.Deploy();

            debugLog.Add("Better Upgrade: Mod successfully intialized.");
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            foreach (string s in debugLog) 
            {
                Debug.Log(s);
            }   
        }

        public override void OnReleased()
        {
            base.OnReleased();

            debugLog.Clear();

            Debug.Log("Better Upgrade: Reverting detoured methods...");

            Detour.PrivateBuildingAIDetour.Revert();
            Detour.BuildingInfoDetour.Revert();

            Debug.Log("Better Upgrade: Done!");
        }
    }
}
