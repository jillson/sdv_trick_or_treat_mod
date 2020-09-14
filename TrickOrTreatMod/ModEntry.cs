using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;


namespace TrickOrTreatMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private Boolean candyEnabled;
        private Boolean eggsEnabled;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            this.candyEnabled = false;
            this.eggsEnabled = false;
        }

        /*********
        ** Private methods
        *********/


        private void OnDayStarted(object sender, DayStartedEventArgs args)
        {
            // Check if we're on the right day
            // MAYBE TODO: add config to enable candy / egg separately?
            this.candyEnabled = true; // (Game1.Date.DayOfMonth == 27 && Game1.Date.SeasonIndex == 2);
            this.eggsEnabled = (Game1.Date.DayOfMonth == 13 && Game1.Date.SeasonIndex == 0);
            return;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs args) {

            int itemId = 0;

            if (!Context.IsWorldReady) {
                return;
            }

            if (args.NewMenu == null)
            {
                return;
            }

            if (args.OldMenu != null)
            {
                return;
            }

            if (!this.eggsEnabled && !this.candyEnabled)
            {
                return;
            }

            if (args.NewMenu is StardewValley.Menus.DialogueBox)
            {

                try
                {
                    var random = new Random();
                    List<int> Items = null ;

                    if (this.candyEnabled)
                    {
                        //TODO: patch in some more / better candy options
                        Items = new List<int> { 279, 612, 731, 245 }; 
                        
                    }
                    else
                    {
                        //TODO: add all candy from above to this list of eggs
                        Items = new List<int> { 107, 558, 442, 305, 180, 182, 176, 174 };
                    }

                    int index = random.Next(Items.Count());
                    itemId = Items[index];
                    var y = new SObject(itemId, 1);
                    Game1.player.addItemByMenuIfNecessary(y);                
                }
                catch (System.Collections.Generic.KeyNotFoundException ex)
                {
                    this.Monitor.Log($"Object { itemId } was not found { ex }", LogLevel.Debug);
                    return;
                }
            }
            return;
        }
    }
}