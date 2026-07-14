using System;

using CmdPal.Ext.Power.Enumerations;
using CmdPal.Ext.Power.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class PowerModeCatalogTests
	{
		[TestMethod]
		public void FromGuidMapsKnownModes()
		{
			Assert.AreEqual(UserPowerMode.BestEfficiency, PowerModeCatalog.FromGuid(PowerModeCatalog.BestEfficiency.Guid));
			Assert.AreEqual(UserPowerMode.Balanced, PowerModeCatalog.FromGuid(PowerModeCatalog.Balanced.Guid));
			Assert.AreEqual(UserPowerMode.BestPerformance, PowerModeCatalog.FromGuid(PowerModeCatalog.BestPerformance.Guid));
		}

		[TestMethod]
		public void FromGuidUnknownGuidReturnsUnknown() => Assert.AreEqual(UserPowerMode.Unknown, PowerModeCatalog.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111111")));

		[TestMethod]
		public void ToGuidRoundTripsKnownModes()
		{
			Assert.AreEqual(PowerModeCatalog.BestEfficiency.Guid, PowerModeCatalog.ToGuid(UserPowerMode.BestEfficiency));
			Assert.AreEqual(PowerModeCatalog.Balanced.Guid, PowerModeCatalog.ToGuid(UserPowerMode.Balanced));
			Assert.AreEqual(PowerModeCatalog.BestPerformance.Guid, PowerModeCatalog.ToGuid(UserPowerMode.BestPerformance));
		}
	}
}
