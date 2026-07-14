using System.Collections.Generic;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Constants;
using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Properties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class PowerPlanCatalogTests
	{
		[TestMethod]
		public void TryGetKnownDescriptionBalancedReturnsBalancedDescription()
		{
			Assert.IsTrue(PowerPlanCatalog.TryGetKnownDescription(PowerPlanGuids.Balanced, out string? description));
			Assert.AreEqual(Resources.power_plan_desc_balanced, description);
		}

		[TestMethod]
		public void TryGetKnownDescriptionHighPerformanceReturnsHighPerformanceDescription()
		{
			Assert.IsTrue(PowerPlanCatalog.TryGetKnownDescription(PowerPlanGuids.HighPerformance, out string? description));
			Assert.AreEqual(Resources.power_plan_desc_high_performance, description);
		}

		[TestMethod]
		public void TryGetKnownDescriptionPowerSaverReturnsPowerSaverDescription()
		{
			Assert.IsTrue(PowerPlanCatalog.TryGetKnownDescription(PowerPlanGuids.PowerSaver, out string? description));
			Assert.AreEqual(Resources.power_plan_desc_power_saver, description);
		}

		[TestMethod]
		public void TryGetKnownDescriptionUltimatePerformanceReturnsUltimatePerformanceDescription()
		{
			Assert.IsTrue(PowerPlanCatalog.TryGetKnownDescription(PowerPlanGuids.UltimatePerformance, out string? description));
			Assert.AreEqual(Resources.power_plan_desc_ultimate_performance, description);
		}

		[TestMethod]
		public void CompareBySpeedOrdersKnownPlansSlowestToFastest()
		{
			List<PowerPlanInfo> plans =
			[
				new(PowerPlanGuids.UltimatePerformance, "Ultimate Performance", string.Empty),
				new(PowerPlanGuids.Balanced, "Balanced", string.Empty),
				new(PowerPlanGuids.HighPerformance, "High performance", string.Empty),
				new(PowerPlanGuids.PowerSaver, "Power saver", string.Empty),
			];

			PowerPlanCatalog.SortBySpeed(plans);

			Assert.AreEqual(PowerPlanGuids.PowerSaver, plans[0].SchemeGuid);
			Assert.AreEqual(PowerPlanGuids.Balanced, plans[1].SchemeGuid);
			Assert.AreEqual(PowerPlanGuids.HighPerformance, plans[2].SchemeGuid);
			Assert.AreEqual(PowerPlanGuids.UltimatePerformance, plans[3].SchemeGuid);
		}
	}
}
