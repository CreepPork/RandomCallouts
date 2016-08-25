using Rage;
using LSPD_First_Response.Mod.API;
using RandomCallouts.Callouts;
using RandomCallouts.VersionCheckers;
using RandomCallouts.InteractionMenu;
using System.Reflection;

namespace RandomCallouts
{
    // Do not rename! Attributes or inheritance based plugins will follow when the API is more in depth.
    public class Main : Plugin
    {
        // Constructor for the main class, same as the class, do not rename.
        public Main()
        {

        }

        // Called when the plugin ends or is terminated to cleanup
        public override void Finally()
        {
            Game.DisplayHelp("Random Callouts wishes you a good time off duty!");
        }

        // Called when the plugin is first loaded by LSPDFR
        public override void Initialize()
        {
            // Initialize our Interaction Menu
            InteractionMenuClass.Main();

            // Checks for RAGE version
            checkForRageVersionClass.checkForRageVersion(0.37f);

            // Doesn't work, and a pain in the ass
            //checkForRandomCalloutsUpdate.checkForRandomCalloutsUpdateMain();

            // Event handler for detecting if the player goes on duty
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
            Game.LogTrivial("Random Callouts plugin has loaded!");
        }

        // The event handler mentioned above,
        static void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {
                string version = Assembly.GetExecutingAssembly()
                    .GetName()
                    .Version
                    .ToString();

                Game.DisplayNotification("Thank you for installing ~r~Random Callouts " + version +" Dev~w~ by ~b~CreepPork_LV~w~!");

                // If the player goes on duty we need to register our custom callouts
                // Here we register our RandomCallouts class which is inside our Callouts folder (APIExample.Callouts namespace)
                Functions.RegisterCallout(typeof(TractorCallout));
                Functions.RegisterCallout(typeof(PoliceCarStolen));
                Functions.RegisterCallout(typeof(KidnappingCallout));
                //Functions.RegisterCallout(typeof(LorryChaseCallout));

                // Remove this callout for the full release to avoid some issues.
                Functions.RegisterCallout(typeof(Mugging));

                Functions.RegisterCallout(typeof(StolenMilitaryHardware));
                //Functions.RegisterCallout(typeof(BankCarRobbery));
                Functions.RegisterCallout(typeof(GangAttack));
                Functions.RegisterCallout(typeof(HighPerformanceVehicle));
                Functions.RegisterCallout(typeof(FootPursuit));
                Functions.RegisterCallout(typeof(CrashedCar));
                Functions.RegisterCallout(typeof(LooseLivestock));
                Functions.RegisterCallout(typeof(PublicDisorder));
                Functions.RegisterCallout(typeof(StolenArmoredCar));
            }
        }
    }
}
