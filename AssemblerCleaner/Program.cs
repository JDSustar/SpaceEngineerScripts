using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
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
        List<IMyTerminalBlock> containers;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        void Main()
        {
            List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>();
            containers = new List<IMyTerminalBlock>();

            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers, c => c.IsSameConstructAs(Me));

            for (int i = 0; i < assemblers.Count; i++)
            {
                CleanAssembler(assemblers[i] as IMyAssembler);
            }
        }

        void CleanAssembler(IMyAssembler assembler)
        {
            if (assembler.IsProducing || !IsFull(assembler.GetInventory(0)))
                return;

            IMyInventory containerDestination = null;

            // search our containers until we find an empty one
            for (int n = 0; n < containers.Count; n++)
            {
                var container = containers[n];
                var containerInv = container.GetInventory(0);
                if (!IsFull(containerInv))
                {
                    Echo("Cont Dest: " + container.CustomName);
                    containerDestination = containerInv;
                    break;
                }
            }

            if (containerDestination == null)
                return;

            var assemblerInv = assembler.GetInventory(0);
            List<MyInventoryItem> assemblerItems = new List<MyInventoryItem>();
            assemblerInv.GetItems(assemblerItems);
            for (int i = 0; i < assemblerItems.Count; i++)
            {
                Echo("Trying to move from " + assembler.CustomName);
                assemblerInv.TransferItemTo(containerDestination, i, null, true, null);
            }
        }

        float GetPercentFull(IMyInventory inv)
        {
            return ((float)inv.CurrentVolume / (float)inv.MaxVolume) * 100f;
        }

        bool IsFull(IMyInventory inv)
        {
            if (GetPercentFull(inv) >= 99)
                return true;
            else
                return false;
        }
    }
}
