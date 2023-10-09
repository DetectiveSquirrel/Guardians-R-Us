﻿using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using Vector4 = System.Numerics.Vector4;

namespace Guardians_R_Us
{
    public class Guardians_R_Us : BaseSettingsPlugin<Guardians_R_UsSettings>
    {
        public override void DrawSettings()
        {
            try
            {
                if (!Settings.Enable || !GameController.IngameState.InGame)
                    return;

                // Guardian Inventory (which is ours)
                var guardianItems = GameController.IngameState.Data.ServerData.GetPlayerInventoryByType(ExileCore.Shared.Enums.InventoryTypeE.AnimatedArmour).Items;

                // Create a list to store all matching entities
                List<Entity> matchingEntities = new List<Entity>();

                if (GameController.IngameState.Data.LocalPlayer.TryGetComponent<Actor>(out var playerActorComp))
                {
                    var deployedObjectsCopy = playerActorComp.DeployedObjects.ToList();

                    var guardianEntities = (from deployedObject in deployedObjectsCopy
                                            where deployedObject.Entity != null && deployedObject.Entity.IsValid &&
                                                  deployedObject.Entity.Metadata != null && deployedObject.Entity.Metadata == "Metadata/Monsters/AnimatedItem/AnimatedArmour"
                                            select deployedObject.Entity).ToList();

                    matchingEntities.AddRange(guardianEntities);
                }

                var green = new Vector4(0.102f, 0.388f, 0.106f, 1.000f);
                var red = new Vector4(0.388f, 0.102f, 0.102f, 1.000f);
                var collapsingHeaderFlags = ImGuiTreeNodeFlags.CollapsingHeader;

                var PUSHID = 1000;

                // Render all matching entities
                foreach (var entity in matchingEntities)
                {
                    entity.TryGetComponent<Stats>(out var guardianStatsComp);
                    entity.TryGetComponent<Life>(out var lifeComp);

                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                    ImGui.PushID(PUSHID);

                    string headerText = $"{entity.RenderName} (Life:{lifeComp?.MaxHP} / ES:{lifeComp?.MaxES} / Mana:{lifeComp?.Mana.Max})##{entity.GetHashCode}";

                    if (ImGui.TreeNodeEx(headerText, collapsingHeaderFlags))
                    {
                        ImGui.Indent();
                        foreach (var stat in guardianStatsComp.StatDictionary)
                        {
                            ImGui.Text($@"{stat}");
                        }
                        ImGui.Unindent();
                    }
                    PUSHID++;
                }

                // Render Items
                foreach (var item in guardianItems)
                {
                    item.TryGetComponent<Base>(out var baseComp);
                    item.TryGetComponent<Mods>(out var modsComp);

                    ImGui.PushStyleColor(ImGuiCol.Header, red);
                    ImGui.PushID(PUSHID);

                    string itemlabelName = modsComp.UniqueName != ""
                        ? $"{modsComp.UniqueName} {baseComp.Name}##{item.GetHashCode}"
                        : $"{baseComp.Name}##{item.GetHashCode}";

                    if (ImGui.TreeNodeEx($@"{itemlabelName}##{item.GetHashCode}", collapsingHeaderFlags))
                    {
                        ImGui.Indent();
                        ImGui.Text($@"Name: {baseComp.Name}");
                        ImGui.Text($@"Unique Name: {modsComp.UniqueName}");
                        ImGui.Text($@"Mods: {modsComp.ItemMods.Count}");
                        foreach (var mod in modsComp.ItemMods)
                        {
                            ImGui.Indent();
                            ImGui.Text($@"{mod}");
                            ImGui.Unindent();
                        }
                        ImGui.Unindent();
                    }
                    PUSHID++;
                }
            }
            catch (Exception ex)
            {
                LogError(@$"[Guardians-R-Us] {ex}", 15);
            }
        }
    }
}