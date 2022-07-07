using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.ModHelper;
using NoMansSky.Api;
using System.IO;
using Newtonsoft.Json;
using libMBIN.NMS.GameComponents;

namespace NoMansSky.ModTemplate
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : NMSMod
    {
        /// <summary>
        /// Initializes your mod along with some necessary info.
        /// </summary>
        public Mod(IModConfig _config, IReloadedHooks _hooks, IModLogger _logger) : base(_config, _hooks, _logger)
        {
            Logger.WriteLine("Welcome to the NMS ColorTester! You can use this to mod Space Colors in NMS.");
            Logger.WriteLine($"This mod will export SpaceColor files to \"{MyModFolder}\". Go there, edit the files, then press Ctrl + RightArrow in game to refresh the colors.");

            Game.SpaceColors.OnColorLoaded.AddListener(spaceColor =>
            {
                Logger.WriteLine("Space color loaded!");
                string savePath = $"{MyModFolder}\\{spaceColor.MBinName}.json";
                if (File.Exists(savePath))
                    return;

                Logger.WriteLine($"Saving {spaceColor.MBinName} to {savePath}");
                var colorSettings = spaceColor.GetValue();
                var valueToWrite = colorSettings.Settings[0];
                
                var json = JsonConvert.SerializeObject(valueToWrite, Formatting.Indented);
                File.WriteAllText(savePath, json);
                Logger.WriteLine($"{spaceColor.MBinName} successfully saved. You can modify it there. When you're done press Ctrl + RightArrow in game to update");
            });
        }

        /// <summary>
        /// Called once every frame.
        /// </summary>
        public override void Update()
        {
            if (!Game.IsInGame)
                return;

            if (Keyboard.IsHeld(Key.Control) && Keyboard.IsPressed(Key.RightArrow))
            {
                Logger.WriteLine("Updating colors");
                string spaceColorsPath = $"{MyModFolder}\\SPACESKYCOLOURS.json";
                string spaceColorsJson = File.ReadAllText(spaceColorsPath);
                var spaceColors = JsonConvert.DeserializeObject<GcSolarSystemSkyColourData>(spaceColorsJson);
                Game.SpaceColors.DefaulColorSettings.Modify(colorList =>
                {
                    for (int i = 0; i < colorList.Settings.Count; i++)
                    {
                        colorList.Settings[i] = spaceColors;
                    }
                });


                string rareSpaceColorsPath = $"{MyModFolder}\\SPACERARESKYCOLOURS.json";
                string rareSpaceColorsJson = File.ReadAllText(rareSpaceColorsPath);
                var rareSpaceColors = JsonConvert.DeserializeObject<GcSolarSystemSkyColourData>(rareSpaceColorsJson);
                Game.SpaceColors.RareColorSettings.Modify(colorList =>
                {
                    for (int i = 0; i < colorList.Settings.Count; i++)
                    {
                        colorList.Settings[i] = rareSpaceColors;
                    }
                });
            }
        }
    }
}