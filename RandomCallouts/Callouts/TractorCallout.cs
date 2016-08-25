using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("TractorCallout", CalloutProbability.Low)]
    class TractorCallout : Callout
    {
        // Here we declare our variables, things we need for our callout
        private Vehicle myVehicle;
        private Ped myPed;
        private Vector3 SpawnPoint;
        private Blip myBlip;
        private LHandle pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
          // Set our spawn point to be on a street around 300f near our player.
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

         // Create our ped in the world
            myPed = new Ped("a_m_m_hillbilly_01", SpawnPoint, 0f);

            int r = new Random().Next(1, 3);

            if (r == 1)
            {
                myVehicle = new Vehicle("TRACTOR", SpawnPoint);
            }
            else
            {
                myVehicle = new Vehicle("TRACTOR2", SpawnPoint);
            }

         // Now we have spawned them, check they actually exist and if not return false (callout aborted).
            if (!myPed.Exists()) return false;
            if (!myVehicle.Exists()) return false;

        // If we made it this far put the driver in the driver seat.
            myPed.WarpIntoVehicle(myVehicle, -1);

        // Show the user where the pursuit is about to happen and block very close peds.
            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            this.AddMinimumDistanceCheck(5f, myPed.Position);

        // Give the person a weapon
            myPed.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);

        // Set up our callout message and location.
            this.CalloutMessage = "Pursuit of a Tractor";
            this.CalloutPosition = SpawnPoint;

        // Play the police scanner audio for this callout.
            Functions.PlayScannerAudioUsingPosition("CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_01 FOR CRIME_STOLEN_VEH_01", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        // OnCalloutAccepted is where we begin our callout's logic. In this instance we create our pursuit and add our ped from earlier to the pursuit as well
        public override bool OnCalloutAccepted()
        {
            // We accepted the callout, so lets initialize our blip from before and attach it to our ped so we know where he is.
            myBlip = myPed.AttachBlip();

            Game.DisplaySubtitle("Get to the ~r~scene~w~.", 6500);
            this.pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(this.pursuit, this.myPed);
            Functions.RequestBackup(SpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);

            return base.OnCalloutAccepted();
        }

        // If we don't accept the callout this will be called and we will clear anything that we have spawned to prevent it staying in the game.
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();

            if (myPed.Exists()) myPed.Delete();
            if (myVehicle.Exists()) myVehicle.Delete();
            if (myBlip.Exists()) myBlip.Delete();
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
        // This is also important as this wil lbe called if a callout gets aborted (for example if you force a new callout)
        public override void End()
        {
            base.End();
            if (myBlip.Exists()) myBlip.Delete();
            if (myVehicle.Exists()) myVehicle.Dismiss();
            if (myPed.Exists()) myPed.Dismiss();
        }
    }
}
