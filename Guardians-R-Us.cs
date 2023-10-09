using ExileCore;
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

                Entity guardianObj = null;

                if (GameController.IngameState.Data.LocalPlayer.TryGetComponent<Actor>(out var playerActorComp))
                {
                    var deployedObjectsCopy = playerActorComp.DeployedObjects.ToList();

                    var guardianEntities = (from deployedObject in deployedObjectsCopy
                                            where deployedObject.Entity != null && deployedObject.Entity.IsValid && deployedObject.Entity.Metadata != null && deployedObject.Entity.Metadata == "Metadata/Monsters/AnimatedItem/AnimatedArmour"
                                            select deployedObject).ToList();

                    if (guardianEntities != null)
                    {
                        if (guardianEntities.Count > 1)
                            LogError("[Guardians-R-Us] guardianEntities.Count > 1; Please check what's happening");
                        else if (guardianEntities.Count == 1)
                            guardianObj = guardianEntities.FirstOrDefault().Entity;
                    }
                }

                var green = new Vector4(0.102f, 0.388f, 0.106f, 1.000f);
                var red = new Vector4(0.388f, 0.102f, 0.102f, 1.000f);
                var collapsingHeaderFlags = ImGuiTreeNodeFlags.CollapsingHeader;

                var PUSHID = 1000;

                // Render Guardians relevant stats/mods
                if (guardianObj != null)
                {
                    guardianObj.TryGetComponent<Stats>(out var guardianStatsComp);
                    guardianObj.TryGetComponent<Life>(out var LifeComp);

                    ImGui.PushStyleColor(ImGuiCol.Header, green);
                    ImGui.PushID(PUSHID);
                    if (ImGui.TreeNodeEx($@"Guardian (Life:{LifeComp?.MaxHP} / ES:{LifeComp?.MaxES} / Mana:{LifeComp?.Mana.Max})##{guardianObj.GetHashCode}", collapsingHeaderFlags))
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

                    ImGui.PushStyleColor(ImGuiCol.Header, green);
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