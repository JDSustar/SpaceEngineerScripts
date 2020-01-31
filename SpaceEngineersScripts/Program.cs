using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using Sandbox.Game;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        // to learn more about ingame scripts.

        private List<IMyAssembler> _assemblers = new List<IMyAssembler>();
        private List<IMyCargoContainer> _cargos = new List<IMyCargoContainer>();
        private List<MyProductionItem> _items = new List<MyProductionItem>();
        private List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
        private IMyTextPanel _lcd;
        private IMyInventory _inventory;
        private Dictionary<string, ItemTypes> _itemLookup = new Dictionary<string, ItemTypes>();
        private Dictionary<string, int> _itemQuotas = new Dictionary<string, int>();
        private Dictionary<string, int> _currentInventory = new Dictionary<string, int>();
        private Dictionary<string, int> _currentProduction = new Dictionary<string, int>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            GridTerminalSystem.GetBlocksOfType(_assemblers);
            _assemblers = _assemblers.Where(a => a.CustomName.Contains("[AAM]")).ToList();
            GridTerminalSystem.GetBlocksOfType(_cargos, c => c.IsSameConstructAs(Me));

            GridTerminalSystem.GetBlocksOfType(_blocks);

            _lcd = GridTerminalSystem.GetBlockWithName("JDS Test LCD") as IMyTextPanel;

            _itemLookup.Add("Bulletproof Glass", new ItemTypes("Bulletproof Glass", "MyObjectBuilder_Component", "BulletproofGlass", "BulletproofGlass"));
            _itemLookup.Add("Canvas", new ItemTypes("Canvas", "MyObjectBuilder_Component", "Canvas", "Canvas"));
            _itemLookup.Add("Computer", new ItemTypes("Computer", "MyObjectBuilder_Component", "Computer", "ComputerComponent"));
            _itemLookup.Add("Construction Comp", new ItemTypes("Construction Comp", "MyObjectBuilder_Component", "Construction", "ConstructionComponent"));
            _itemLookup.Add("Detector Comp", new ItemTypes("Detector Comp", "MyObjectBuilder_Component", "Detector", "DetectorComponent"));
            _itemLookup.Add("Display", new ItemTypes("Display", "MyObjectBuilder_Component", "Display", "Display"));
            _itemLookup.Add("Explosives", new ItemTypes("Explosives", "MyObjectBuilder_Component", "Explosives", "ExplosivesComponent"));
            _itemLookup.Add("Girder", new ItemTypes("Girder", "MyObjectBuilder_Component", "Girder", "GirderComponent"));
            _itemLookup.Add("Gravity Gen. Comp", new ItemTypes("Gravity Gen. Comp", "MyObjectBuilder_Component", "GravityGenerator", "GravityGeneratorComponent"));
            _itemLookup.Add("Interior Plate", new ItemTypes("Interior Plate", "MyObjectBuilder_Component", "InteriorPlate", "InteriorPlate"));
            _itemLookup.Add("Large Steel Tube", new ItemTypes("Large Steel Tube", "MyObjectBuilder_Component", "LargeTube", "LargeTube"));
            _itemLookup.Add("Medical Comp", new ItemTypes("Medical Comp", "MyObjectBuilder_Component", "Medical", "MedicalComponent"));
            _itemLookup.Add("Metal Grid", new ItemTypes("Metal Grid", "MyObjectBuilder_Component", "MetalGrid", "MetalGrid"));
            _itemLookup.Add("Motor", new ItemTypes("Motor", "MyObjectBuilder_Component", "Motor", "MotorComponent"));
            _itemLookup.Add("Power Cell", new ItemTypes("Power Cell", "MyObjectBuilder_Component", "PowerCell", "PowerCell"));
            _itemLookup.Add("Radio Comm. Comp", new ItemTypes("Radio Comm. Comp", "MyObjectBuilder_Component", "RadioCommunication", "RadioCommunicationComponent"));
            _itemLookup.Add("Reactor Comp", new ItemTypes("Reactor Comp", "MyObjectBuilder_Component", "Reactor", "ReactorComponent"));
            _itemLookup.Add("Small Steel Tube", new ItemTypes("Small Steel Tube", "MyObjectBuilder_Component", "SmallTube", "SmallTube"));
            _itemLookup.Add("Solar Cell", new ItemTypes("Solar Cell", "MyObjectBuilder_Component", "SolarCell", "SolarCell"));
            _itemLookup.Add("Steel Plate", new ItemTypes("Steel Plate", "MyObjectBuilder_Component", "SteelPlate", "SteelPlate"));
            _itemLookup.Add("Superconductor", new ItemTypes("Superconductor", "MyObjectBuilder_Component", "Superconductor", "Superconductor"));
            _itemLookup.Add("Thruster Comp", new ItemTypes("Thruster Comp", "MyObjectBuilder_Component", "Thrust", "ThrustComponent"));

            _itemQuotas.Add("Bulletproof Glass", 5000);
            _itemQuotas.Add("Canvas", 0);
            _itemQuotas.Add("Computer", 5000);
            _itemQuotas.Add("Construction Comp", 5000);
            _itemQuotas.Add("Detector Comp", 500);
            _itemQuotas.Add("Display", 5000);
            _itemQuotas.Add("Explosives", 0);
            _itemQuotas.Add("Girder", 5000);
            _itemQuotas.Add("Gravity Gen. Comp", 100);
            _itemQuotas.Add("Interior Plate", 5000);
            _itemQuotas.Add("Large Steel Tube", 5000);
            _itemQuotas.Add("Medical Comp", 100);
            _itemQuotas.Add("Metal Grid", 5000);
            _itemQuotas.Add("Motor", 5000);
            _itemQuotas.Add("Power Cell", 2000);
            _itemQuotas.Add("Radio Comm. Comp", 500);
            _itemQuotas.Add("Reactor Comp", 500);
            _itemQuotas.Add("Small Steel Tube", 5000);
            _itemQuotas.Add("Solar Cell", 1000);
            _itemQuotas.Add("Steel Plate", 10000);
            _itemQuotas.Add("Superconductor", 2000);
            _itemQuotas.Add("Thruster Comp", 1000);


            //_itemLookup.Add("Automatic Rifle", new ItemTypes("Automatic Rifle", "MyObjectBuilder_PhysicalGunObject", "AutomaticRifleItem", "AutomaticRifle"));
            //_itemLookup.Add("Precise Rifle", new ItemTypes("Precise Rifle", "MyObjectBuilder_PhysicalGunObject", "PreciseAutomaticRifleItem", "PreciseAutomaticRifle"));
            //_itemLookup.Add("Rapid Fire Rifle", new ItemTypes("Rapid Fire Rifle", "MyObjectBuilder_PhysicalGunObject", "RapidFireAutomaticRifleItem", "RapidFireAutomaticRifle"));
            //_itemLookup.Add("Ultimate Rifle", new ItemTypes("Ultimate Rifle", "MyObjectBuilder_PhysicalGunObject", "UltimateAutomaticRifleItem", "UltimateAutomaticRifle"));
            //_itemLookup.Add("Welder 1", new ItemTypes("Welder 1", "MyObjectBuilder_PhysicalGunObject", "WelderItem", "Welder"));
            //_itemLookup.Add("Welder 2", new ItemTypes("Welder 2", "MyObjectBuilder_PhysicalGunObject", "Welder2Item", "Welder2"));
            //_itemLookup.Add("Welder 3", new ItemTypes("Welder 3", "MyObjectBuilder_PhysicalGunObject", "Welder3Item", "Welder3"));
            //_itemLookup.Add("Welder 4", new ItemTypes("Welder 4", "MyObjectBuilder_PhysicalGunObject", "Welder4Item", "Welder4"));
            //_itemLookup.Add("Grinder 1", new ItemTypes("Grinder 1", "MyObjectBuilder_PhysicalGunObject", "AngleGrinderItem", "AngleGrinder"));
            //_itemLookup.Add("Grinder 2", new ItemTypes("Grinder 2", "MyObjectBuilder_PhysicalGunObject", "AngleGrinder2Item", "AngleGrinder2"));
            //_itemLookup.Add("Grinder 3", new ItemTypes("Grinder 3", "MyObjectBuilder_PhysicalGunObject", "AngleGrinder3Item", "AngleGrinder3"));
            //_itemLookup.Add("Grinder 4", new ItemTypes("Grinder 4", "MyObjectBuilder_PhysicalGunObject", "AngleGrinder4Item", "AngleGrinder4"));
            //_itemLookup.Add("Drill 1", new ItemTypes("Drill 1", "MyObjectBuilder_PhysicalGunObject", "HandDrillItem", "HandDrill"));
            //_itemLookup.Add("Drill 2", new ItemTypes("Drill 2", "MyObjectBuilder_PhysicalGunObject", "HandDrill2Item", "HandDrill2"));
            //_itemLookup.Add("Drill 3", new ItemTypes("Drill 3", "MyObjectBuilder_PhysicalGunObject", "HandDrill3Item", "HandDrill3"));
            //_itemLookup.Add("Drill 4", new ItemTypes("Drill 4", "MyObjectBuilder_PhysicalGunObject", "HandDrill4Item", "HandDrill4"));
            //_itemLookup.Add("Oxygen Bottle", new ItemTypes("Oxygen Bottle", "MyObjectBuilder_OxygenContainerObject", "OxygenBottle", "OxygenBottle"));
            //_itemLookup.Add("Hydrogen Bottle", new ItemTypes("Hydrogen Bottle", "MyObjectBuilder_GasContainerObject", "HydrogenBottle", "HydrogenBottle"));
            //_itemLookup.Add("NATO 5.56x45mm", new ItemTypes("NATO 5.56x45mm", "MyObjectBuilder_AmmoMagazine", "NATO_5p56x45mm", "NATO_5p56x45mmMagazine"));
            //_itemLookup.Add("NATO 25x184mm", new ItemTypes("NATO 25x184mm", "MyObjectBuilder_AmmoMagazine", "NATO_25x184mm", "NATO_25x184mmMagazine"));
            //_itemLookup.Add("Missile 200mm", new ItemTypes("Missile 200mm", "MyObjectBuilder_AmmoMagazine", "Missile200mm", "Missile200mm"));
            //_itemLookup.Add("Cobalt Ore", new ItemTypes("Cobalt Ore", "MyObjectBuilder_Ore", "Cobalt", "None"));
            //_itemLookup.Add("Gold Ore", new ItemTypes("Gold Ore", "MyObjectBuilder_Ore", "Gold", "None"));
            //_itemLookup.Add("Ice", new ItemTypes("Ice", "MyObjectBuilder_Ore", "Ice", "None"));
            //_itemLookup.Add("Iron Ore", new ItemTypes("Iron Ore", "MyObjectBuilder_Ore", "Iron", "None"));
            //_itemLookup.Add("Magnesium Ore", new ItemTypes("Magnesium Ore", "MyObjectBuilder_Ore", "Magnesium", "None"));
            //_itemLookup.Add("Nickel Ore", new ItemTypes("Nickel Ore", "MyObjectBuilder_Ore", "Nickel", "None"));
            //_itemLookup.Add("Platinum Ore", new ItemTypes("Platinum Ore", "MyObjectBuilder_Ore", "Platinum", "None"));
            //_itemLookup.Add("Scrap Ore", new ItemTypes("Scrap Ore", "MyObjectBuilder_Ore", "Scrap", "None"));
            //_itemLookup.Add("Silicon Ore", new ItemTypes("Silicon Ore", "MyObjectBuilder_Ore", "Silicon", "None"));
            //_itemLookup.Add("Silver Ore", new ItemTypes("Silver Ore", "MyObjectBuilder_Ore", "Silver", "None"));
            //_itemLookup.Add("Stone", new ItemTypes("Stone", "MyObjectBuilder_Ore", "Stone", "None"));
            //_itemLookup.Add("Uranium Ore", new ItemTypes("Uranium Ore", "MyObjectBuilder_Ore", "Uranium", "None"));
            //_itemLookup.Add("Cobalt Ingot", new ItemTypes("Cobalt Ingot", "MyObjectBuilder_Ingot", "Cobalt", "None"));
            //_itemLookup.Add("Gold Ingot", new ItemTypes("Gold Ingot", "MyObjectBuilder_Ingot", "Gold", "None"));
            //_itemLookup.Add("Gravel", new ItemTypes("Gravel", "MyObjectBuilder_Ingot", "Stone", "None"));
            //_itemLookup.Add("Iron Ingot", new ItemTypes("Iron Ingot", "MyObjectBuilder_Ingot", "Iron", "None"));
            //_itemLookup.Add("Magnesium Powder", new ItemTypes("Magnesium Powder", "MyObjectBuilder_Ingot", "Magnesium", "None"));
            //_itemLookup.Add("Nickel Ingot", new ItemTypes("Nickel Ingot", "MyObjectBuilder_Ingot", "Nickel", "None"));
            //_itemLookup.Add("Platinum Ingot", new ItemTypes("Platinum Ingot", "MyObjectBuilder_Ingot", "Platinum", "None"));
            //_itemLookup.Add("Silicon Wafer", new ItemTypes("Silicon Wafer", "MyObjectBuilder_Ingot", "Silicon", "None"));
            //_itemLookup.Add("Silver Ingot", new ItemTypes("Silver Ingot", "MyObjectBuilder_Ingot", "Silver", "None"));
            //_itemLookup.Add("Uranium Ingot", new ItemTypes("Uranium Ingot", "MyObjectBuilder_Ingot", "Uranium", "None"));

        }

        private void UpdateCurrentInventory()
        { 
            foreach (var e in _blocks)
            {
                if (e.HasInventory)
                {
                    for (int i = 0; i < e.InventoryCount; i++)
                    {
                        _inventory = e.GetInventory(i);

                        foreach (var itemT in _itemLookup.Values)
                        {
                            _currentInventory[itemT.ItemName] += (int)_inventory.GetItemAmount(itemT.SEItemType);
                        }
                    }
                }
            }
        }

        private void UpdateCurrentProduction()
        {
            foreach (var e in _blocks.Where(e => e is IMyAssembler))
            { 
                var a = e as IMyAssembler;
                a.GetQueue(_items);

                foreach (var itemT in _itemLookup.Values)
                {
                    _currentProduction[itemT.ItemName] += (int)_items.Where(i => i.BlueprintId == itemT.SEItemBlueprintDefinitionId).Sum(i => (double)i.Amount);
                }
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Update100)
            {
                Echo(Runtime.MaxInstructionCount.ToString());

                _lcd.WriteText("Current Production:\n", false);

                Echo(Runtime.CurrentInstructionCount.ToString());

                _currentInventory.Clear();
                _currentProduction.Clear();
                foreach (var itemName in _itemLookup.Keys)
                {
                    _currentInventory.Add(itemName, 0);
                    _currentProduction.Add(itemName, 0);
                }

                UpdateCurrentInventory();
                UpdateCurrentProduction();

                foreach (var i in _itemQuotas.Keys)
                {
                    Echo(Runtime.CurrentInstructionCount.ToString());
                    var SEi = _itemLookup[i];
                    var quota = _itemQuotas[i];

                    if (_currentInventory[SEi.ItemName] + _currentProduction[SEi.ItemName] < quota)
                    {
                        Echo("Queuing: " + SEi.ItemName + " x " + quota / 10);
                        _assemblers[0].AddQueueItem(SEi.SEItemBlueprintDefinitionId, (double)quota / 10);
                    }

                    Echo(Runtime.CurrentInstructionCount.ToString());
                    Echo(SEi.ItemName + ": " + _currentProduction[SEi.ItemName].ToString());

                    if (_currentProduction[SEi.ItemName] > 0)
                    {
                        _lcd.WriteText(SEi.ItemName + ": " + _currentProduction[SEi.ItemName].ToString() + "\n", true);
                    }
                    Echo(Runtime.CurrentInstructionCount.ToString());
                }
            }

            Echo("Done: " + DateTime.Now);
        }
    }
}
