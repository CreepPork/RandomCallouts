using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RandomCallouts.Callouts
{
    //Name the callout, and set the probability.
    [CalloutInfo("PublicDisorder", CalloutProbability.Medium)]
    //Inherit the Callout class, since we're making a callout.
    public class PublicDisorder : Callout
    {
        /// <summary>
        /// This callout waits until the player is on scene to start, which is the purpose of EMuggingState, so we know when to start running the callout's logic.
        /// </summary>
        private EDisorderState state;
        private LHandle pursuit;
        private Vector3 spawnPoint;
        private Blip B1;
        private Ped A1;
        private EGenderState genderState;

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
        /// Called before the callout is displayed. Do all spawning here, so that if spawning isn't successful, the player won't notice, as the callout won't be shown.
        /// </summary>
        /// <returns></returns>
        public override bool OnBeforeCalloutDisplayed()
        {
            //Get a valid spawn point for the callout, and spawn the Aggressor there
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            int r = new Random().Next(1, 4);

            if (r == 1)
            {
                A1 = new Ped("a_f_y_topless_01", spawnPoint, 0f);
                genderState = EGenderState.FemaleState;
            }
            else if (r == 2)
            {
                A1 = new Ped("u_m_y_militarybum", spawnPoint, 0f);
                genderState = EGenderState.MaleState;
            }
            else if (r == 3)
            {
                A1 = new Ped(spawnPoint);
            }

            //If for some reason, the spawning of either two peds failed, don't display the callout
            if (!A1.Exists()) return false;

            //If the peds are valid, display the area that the callout is in.
            this.ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 15f);
            this.AddMinimumDistanceCheck(5f, spawnPoint);

            //Set the callout message(displayed in the notification), and the position(also shown in the notification)
            this.CalloutMessage = "Public Disorder";
            this.CalloutPosition = spawnPoint;

            int r1 = new Random().Next(1,3);
            //Play the scanner audio.
            if (r1 == 1)
            {
                Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 A_DISTURBANCE UNITS_RESPOND_CODE_02", spawnPoint);
            }
            else
            {
                Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_02 PUBLIC_INTOX UNITS_RESPOND_CODE_02", spawnPoint);
            }

            return base.OnBeforeCalloutDisplayed();
        }

        /// <summary>
        /// Called when the player accepts the callout
        /// </summary>
        /// <returns></returns>
        public override bool OnCalloutAccepted()
        {
            //Set the player as en route to the scene
            state = EDisorderState.EnRoute;

            //Attach a blip to the Aggressor, so the player knows where to go, and can find the aggressor if he flees
            B1 = A1.AttachBlip();

            A1.BlockPermanentEvents = true;

            //Display a message to let the user know that the callout was accepted.
            Game.DisplaySubtitle("Get to the ~y~scene~w~.", 6500);
            B1.EnableRoute(Color.Yellow);
            B1.Color = Color.Yellow;

            // Make the ped walk as drunk
            //AnimationSet animSet = new AnimationSet("move_m@drunk@verydrunk");
            //animSet.LoadAndWait();
            //A1.MovementAnimationSet = animSet;

            // Playing an animation
            // The list of hyper links on the Rage Anim list page is a list of Anim Dictionaries
            // Each one has a set of animation names underneath it
            // "amb@world_human_bum_standing@drunk@idle_a" is the dictionary name
            // And "idle_a" is the anim name
            // I'm not exactly sure how blend speed (not anim speed, but apparently its the speed that the anim blends in) affects anything, but I just set it to 1
            // Anim flags tells GTA to loop the anim, or ragdoll the ped when something hits it, etc.
            // The object browser or Rage docs can tell you all the AnimationFlags
            //A1.Tasks.PlayAnimation(new AnimationDictionary("amb@world_human_bum_standing@drunk@idle_a"), "idle_a", 1f, AnimationFlags.RagdollOnCollision);

            if (genderState == EGenderState.FemaleState && A1.IsFemale)
            {
                A1.Tasks.PlayAnimation(new AnimationDictionary("amb@world_human_partying@female@partying_beer@base"), "base", 1f, AnimationFlags.Loop);
            }
            else if (genderState == EGenderState.MaleState && A1.IsMale)
            {
                AnimationSet animSet = new AnimationSet("move_m@drunk@verydrunk");
                animSet.LoadAndWait();
                A1.MovementAnimationSet = animSet;
                A1.Tasks.PlayAnimation(new AnimationDictionary("amb@world_human_bum_standing@drunk@idle_a"), "idle_a", 1f, AnimationFlags.Loop);
            }

            return base.OnCalloutAccepted();
        }

        /// <summary>
        /// Called if the player ignores the callout
        /// </summary>
        public override void OnCalloutNotAccepted()
        {
            //Clean up what we spawned earlier, since the player didn't accept the callout.
            if (A1.Exists()) A1.Delete();
            if (B1.Exists()) B1.Delete();
            base.OnCalloutNotAccepted();
        }

        /// <summary>
        /// All callout logic should be done here.
        /// </summary>
        public override void Process()
        {
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

                        Game.DisplayNotification("The ~y~Public Disorder~w~ is ~g~Code 4~w~.");
                        Functions.PlayScannerAudio("WE_ARE_CODE_4 NO_FURTHER_UNITS_REQUIRED");
                        A1.Dismiss();

                        this.End();
                    }
                }
            }, "keyCheckerForPublicDisorder");

            if (A1.IsDead)
            {
                End();
            }

            //If the player is driving to the scene, and their distance to the scene is less than 15, start the callout's logic.
            if (state == EDisorderState.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(spawnPoint) <= 15)
            {
                //Set the player as on scene
                state = EDisorderState.OnScene;

                //Start the callout's logic. You can paste the logic from StartMuggingScenario straight into here, but I don't, since I like it to look clean, and place any long methods towards the bottom of the class.
                StartDisorderLogic();
            }

            //If the state is DecisionMade(The aggressor already decided what random outcome to execute), and the pursuit isn't running anymore, end the callout.
            if (state == EDisorderState.DecisionMade && !Functions.IsPursuitStillRunning(pursuit))
            {
                this.End();
            }
        }

        /// <summary>
        /// Called when the callout ends
        /// </summary>
        public override void End()
        {
            //Dismiss the aggressor and victim, so they can be deleted by the game once the player leaves the scene.

            //Delete the blip attached to the aggressor
            if (B1.Exists()) B1.Delete();
            if (A1.Exists()) A1.Dismiss();
            base.End();
        }

        /// <summary>
        /// The method that contains the callout's logic
        /// </summary>
        public void StartDisorderLogic()
        {
            //ALWAYS START A NEW GAME FIBER IF YOU'RE GOING TO USE GameFiber.Sleep, DON'T SLEEP THE MAIN FIBER.
            GameFiber.StartNew(delegate
            {
                //Create the pursuit
                pursuit = Functions.CreatePursuit();

                //Set the state to decision made, since the outcome is chosen.
                state = EDisorderState.DecisionMade;

                int r = new Random().Next(1, 3);

                //Execute one of the random outcomes
                if (r == 1)
                {
                    if (new Random().Next(1, 3) == 2)
                    {
                        //The aggressor attacks the player.
                        NativeFunction.CallByName<uint>("TASK_COMBAT_PED", A1, Game.LocalPlayer.Character, 0, 1);

                        //We wait 4.5 seconds before adding the ped to a pursuit, since as soon as we add the aggressor to a pursuit, LSPDFR takes over the AI, and they won't attack the player anymore. They'll flee instead.
                        GameFiber.Sleep(4500);
                    }

                    //Dismiss the aggressor from our plugin
                    A1.Dismiss();

                    //Add the aggressor to a pursuit
                    Functions.AddPedToPursuit(pursuit, A1);

                    var persona = Functions.GetPersonaForPed(A1);
                    Functions.SetPersonaForPed(A1, new Persona(A1, persona.Gender, persona.BirthDay, persona.Citations, persona.Forename, persona.Surname, persona.LicenseState, persona.TimesStopped, true, false, false));

                    //Dispatch a backup unit.
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
                }
                else if (r == 2)
                {
                    GameFiber.StartNew(delegate
                    {
                        GameFiber.Wait(5000);

                        Game.DisplayHelp("You can End the callout by pressing your End Callout key.");

                    }, "displayNotificationPublicDisorder");
                }
            });
        }
    }

    /// <summary>
    /// Mugging states
    /// </summary>
    public enum EDisorderState
    {
        EnRoute,
        OnScene,
        DecisionMade
    }
    public enum EGenderState
    {
        MaleState,
        FemaleState
    }
}
