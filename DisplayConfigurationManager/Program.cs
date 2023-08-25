using System;
using System.Runtime.InteropServices;
using CommandLine;

// A program to change the display configuration (resolution, orientation, etc.) of a Windows machine.
namespace DisplayConfigurationManager
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            // The MarshalAs attribute is used to specify how data should be marshaled between managed and unmanaged memory.
            // The MarshalAs attribute can be applied to most parameters that are exposed to unmanaged code, and to fields of structures that are passed to unmanaged code.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

            // dmDeviceName is a string that specifies the device name of the device.
            public string dmDeviceName;
            // dmSpecVersion is the version number of the initialization data specification on which the structure is based.
            public short dmSpecVersion;
            // dmDriverVersion is the version number of the display driver (for example, 3.0).
            public short dmDriverVersion;
            // dmSize is the size of the DEVMODE structure, in bytes.
            public short dmSize;
            // dmDriverExtra is the number of bytes of private driver-data that follow this structure.
            public short dmDriverExtra;

            // dmFields is a collection of flags that indicate which of the following members contain valid data:
            public int dmFields;

            // dmPositionX is the x-coordinate of the upper-left corner of the display surface.
            public int dmPositionX;
            // dmPositionY is the y-coordinate of the upper-left corner of the display surface.
            public int dmPositionY;
            // dmDisplayOrientation is the orientation of the display device in degrees.
            public int dmDisplayOrientation;
            // dmDisplayFixedOutput is the fixed output of the display device.
            public int dmDisplayFixedOutput;
            // dmColor is the color resolution, in bits per pixel, of the display device.
            public short dmColor;
            // dmDuplex is the duplex flag.
            public short dmDuplex;
            // dmYResolution is the y-resolution, in dots per inch, of the display device.
            public short dmYResolution;
            // dmTTOption is the TrueType option.
            public short dmTTOption;
            // dmCollate is the collation flag.
            public short dmCollate;

            // Another MarshalAs attribute, this time for a character array.
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

            // dmFormName is a zero-terminated character array that specifies the name of the form to use for printing the document.
            public string dmFormName;
            // dmLogPixels is the number of pixels per logical inch.
            public short dmLogPixels;
            // dmBitsPerPel is the color resolution, in bits per pixel, of the display device.
            public short dmBitsPerPel;
            // dmPelsWidth is the width, in pixels, of the visible device surface.
            public int dmPelsWidth;
            // dmPelsHeight is the height, in pixels, of the visible device surface.
            public int dmPelsHeight;

            // dmDisplayFlags is the driver-specific flags.
            public int dmDisplayFlags;
            // dmDisplayFrequency is the frequency, in hertz (cycles per second), of the display device in a particular mode.
            public int dmDisplayFrequency;
            // dmICMMethod is the method used to match colors.
            public int dmICMMethod;
            // dmICMIntent is the intent of the image.
            public int dmICMIntent;

            // dmMediaType is the media type.
            public int dmMediaType;
            // dmDitherType is the dither type.
            public int dmDitherType;

            // dmReserved1 is reserved; do not use.
            public int dmReserved1;
            // dmReserved2 is reserved; do not use.
            public int dmReserved2;

            // dmPanningWidth is the width, in pixels, of the display device.
            public int dmPanningWidth;
            // dmPanningHeight is the height, in pixels, of the display device.
            public int dmPanningHeight;
        }

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int ChangeDisplaySettings(ref DEVMODE lpDevMode, int dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern bool EnumDisplayDevices(
            string lpDevice,
            uint iDevNum,
            ref DISPLAY_DEVICE lpDisplayDevice,
            uint dwFlags
        );

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern bool EnumDisplaySettings(
            string? lpszDeviceName,
            int iModeNum,
            ref DEVMODE lpDevMode
        );


        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(
            string lpszDeviceName,
            ref DEVMODE lpDevMode,
            IntPtr hwnd,
            int dwflags,
            IntPtr lParam
        );

        private const int ENUM_CURRENT_SETTINGS = -1;

        public enum DISP_CHANGE : int
        {
            SUCCESSFUL = 0,
            RESTART = 1,
            FAILED = -1,
            BADMODE = -2,
            NOTUPDATED = -3
        }

        public static void HandleDisplayChangeResult(DISP_CHANGE result, string deviceName, int width, int height)
        {
            switch (result)
            {
                case DISP_CHANGE.SUCCESSFUL:
                    Console.WriteLine($"Resolution Changed to {width}x{height} for device {deviceName}.");
                    break;
                case DISP_CHANGE.RESTART:
                    Console.WriteLine($"The computer needs to be restarted for the changes on device {deviceName} to take effect.");
                    break;
                case DISP_CHANGE.FAILED:
                    Console.WriteLine($"Failed to change the display settings for device {deviceName}.");
                    break;
                case DISP_CHANGE.BADMODE:
                    Console.WriteLine($"The graphics mode is not supported for device {deviceName}.");
                    break;
                case DISP_CHANGE.NOTUPDATED:
                    Console.WriteLine($"Unable to write settings to the registry for device {deviceName}.");
                    break;
                default:
                    Console.WriteLine("Unknown error.");
                    break;
            }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed(o =>
                  {

                      if (o.Verbose)
                      {
                          Console.WriteLine("Verbose mode activated. Showing detailed output...");
                          // Other verbose output goes here
                      }

                      if (o.ListDevices)
                      {
                          EnumerateDevices(o.Verbose);
                          return;  // Exit after listing devices
                      }

                      if (o.Query != null)
                      {
                          QueryDisplaySettings(o.Query);
                          return;
                      }
                      else if (o.QueryAll)
                      {
                          QueryDisplaySettings();
                          return;
                      }

                      DEVMODE currentSettings = new DEVMODE();
                      currentSettings.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

                      // If width, height, and device name are provided, change the resolution
                      if (o.DeviceName != null && o.Width.HasValue && o.Height.HasValue)
                      {
                          if (IsResolutionSupported(o.DeviceName, o.Width.Value, o.Height.Value))
                          {
                              var result = ChangeResolution(o.DeviceName, o.Width.Value, o.Height.Value);
                              HandleDisplayChangeResult(result, o.DeviceName, o.Width.Value, o.Height.Value);
                          }
                          else
                          {
                              Console.WriteLine($"Resolution {o.Width}x{o.Height} is not supported on device {o.DeviceName}.");
                          }
                      }

                      // Handle other options...
                  })
                  .WithNotParsed(errors =>
                  {
                      // Handle errors here
                      foreach (var error in errors)
                      {
                          Console.WriteLine(error.Tag);
                      }
                  });
        }

        public static void EnumerateDevices(bool verbose = false)
        {
            if (verbose) Console.WriteLine("Entering EnumerateDevices...");

            DISPLAY_DEVICE dd = new DISPLAY_DEVICE();
            dd.cb = Marshal.SizeOf(dd);

            int deviceIndex = 0;
            HashSet<string> deviceNames = new HashSet<string>();

            // Enumerate through devices
            while (EnumDisplayDevices(null, (uint)deviceIndex, ref dd, 0))
            {
                deviceNames.Add(dd.DeviceName);
                if (verbose)
                {
                    Console.WriteLine($"Found device {dd.DeviceName}");
                }

                DEVMODE dm = new DEVMODE();
                dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
                int modeNum = 0;

                // Enumerate through display settings for each device
                while (EnumDisplaySettings(dd.DeviceName, modeNum, ref dm))
                {
                    if (verbose) Console.WriteLine($"Checking mode {modeNum} for device {dd.DeviceName}...");
                    if (verbose)
                    {
                        Console.WriteLine($"Resolution {dm.dmPelsWidth}x{dm.dmPelsHeight}");
                    }
                    modeNum++;
                }

                if (verbose) Console.WriteLine($"Finished checking {modeNum} modes for device {dd.DeviceName}.");

                deviceIndex++;
            }

            Console.WriteLine("Available Devices:");
            foreach (var deviceName in deviceNames)
            {
                Console.WriteLine(deviceName);
            }
        }

        public static void QueryDisplaySettings(string? specificDevice = null)
        {
            DISPLAY_DEVICE dd = new DISPLAY_DEVICE();
            dd.cb = Marshal.SizeOf(dd);

            int deviceIndex = 0;
            while (EnumDisplayDevices(null, (uint)deviceIndex, ref dd, 0))
            {
                if (specificDevice != null && dd.DeviceName != specificDevice)
                {
                    deviceIndex++;
                    continue;
                }

                DEVMODE dm = new DEVMODE();
                dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

                if (EnumDisplaySettings(dd.DeviceName, -1, ref dm))
                {
                    Console.WriteLine($"Device: {dd.DeviceName}");
                    Console.WriteLine($"Resolution: {dm.dmPelsWidth}x{dm.dmPelsHeight}");
                    Console.WriteLine($"Position: X = {dm.dmPositionX}, Y = {dm.dmPositionY}");
                    Console.WriteLine("-------------");
                }

                deviceIndex++;

                if (specificDevice != null)
                {
                    break;
                }
            }
        }
        private static void ChangeDisplayPosition(string deviceName, int x, int y)
        {
            DEVMODE currentSettings = new DEVMODE();
            currentSettings.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

            if (EnumDisplaySettings(deviceName, ENUM_CURRENT_SETTINGS, ref currentSettings))
            {
                currentSettings.dmPositionX = x;  // Note the corrected members here
                currentSettings.dmPositionY = y;  // Note the corrected members here

                ChangeDisplaySettingsEx(deviceName, ref currentSettings, IntPtr.Zero, 0, IntPtr.Zero);
                Console.WriteLine($"Position Changed to X={x}, Y={y} for device {deviceName}.");
            }
            else
            {
                Console.WriteLine($"Failed to get current settings for device {deviceName}.");
            }
        }

        public static DISP_CHANGE ChangeResolution(string deviceName, int width, int height)
        {
            DEVMODE currentSettings = new DEVMODE();
            currentSettings.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

            EnumDisplaySettings(deviceName, -1, ref currentSettings);
            currentSettings.dmPelsWidth = width;
            currentSettings.dmPelsHeight = height;

            return (DISP_CHANGE)ChangeDisplaySettings(ref currentSettings, 0);
        }

        public static bool IsResolutionSupported(string deviceName, int width, int height)
        {
            DEVMODE dm = new DEVMODE();
            int modeNum = 0;
            while (EnumDisplaySettings(deviceName, modeNum, ref dm))
            {
                if (dm.dmPelsWidth == width && dm.dmPelsHeight == height)
                    return true;
                modeNum++;
            }
            return false;
        }

    }
}
