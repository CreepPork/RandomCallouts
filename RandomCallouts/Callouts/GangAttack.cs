using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;
using System;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("GangAttack", CalloutProbability.Low)]
    class GangAttack : Callout
    {
        // Declare our variables
        private Ped A1;
        private Ped A2;
        private Ped A3;
        private Ped A4;
        private Ped A5;
        private Ped A6;
        private Ped A7;
        private Ped A8;
        private Ped A9;
        private Ped A10;
        private Vector3 spawnPoint;
        private Blip B1;
        private Blip B2;
        private Blip B3;
        private Blip B4;
        private Blip B5;
        private Blip B6;
        private Blip B7;
        private Blip B8;
        private Blip B9;
        private Blip B10;
        private LHandle pursuit;
        private EGangAttackState state;

        public override bool OnBeforeCalloutDisplayed()
        {
            // Set our spawn point
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            // Spawn our Ballas peds
            A1 = new Ped("g_m_y_ballasout_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 20f), 0f);
            A2 = new Ped("g_m_y_ballasout_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 30f), 0f);
            A3 = new Ped("g_m_y_ballasout_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 22f), 0f);
            A4 = new Ped("g_m_y_ballasout_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 10f), 0f);
            A5 = new Ped("g_m_y_ballasout_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 18f), 0f);
            // Spawn our Grove peds
            A6 = new Ped("g_m_y_famca_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 24f), 0f);
            A7 = new Ped("g_m_y_famca_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 26f), 0f);
            A8 = new Ped("g_m_y_famca_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 8f), 0f);
            A9 = new Ped("g_m_y_famdnf_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 14f), 0f);
            A10 = new Ped("g_m_y_famdnf_01", Extensions.Vector3Extension.ExtensionAround(spawnPoint, 9f), 0f);

       
            // Check if our peds exist
            if (!A1.Exists()) return false;
            if (!A2.Exists()) return false;
            if (!A3.Exists()) return false;
            if (!A4.Exists()) return false;
            if (!A5.Exists()) return false;
            if (!A6.Exists()) return false;
            if (!A7.Exists()) return false;
            if (!A8.Exists()) return false;
            if (!A9.Exists()) return false;
            if (!A10.Exists()) return false;

            // Set our stuff
            this.ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 70f);
            this.AddMinimumDistanceCheck(10f, spawnPoint);

            // Give the weapons
            A1.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            A2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
            A3.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            A4.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
            A5.Inventory.GiveNewWeapon("WEAPON_SAWNOFFSHOTGUN", 5000, true);
            // Grove below
            A6.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            A7.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
            A8.Inventory.GiveNewWeapon("WEAPON_MICROSMG", 5000, true);
            A9.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
            A10.Inventory.GiveNewWeapon("WEAPON_SAWNOFFSHOTGUN", 5000, true);

            // Set our notifications
            this.CalloutMessage = "Gang Shootout";
            this.CalloutPosition = spawnPoint;

            // Play the audio.
            Functions.PlayScannerAudioUsingPosition("ATTENTION_01 CITIZENS_REPORT_01 GANG_RELATED_VIOLENCE UNITS_RESPOND_CODE_99_01", spawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            // Set the player as responding to the call.
            state = EGangAttackState.EnRoute;

            // Attach a blip to our peds and so the player knows where to go.
            B1 = A1.AttachBlip();
            B2 = A2.AttachBlip();
            B3 = A3.AttachBlip();
            B4 = A4.AttachBlip();
            B5 = A5.AttachBlip();
            B6 = A6.AttachBlip();
            B7 = A7.AttachBlip();
            B8 = A8.AttachBlip();
            B9 = A9.AttachBlip();
            B10 = A10.AttachBlip();

            // Display the message
            Game.DisplaySubtitle("Get to the ~r~scene~w~.", 6500);
            B1.EnableRoute(Color.Red);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            // Clean up!
            try
            {
	            if (B1.Exists()) B1.Delete();
	            if (B2.Exists()) B2.Delete();
	            if (B3.Exists()) B3.Delete();
	            if (B4.Exists()) B4.Delete();
	            if (B5.Exists()) B5.Delete();
	            if (B6.Exists()) B6.Delete();
	            if (B7.Exists()) B7.Delete();
	            if (B8.Exists()) B8.Delete();
	            if (B9.Exists()) B9.Delete();
	            if (B10.Exists()) B10.Delete();
	
	            if (A1.Exists()) A1.Delete();
	            if (A2.Exists()) A2.Delete();
	            if (A3.Exists()) A3.Delete();
	            if (A4.Exists()) A4.Delete();
	            if (A5.Exists()) A5.Delete();
	            if (A6.Exists()) A6.Delete();
	            if (A7.Exists()) A7.Delete();
	            if (A8.Exists()) A8.Delete();
	            if (A9.Exists()) A9.Delete();
	            if (A10.Exists()) A10.Delete();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("An exception occurred in the Gang Attack callout, please contact CreepPork_LV and send this log. Error occurred in the OnCalloutNotAccepted() part. Detailed error is: " + ex);
            }

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            // Remove the dead people blips
            if (A1.IsDead)
            {
                if (B1.Exists())B1.Delete();
            }
            if (A2.IsDead)
            {
                if (B2.Exists()) B2.Delete();
            }
            if (A3.IsDead)
            {
                if (B3.Exists()) B3.Delete();
            }
            if (A4.IsDead)
            {
                if (B4.Exists()) B4.Delete();
            }
            if (A5.IsDead)
            {
                if (B5.Exists()) B5.Delete();
            }
            if (A6.IsDead)
            {
                if (B6.Exists()) B6.Delete();
            }
            if (A7.IsDead)
            {
                if (B7.Exists()) B7.Delete();
            }
            if (A8.IsDead)
            {
                if (B8.Exists()) B8.Delete();
            }
            if (A9.IsDead)
            {
                if (B9.Exists()) B9.Delete();
            }
            if (A10.IsDead)
            {
                if (B10.Exists()) B10.Delete();
            }


            if (A1.IsCuffed)
            {
                if (B1.Exists()) B1.Delete();
            }
            if (A2.IsCuffed)
            {
                if (B2.Exists()) B2.Delete();
            }
            if (A3.IsCuffed)
            {
                if (B3.Exists()) B3.Delete();
            }
            if (A4.IsCuffed)
            {
                if (B4.Exists()) B4.Delete();
            }
            if (A5.IsCuffed)
            {
                if (B5.Exists()) B5.Delete();
            }
            if (A6.IsCuffed)
            {
                if (B6.Exists()) B6.Delete();
            }
            if (A7.IsCuffed)
            {
                if (B7.Exists()) B7.Delete();
            }
            if (A8.IsCuffed)
            {
                if (B8.Exists()) B8.Delete();
            }
            if (A9.IsCuffed)
            {
                if (B9.Exists()) B9.Delete();
            }
            if (A10.IsCuffed)
            {
                if (B10.Exists()) B10.Delete();
            }

            // If the player is driving to the scene, and their distance to the scene is less than 15, start the callout's logic
            if (state == EGangAttackState.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(spawnPoint) <= 50f)
            {
                // Set the player as on scene
                state = EGangAttackState.OnScene;

                // Start the callout's logic
                StartGangAttackScenario();

            }
            if (state == EGangAttackState.DecisionMade && !Functions.IsPursuitStillRunning(pursuit) || A1.IsDead && A2.IsDead && A3.IsDead && A4.IsDead && A5.IsDead && A6.IsDead && A7.IsDead && A8.IsDead && A9.IsDead && A10.IsDead)
            {
                this.End();
            }
        }

        public override void End()
        {
            try
            {
	            if (B1.Exists()) B1.Delete();
	            if (B2.Exists()) B2.Delete();
	            if (B3.Exists()) B3.Delete();
	            if (B4.Exists()) B4.Delete();
	            if (B5.Exists()) B5.Delete();
	            if (B6.Exists()) B6.Delete();
	            if (B7.Exists()) B7.Delete();
	            if (B8.Exists()) B8.Delete();
	            if (B9.Exists()) B9.Delete();
	            if (B10.Exists()) B10.Delete();
	
	            if (A1.Exists()) A1.Dismiss();
	            if (A2.Exists()) A2.Dismiss();
	            if (A3.Exists()) A3.Dismiss();
	            if (A4.Exists()) A4.Dismiss();
	            if (A5.Exists()) A5.Dismiss();
	            if (A6.Exists()) A6.Dismiss();
	            if (A7.Exists()) A7.Dismiss();
	            if (A8.Exists()) A8.Dismiss();
	            if (A9.Exists()) A9.Dismiss();
	            if (A10.Exists()) A10.Dismiss();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("An exception occurred in the Gang Attack callout, in the End(). Send your log to CreepPork_LV. Detailed error is: " + ex);
            }

            base.End();
        }

        public void StartGangAttackScenario()
        {
            GameFiber.StartNew(delegate
            {
                // Create our pursuit
                pursuit = Functions.CreatePursuit();

                // Set the state to decision made
                state = EGangAttackState.DecisionMade;

                // Make everybody point their guns
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A1, A6, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A2, A7, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A3, A8, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A4, A9, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A5, A10, -1, true);

                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A6, A1, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A7, A2, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A8, A3, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A9, A4, -1, true);
                NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", A10, A5, -1, true);

                // Request backup
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);

                new RelationshipGroup("BALLAS");
                new RelationshipGroup("GROVE");

                A1.RelationshipGroup = "BALLAS";
                A2.RelationshipGroup = "BALLAS";
                A3.RelationshipGroup = "BALLAS";
                A4.RelationshipGroup = "BALLAS";
                A5.RelationshipGroup = "BALLAS";

                A6.RelationshipGroup = "GROVE";
                A7.RelationshipGroup = "GROVE";
                A8.RelationshipGroup = "GROVE";
                A9.RelationshipGroup = "GROVE";
                A10.RelationshipGroup = "GROVE";

                Game.SetRelationshipBetweenRelationshipGroups("BALLAS", "GROVE", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("GROVE", "BALLAS", Relationship.Hate);

                Game.SetRelationshipBetweenRelationshipGroups("BALLAS", "BLUE", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("BLUE", "BALLAS", Relationship.Hate);

                Game.SetRelationshipBetweenRelationshipGroups("GROVE", "BLUE", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("BLUE", "GROVE", Relationship.Hate);

                A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                A2.Tasks.FightAgainstClosestHatedTarget(1000f);
                A3.Tasks.FightAgainstClosestHatedTarget(1000f);
                A4.Tasks.FightAgainstClosestHatedTarget(1000f);
                A5.Tasks.FightAgainstClosestHatedTarget(1000f);
                A6.Tasks.FightAgainstClosestHatedTarget(1000f);
                A7.Tasks.FightAgainstClosestHatedTarget(1000f);
                A8.Tasks.FightAgainstClosestHatedTarget(1000f);
                A9.Tasks.FightAgainstClosestHatedTarget(1000f);
                A10.Tasks.FightAgainstClosestHatedTarget(1000f);

                //// Start one of the random outcomes
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A1, A6, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A2, Game.LocalPlayer.Character, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A3, A8, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A4, A9, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A5, Game.LocalPlayer.Character, 0, 1);
                //// Grove st.
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A6, A1, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A7, Game.LocalPlayer.Character, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A8, A3, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A9, A4, 0, 1);
                //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A10, Game.LocalPlayer.Character, 0, 1);

                // Wait 20 seconds before adding 5 people to the pursuit the other ones will continue shooting at the player or the other gangsters
                GameFiber.Sleep(25000);

                if (A1.IsDead)
                {
                    B1.Delete();
                }
                if (A2.IsDead)
                {
                    B2.Delete();
                }
                if (A3.IsDead)
                {
                    B3.Delete();
                }
                if (A4.IsDead)
                {
                    B4.Delete();
                }
                if (A5.IsDead)
                {
                    B5.Delete();
                }
                if (A6.IsDead)
                {
                    B6.Delete();
                }
                if (A7.IsDead)
                {
                    B7.Delete();
                }
                if (A8.IsDead)
                {
                    B8.Delete();
                }
                if (A9.IsDead)
                {
                    B9.Delete();
                }
                if (A10.IsDead)
                {
                    B10.Delete();
                }

                //Functions.AddPedToPursuit(pursuit, A1);
                //Functions.AddPedToPursuit(pursuit, A3);
                //Functions.AddPedToPursuit(pursuit, A4);
                //Functions.AddPedToPursuit(pursuit, A7);
                //Functions.AddPedToPursuit(pursuit, A9);

                Game.LogTrivialDebug("Adding peds to pursuit.");
                if (A1.IsAlive) Functions.AddPedToPursuit(pursuit, A1);
                if (A2.IsAlive) Functions.AddPedToPursuit(pursuit, A2);
                if (A3.IsAlive) Functions.AddPedToPursuit(pursuit, A3);
                if (A4.IsAlive) Functions.AddPedToPursuit(pursuit, A4);
                if (A5.IsAlive) Functions.AddPedToPursuit(pursuit, A5);
                if (A6.IsAlive) Functions.AddPedToPursuit(pursuit, A6);
                if (A7.IsAlive) Functions.AddPedToPursuit(pursuit, A7);
                if (A8.IsAlive) Functions.AddPedToPursuit(pursuit, A8);
                if (A9.IsAlive) Functions.AddPedToPursuit(pursuit, A9);
                if (A10.IsAlive) Functions.AddPedToPursuit(pursuit, A10);
                Game.LogTrivialDebug("Added peds to pursuit.");
            });
        }

    }

    public enum EGangAttackState
    {
        EnRoute,
        OnScene,
        DecisionMade
    }

}
