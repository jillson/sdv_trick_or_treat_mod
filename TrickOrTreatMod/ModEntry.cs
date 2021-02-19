using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;
using JsonAssets;


namespace TrickOrTreatMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : StardewModdingAPI.Mod
    {
        private string holiday = "none";
        private int []candyList;
        private Random random;

        /*********
** Public methods
*********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            this.random = new Random();
        }

        /*********
        ** Private methods
        *********/

        private int []defaultLoad()
        {
            if (holiday == "halloween")
            {
                return new int[] { 279, 612, 731, 245 };
            }
            else
            {
                return new int[] { 107, 558, 442, 305, 180, 182, 176, 174 };
            }

        }

        private void OnDayStarted(object sender, DayStartedEventArgs args)
        {
            int i = 0;
            // Check if we're on a day we give out candy
            
            if (Game1.Date.DayOfMonth == 27 && Game1.Date.SeasonIndex == 2)
            {
                holiday = "halloween";
            }
            else if (Game1.Date.DayOfMonth == 13 && Game1.Date.SeasonIndex == 0)
            {
                holiday = "easter";
            }
            else
            {
                holiday = "none"; // TODO (maybe) have config option so every day can be a candy day
            }
            string specificPack = "dadofgwen.halloweencandycontent";

            if (holiday == "easter")
            {
                specificPack = "dadofgwen.eastercandycontent";
            }

            if (holiday == "none")
            {
                return;
            }

            if (true) { 
                var jam = this.Helper.ModRegistry.GetApi("spacechase0.JsonAssets");
                if (jam != null)
                {
                    var jama = (JsonAssets.Api)jam;
                    var candylist = jama.GetAllObjectsFromContentPack("dadofgwen.candycontent");
                    
                    //candylist = new List<int> {  }; 
                    var specificList = jama.GetAllObjectsFromContentPack(specificPack);
                    if (candylist == null || specificList == null)
                    {
                        this.Monitor.Log("We couldn't open our content packs, falling back to built in content ", LogLevel.Debug);
                        defaultLoad();
                    }
                    else
                    {
                        candyList = new int[candylist.Count + specificList.Count];
                        foreach (var candy in candylist)
                        {
                            var cname = candy.ToString();
                            candyList[i] = jama.GetObjectId(cname);
                            i++;
                        }
                        foreach (var candy in specificList)
                        {
                            var cname = candy.ToString();
                            candyList[i] = jama.GetObjectId(cname);
                            i++;
                        }
                    }
                }
                else
                {
                    Monitor.Log($"Couldn't load JsonAssets.Api ", LogLevel.Debug);
                    defaultLoad();
                }
            }
            return;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs args) {

            int itemId = 0;

            if (!Context.IsWorldReady || args.NewMenu == null || this.holiday == "none") {
                return;
            }

            if (args.NewMenu is StardewValley.Menus.DialogueBox dbox)
            {
                if (dbox.characterDialogue == null || this.candyList.Count() == 0)
                {
                    return;
                }

                try
                {
                    
                    int index = random.Next(this.candyList.Count());
                    itemId = this.candyList[index];
                    var item = new SObject(itemId, 1);
                    Game1.player.addItemByMenuIfNecessary(item);
                }
                catch (System.Collections.Generic.KeyNotFoundException ex)
                {
                    this.Monitor.Log($"Object { itemId } was not found { ex }", LogLevel.Debug);
                }
            }
            return;
        }
    }
}