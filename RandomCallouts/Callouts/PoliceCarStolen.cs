using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("PoliceCarStolen", CalloutProbability.Low)]
    class PoliceCarStolen : Callout
    {
        // Here we declare our variables, things we need for our callout
        private Vehicle PoliceCar;
        private Ped Aggressor;
        private Vector3 SpawnPoint;
        private Blip ABlip;
        private LHandle pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
          // Set our spawn point to be on a street around 300f near our player.
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            int r = new Random().Next(1, 3);
            if (r == 1)
            {
                // If the outcome is 1 then we choose this police vehicle
                PoliceCar = new Vehicle("POLICE", SpawnPoint);
            }
            if (r == 2)
            {
                PoliceCar = new Vehicle("POLICE4", SpawnPoint);
            }

         // Create our Aggressor ped in the world
            Aggressor = new Ped(SpawnPoint);

            // Reserve method
         // Create the vehicle for our ped
            //PoliceCar = new Vehicle("POLICE2", SpawnPoint);

         // Now we have spawned them, check they actually exist and if not return false (callout aborted).
            if (!Aggressor.Exists()) return false;
            if (!PoliceCar.Exists()) return false;

        // If we made it this far put the driver in the driver seat.
            Aggressor.WarpIntoVehicle(PoliceCar, -1);

        // Show the user where the pursuit is about to happen and block very close peds.
            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            this.AddMinimumDistanceCheck(5f, Aggressor.Position);

        // Give the person a weapon
            Aggressor.Inventory.GiveNewWeapon("WEAPON_PUMPSHOTGUN", 500, true);

            // Makes the driver not freak out and leave the vehicle, doesn't work for some reason.
            Aggressor.BlockPermanentEvents = true;

            // Turn on the vehicle's siren
            PoliceCar.IsSirenOn = true;

        // Set up our callout message and location.
            this.CalloutMessage = "Stolen Police Vehicle";
            this.CalloutPosition = SpawnPoint;

        // Play the police scanner audio for this callout.
            Functions.PlayScannerAudioUsingPosition("CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_STOLEN_POLICE_VEHICLE UNITS_RESPOND_CODE_3", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        // OnCalloutAccepted is where we begin our callout's logic. In this instance we create our pursuit and add our ped from earlier to the pursuit as well
        public override bool OnCalloutAccepted()
        {
            // We accepted the callout, so lets initialize our blip from before and attach it to our ped so we know where he is.
            ABlip = Aggressor.AttachBlip();
            this.pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(this.pursuit, this.Aggressor);
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
            if (PoliceCar.Exists()) PoliceCar.Delete();
            if (ABlip.Exists()) ABlip.Delete();
        }

        // This is were it all happens, run all of your callouts logic here.
        public override void Process()
        {
            base.Process();

            // A simple check, if our pursuit has ended we end the callout
            if (!Functions.IsPursuitStillRunning(pursuit))
            {
                this.End();
            }
        }
        
        // More cleanup, when we end the callout you clean away anything that's left over
        // This is also important as this will be called if a callout gets aborted (for example if you force a new callout)
        public override void End()
        {
            base.End();
            if (ABlip.Exists()) ABlip.Delete();
            if (PoliceCar.Exists()) PoliceCar.Dismiss();
            if (Aggressor.Exists()) Aggressor.Dismiss();
        }
    }
}
