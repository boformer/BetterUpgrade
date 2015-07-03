
using ColossalFramework;
using System;
using System.Reflection;
using UnityEngine;
namespace BetterUpgrade.Detour
{
    public class PrivateBuildingAIDetour
    {
        private static bool deployed = false;

        private static RedirectCallsState _PrivateBuildingAI_StartUpgrading_state;
        private static MethodInfo _PrivateBuildingAI_StartUpgrading_original;
        private static MethodInfo _PrivateBuildingAI_StartUpgrading_detour;

        public static void Deploy()
        {
            if (!deployed)
            {
                try {
                _PrivateBuildingAI_StartUpgrading_original = typeof(PrivateBuildingAI).GetMethod("StartUpgrading", BindingFlags.Instance | BindingFlags.NonPublic);
                _PrivateBuildingAI_StartUpgrading_detour = typeof(PrivateBuildingAIDetour).GetMethod("StartUpgrading", BindingFlags.Instance | BindingFlags.NonPublic);
                _PrivateBuildingAI_StartUpgrading_state = RedirectionHelper.RedirectCalls(_PrivateBuildingAI_StartUpgrading_original, _PrivateBuildingAI_StartUpgrading_detour);

                }
                catch (Exception e) 
                { 
                    BetterUpgradeMod.debugLog.Add("Detour 0 failed: " + e.Message + ":" + e.StackTrace);
                }

                deployed = true;

                BetterUpgradeMod.debugLog.Add("Better Upgrade: PrivateBuildingAI Methods detoured!");
            }
        }

        public static void Revert()
        {
            if (deployed)
            {
                RedirectionHelper.RevertRedirect(_PrivateBuildingAI_StartUpgrading_original, _PrivateBuildingAI_StartUpgrading_state);
                _PrivateBuildingAI_StartUpgrading_original = null;
                _PrivateBuildingAI_StartUpgrading_detour = null;

                deployed = false;

                BetterUpgradeMod.debugLog.Add("Better Upgrade: PrivateBuildingAI Methods restored!");
            }
        }

        protected void StartUpgrading(ushort buildingID, ref Building buildingData)
        {
            Debug.LogFormat("StartUpgrading called: {0}", buildingData.Info.name);
            
            buildingData.m_frame0.m_constructState = 0;
            buildingData.m_frame1.m_constructState = 0;
            buildingData.m_frame2.m_constructState = 0;
            buildingData.m_frame3.m_constructState = 0;
            Building.Flags flags = buildingData.m_flags;
            flags |= Building.Flags.Upgrading;
            flags &= ~Building.Flags.Completed;
            flags &= ~Building.Flags.LevelUpEducation;
            flags &= ~Building.Flags.LevelUpLandValue;
            buildingData.m_flags = flags;
            BuildingManager instance = Singleton<BuildingManager>.instance;
            instance.UpdateBuildingRenderer(buildingID, true);
            EffectInfo levelupEffect = instance.m_properties.m_levelupEffect;
            if (levelupEffect != null)
            {
                InstanceID instance2 = default(InstanceID);
                instance2.Building = buildingID;
                Vector3 pos;
                Quaternion q;
                buildingData.CalculateMeshPosition(out pos, out q);
                Matrix4x4 matrix = Matrix4x4.TRS(pos, q, Vector3.one);
                EffectInfo.SpawnArea spawnArea = new EffectInfo.SpawnArea(matrix, buildingData.Info.m_lodMeshData);
                Singleton<EffectManager>.instance.DispatchEffect(levelupEffect, instance2, spawnArea, Vector3.zero, 0f, 1f, instance.m_audioGroup);
            }
            Vector3 position = buildingData.m_position;
            position.y += buildingData.Info.m_size.y;
            Singleton<NotificationManager>.instance.AddEvent(NotificationEvent.Type.LevelUp, position, 1f);
            Singleton<SimulationManager>.instance.m_currentBuildIndex += 1u;
        }
    }
}
