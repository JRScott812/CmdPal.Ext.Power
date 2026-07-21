using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CmdPal.Ext.Power.Properties;

using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace CmdPal.Ext.Power.Helpers
{
	/// <summary>
	/// Start-menu / Explorer launch has no extension UI. Open Command Palette when
	/// possible so Store certification does not treat a silent exit as a crash.
	/// </summary>
	internal static class DirectLaunchHelper
	{
		private const string CommandPalettePackageFamilyName = "Microsoft.CommandPalette_8wekyb3d8bbwe";
		private const string CommandPaletteDevPackageFamilyName = "Microsoft.CommandPalette.Dev_8wekyb3d8bbwe";
		private const string PowerToysStoreLink = "ms-windows-store://pdp/?productid=XP89DCGQ3K6VLD";

		internal static void Handle()
		{
			try
			{
				HandleAsync().GetAwaiter().GetResult();
			}
			catch
			{
				ShowInfo(Resources.power_direct_launch_message);
			}
		}

		private static async Task HandleAsync()
		{
			Package? retail = null;
			Package? dev = null;

			try
			{
				PackageManager packageManager = new();
				retail = packageManager.FindPackagesForUser(string.Empty, CommandPalettePackageFamilyName)?.FirstOrDefault();
				dev = packageManager.FindPackagesForUser(string.Empty, CommandPaletteDevPackageFamilyName)?.FirstOrDefault();
			}
			catch
			{
				// Fall through to the missing-CmdPal path.
			}

			Package? package = retail ?? dev;
			if (package is null)
			{
				ShowInfo(Resources.power_direct_launch_cmdpal_missing);
				TryOpenStore();
				return;
			}

			bool started;
			try
			{
				started = await TryStartCommandPaletteAsync(package).ConfigureAwait(false);
			}
			catch
			{
				started = false;
			}

			if (!started)
			{
				ShowInfo(Resources.power_direct_launch_cmdpal_failed);
			}
		}

		private static async Task<bool> TryStartCommandPaletteAsync(Package package)
		{
			var entries = await package.GetAppListEntriesAsync();
			if (entries is null || entries.Count == 0 || entries[0] is null)
			{
				return false;
			}

			return await entries[0]!.LaunchAsync();
		}

		private static void TryOpenStore()
		{
			try
			{
				_ = Process.Start(new ProcessStartInfo(PowerToysStoreLink) { UseShellExecute = true });
			}
			catch
			{
			}
		}

		private static void ShowInfo(string message)
		{
			_ = PInvoke.MessageBox(
				default,
				message,
				Resources.power_direct_launch_caption,
				MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONINFORMATION);
		}
	}
}
