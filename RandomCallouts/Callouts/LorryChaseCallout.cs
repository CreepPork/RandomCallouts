using System;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System.Drawing;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("LorryChaseCallout", CalloutProbability.Medium)]
    class LorryChaseCallout : Callout
    {
        // Here we declare our variables, things we need for our callout
        private Vehicle Lorry;
        private Vehicle Tanker;
        private Ped Aggressor;
        private Vector3 SpawnPoint;
        private Vector3 tankerSpawnPoint;
        private Blip ABlip;
        private LHandle pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
            // Set our spawn point to be on a street around 300f near our player.
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));

            // Set our spawn point for the tanker
            tankerSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(340f));

            // Create our Aggressor ped in the world
            Aggressor = new Ped("s_m_m_trucker_01", SpawnPoint, 0f);

            // Create the vehicle for our ped
            Lorry = new Vehicle("PHANTOM", SpawnPoint);

            int r = new Random().Next(1, 3);

            if (r == 1)
            {
                Tanker = new Vehicle("TRAILERS", tankerSpawnPoint);
            }
            else
            {
                Tanker = new Vehicle("TANKER", tankerSpawnPoint);
            }

            //Tanker = new Vehicle("TANKER", tankerSpawnPoint);
            // Now we have spawned them, check they actually exist and if not return false (callout aborted).
            if (!Aggressor.Exists()) return false;
            if (!Tanker.Exists()) return false;
            if (!Lorry.Exists()) return false;

            // If we made it this far put the driver in the driver seat.
            Aggressor.WarpIntoVehicle(Lorry, -1);

            // Attaches the trailer
            Lorry.Trailer = Tanker;

            // Show the user where the pursuit is about to happen and block very close peds.
            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            this.AddMinimumDistanceCheck(5f, Aggressor.Position);

            // Set up our callout message and location.
            this.CalloutMessage = "Pursuit of a Truck";
            this.CalloutPosition = SpawnPoint;

            // Play the police scanner audio for this callout.
            Functions.PlayScannerAudioUsingPosition("CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_RESIST_ARREST_02", SpawnPoint);



            return base.OnBeforeCalloutDisplayed();
        }

        // OnCalloutAccepted is where we begin our callout's logic. In this instance we create our pursuit and add our ped from earlier to the pursuit as well
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

            // We accepted the callout, so lets initialize our blip from before and attach it to our ped so we know where he is.
            ABlip = Aggressor.AttachBlip();
            this.pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(this.pursuit, this.Aggressor);
            Functions.RequestBackup(SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            Functions.RequestBackup(SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);

            ABlip.EnableRoute(Color.Red);

            return base.OnCalloutAccepted();
        }

        // If we don't accept the callout this will be called and we will clear anything that we have spawned to prevent it staying in the game.
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();

            if (Aggressor.Exists()) Aggressor.Delete();
            if (Lorry.Exists()) Lorry.Delete();
            if (ABlip.Exists()) ABlip.Delete();
            if (Tanker.Exists()) Tanker.Delete();
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
            if (Lorry.Exists()) Lorry.Dismiss();
            if (Tanker.Exists()) Tanker.Dismiss();
            if (Aggressor.Exists()) Aggressor.Dismiss();
        }
    }
}
