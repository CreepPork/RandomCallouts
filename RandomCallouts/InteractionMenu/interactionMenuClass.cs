namespace RandomCallouts.InteractionMenu
{
    using System;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Rage;
    using Rage.Native;
    using LSPD_First_Response.Mod.API;

    public static class InteractionMenuClass
    {
        private static UIMenu mainMenu;
        private static UIMenu armorMenu;
        private static UIMenu weaponsMenu;
        private static UIMenu devMenuStartCallout;
        private static UIMenu devTestDistancesMenu;
        private static UIMenu carMenu;

        private static UIMenuItem removeArmorItem;
        private static UIMenuItem addArmorItem;
        private static UIMenuItem addHealthItem;
        private static UIMenuItem addTaser;
        private static UIMenuItem addPistolItem;
        private static UIMenuItem addShotgunItem;
        private static UIMenuItem addM4;
        private static UIMenuItem addBaton;
        private static UIMenuItem dropWeapon;

        private static UIMenuItem devMenuStartCalloutBankCarRobberyItem;
        private static UIMenuItem devMenuStartCalloutCrashedCarItem;
        private static UIMenuItem devMenuStartCalloutFootPursuitItem;
        private static UIMenuItem devMenuStartCalloutGangAttackItem;
        private static UIMenuItem devMenuStartCalloutHighPerformanceVehicleItem;
        private static UIMenuItem devMenuStartCalloutKidnappingCalloutItem;
        private static UIMenuItem devMenuStartCalloutLooseLivestockItem;
        private static UIMenuItem devMenuStartCalloutLorryChaseCalloutItem;
        private static UIMenuItem devMenuStartCalloutMuggingItem;
        private static UIMenuItem devMenuStartCalloutStolenArmoredCarItem;
        private static UIMenuItem devMenuStartCalloutPoliceCarStolenItem;
        private static UIMenuItem devMenuStartCalloutPublicDisorderItem;
        private static UIMenuItem devMenuStartCalloutStolenMilitaryHardwareItem;
        private static UIMenuItem devMenuStartCalloutTractorCalloutItem;

        private static UIMenuItem onePed;
        private static UIMenuItem twoPed;
        private static UIMenuItem threePed;
        private static UIMenuItem fourPed;
        private static UIMenuItem fivePed;
        private static UIMenuItem tenPed;
        private static UIMenuItem twentyPed;
        private static UIMenuItem thirtyPed;
        private static UIMenuItem fourtyPed;
        private static UIMenuItem fiftyPed;
        private static UIMenuItem sixtyPed;
        private static UIMenuItem seventyPed;
        private static UIMenuItem eightyPed;
        private static UIMenuItem nintyPed;
        private static UIMenuItem hundredPed;

        private static UIMenuItem repairVehicle;
        private static UIMenuItem cleanVehicle;
        private static UIMenuItem spawnVehicle;   
        private static UIMenuListItem carList;
        private static Vehicle spawnedVehicle;

        private static MenuPool _menuPool;


        public static InitializationFile initialiseFile()
        {
            //InitializationFile is a Rage class.
            InitializationFile ini = new InitializationFile("Plugins/LSPDFR/RandomCallouts.ini");
            ini.Create();
            return ini;
        }

        public static string getEndKey()
        {
            InitializationFile ini = initialiseFile();

            //ReadString takes 3 parameters: the first is the category, the second is the name of the entry and the third is the default value should the user leave the field blank.
            //Take a look at the example .ini file to understand this better.
            string keyBinding = ini.ReadString("Keys", "InteractionMenu", "F5");
            return keyBinding;
        }

        public static string getEndCalloutKeyModifier()
        {
            InitializationFile ini = initialiseFile();

            string keyBinding = ini.ReadString("Keys", "InteractionMenuModifierKey", "LShiftKey");
            return keyBinding;
        }

        /// <summary>
        /// Main part of the interaction menu
        /// </summary>
        public static void Main()
        {
            Game.FrameRender += Process;

            _menuPool = new MenuPool();

            mainMenu = new UIMenu("Interaction Menu", "~b~Random Callouts Interaction Menu");

            _menuPool.Add(mainMenu);
            
            // Adds our items
            var armorMenuItem = new UIMenuItem("Armor Menu", "Open up the Armor Menu.");
            mainMenu.AddItem(armorMenuItem);

            var weaponsMenuItem = new UIMenuItem("Weapons Menu", "Open up the Weapons Menu.");
            mainMenu.AddItem(weaponsMenuItem);

            var devMenuStartCalloutItem = new UIMenuItem("Start Callout Menu", "Open the menu for the testers and the developers.");
            mainMenu.AddItem(devMenuStartCalloutItem);

            var devMenuTestDistancesItem = new UIMenuItem("Distance Checking Menu", "Open up the menu that allows you to check how long a ped will spawn away.");
            mainMenu.AddItem(devMenuTestDistancesItem);

            var carMenuItem = new UIMenuItem("Car Menu", "Open up the Car Menu");
            mainMenu.AddItem(carMenuItem);

            mainMenu.AddItem(addHealthItem = new UIMenuItem("Use a Med Kit", "Use a med kit and clean your clothes."));

            mainMenu.RefreshIndex();

            mainMenu.OnItemSelect += OnItemSelectMainMenu;

            // Car Menu
            carMenu = new UIMenu("Interaction Menu", "~b~Car Menu");
            _menuPool.Add(carMenu);

            carMenu.AddItem(repairVehicle = new UIMenuItem("Repair your Vehicle", "Repair your current vehicle you are in."));

            carMenu.AddItem(cleanVehicle = new UIMenuItem("Clean your Vehicle", "Clean your current vehicle you are in."));

            var carsModels = new List<dynamic>
            {
                "Police",
                "Police2",
                "Police3",
                "Police4",
                "Sheriff",
                "Sheriff2",
                "PoliceB",
                "PoliceT",
                "Riot",
                "FBI",
                "FBI2",
                "PRanger"
            };

            carMenu.AddItem(carList = new UIMenuListItem("Vehicle Models", carsModels, 0));

            carMenu.AddItem(spawnVehicle = new UIMenuItem("Spawn the selected Vehicle", "Spawn the selected vehicle from the list above."));

            carMenu.RefreshIndex();

            carMenu.OnItemSelect += OnItemSelectCarMenu;

            mainMenu.BindMenuToItem(carMenu, carMenuItem);

            // Dev distances menu
            devTestDistancesMenu = new UIMenu("Interaction Menu", "~b~Developer Distance Menu");
            _menuPool.Add(devTestDistancesMenu);

            //distancesList = Enumerable.Range(1, 100).ToList();
            //var x = new List<dynamic>(distancesList.Count);
            //x.AddRange(distancesList.Cast<dynamic>());
            //dynamic Offset = x.IndexToItem(distancesList.Index);

            //var distancesList = Enumerable.Range(1, 100).ToList();
            //d = distancesList;


            //devTestDistancesMenu.AddItem(distancesListItem = new UIMenuListItem("Distances", d, 0));

            //devTestDistancesMenu.AddItem(spawnPed = new UIMenuItem("Spawn Ped", "Spawn the Ped at the selected distance."));


            devTestDistancesMenu.AddItem(onePed = new UIMenuItem("1 Distance", "1 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(twoPed = new UIMenuItem("2 Distance", "2 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(threePed = new UIMenuItem("3 Distance", "3 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(fourPed = new UIMenuItem("4 Distance", "4 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(fivePed = new UIMenuItem("5 Distance", "5 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(tenPed = new UIMenuItem("10 Distance", "10 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(twentyPed = new UIMenuItem("20 Distance", "20 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(thirtyPed = new UIMenuItem("30 Distance", "30 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(fourtyPed = new UIMenuItem("40 Distance", "40 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(fiftyPed = new UIMenuItem("50 Distance", "50 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(sixtyPed = new UIMenuItem("60 Distance", "60 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(seventyPed = new UIMenuItem("70 Distance", "70 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(eightyPed = new UIMenuItem("80 Distance", "80 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(nintyPed = new UIMenuItem("90 Distance", "90 F distance to spawn a ped."));

            devTestDistancesMenu.AddItem(hundredPed = new UIMenuItem("100 Distance", "100 F distance to spawn a ped."));

            devTestDistancesMenu.RefreshIndex();

            devTestDistancesMenu.OnItemSelect += OnItemSelectDevTestDistances;

            mainMenu.BindMenuToItem(devTestDistancesMenu, devMenuTestDistancesItem);

            // Creates our Weapons Menu
            weaponsMenu = new UIMenu("Interaction Menu", "~b~Weapons Menu");
            _menuPool.Add(weaponsMenu);

            weaponsMenu.AddItem(addBaton = new UIMenuItem("Take a Nightstick", "Take a Nightstick and add it to your inventory."));

            weaponsMenu.AddItem(addTaser = new UIMenuItem("Take a Stun Gun", "Take a Stun Gun and add it to your inventory."));

            weaponsMenu.AddItem(addPistolItem = new UIMenuItem("Take a Pistol", "Take a pistol with a flashlight."));

            weaponsMenu.AddItem(addShotgunItem = new UIMenuItem("Take a Shotgun", "Take a shotgun with a flashlight."));

            weaponsMenu.AddItem(addM4 = new UIMenuItem("Take a Carbine Rifle", "Take a Carbine Rifle with a scope, flashlight and a grip"));

            weaponsMenu.AddItem(dropWeapon = new UIMenuItem("Drop your Current Weapon", "Drop your currently equipped weapon."));

            weaponsMenu.RefreshIndex();

            weaponsMenu.OnItemSelect += OnItemSelectWeaponsMenu;

            mainMenu.BindMenuToItem(weaponsMenu, weaponsMenuItem);

            // Creates our Armor Menu 
            armorMenu = new UIMenu("Interaction Menu", "~b~Armor Menu");
            _menuPool.Add(armorMenu);

            armorMenu.AddItem(addArmorItem = new UIMenuItem("Add Armor", "Put on Armor"));

            armorMenu.AddItem(removeArmorItem = new UIMenuItem("Remove Armor", "Remove your Armor."));

            armorMenu.RefreshIndex();

            armorMenu.OnItemSelect += OnItemSelectArmorMenu;

            // Binds the armor menu to an item in the main menu.
            mainMenu.BindMenuToItem(armorMenu, armorMenuItem);

            // Create our Start Callout menu
            devMenuStartCallout = new UIMenu("Interaction Menu", "~b~Start Callout Menu");
            _menuPool.Add(devMenuStartCallout);

            devMenuStartCallout.AddItem(devMenuStartCalloutBankCarRobberyItem = new UIMenuItem("Don't call this callout", "Don't call this callout as it may result in a crash, it's just a placeholder."));

            devMenuStartCallout.AddItem(devMenuStartCalloutFootPursuitItem = new UIMenuItem("Foot Pursuit", "Start the Foot Pursuit callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutGangAttackItem = new UIMenuItem("Gang Attack", "Start the Gang Attack callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutKidnappingCalloutItem = new UIMenuItem("Kidnapping Callout", "Start the Kidnapping Callout callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutLooseLivestockItem = new UIMenuItem("Loose Animal", "Start the Loose Animal callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutCrashedCarItem = new UIMenuItem("Motor Vehicle Accident", "Start the Motor Vehicle Accident callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutMuggingItem = new UIMenuItem("Mugging", "Start the Mugging callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutPublicDisorderItem = new UIMenuItem("Public Disorder", "Start the Public Disorder callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutLorryChaseCalloutItem = new UIMenuItem("Pursuit of a Truck", "Start the Pursuit of a Truck callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutStolenArmoredCarItem = new UIMenuItem("Stolen Armored Car", "Start the Stolen Armored Car callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutStolenMilitaryHardwareItem = new UIMenuItem("Stolen Military Hardware", "Start the Stolen Military Hardware callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutPoliceCarStolenItem = new UIMenuItem("Stolen Police Vehicle", "Start the Stolen Police Vehicle callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutHighPerformanceVehicleItem = new UIMenuItem("Stolen Super Car", "Start the Stolen Super Car callout."));

            devMenuStartCallout.AddItem(devMenuStartCalloutTractorCalloutItem = new UIMenuItem("Stolen Tractor", "Start the Stolen Tractor callout."));

            devMenuStartCallout.RefreshIndex();

            devMenuStartCallout.OnItemSelect += OnItemSelectDevMenuStartCallout;

            mainMenu.BindMenuToItem(devMenuStartCallout, devMenuStartCalloutItem);
        }

        private static void OnItemSelectCarMenu(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != carMenu) return;

            try
            {
	            if (selectedItem == repairVehicle)
	            {
	                GameFiber.StartNew(delegate
	                {
	                    if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
                        {
                            Game.LocalPlayer.Character.CurrentVehicle.Repair();
                            Game.DisplaySubtitle("Your vehicle has been ~r~repaired~w~.", 3500);
                        }
	                }, "repairVehicleMenuFiber");
	            }
	            else if (selectedItem == cleanVehicle)
	            {
	                GameFiber.StartNew(delegate
	                {
                        if (Game.LocalPlayer.Character.IsInAnyVehicle(true))
                        {
                            Game.LocalPlayer.Character.CurrentVehicle.DirtLevel = 0;
                            Game.DisplaySubtitle("Your vehicle has been ~r~cleaned~w~.", 3500);
                        }
	                }, "cleanVehicleMenuFiber");
	            }
	            else if (selectedItem == spawnVehicle)
	            {
	                GameFiber.StartNew(delegate
	                {
	                    spawnedVehicle = new Vehicle(((string)carList.IndexToItem(carList.Index)).ToLower(), Game.LocalPlayer.Character.GetOffsetPositionFront(5f));
	                    spawnedVehicle.Dismiss();
	                }, "spawnVehicleMenuFiber");
	            }
            }
            catch (Exception ex)
            {
                Game.LogTrivial("An error occurred in the Car Menu. Error is: " + ex);
            }
        }

        /// <summary>
        /// Checks for button presses in the Developer Menu, Distances Checking menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItem"></param>
        /// <param name="index"></param>
        private static void OnItemSelectDevTestDistances(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != devTestDistancesMenu) return;

            GameFiber.StartNew(delegate
            {
                try
                {
                    //if (selectedItem == spawnPed)
                    //{
                    //    int Offset = d.IndexToItem(d.Index);
                    //    Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, Offset, 0f));
                    //    Ped A1 = new Ped(pedOffset);
                    //    Blip B1 = new Blip(A1);
                    //    GameFiber.Sleep(5000);
                    //    A1.Delete();
                    //    B1.Delete();
                    //}

                    if (selectedItem == onePed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 1f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == twoPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 2f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == threePed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 3f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == fourPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 4f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == fivePed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 5f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == tenPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 10f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);   
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == twentyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 20f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == thirtyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 30f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == fourtyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 40f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == fiftyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 50f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == sixtyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 60f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == seventyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 70f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == eightyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 80f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == nintyPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 90f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                    else if (selectedItem == hundredPed)
                    {
                        Vector3 pedOffset = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 100f, 0f));
                        Ped A1 = new Ped(pedOffset);
                        Blip B1 = new Blip(A1);
                        GameFiber.Sleep(5000);
                        B1.Delete();
                        A1.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial("An error occurred while spawning the peds. Error is: " + ex);
                }

            }, "devTestDistancesGameFiber");
        }

        /// <summary>
        /// Checks for button presses in the Developer Menu, Start Callout menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItem"></param>
        /// <param name="index"></param>
        private static void OnItemSelectDevMenuStartCallout(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != devMenuStartCallout) return;

            GameFiber.StartNew(delegate
            {
                if (selectedItem == devMenuStartCalloutBankCarRobberyItem)
                {
                    try
                    {
                        Functions.StartCallout("BankCarRobbery");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutStolenArmoredCarItem)
                {
                    try
                    {
                        Functions.StartCallout("StolenArmoredCar");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutCrashedCarItem)
                {
                    try
                    {
                        Functions.StartCallout("CrashedCar");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutFootPursuitItem)
                {
                    try
                    {
                        Functions.StartCallout("FootPursuit");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutGangAttackItem)
                {
                    try
                    {
                        Functions.StartCallout("GangAttack");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutHighPerformanceVehicleItem)
                {
                    try
                    {
                        Functions.StartCallout("HighPerformanceVehicle");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutKidnappingCalloutItem)
                {
                    try
                    {
                        Functions.StartCallout("KidnappingCallout");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutLooseLivestockItem)
                {
                    try
                    {
                        Functions.StartCallout("LooseLivestock");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutLorryChaseCalloutItem)
                {
                    try
                    {
                        Functions.StartCallout("LorryChaseCallout");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutMuggingItem)
                {
                    try
                    {
                        Functions.StartCallout("Mugging");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutPoliceCarStolenItem)
                {
                    try
                    {
                        Functions.StartCallout("PoliceCarStolen");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutPublicDisorderItem)
                {
                    try
                    {
                        Functions.StartCallout("PublicDisorder");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutStolenMilitaryHardwareItem)
                {
                    try
                    {
                        Functions.StartCallout("StolenMilitaryHardware");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
                else if (selectedItem == devMenuStartCalloutTractorCalloutItem)
                {
                    try
                    {
                        Functions.StartCallout("TractorCallout");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogTrivial("Failed to call a callout. Error is: " + ex);
                    }
                }
            }, "devMenuFiber");
        }

        /// <summary>
        /// Checks for button presses in the Main Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItem"></param>
        /// <param name="index"></param>
        private static void OnItemSelectMainMenu(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != mainMenu) return;

            if (selectedItem == addHealthItem)
            {
                Game.LocalPlayer.Character.Health = 1000;
                Game.LocalPlayer.Character.ClearBlood();
                Game.DisplaySubtitle("You have ~r~taken~w~ a med kit.", 3500);
            }
        }

        /// <summary>
        /// Checks for button presses in the Weapons Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItem"></param>
        /// <param name="index"></param>
        private static void OnItemSelectWeaponsMenu(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            GameFiber.StartNew(delegate
            {
                if (sender != weaponsMenu) return;

                if (selectedItem == addBaton)
                {
                    try
                    {
	                    Game.LocalPlayer.Character.Inventory.GiveNewWeapon(WeaponHash.Nightstick, 1, true);
                        Game.DisplaySubtitle("You have taken the ~r~Nightstick~w~.", 3500);
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogVerbose("An error occurred when equipping the Nightstick. The error is: " + ex);
                    }
                }

                if (selectedItem == addTaser)
                {
                    try
                    {
                        Game.LocalPlayer.Character.Inventory.GiveNewWeapon(WeaponHash.StunGun, 1, true);
                        Game.DisplaySubtitle("You have taken the ~r~Stun Gun~w~.", 3500);
                    }
                    catch (Exception ex)
                    {
                        Game.LogTrivial("An error occurred when equipping the Stun Gun. The error is: " + ex);
                    }
                }

                if (selectedItem == addPistolItem)
                {
                    try
                    {
	                    Game.LocalPlayer.Character.Inventory.GiveNewWeapon("WEAPON_PISTOL", 5000, true);
	                    Game.LocalPlayer.Character.Inventory.AddComponentToWeapon("WEAPON_PISTOL", "COMPONENT_AT_PI_FLSH");
                        Game.DisplaySubtitle("You have taken the ~r~Pistol~w~.", 3500);
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogVerbose("An error occurred when equipping the Pistol. The error is: " + ex);
                    }
                }
                else if (selectedItem == addShotgunItem)
                {
                    try
                    {
	                    Game.LocalPlayer.Character.Inventory.GiveNewWeapon("WEAPON_PUMPSHOTGUN", 5000, true);
	                    Game.LocalPlayer.Character.Inventory.AddComponentToWeapon("WEAPON_PUMPSHOTGUN", "COMPONENT_AT_AR_FLSH");
                        Game.DisplaySubtitle("You have taken the ~r~Shotgun~w~.", 3500);
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogVerbose("An error occurred when equipping the Pump Shotgun. The error is: " + ex);
                    }
                }
                else if (selectedItem == addM4)
                {
                    try
                    {
                        GameFiber.StartNew(delegate
                        {
                            Game.LocalPlayer.Character.Inventory.GiveNewWeapon("WEAPON_CARBINERIFLE", 5000, true);
                            GameFiber.Sleep(250);
                            Game.LocalPlayer.Character.Inventory.AddComponentToWeapon("WEAPON_CARBINERIFLE", "COMPONENT_AT_AR_FLSH");
                            Game.LocalPlayer.Character.Inventory.AddComponentToWeapon("WEAPON_CARBINERIFLE", "COMPONENT_AT_AR_AFGRIP");
                            Game.LocalPlayer.Character.Inventory.AddComponentToWeapon("WEAPON_CARBINERIFLE", "COMPONENT_AT_SCOPE_MEDIUM");
                            Game.DisplaySubtitle("You have taken the ~r~Carbine Rifle~w~.", 3500);
                        }, "addAttachmentsAndCarbineRifle");
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogVerbose("An error occurred when equipping the Carbine Rifle. The error is: " + ex);
                    }
                }
                else if (selectedItem == dropWeapon)
                {
                    try
                    {
                        NativeFunction.CallByName<uint>("SET_PED_DROPS_WEAPON", Game.LocalPlayer.Character);
                        Game.DisplaySubtitle("You have ~r~dropped~w~ your currently equipped weapon.", 3500);
                    }
                    catch (System.Exception ex)
                    {
                        Game.LogVerbose("An error occurred when dropping the currently equipped weapon. The error is: " + ex);
                        Game.DisplayHelp("~r~Failed~w~ to drop the currently equipped weapon, report to CreepPork_LV.");
                    }
                }
            }, "allWeaponsGameFiber");
        }

        /// <summary>
        /// Checks for our items if they have been pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItem"></param>
        /// <param name="index"></param>
        private static void OnItemSelectArmorMenu(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != armorMenu) return;

            if (selectedItem == addArmorItem)
            {
                Game.DisplaySubtitle("You have ~r~put on~w~ an armor vest.", 3500);
                Game.LocalPlayer.Character.Armor = 100;
            }

            else if (selectedItem == removeArmorItem)
            {
                Game.DisplaySubtitle("You have ~r~removed~w~ your armor vest.", 3500);
                Game.LocalPlayer.Character.Armor = 0;
            }
        }

        /// <summary>
        /// Main process that enters our key and makes the menu enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Process(object sender, GraphicsEventArgs e)
        {


            //A keys converter is used to convert a string to a key.
            KeysConverter kc = new KeysConverter();

            //We create two variables: one is a System.Windows.Keys, the other is a string.
            Keys EndCalloutKey;
            Keys EndCalloutKeyModifier;


            //Use a try/catch, because reading values from files is risky: we can never be sure what we're going to get and we don't want our plugin to crash.
            try
            {
                //We assign myKeyBinding the value of the string read by the method getMyKeyBinding(). We then use the kc.ConvertFromString method to convert this to a key.
                //If the string does not represent a valid key (see .ini file for a link) an exception is thrown. That's why we need a try/catch.
                EndCalloutKey = (Keys)kc.ConvertFromString(getEndKey());
                EndCalloutKeyModifier = (Keys)kc.ConvertFromString(getEndCalloutKeyModifier());
            }
            //If there was an error reading the values, we set them to their defaults. We also let the user know via a notification.
            catch
            {
                EndCalloutKey = Keys.End;
                EndCalloutKeyModifier = Keys.LShiftKey;
                Game.DisplayNotification("There was an error reading the .ini file. Setting defaults...");
            }

            if (Game.IsKeyDownRightNow(EndCalloutKeyModifier) && Game.IsKeyDownRightNow(EndCalloutKey) && !_menuPool.IsAnyMenuOpen()) // Our menu on/off switch.
                mainMenu.Visible = !mainMenu.Visible;

            _menuPool.ProcessMenus();       // Process all our menus: draw the menu and process the key strokes and the mouse. 
        }
    }
}
