using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System.Windows.Forms;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;
using System;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("KidnappingCallout", CalloutProbability.VeryLow)]
    class KidnappingCallout : Callout
    {
        // Here we declare our variables, things we need for our callout
        private Vehicle PedoVan;
        private Ped Aggressor;
        private Ped Aggressor2;
        private Ped Victim;
        private Vector3 SpawnPoint;
        private Blip ABlip;
        private Blip ABlip2;
        private Blip B1;
        private LHandle pursuit;

        public static InitializationFile initialiseFile()
        {
            InitializationFile ini = new InitializationFile("Plugins/LSPDFR/RandomCallouts.ini");
            ini.Create();
            return ini;
        }

        public static String getEndKey()
        {
            InitializationFile ini = initialiseFile();
            //ReadString takes 3 parameters: the first is the category, the second is the name of the entry and the third is the default value should the user leave the field blank.
            //Take a look at the example .ini file to understand this better.
            string keyBinding = ini.ReadString("Keys", "EndCalloutKey", "End");
            return keyBinding;
        }

        public static String getKidnappingKey()
        {
            InitializationFile ini = initialiseFile();
            string keyBinding1 = ini.ReadString("Keys", "AskToGetOutOfVanKey", "Y");
            return keyBinding1;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            // Set our spawn point to be on a street around 300f near our player.
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            // Create our Aggressor ped in the world
            Aggressor = new Ped(SpawnPoint);
            Aggressor2 = new Ped(SpawnPoint);

            Victim = new Ped(SpawnPoint);

            int r = new Random().Next(1, 4);

            if (r == 1)
            {
                try
                {
	                // Create the vehicle for our ped
	                PedoVan = new Vehicle("SPEEDO", SpawnPoint);
                }
                catch (System.Exception ex)
                {
                    Game.LogVerbose("Failed to spawn the Speedo Vehicle. Error is: " + ex);
                    return false;
                }
            }
            else if (r == 2)
            {
                try
                {
                	PedoVan = new Vehicle("RUMPO", SpawnPoint);
                }
                catch (System.Exception ex)
                {
                    Game.LogVerbose("Failed to spawn the Rumpo Vehicle. Error is: " + ex);
                    return false;
                }
            }
            else if (r == 3)
            {
                try
                {
                	PedoVan = new Vehicle("BURRITO", SpawnPoint);
                }
                catch (System.Exception ex)
                {
                    Game.LogVerbose("Failed to spawn the Burrito Vehicle. Error is: " + ex);
                    return false;
                }
            }

            // Now we have spawned them, check they actually exist and if not return false (callout aborted).
            if (!Aggressor.Exists()) return false;
            if (!Aggressor2.Exists()) return false;
            if (!Victim.Exists()) return false;
            if (!PedoVan.Exists()) return false;

            //Victim.BlockPermanentEvents = true;

            // If we made it this far put the driver in the driver seat.
            Aggressor.WarpIntoVehicle(PedoVan, -1);
            Aggressor2.WarpIntoVehicle(PedoVan, -2);
            Victim.WarpIntoVehicle(PedoVan, -2);

            // Show the user where the pursuit is about to happen and block very close peds.
            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            this.AddMinimumDistanceCheck(5f, Aggressor.Position);

            // Give the person a weapon
            Aggressor.Inventory.GiveNewWeapon("WEAPON_ASSAULTRIFLE", 1000, true);
            Aggressor2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);

            // Set up our callout message and location.
            CalloutMessage = "A Kidnapping";
            CalloutPosition = SpawnPoint;

            Victim.BlockPermanentEvents = true;

            // Play the police scanner audio for this callout.
            Functions.PlayScannerAudioUsingPosition("CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_KIDNAPPING UNITS_RESPOND_CODE_3", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        // OnCalloutAccepted is where we begin our callout's logic. In this instance we create our pursuit and add our ped from earlier to the pursuit as well
        public override bool OnCalloutAccepted()
        {
            // We accepted the callout, so lets initialize our blip from before and attach it to our ped so we know where he is.
            ABlip = Aggressor.AttachBlip();
            ABlip2 = Aggressor2.AttachBlip();
            B1 = Victim.AttachBlip();
            B1.Color = Color.Orange;

            Game.DisplaySubtitle("Get to the ~r~pursuit~w~.", 6500);

            this.pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(pursuit, Aggressor);
            Functions.AddPedToPursuit(pursuit, Aggressor2);
            Functions.RequestBackup(SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);


            return base.OnCalloutAccepted();
        }

        // If we don't accept the callout this will be called and we will clear anything that we have spawned to prevent it staying in the game.
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();

            if (Aggressor.Exists()) Aggressor.Delete();
            if (Aggressor2.Exists()) Aggressor2.Delete();
            if (Victim.Exists()) Victim.Delete();
            if (PedoVan.Exists()) PedoVan.Delete();
            if (ABlip.Exists()) ABlip.Delete();
            if (ABlip2.Exists()) ABlip2.Delete();
            if (B1.Exists()) B1.Delete();
        }

        // This is were it all happens, run all of your callout's logic here.
        public override void Process()
        {
            base.Process();

            GameFiber.StartNew(delegate
            {
                {

                    //A keys converter is used to convert a string to a key.
                    KeysConverter kc = new KeysConverter();

                    //We create two variables: one is a System.Windows.Keys, the other is a string.
                    Keys EndCalloutKey;
                    Keys KidnappingKey;


                    //Use a try/catch, because reading values from files is risky: we can never be sure what we're going to get and we don't want our plugin to crash.
                    try
                    {
                        //We assign myKeyBinding the value of the string read by the method getMyKeyBinding(). We then use the kc.ConvertFromString method to convert this to a key.
                        //If the string does not represent a valid key (see .ini file for a link) an exception is thrown. That's why we need a try/catch.
                        EndCalloutKey = (Keys)kc.ConvertFromString(getEndKey());
                        KidnappingKey = (Keys)kc.ConvertFromString(getKidnappingKey());
                    }
                    //If there was an error reading the values, we set them to their defaults. We also let the user know via a notification.
                    catch
                    {
                        EndCalloutKey = Keys.End;
                        KidnappingKey = Keys.Y;
                        Game.DisplayNotification("There was an error reading the .ini file. Setting defaults...");
                    }

                    if (Game.LocalPlayer.Character.Position.DistanceTo(Victim) <= 10)
                    {
                        Game.DisplayHelp("Press ~b~" + KidnappingKey + "~w~ to ask the Victim out of the van.");
                        if (Game.IsKeyDown(KidnappingKey))
                        {
                            if (Victim.Exists())
                            {
                                Victim.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
                                if (B1.Exists()) B1.Delete();
                                if (Victim.Exists()) Victim.Dismiss(); 
                            }
                        }
                    }

                    else if (Game.IsKeyDown(EndCalloutKey))
                    {

                        try
                        {
	                        if (B1.Exists()) B1.Delete();
	
	                        Game.DisplayNotification("~r~Kidnapping Callout~w~ is ~g~Code 4~w~.");
	                        Functions.PlayScannerAudio("WE_ARE_CODE_4 NO_FURTHER_UNITS_REQUIRED");
	
	                        End();
                        }
                        catch (System.Exception ex)
                        {
                            Game.LogTrivial("Failed to execute the End callout. Error is: " + ex);
                        }
                    }

                    GameFiber.StartNew(delegate
                    {
                        GameFiber.Wait(5000);

                        Game.DisplayHelp("To end the callout press ~b~" + EndCalloutKey + "~w~.");

                    }, "waitingForNotification");

                }
            }, "getOutOfPedoVanVictimKeyCheckerThingy");

            if (Aggressor.IsCuffed)
            {
                ABlip.Delete();
            }
            else if(Aggressor2.IsCuffed)
            {
                ABlip2.Delete();
            }

            else if(Aggressor.IsDead)
            {
                ABlip.Delete();
            }
            else if(Aggressor2.IsDead)
            {
                ABlip2.Delete();
            }
            else if(Victim.IsDead)
            {
                B1.Delete();
            }
        }

        // More cleanup, when we end the callout you clean away anything that's left over
        // This is also important as this will be called if a callout gets aborted (for example if you force a new callout)
        public override void End()
        {
            base.End();
            if (ABlip.Exists()) ABlip.Delete();
            if (ABlip2.Exists()) ABlip2.Delete();
            if (B1.Exists()) B1.Delete();
            if (PedoVan.Exists()) PedoVan.Dismiss();
            if (Aggressor.Exists()) Aggressor.Dismiss();
            if (Aggressor2.Exists()) Aggressor2.Dismiss();
            if (Victim.Exists()) Victim.Delete();
        }
    }
}
