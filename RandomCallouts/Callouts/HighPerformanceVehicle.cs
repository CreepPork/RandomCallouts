using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage.Native;
using System;
using System.Drawing;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("HighPerformanceVehicleStolen", CalloutProbability.Low)]
    class HighPerformanceVehicle : Callout
    {
        Vehicle FastVehicle;
        Ped A1;
        Ped A2;
        Vector3 spawnPoint;
        Vector3 vehicleSpawnPoint;
        Blip B1;
        Blip B2;
        LHandle pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
            // Spawn points
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            vehicleSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1010f));

            // Spawn peds
            A1 = new Ped(spawnPoint);
            A2 = new Ped(spawnPoint);

            // Set our randomness
            int r = new Random().Next(1, 6);

            if (r == 1)
            {
                FastVehicle = new Vehicle("ADDER", vehicleSpawnPoint);
            }
            if (r == 2)
            {
                FastVehicle = new Vehicle("T20", vehicleSpawnPoint);
            }
            if (r == 3)
            {
                FastVehicle = new Vehicle("OSIRIS", vehicleSpawnPoint);
            }
            if (r == 4)
            {
                FastVehicle = new Vehicle("FELTZER3", vehicleSpawnPoint);
            }
            if (r == 5)
            {
                FastVehicle = new Vehicle("SCHAFTER3", vehicleSpawnPoint);
            }

            // Check if they spawned
            if (!A1.Exists()) return false;
            if (!A2.Exists()) return false;
            if (!FastVehicle.Exists()) return false;

            // Set the peds in the car
            A1.WarpIntoVehicle(FastVehicle, -1);
            A2.WarpIntoVehicle(FastVehicle, -2);

            // Give the weapons
            A1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            A2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);

            // Show the stuff
            this.ShowCalloutAreaBlipBeforeAccepting(vehicleSpawnPoint, 30f);
            this.AddMinimumDistanceCheck(10f, A1.Position);

            // Show the messages
            this.CalloutMessage = "Stolen Super Car";
            this.CalloutPosition = vehicleSpawnPoint;

            // Play the audio
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_01 CRIME_STOLEN_VEH_01 UNITS_RESPOND_CODE_03", vehicleSpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            int r = new Random().Next(1, 3);

            // Create the pursuit and add the peds to the pursuit
            pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(pursuit, A1);
            Functions.AddPedToPursuit(pursuit, A2);

            // Attach the blips
            B1 = A1.AttachBlip();
            B2 = A2.AttachBlip();

            if (r == 1)
            {
                // Make the peds attack the player
                //A2.Tasks.FightAgainstClosestHatedTarget(500f);
                NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A2, Game.LocalPlayer.Character, 0, 1);
            }

            // Request backup
            Functions.RequestBackup(vehicleSpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
            Functions.RequestBackup(vehicleSpawnPoint, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);


            // Shows the player to respond to the scene.
            Game.DisplaySubtitle("Get to the ~r~pursuit~w~.", 6500);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            // If the player didn't accept the callout we cleanup this mess
            if (A1.Exists()) A1.Delete();
            if (A2.Exists()) A2.Delete();
            if (FastVehicle.Exists()) FastVehicle.Delete();
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
            if (A2.IsDead)
            {
                B2.Delete();
            }

            // Check if the pursuit is still running and if it isn't then end the callout
            if (!Functions.IsPursuitStillRunning(pursuit))
            {
                this.End();
            }

            base.Process();
        }

        public override void End()
        {
            // Deletes the blips and removes some of the stuff
            if (B1.Exists()) B1.Delete();
            if (B2.Exists()) B2.Delete();
            if (FastVehicle.Exists()) FastVehicle.Dismiss();
            if (A1.Exists()) A1.Dismiss();
            if (A2.Exists()) A2.Dismiss();

            base.End();
        }
    }
}
