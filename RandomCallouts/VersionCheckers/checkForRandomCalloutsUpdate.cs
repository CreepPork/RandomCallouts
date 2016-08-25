using System;
using System.Xml;
using Rage;
using System.Windows.Forms;
using System.Diagnostics;

namespace RandomCallouts.VersionCheckers
{
    class checkForRandomCalloutsUpdate
    {
        public static void checkForRandomCalloutsUpdateMain()
        {
            string downloadUrl = "";
            Version newVersion = null;
            string xmlUrl = "http://192.166.8.100/RandomCallouts/update.xml";
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(xmlUrl);
                reader.MoveToContent();
                string elementName = "";
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "randomCallouts"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            elementName = reader.Name;
                        }
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                            {
                                switch(elementName)
                                {
                                    case "version":
                                        newVersion = new Version(reader.Value);
                                        break;
                                    case "url":
                                        downloadUrl = reader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Game.LogTrivial("Catch exception occurred, failed to check for updates...");
                Game.LogTrivial("Error is: " + ex.Message);
                Game.DisplayNotification("Failed to check for updates, for Random Callouts, please contact CreepPork_LV and send your log!");
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            Version applicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (applicationVersion.CompareTo(newVersion) < 0 )
            {
                Game.LogTrivial("Version " + newVersion.Major + "." + newVersion.Minor + "." + newVersion.Build + " of Random Callouts is now available!");
                Game.DisplaySubtitle("Version " + newVersion.Major + "." + newVersion.Minor + "." + newVersion.Build + " of Random Callouts is now available!", 8000);
                GameFiber.StartNew(delegate
                {
                    while (Game.IsLoading)
                    {
                        GameFiber.Yield();
                    }
                    //If you decide to use this in your plugin, I would appreciate some credit :)
                    Game.LogTrivial("Preparing redirect...");
                    Game.DisplayNotification("You are being redirected to LCPDFR.com website so you can download the latest version of Random Callouts.");
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
                            Process.Start("http://192.168.8.100");
                            break;
                        }
                    }

                }, "checkForRandomCalloutsUpdate");
            }
            else
            {
                Game.LogTrivial("Random Callouts is up to date!");
            }
        }
    }
}

