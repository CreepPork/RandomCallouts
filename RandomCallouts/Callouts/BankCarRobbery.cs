using System;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage.Native;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("StolenBankCar", CalloutProbability.Low)]
    class BankCarRobbery : Callout
    {
        // Set all of our variables here.
        private Vehicle stockade;
        private Ped Aggressor1;
        private Ped Aggressor2;
        private Ped Aggressor3;
        private Ped Aggressor4;
        private Vector3 spawnPoint;
        private Vector3 vehicleSpawnPoint;
        private Blip ABlip1;
        private Blip ABlip2;
        private Blip ABlip3;
        private Blip ABlip4;
        private LHandle pursuit;
        int r = new Random().Next(1, 3);

        public override bool OnBeforeCalloutDisplayed()
        {
            // Set our spawn points
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            vehicleSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(320f));

            // Spawn our armored van
            stockade = new Vehicle("STOCKADE", vehicleSpawnPoint);

            // Spawn our aggressors
            Aggressor1 = new Ped("csb_mweather", spawnPoint, 0f);
            Aggressor2 = new Ped("csb_mweather", spawnPoint, 0f);
            if (r == 1)
            {
                // Spawn our aggressors
                Aggressor3 = new Ped("csb_mweather", spawnPoint, 0f);
                Aggressor4 = new Ped("csb_mweather", spawnPoint, 0f);

                // Check if the peds exists
                if (!Aggressor3.Exists()) return false;
                if (!Aggressor4.Exists()) return false;

                // Warp the peds into the vehicle
                Aggressor3.WarpIntoVehicle(stockade, 1);
                Aggressor4.WarpIntoVehicle(stockade, 2);

                // Give weapons
                Aggressor3.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
                Aggressor4.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);

                // Give armor
                Aggressor3.Armor = 100;
                Aggressor4.Armor = 100;
            }

            // Check if our van exists and our aggressors
            if (!Aggressor1.Exists()) return false;
            if (!Aggressor2.Exists()) return false;
            if (!stockade.Exists()) return false;

            // Warp in the vehicle
            Aggressor1.WarpIntoVehicle(stockade, -1);
            Aggressor2.WarpIntoVehicle(stockade, -2);

            // Set our minimum distance check
            this.CalloutMessage = "Stolen Armored Car";
            this.CalloutPosition = vehicleSpawnPoint;

            // Give weapons
            Aggressor1.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
            Aggressor2.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);

            // Give armor
            Aggressor1.Armor = 100;
            Aggressor2.Armor = 100;

            // Play the audio
            Functions.PlayScannerAudioUsingPosition("ATTENTION_01 OFFICERS_REPORT_01 BANK_CAR UNITS_RESPOND_CODE_99_01", vehicleSpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            try
            {
                // Show the player to respond
                Game.DisplaySubtitle("~Get to the ~r~pursuit~w~.", 6500);
            }
            catch (System.Exception ex)
            {
                Game.LogVerbose("An error occurred when trying to display the text. Error is: " + ex);
            }

            // Create the pursuit
            pursuit = Functions.CreatePursuit();

            // Add our peds to the pursuits
            Functions.AddPedToPursuit(pursuit, Aggressor1);
            Functions.AddPedToPursuit(pursuit, Aggressor2);
            if (Aggressor3.Exists()) Functions.AddPedToPursuit(pursuit, Aggressor3);
            if (Aggressor4.Exists()) Functions.AddPedToPursuit(pursuit, Aggressor4);

            // Add blips to our peds
            ABlip1 = Aggressor1.AttachBlip();
            ABlip2 = Aggressor2.AttachBlip();
            if (Aggressor3.Exists()) ABlip3 = Aggressor3.AttachBlip();
            if (Aggressor4.Exists()) ABlip4 = Aggressor4.AttachBlip();

            if (Aggressor3.Exists() && Aggressor4.Exists())
            {
                // Make the peds attack the player
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", Aggressor3, Game.LocalPlayer.Character, 0, 1);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", Aggressor4, Game.LocalPlayer.Character, 0, 1);
                //Aggressor3.Tasks.FightAgainstClosestHatedTarget(500f);
                //Aggressor4.Tasks.FightAgainstClosestHatedTarget(500f);
            }

            // Request a NOOSE unit
            Functions.RequestBackup(vehicleSpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.SwatTeam);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            // Delete all of our spawned items if player doesn't accept the callout.
            if (Aggressor1.Exists()) Aggressor1.Delete();
            if (Aggressor2.Exists()) Aggressor2.Delete();
            if (Aggressor3.Exists()) Aggressor3.Delete();
            if (Aggressor4.Exists()) Aggressor4.Delete();
            if (stockade.Exists()) stockade.Delete();
            if (ABlip1.Exists()) ABlip1.Delete();
            if (ABlip2.Exists()) ABlip2.Delete();
            if (ABlip3.Exists()) ABlip3.Delete();
            if (ABlip4.Exists()) ABlip4.Delete();

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            // If someone of the aggressors dies then remove their blip
            if (Aggressor1.IsDead)
            {
                ABlip1.Delete();
            }

            if (Aggressor2.IsDead)
            {
                ABlip2.Delete();
            }

            if (Aggressor3.Exists() && Aggressor3.IsDead)
            {
                ABlip3.Delete();
            }

            if (Aggressor4.Exists() && Aggressor4.IsDead)
            {
                ABlip4.Delete();
            }

            //if (Aggressor1.IsCuffed)
            //{
            //  ABlip1.Delete();
            //}

                //if (Aggressor2.IsCuffed)
                //{
                //ABlip2.Delete();
                //}

                // Checks if the pursuit is still running
                if (!Functions.IsPursuitStillRunning(pursuit))
            {
                End();
            }

            base.Process();
        }

        public override void End()
        {
            // Delete the entities that we spawned
            if (ABlip1.Exists()) ABlip1.Delete();
            if (ABlip2.Exists()) ABlip2.Delete();
            if (ABlip3.Exists()) ABlip3.Delete();
            if (ABlip4.Exists()) ABlip4.Delete();
            if (stockade.Exists()) stockade.Dismiss();

            base.End();
        }

    }
}
