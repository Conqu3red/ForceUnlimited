using BepInEx;
using Logger = BepInEx.Logging.Logger;
using HarmonyLib;
using System.Reflection;
using BepInEx.Configuration;


namespace ForceUnlimited
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class ForceUnlimited : BaseUnityPlugin
    {
        public const string
            PluginGuid = "Conqu3red.ForceUnlimited",
            PluginName = "Force Unlimited Materials/Budget",
            PluginVersion = "1.0.0";
        
        public static ForceUnlimited instance;
        public static ConfigEntry<bool> modEnabled;

        public static MethodInfo ApplyForceUnlimitedBudget, ApplyForceUnlimitedMaterial;
        Harmony harmony;
        void Awake()
        {
			if (instance == null) instance = this;
            // Use this if you wish to make the mod trigger cheat mode ingame.
            // Set this true if your mod effects physics or allows mods that you can't normally do.
            modEnabled = Config.Bind("Force Unlimited Budget/Materials", "modEnabled", true, "Enable Mod");

            harmony = new Harmony("Conqu3red.ForceUnlimited");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            

        }

        [HarmonyPatch(typeof(Main))]
        [HarmonyPatch("InstantiateGameUI")]
        private static class StartPath {
            public static void Postfix(){
                ApplyForceUnlimitedBudget = typeof(Budget).GetMethod("ApplyForceUnlimitedBudget", BindingFlags.NonPublic | BindingFlags.Static);
                ApplyForceUnlimitedMaterial = typeof(Budget).GetMethod("ApplyForceUnlimitedMaterial", BindingFlags.NonPublic | BindingFlags.Static);
            }
        }

        [HarmonyPatch(typeof(Campaign), "LoadLayout")]
        private static class LoadPatch {
            public static void Postfix(CampaignLevel level){
                if (modEnabled.Value){
                    instance.Logger.LogInfo(ApplyForceUnlimitedBudget == null);
                    instance.Logger.LogInfo(ApplyForceUnlimitedMaterial == null);

                    Budget.m_UsingForcedUnlimitedBudget = true;
                    ApplyForceUnlimitedBudget.Invoke(null, new object[] {});
                    
                    Budget.m_UsingForcedUnlimitedMaterial = true;
                    ApplyForceUnlimitedMaterial.Invoke(null, new object[] {});
                    GameUI.m_Instance.m_Materials.SetMaterialIconsAlpha();
                    
                    BridgeCheat.m_Cheated = true;
                }
            }
        }
    
    }
}