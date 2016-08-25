using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("StolenArmoredCar", CalloutProbability.Low)]
    class StolenArmoredCar : Callout
    {
        private Ped A1;
        private Ped A2;
        private Ped A3;
        private Ped A4;
        private Blip B1;
        private Blip B2;
        private Blip B3;
        private Blip B4;
        private Vehicle ArmoredCar;
        private Vector3 spawnPoint;
        private LHandle pursuit;
        //private int r = new Random().Next(1, 3);

        /// <summary>
        /// In here you put in all of the things you want to be happening before the callout is displayed to the player.
        /// </summary>
        /// <returns></returns>
        public override bool OnBeforeCalloutDisplayed()
        {
            try
            {
                spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

                ArmoredCar = new Vehicle("STOCKADE", spawnPoint);

                A1 = new Ped("s_m_m_armoured_02", spawnPoint, 0f);
                A2 = new Ped("s_m_m_armoured_01", spawnPoint, 0f);
                A3 = new Ped("s_m_m_armoured_01", spawnPoint, 0f);
                A4 = new Ped("s_m_m_armoured_02", spawnPoint, 0f);


                if (!ArmoredCar.Exists()) return false;
                if (!A1.Exists()) return false;
                if (!A2.Exists()) return false;
                if (!A3.Exists()) return false;
                if (!A4.Exists()) return false;

                A1.WarpIntoVehicle(ArmoredCar, -1);
                A2.WarpIntoVehicle(ArmoredCar, 0);
                A3.WarpIntoVehicle(ArmoredCar, 1);
                A4.WarpIntoVehicle(ArmoredCar, 2);

                A1.Armor = 100;
                A2.Armor = 100;
                A3.Armor = 100;
                A4.Armor = 100;

                A1.Inventory.GiveNewWeapon(WeaponHash.Pistol, 5000, false);
                A2.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, 5000, false);
                A3.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 5000, true);
                A4.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, 5000, true);

                ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 50f);
                AddMinimumDistanceCheck(50f, ArmoredCar.Position);

                CalloutMessage = "Stolen Armored Vehicle";
                CalloutPosition = ArmoredCar.Position;

                Functions.PlayScannerAudioUsingPosition("ATTENTION_THIS_IS_DISPATCH WE_HAVE_02 BANK_CAR INTRO_01 CRIME_GUNFIRE_03 UNITS_RESPOND_CODE_99_01", spawnPoint);
            }
            catch (Exception ex)
            {
                Game.LogTrivial("Random Callouts, Stolen Armored Car callout failed to initialize in the OnBeforeCalloutDisplayed method. Send the log to the developer, CreepPork_LV. A detailed message is available: " + ex);
            }

            return base.OnBeforeCalloutDisplayed();
        }

        /// <summary>
        /// In here you put in all of the things you want to do if the player accepts this callout.
        /// </summary>
        /// <returns></returns>
        public override bool OnCalloutAccepted()
        {
            try
            {
                B1 = A1.AttachBlip();
                B2 = A2.AttachBlip();
                B3 = A3.AttachBlip();
                B4 = A4.AttachBlip();

                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, A1);
                Functions.AddPedToPursuit(pursuit, A2);
                Functions.AddPedToPursuit(pursuit, A3);
                Functions.AddPedToPursuit(pursuit, A4);

                Game.DisplaySubtitle("Get to the ~r~pursuit~w~.", 6500);

                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.SwatTeam);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.NooseTeam);

                Game.LogTrivialDebug("A2 Is being registered");
                A2.RelationshipGroup = "RED";
                Game.LogTrivialDebug("A2 has been registered");
                Game.LogTrivialDebug("A3 is being registered");
                A3.RelationshipGroup = "RED";
                Game.LogTrivialDebug("A3 has been registered");
                Game.LogTrivialDebug("A4 is being registered");
                A4.RelationshipGroup = "RED";
                Game.LogTrivialDebug("A4 has been registered");

                Game.LocalPlayer.Character.RelationshipGroup = "BLUE";

                Game.SetRelationshipBetweenRelationshipGroups("BLUE", "RED", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("RED", "BLUE", Relationship.Hate);

                A2.Tasks.FightAgainstClosestHatedTarget(1000f);
                A3.Tasks.FightAgainstClosestHatedTarget(1000f);
                A4.Tasks.FightAgainstClosestHatedTarget(1000f);

                //if (A3.Exists() && A4.Exists())
                //{
                //    //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A3, Game.LocalPlayer.Character, 0, 1);
                //    //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A4, Game.LocalPlayer.Character, 0, 1);
                //    A3.RelationshipGroup = "RED";
                //    A4.RelationshipGroup = "RED";

                //    Game.LocalPlayer.Character.RelationshipGroup = "BLUE";

                //    Game.SetRelationshipBetweenRelationshipGroups("BLUE", "RED", Relationship.Hate);
                //    Game.SetRelationshipBetweenRelationshipGroups("RED", "BLUE", Relationship.Hate);
                //    Game.LogVerboseDebug("[DEBUG] A3 and A4 code has been executed");
                //}
            }
            catch (Exception ex)
            {
                Game.LogTrivial("Random Callouts, Stolen Armored Car callout failed to initialize in the OnCalloutAccepted method. Send the log to the developer, CreepPork_LV. A detailed message is available: " + ex);
            }

            return base.OnCalloutAccepted();
        }

        /// <summary>
        /// In here you put in all of the things you want to do if the player doesn't accept the callout.
        /// </summary>
        public override void OnCalloutNotAccepted()
        {
            try
            {
                if (A1.Exists()) A1.Delete();
                if (A2.Exists()) A2.Delete();
                if (A3.Exists()) A3.Delete();
                if (A4.Exists()) A4.Delete();
                if (ArmoredCar.Exists()) ArmoredCar.Delete();
                if (B1.Exists()) B1.Delete();
                if (B2.Exists()) B2.Delete();
                if (B3.Exists()) B3.Delete();
                if (B4.Exists()) B4.Delete();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("Random Callouts, Stolen Armored Car callout failed to initialize in the OnCalloutNotAccepted method. Send the log to the developer, CreepPork_LV. A detailed message is available: " + ex);
            }

            base.OnCalloutNotAccepted();
        }

        /// <summary>
        /// In here you put in all of the things you want to do while the callout is running (after the OnCalloutAccepted has been completed) (Beware this Process runs in a loop until you tell it when to end!).
        /// </summary>
        public override void Process()
        {
            try
            {
                if (A1.IsDead)
                {
                    if (B1.Exists()) B1.Delete();
                }
                else if (A2.IsDead)
                {
                    if (B2.Exists()) B2.Delete();
                }
                else if (A3.IsDead)
                {
                    if (B3.Exists()) B3.Delete();
                }
                else if (A4.IsDead)
                {
                    if (B4.Exists()) B4.Delete();
                }

                if (!Functions.IsPursuitStillRunning(pursuit))
                {
                    End();
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("Random Callouts, Stolen Armored Car callout failed to initialize in the Process method. Send the log to the developer, CreepPork_LV. A detailed message is available: " + ex);
            }

            base.Process();
        }

        /// <summary>
        /// In here you put all of the things you want to do when the callout ends.
        /// </summary>
        public override void End()
        {
            try
            {
                if (ArmoredCar.Exists()) ArmoredCar.Dismiss();
                if (A1.Exists()) A1.Dismiss();
                if (A2.Exists()) A2.Dismiss();
                if (A3.Exists()) A3.Dismiss();
                if (A4.Exists()) A4.Dismiss();
                if (B1.Exists()) B1.Delete();
                if (B2.Exists()) B2.Delete();
                if (B3.Exists()) B3.Delete();
                if (B4.Exists()) B4.Delete();
            }
            catch (Exception ex)
            {
                Game.LogTrivial("Random Callouts, Stolen Armored Car callout failed to initialize in the End method. Send the log to the developer, CreepPork_LV. A detailed message is available: " + ex);
            }

            base.End();
        }
    }
}
