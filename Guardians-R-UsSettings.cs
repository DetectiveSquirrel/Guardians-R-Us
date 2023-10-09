using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace Guardians_R_Us
{
    public class Guardians_R_UsSettings : ISettings
    {
        //Mandatory setting to allow enabling/disabling your plugin
        public ToggleNode Enable { get; set; } = new ToggleNode(false);

        //Put all your settings here if you can.
        //There's a bunch of ready-made setting nodes,
        //nested menu support and even custom callbacks are supported.
        //If you want to override DrawSettings instead, you better have a very good reason.
    }
}