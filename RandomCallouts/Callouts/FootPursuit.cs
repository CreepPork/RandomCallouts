using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System;
using System.Drawing;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("FootPursuit", CalloutProbability.Medium)]
    class FootPursuit : Callout
    {
        Ped A1;
        Ped C1;
        Vector3 spawnPoint;
        Blip B1;
        Blip B2;
        LHandle pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
            // Spawn points
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));

            // Spawn peds
            A1 = new Ped(spawnPoint);
            C1 = new Ped("s_m_y_cop_01", A1.GetOffsetPosition(new Vector3(15f, 15f, 0)), 0);

            // Set our randomness
            int r = new Random().Next(1, 3);

            if (r == 1)
            {
                C1.Inventory.GiveNewWeapon("WEAPON_STUNGUN", 500, false);
            }
            else
            {
                A1.Inventory.GiveNewWeapon("WEAPON_PISTOL", 100, false);
            }

            // Check if they spawned
            if (!A1.Exists()) return false;
            if (!C1.Exists()) return false;

            // Show the stuff
            this.ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 30f);
            this.AddMinimumDistanceCheck(10f, C1.Position);

            // Show the messages
            this.CalloutMessage = "Foot Pursuit";
            this.CalloutPosition = spawnPoint;

            // Play the audio
            Functions.PlayScannerAudioUsingPosition("CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_02 CRIME_RESIST_ARREST_04", spawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            try
            {
                // Attach the blips
                B1 = A1.AttachBlip();
                B2 = C1.AttachBlip();

                B2.Color = Color.Blue;
	
	            A1.BlockPermanentEvents = true;
	            C1.BlockPermanentEvents = true;
	
	            // Shows the player to respond to the scene.
	            Game.DisplaySubtitle("Get to the ~r~pursuit~w~.", 6500);
	            B1.EnableRoute(Color.Red);

                // Create the pursuit and add the peds to the pursuit
                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, A1);
                Functions.AddCopToPursuit(pursuit, C1);

                // Request backup
                Functions.RequestBackup(spawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            }
            catch (System.Exception ex)
            {
                Game.LogTrivial("Error is: " + ex);
            }

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            // If the player didn't accept the callout we cleanup this mess
            if (A1.Exists()) A1.Delete();
            if (C1.Exists()) C1.Delete();
            if (B1.Exists()) B1.Delete();
            if (B2.Exists()) B2.Delete();

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {

            // If one of the peds dies then remove their blip
            if (A1.IsDead)
            {
                B1.Delete();
            }
            else if (C1.IsDead)
            {
                B2.Delete();
            }

            // Check if the pursuit is still running and if it isn't then end the callout
            if (!Functions.IsPursuitStillRunning(pursuit))
            {
                End();
            }

            base.Process();
        }

        public override void End()
        {
            // Deletes the blips and removes some of the stuff
            if (B1.Exists()) B1.Delete();
            if (B2.Exists()) B2.Delete();
            if (A1.Exists()) A1.Dismiss();
            if (C1.Exists()) C1.Dismiss();

            base.End();
        }

    }
}
