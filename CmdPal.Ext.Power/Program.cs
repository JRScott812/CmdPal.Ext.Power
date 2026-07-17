using System;
using System.Threading;

using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;

using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT;

using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace CmdPal.Ext.Power
{
	public class Program
	{
		[MTAThread]
		public static void Main(string[] args)
		{
			if (args.Length > 0 && args[0] == "-RegisterProcessAsComServer")
			{
				global::Shmuelie.WinRTServer.ComServer server = new();

				ManualResetEvent extensionDisposedEvent = new(false);

				// Single extension instance returned for every IExtension request from the host.
				CmdPalExtPower extensionInstance = new(extensionDisposedEvent);
				_ = server.RegisterClass<CmdPalExtPower, IExtension>(() => extensionInstance);
				server.Start();

				_ = extensionDisposedEvent.WaitOne();
				server.Stop();
				server.UnsafeDispose();
			}
			else
			{
				// Start-menu / Explorer launch has no UI; show guidance instead of a silent exit.
				_ = PInvoke.MessageBox(
					default,
					Resources.power_direct_launch_message,
					Resources.power_direct_launch_caption,
					MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONINFORMATION);
			}
		}
	}
}
