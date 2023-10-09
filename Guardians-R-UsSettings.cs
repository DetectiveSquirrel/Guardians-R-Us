using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace Guardians_R_Us
{
    public class Guardians_R_UsSettings : ISettings
    {
        //Mandatory setting to allow enabling/disabling your plugin
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
    }
}