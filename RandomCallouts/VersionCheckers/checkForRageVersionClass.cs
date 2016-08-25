using System;
using System.IO;
using Rage;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace RandomCallouts.VersionCheckers
{
    /// <summary>
    /// Class for checking the user's RPH version.
    /// </summary>
    class checkForRageVersionClass
    {
        private static bool correctVersion;

        /// <summary>
        /// Checks whether the person has the specified minimum version or higher. 
        /// </summary>
        /// <param name="minimumVersion">Provide in the format of a float i.e.: 0.22</param>
        /// <returns></returns>
        public static bool checkForRageVersion(float minimumVersion)
        {

            var versionInfo = FileVersionInfo.GetVersionInfo("RAGEPluginHook.exe");
            float Rageversion;
            try
            {
                //If you decide to use this in your plugin, I would appreciate some credit :)
                Rageversion = float.Parse(versionInfo.ProductVersion.Substring(0, 4), CultureInfo.InvariantCulture);
                Game.LogTrivial("RandomCallouts.VersionChecker detected RAGEPluginHook version: " + Rageversion.ToString());

                //If user's RPH version is older than the minimum
                if (Rageversion < minimumVersion)
                {
                    correctVersion = false;
                    GameFiber.StartNew(delegate
                    {
                        while (Game.IsLoading)
                        {
                            GameFiber.Yield();
                        }
                        //If you decide to use this in your plugin, I would appreciate some credit :)
                        Game.DisplayNotification("RAGEPluginHook ~r~v" + Rageversion.ToString() + " ~s~detected. ~b~RandomCallouts.VersionChecker ~s~requires ~b~v" + minimumVersion.ToString() + " ~s~or higher.");
                        GameFiber.Sleep(5000);
                        Game.LogTrivial("RAGEPluginHook version " + Rageversion.ToString() + " detected. RandomCallouts.VersionChecker requires v" + minimumVersion.ToString() + " or higher.");
                        Game.LogTrivial("Preparing redirect...");
                        Game.DisplayNotification("You are being redirected to the RAGEPluginHook website so you can download the latest version.");
                        Game.DisplayNotification("Press Backspace to cancel the redirect.");

                        int count = 0;
                        while (true)
                        {
                            GameFiber.Sleep(10);
                            count++;
                            if (Game.IsKeyDownRightNow(Keys.Back))
                            {
                                Game.DisplayNotification("You have canceled the redirect.");
                                Game.LogTrivial("Redirection canceled.");
                                break;
                            }
                            if (count >= 300)
                            {
                                //URL to the RPH download page.
                                //I use bit.ly to track the number of times this is called: at the moment, it has been called 327 times over the past 2 days! What a time saver for me.
                                Process.Start("http://bit.ly/RPHDownload");
                                break;
                            }
                        }

                    }, "checkForRageVersionClass");
                }
                //If user's RPH version is (above) the specified minimum
                else
                {
                    correctVersion = true;
                }
            }
            catch (Exception e)
            {
                //If for whatever reason the version couldn't be found.
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("Unable to detect your RAGE Plugin Hook installation.");
                if (File.Exists("RAGEPluginHook.exe"))
                {
                    Game.LogTrivial("RAGEPluginHook.exe exists.");
                }
                else { Game.LogTrivial("RAGEPluginHook doesn't exist."); }
                Game.LogTrivial("RAGEPluginHook Version: " + versionInfo.ProductVersion.ToString());
                Game.DisplayNotification("RandomCallouts.VersionChecker unable to detect RagePluginHook installation. Please send me your log file.");
                correctVersion = false;

            }

            return correctVersion;
            
        }
    }
}
// Thanks Albo1125!