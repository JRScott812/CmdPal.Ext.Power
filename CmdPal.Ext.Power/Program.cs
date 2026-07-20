using System;
using System.Threading;

using CmdPal.Ext.Power.Helpers;

using Microsoft.CommandPalette.Extensions;

using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT;

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
				// Start-menu launch: open Command Palette (or guide to install it).
				// A silent exit is often reported as a Store certification "crash on launch".
				DirectLaunchHelper.Handle();
			}
		}
	}
}
