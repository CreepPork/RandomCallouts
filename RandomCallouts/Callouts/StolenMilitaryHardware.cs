using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage.Native;
using System;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("StolenMilitaryHardware", CalloutProbability.Low)]
    class StolenMilitaryHardware : Callout
    {
        // Declare all of our variables
        private Vehicle Barracks;
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

        public override bool OnBeforeCalloutDisplayed()
        {

            // Set our spawn points
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            vehicleSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1010f));

            // Spawn our peds
            Aggressor1 = new Ped("a_m_m_hillbilly_01", spawnPoint, 0f);
            Aggressor2 = new Ped("a_m_m_hillbilly_02", spawnPoint, 0f);
            Aggressor3 = new Ped("a_m_m_hillbilly_02", spawnPoint, 0f);
            Aggressor4 = new Ped("u_m_o_taphillbilly", spawnPoint, 0f);

            // Spawn our barracks.
            Barracks = new Vehicle("BARRACKS", vehicleSpawnPoint);

            // Check if our aggressors exists and our truck, and if false return false
            if (!Aggressor1.Exists()) return false;
            if (!Aggressor2.Exists()) return false;
            if (!Aggressor3.Exists()) return false;
            if (!Aggressor4.Exists()) return false;
            if (!Barracks.Exists()) return false;

            // Warp in the vehicle driver and the occupants
            Aggressor1.WarpIntoVehicle(Barracks, -1);
            Aggressor2.WarpIntoVehicle(Barracks, -2);
            Aggressor3.WarpIntoVehicle(Barracks, -2);
            Aggressor4.WarpIntoVehicle(Barracks, -2);

            //  Set our minimum distance check and the circle that shows our location on the map.
            this.ShowCalloutAreaBlipBeforeAccepting(vehicleSpawnPoint, 30f);
            this.AddMinimumDistanceCheck(5f, Barracks.Position);

            // Give weapons to all of our aggressors
            Aggressor1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            Aggressor2.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            Aggressor3.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            Aggressor4.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);

            // Set our notifications
            this.CalloutMessage = "Pursuit of a Stolen Military Vehicle";
            this.CalloutPosition = vehicleSpawnPoint;

            // Play the audio.
            Functions.PlayScannerAudioUsingPosition("ATTENTION_01 OFFICERS_REPORT_01 POSSIBLE_TERRORIST UNITS_RESPOND_CODE_99_01", vehicleSpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            // Create the pursuit with the name pursuit.
            pursuit = Functions.CreatePursuit();

            //Add the aggressors to a pursuit
            Functions.AddPedToPursuit(this.pursuit, Aggressor1);
            Functions.AddPedToPursuit(this.pursuit, Aggressor2);
            Functions.AddPedToPursuit(this.pursuit, Aggressor3);
            Functions.AddPedToPursuit(this.pursuit, Aggressor4);

            // Make all of our peds fire at the player
            //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", Aggressor1, Game.LocalPlayer.Character, 0, 1);
            //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", Aggressor2, Game.LocalPlayer.Character, 0, 1);
            //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", Aggressor3, Game.LocalPlayer.Character, 0, 1);
            //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", Aggressor4, Game.LocalPlayer.Character, 0, 1);

            //Aggressor1.Tasks.FightAgainstClosestHatedTarget(500f);
            //Aggressor2.Tasks.FightAgainstClosestHatedTarget(500f);
            //Aggressor3.Tasks.FightAgainstClosestHatedTarget(500f);
            //Aggressor4.Tasks.FightAgainstClosestHatedTarget(500f);

            Aggressor1.RelationshipGroup = "RED";
            Aggressor2.RelationshipGroup = "RED";
            Aggressor3.RelationshipGroup = "RED";
            Aggressor4.RelationshipGroup = "RED";

            Game.SetRelationshipBetweenRelationshipGroups("BLUE", "RED", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("RED", "BLUE", Relationship.Hate);

            // Add blips to our aggressors
            ABlip1 = Aggressor1.AttachBlip();
            ABlip2 = Aggressor2.AttachBlip();
            ABlip3 = Aggressor3.AttachBlip();
            ABlip4 = Aggressor4.AttachBlip();

            // Request backup
            Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);

            // Shows the player to respond
            Game.DisplaySubtitle("Get to the ~r~scene~w~.", 6500);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            // If the player didn't accept the callout we delete all of our entities
            if (Aggressor1.Exists()) Aggressor1.Delete();
            if (Aggressor2.Exists()) Aggressor2.Delete();
            if (Aggressor3.Exists()) Aggressor3.Delete();
            if (Aggressor4.Exists()) Aggressor4.Delete();
            if (Barracks.Exists()) Barracks.Delete();
            if (ABlip1.Exists()) ABlip1.Delete();
            if (ABlip2.Exists()) ABlip2.Delete();
            if (ABlip3.Exists()) ABlip3.Delete();
            if (ABlip4.Exists()) ABlip4.Delete();

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            // If one of the peds dies then remove their blip
            if(Aggressor1.IsDead)
            {
                ABlip1.Delete();
            }

            if(Aggressor2.IsDead)
            {
                ABlip2.Delete();
            }

            if (Aggressor3.IsDead)
            {
                ABlip3.Delete();
            }

            if(Aggressor4.IsDead)
            {
                ABlip4.Delete();
            }

            // Checks if the pursuit is still running, and if it isn't call End().
            if (!Functions.IsPursuitStillRunning(pursuit))
            {
                this.End();
            }

            base.Process();
        }

        public override void End()
        {
            // Deletes the blips and dismisses the peds and the vehicle.
            if (ABlip1.Exists()) ABlip1.Delete();
            if (ABlip2.Exists()) ABlip2.Delete();
            if (ABlip3.Exists()) ABlip3.Delete();
            if (ABlip4.Exists()) ABlip4.Delete();
            if (Barracks.Exists()) Barracks.Dismiss();
            if (Aggressor1.Exists()) Aggressor1.Dismiss();
            if (Aggressor2.Exists()) Aggressor2.Dismiss();
            if (Aggressor3.Exists()) Aggressor3.Dismiss();
            if (Aggressor4.Exists()) Aggressor4.Dismiss();

            base.End();
        }
    }
}
