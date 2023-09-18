using CommandLine;

namespace DisplayConfigurationManager
{
    public class Options
    {
        [Option('l', "devices", Required = false, HelpText = "List all available display devices.")]
        public bool ListDevices { get; set; }

        [Option('q', "query", Required = false, HelpText = "Query the current settings of a specific device.")]
        public string? Query { get; set; }

        [Option('a', "qall", Required = false, HelpText = "Query the current settings of all devices.")]
        public bool QueryAll { get; set; }

        [Option('m', "mode", Required = false, HelpText = "Mode to set the display to.")]
        public int? Mode { get; set; }

        [Option('d', "device", Required = false, HelpText = "Name of the device to adjust.")]
        public string? DeviceName { get; set; }

        [Option('x', "set-x", Required = false, HelpText = "Set X position of the display.")]
        public int? SetX { get; set; }

        [Option('y', "set-y", Required = false, HelpText = "Set Y position of the display.")]
        public int? SetY { get; set; }

        [Option('w', "width", Required = false, HelpText = "Width of the resolution.")]
        public int? Width { get; set; }

        [Option('h', "height", Required = false, HelpText = "Height of the resolution.")]
        public int? Height { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

    }
}
