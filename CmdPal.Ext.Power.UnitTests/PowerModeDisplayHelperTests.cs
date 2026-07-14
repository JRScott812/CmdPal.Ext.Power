using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Enumerations;
using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class PowerModeDisplayHelperTests
	{
		[TestMethod]
		public void GetModeItemTagsWhenActiveModeShowsCurrentTag()
		{
			PowerModeSnapshot snapshot = CreateSnapshot(PowerSourceKind.OnBattery, hasBattery: true, isOnAcPower: false, isCharging: false);
			ITag[] tags = PowerModeDisplayHelper.GetModeItemTags(UserPowerMode.Balanced, snapshot);
			Assert.HasCount(1, tags);
			Assert.AreEqual(Resources.power_list_current, tags[0].Text);
		}

		[TestMethod]
		public void GetModeItemTagsWhenInactiveModeReturnsEmpty()
		{
			PowerModeSnapshot snapshot = CreateSnapshot(PowerSourceKind.OnBattery, hasBattery: true, isOnAcPower: false, isCharging: false);
			Assert.IsEmpty(PowerModeDisplayHelper.GetModeItemTags(UserPowerMode.BestPerformance, snapshot));
		}

		[TestMethod]
		public void GetStatusSubtitleWhenUnsupportedShowsNotSupportedMessage()
		{
			PowerModeSnapshot snapshot = CreateSnapshot(PowerSourceKind.NoBattery, hasBattery: false, isOnAcPower: true, isCharging: false, canReadUserMode: false);
			string subtitle = PowerModeDisplayHelper.GetStatusSubtitle(snapshot);
			Assert.AreEqual(Resources.power_mode_not_supported, subtitle);
		}

		[TestMethod]
		public void GetStatusSubtitleWhenSupportedShowsCurrentMode()
		{
			PowerModeSnapshot snapshot = CreateSnapshot(PowerSourceKind.PluggedIn, hasBattery: true, isOnAcPower: true, isCharging: true);
			string subtitle = PowerModeDisplayHelper.GetStatusSubtitle(snapshot);
			Assert.AreEqual(Resources.power_mode_balanced, subtitle);
		}

		private static PowerModeSnapshot CreateSnapshot(
			PowerSourceKind powerSourceKind,
			bool hasBattery,
			bool isOnAcPower,
			bool isCharging,
			bool canReadUserMode = true) =>
			new(
				UserPowerMode.Balanced,
				null,
				powerSourceKind,
				hasBattery,
				isOnAcPower,
				isCharging,
				canReadUserMode);
	}
}
