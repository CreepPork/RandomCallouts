using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using System.Windows.Forms;
using System;
using Rage.Native;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("CrashedCar", CalloutProbability.Medium)]
    class CrashedCar : Callout
    {
        Vehicle car;
        Ped V1;
        Blip B1;
        Vector3 spawnPoint;
        int r = new Random().Next(1, 6);

        public static InitializationFile initialiseFile()
        {
            //InitializationFile is a Rage class.
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

        /// <summary>
        /// What to do before the callout can be accepted or not.
        /// </summary>
        /// <returns></returns>
        public override bool OnBeforeCalloutDisplayed()
        {
            // Spawn points
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            // Spawn peds
            V1 = new Ped(spawnPoint);

            // Spawn car
            if (r == 1)
            {
                car = new Vehicle("ASEA", spawnPoint);
            }
            else if (r == 2)
            {
                car = new Vehicle("ASTEROPE", spawnPoint);
            }
            else if (r == 3)
            {
                car = new Vehicle("FUGITIVE", spawnPoint);
            }
            else if (r == 4)
            {
                car = new Vehicle("INGOT", spawnPoint);
            }
            else if (r == 5)
            {
                car = new Vehicle("TAILGATER", spawnPoint);
            }

            // Check if they spawned
            if (!V1.Exists()) return false;
            if (!car.Exists()) return false;

            V1.WarpIntoVehicle(car, -1);

            // Show the stuff
            ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 30f);
            AddMinimumDistanceCheck(10f, V1.Position);

            // Show the messages
            CalloutMessage = "Motor Vehicle Accident";
            CalloutPosition = spawnPoint;

            // Play the audio
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_01 MOTOR_VEHICLE_ACCIDENT UNITS_RESPOND_CODE_03", spawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        /// <summary>
        /// What to do if the player accepts the callout
        /// </summary>
        /// <returns></returns>
        public override bool OnCalloutAccepted()
        {
            int r1 = new Random().Next(1, 3);

            // Attach the blips
            B1 = V1.AttachBlip();

            B1.Color = Color.Red;

            // Shows the player to respond to the scene.
            Game.DisplaySubtitle("Get to the ~r~scene~w~.", 6500);
            B1.EnableRoute(Color.Red);

            // Wait 5 seconds and then display the notification
            GameFiber.StartNew(delegate
            {
                GameFiber.Wait(5000);

                Game.DisplayHelp("To end the callout press ~b~End~w~.");

            }, "waitingForNotification");


            // If the random is 1 then kill the driver if not keep him/her alive.

            V1.Kill();
            V1.MakePersistent();

            // Damage the vehicle so it looks better and like a crash
            car.MakePersistent();

            //NativeFunction.CallByName<uint>("SET_VEHICLE_DAMAGE", car, 1f, 1f, 1f, 1f, 30f, 15f, true);
            car.Deform(car.Position, 20f, 30f);
            car.EngineHealth = 50f;
            car.FuelTankHealth = 650f;
            car.DirtLevel = 3;
            car.AlarmTimeLeft = new TimeSpan(0, 20, 0);
            car.SetRotationRoll(180f);

            NativeFunction.CallByName<uint>("SMASH_VEHICLE_WINDOW", car, 0);
            NativeFunction.CallByName<uint>("SMASH_VEHICLE_WINDOW", car, 1);
            NativeFunction.CallByName<uint>("SMASH_VEHICLE_WINDOW", car, -1);
            NativeFunction.CallByName<uint>("SET_VEHICLE_DOOR_OPEN", car, 5, true, true);
            NativeFunction.CallByName<uint>("SET_VEHICLE_DOOR_OPEN", car, 3, true, true);
            if (r1 == 1)
            {
                NativeFunction.CallByName<uint>("SET_VEHICLE_TYRE_BURST", car, 0, true, 1000);
                NativeFunction.CallByName<uint>("SET_VEHICLE_DOOR_BROKEN", car, 1, true);
                NativeFunction.CallByName<uint>("SET_VEHICLE_DOOR_BROKEN", car, 2, true);
            }

            return base.OnCalloutAccepted();
        }

        /// <summary>
        /// What to do when the player declines the callout.
        /// </summary>
        public override void OnCalloutNotAccepted()
        {
            // If the player didn't accept the callout we cleanup this mess
            if (V1.Exists()) V1.Delete();
            if (B1.Exists()) B1.Delete();
            if (car.Exists()) car.Delete();

            base.OnCalloutNotAccepted();
        }

        /// <summary>
        /// What to do when the callout is in progress.
        /// </summary>
        public override void Process()
        {
            // Check if the pursuit is still running and if it isn't then end the callout

            base.Process();

            GameFiber.StartNew(delegate
            {
                {

                    //A keys converter is used to convert a string to a key.
                    KeysConverter kc = new KeysConverter();

                    //We create two variables: one is a System.Windows.Keys, the other is a string.
                    Keys EndCalloutKey;


                    //Use a try/catch, because reading values from files is risky: we can never be sure what we're going to get and we don't want our plugin to crash.
                    try
                    {
                        //We assign myKeyBinding the value of the string read by the method getMyKeyBinding(). We then use the kc.ConvertFromString method to convert this to a key.
                        //If the string does not represent a valid key (see .ini file for a link) an exception is thrown. That's why we need a try/catch.
                        EndCalloutKey = (Keys)kc.ConvertFromString(getEndKey());
                    }
                    //If there was an error reading the values, we set them to their defaults. We also let the user know via a notification.
                    catch
                    {
                        EndCalloutKey = Keys.End;
                        Game.DisplayNotification("There was an error reading the .ini file. Setting defaults...");
                    }

                    if (Game.IsKeyDown(EndCalloutKey))
                    {
                        try
                        {
	                        V1.IsPersistent = false;
	                        car.IsPersistent = false;
	
	                        Game.DisplayNotification("~r~Motor Vehicle Accident~w~ is ~g~Code 4~w~.");
	                        Functions.PlayScannerAudio("WE_ARE_CODE_4 NO_FURTHER_UNITS_REQUIRED");
	
	                        this.End();
                        }
                        catch (Exception ex)
                        {
                            Game.LogTrivial("Failed to end the callout. Error is: " + ex);
                        }
                    }
                }
            }, "keyCheckerForCarCrashCallout");
        }

        /// <summary>
        /// What to do when the callout is stopped.
        /// </summary>
        public override void End()
        {
            // Deletes the blips and removes some of the stuff
            if (B1.Exists()) B1.Delete();
            if (car.Exists()) car.Dismiss();
            if (V1.Exists()) V1.Dismiss();

            base.End();
        }
    }
}
