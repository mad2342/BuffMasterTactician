using System.Reflection;
using Harmony;
using BattleTech;
using System.IO;

namespace BuffMasterTactician
{
    public class BuffMasterTactician
    {
        internal static string LogPath;
        internal static string ModDirectory;

        // BEN: DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        internal static int DebugLevel = 1;

        public static void Init(string directory, string settings)
        {
            ModDirectory = directory;
            LogPath = Path.Combine(ModDirectory, "BuffMasterTactician.log");

            Logger.Initialize(LogPath, DebugLevel, ModDirectory, nameof(BuffMasterTactician));

            HarmonyInstance harmony = HarmonyInstance.Create("de.mad.BuffMasterTactician");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }



    [HarmonyPatch(typeof(AbstractActor), "OnNewRound", null)]
    public static class AbstractActor_OnNewRound_Patch
    {
        public static void Postfix(AbstractActor __instance)
        {
            Pilot p = __instance.GetPilot();

            if (p != null && p.PassiveAbilities.Count > 0)
            {
                for (int i = 0; i < p.PassiveAbilities.Count; i++)
                {
                    if (p.PassiveAbilities[i].Def.Description.Id == "AbilityDefT8A")
                    {
                        Logger.Info($"[AbstractActor_OnNewRound_POSTFIX] {p.Callsign} is Master Tactician: ModifyMorale(5)");

                        __instance.team.ModifyMorale(5);

                        // Floatie just for player
                        if (__instance.team.IsLocalPlayer)
                        {
                            __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.GUID, __instance.GUID, "MASTER TACTICIAN: + 5 RESOLVE", FloatieMessage.MessageNature.Buff));
                        }
                    }
                }
            }
        }
    }
}

