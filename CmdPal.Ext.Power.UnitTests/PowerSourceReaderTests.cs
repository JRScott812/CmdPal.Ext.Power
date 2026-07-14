using CmdPal.Ext.Power.Enumerations;
using CmdPal.Ext.Power.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Windows.Win32.System.Power;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class PowerSourceReaderTests
	{
		[TestMethod]
		public void GetPowerSourceKindNoBatteryReturnsNoBattery()
		{
			SYSTEM_POWER_STATUS status = new()
			{
				BatteryFlag = 0x80,
				ACLineStatus = 1,
			};

			Assert.AreEqual(PowerSourceKind.NoBattery, PowerSourceReader.GetPowerSourceKind(in status));
			Assert.IsFalse(PowerSourceReader.HasBattery(in status));
			Assert.IsTrue(PowerSourceReader.UseAcPowerProfile(PowerSourceKind.NoBattery));
		}

		[TestMethod]
		public void GetPowerSourceKindOnBatteryReturnsOnBattery()
		{
			SYSTEM_POWER_STATUS status = new()
			{
				BatteryFlag = 0x01,
				ACLineStatus = 0,
			};

			Assert.AreEqual(PowerSourceKind.OnBattery, PowerSourceReader.GetPowerSourceKind(in status));
			Assert.IsTrue(PowerSourceReader.HasBattery(in status));
			Assert.IsFalse(PowerSourceReader.UseAcPowerProfile(PowerSourceKind.OnBattery));
		}

		[TestMethod]
		public void GetPowerSourceKindPluggedInReturnsPluggedIn()
		{
			SYSTEM_POWER_STATUS status = new()
			{
				BatteryFlag = 0x01,
				ACLineStatus = 1,
			};

			Assert.AreEqual(PowerSourceKind.PluggedIn, PowerSourceReader.GetPowerSourceKind(in status));
			Assert.IsTrue(PowerSourceReader.HasBattery(in status));
			Assert.IsTrue(PowerSourceReader.IsOnAcPower(in status));
			Assert.IsTrue(PowerSourceReader.UseAcPowerProfile(PowerSourceKind.PluggedIn));
		}

		[TestMethod]
		public void IsChargingWhenChargingFlagSetReturnsTrue()
		{
			SYSTEM_POWER_STATUS status = new()
			{
				BatteryFlag = 0x09,
				ACLineStatus = 1,
			};

			Assert.IsTrue(PowerSourceReader.IsCharging(in status));
		}
	}
}
