using Rage;
using Rage.Native;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RandomCallouts.Callouts
{
    [CalloutInfo("LooseLivestock", CalloutProbability.Low)]
    class LooseLivestock : Callout
    {
        Ped A1;
        Vector3 spawnPoint;
        Blip B1;
        EAnimalState state;

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
        /// Things to do when before the message is show to the player about the callout
        /// </summary>
        /// <returns></returns>
        public override bool OnBeforeCalloutDisplayed()
        {
            // Spawn points
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            //Ped A1 = new Ped("a_c_mtlion", (Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0, 1.8f, 0))), 0f);
            //A1.BlockPermanentEvents = true;
            //Game.LocalPlayer.Character.RelationshipGroup = "BLUE";
            //Game.SetRelationshipBetweenRelationshipGroups("BLUE", "RED", Relationship.Hate);
            //Game.SetRelationshipBetweenRelationshipGroups("RED", "BLUE", Relationship.Hate);
            //A1.RelationshipGroup = "RED";
            //A1.Inventory.GiveNewWeapon(WeaponHash.Cougar, 1, true);
            //NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A1, Game.LocalPlayer.Character, 0, 16);

            // Set our randomness
            int r = new Random().Next(1, 4);

            if (r == 1)
            {
                A1 = new Ped("a_c_cow", spawnPoint, 0f);
                state = EAnimalState.Cow;
            }
            else if (r == 2)
            {
                A1 = new Ped("a_c_coyote", spawnPoint, 0f);
                state = EAnimalState.Coyote;
            }
            else if (r == 3)
            {
                A1 = new Ped("a_c_mtlion", spawnPoint, 0f);
                state = EAnimalState.MtLion;
            }
            else if (r == 4)
            {
                A1 = new Ped("a_c_boar", spawnPoint, 0f);
                state = EAnimalState.Boar;
            }

            // Check if they spawned
            if (!A1.Exists()) return false;

            // Show the stuff
            this.ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 30f);
            this.AddMinimumDistanceCheck(10f, A1.Position);

            // Show the messages
            this.CalloutMessage = "Loose Animal";
            this.CalloutPosition = spawnPoint;

            // Play the audio
            Functions.PlayScannerAudioUsingPosition("WE_HAVE_01 VISCIOUS_ANIMAL_ON_THE_LOOSE UNITS_RESPOND_CODE_3", spawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        /// <summary>
        /// What to do when the callout is accepted
        /// </summary>
        /// <returns></returns>
        public override bool OnCalloutAccepted()
        {

            // Attach the blips
            B1 = A1.AttachBlip();

            // Shows the player to respond to the scene.
            Game.DisplaySubtitle("Get to the ~p~scene~w~.", 6500);
            B1.EnableRoute(Color.Purple);
            B1.Color = Color.Purple;

            //A1.Tasks.Wander();

            int r = new Random().Next(1, 3);
            A1.RelationshipGroup = "RED";
            Game.LocalPlayer.Character.RelationshipGroup = "BLUE";
            Game.SetRelationshipBetweenRelationshipGroups("RED", "BLUE", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("BLUE", "RED", Relationship.Hate);

            if (r == 1)
            {
                if (state == EAnimalState.Cow)
                {
                    A1.Inventory.GiveNewWeapon(WeaponHash.Cow, 1, true);
                }
                else if (state == EAnimalState.Coyote)
                {
                    A1.Inventory.GiveNewWeapon(WeaponHash.Coyote, 1, true);
                    A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                }
                else if (state == EAnimalState.MtLion)
                {
                    A1.Inventory.GiveNewWeapon(WeaponHash.Cougar, 1, true);
                    A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                }
                else if (state == EAnimalState.Boar)
                {
                    A1.Inventory.GiveNewWeapon(WeaponHash.Boar, 1, true);
                    A1.Tasks.FightAgainstClosestHatedTarget(1000f);
                }
            }
            else
            {
                A1.Tasks.Wander();
            }
            return base.OnCalloutAccepted();
        }

        /// <summary>
        /// What to do when the callout is not accepted
        /// </summary>
        public override void OnCalloutNotAccepted()
        {
            // If the player didn't accept the callout we cleanup this mess
            if (A1.Exists()) A1.Delete();
            if (B1.Exists()) B1.Delete();

            base.OnCalloutNotAccepted();
        }

        /// <summary>
        /// Where the main things happen...
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (A1.IsDead)
            {
                End();
            }

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

                        GameFiber.Wait(5000);
                        Game.DisplayHelp("You can end the callout by pressing ~b~" + EndCalloutKey + "~w~.");
                    }
                    //If there was an error reading the values, we set them to their defaults. We also let the user know via a notification.
                    catch
                    {
                        EndCalloutKey = Keys.End;
                        Game.DisplayNotification("There was an error reading the .ini file. Setting defaults...");
                    }

                    if (Game.IsKeyDown(EndCalloutKey))
                    {

                        

                        this.End();
                    }
                }
            }, "keyCheckerForLooseAnimal");
        }

        /// <summary>
        /// End the callout, cleanup
        /// </summary>
        public override void End()
        {
            // Deletes the blips and removes some of the stuff
            Game.DisplayNotification("~y~Loose Animal~w~ callout is ~g~Code 4~w~.");
            Functions.PlayScannerAudio("WE_ARE_CODE_4 NO_FURTHER_UNITS_REQUIRED");
            if (B1.Exists()) B1.Delete();
            if (A1.Exists()) A1.Dismiss();
            base.End();
        }
    }

    public enum EAnimalState
    {
        Cow,
        Coyote,
        MtLion,
        Boar
    }
}
