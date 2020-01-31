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
    partial class Program
    {
        private const string BLUEPRINT_TYPE = "MyObjectBuilder_BlueprintDefinition/";

        public class ItemTypes
        {
            public string SEItemTypeString { get; private set; }
            public string SEItemSubTypeString { get; private set; }
            public string SEItemFullTypeString => this.SEItemTypeString + "/" + SEItemSubTypeString;
            public string SEItemBlueprintSubTypeString { get; private set; }
            public string SEItemBlueprintFullTypeString => BLUEPRINT_TYPE + SEItemBlueprintSubTypeString;
            public string ItemName { get; private set; }
            public MyDefinitionId SEItemBlueprintDefinitionId => MyDefinitionId.Parse(SEItemBlueprintFullTypeString);
            public MyItemType SEItemType => MyItemType.Parse(SEItemFullTypeString);

            public ItemTypes(string itemName, string seItemTypeString, string seItemSubTypeString, string seItemBlueprintSubTypeString)
            {
                ItemName = itemName;
                SEItemTypeString = seItemTypeString;
                SEItemSubTypeString = seItemSubTypeString;
                SEItemBlueprintSubTypeString = seItemBlueprintSubTypeString;
            }
        }
    }
}
