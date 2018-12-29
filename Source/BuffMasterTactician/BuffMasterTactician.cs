using System.Reflection;
using Harmony;
using BattleTech;
using System.IO;



namespace BuffMasterTactician
{
    public class BuffMasterTactician
    {
        public static string LogPath;
        public static string ModDirectory;

        public static void Init(string directory, string settingsJSON)
        {
            ModDirectory = directory;

            LogPath = Path.Combine(ModDirectory, "BuffMasterTactician.log");
            File.CreateText(BuffMasterTactician.LogPath);

            var harmony = HarmonyInstance.Create("de.ben.BuffMasterTactician");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(AbstractActor), "OnNewRound", null)]
    public static class Tactics_Morale_Bonus
    {
        public static void Postfix(AbstractActor __instance)
        {
            using (var Logger = File.AppendText(BuffMasterTactician.LogPath))
            {
                Pilot currentPilot = __instance.GetPilot();
                if (currentPilot != null && currentPilot.PassiveAbilities.Count > 0)
                {
                    for (int i = 0; i < currentPilot.PassiveAbilities.Count; i++)
                    {
                        if (currentPilot.PassiveAbilities[i].Def.Description.Id == "AbilityDefT8A")
                        {
                            Logger?.WriteLine("[BuffMasterTactician] Pilot is Master Tactician: ModifyMorale(3)");

                            __instance.team.ModifyMorale(3);
                        }
                    }
                }
            }
        }
    }
}

