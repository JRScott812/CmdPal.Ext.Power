using System;
using System.Collections.Generic;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Constants;
using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class PowerPlanDisplayHelperTests
	{
		[TestMethod]
		public void GetStatusSubtitleWhenUnavailableShowsUnavailableMessage()
		{
			PowerPlanSnapshot snapshot = CreateSnapshot(canReadPlans: false);
			Assert.AreEqual(Resources.power_plan_status_unavailable, PowerPlanDisplayHelper.GetStatusSubtitle(snapshot));
		}

		[TestMethod]
		public void GetStatusSubtitleWhenActivePlanKnownShowsActivePlanName()
		{
			PowerPlanInfo activePlan = new(PowerPlanGuids.Balanced, "Balanced", Resources.power_plan_desc_balanced);
			PowerPlanSnapshot snapshot = CreateSnapshot(activePlan: activePlan);
			string subtitle = PowerPlanDisplayHelper.GetStatusSubtitle(snapshot);
			StringAssert.Contains(subtitle, "Balanced");
		}

		[TestMethod]
		public void GetPlanItemTagsWhenActivePlanShowsCurrentTag()
		{
			PowerPlanInfo activePlan = new(PowerPlanGuids.Balanced, "Balanced", Resources.power_plan_desc_balanced);
			PowerPlanSnapshot snapshot = CreateSnapshot(activePlan: activePlan);
			ITag[] tags = PowerPlanDisplayHelper.GetPlanItemTags(activePlan, snapshot);
			Assert.HasCount(1, tags);
			Assert.AreEqual(Resources.power_list_current, tags[0].Text);
		}

		[TestMethod]
		public void GetPlanItemTagsWhenNotActiveReturnsEmpty()
		{
			PowerPlanInfo activePlan = new(PowerPlanGuids.Balanced, "Balanced", Resources.power_plan_desc_balanced);
			PowerPlanInfo otherPlan = new(PowerPlanGuids.HighPerformance, "High performance", Resources.power_plan_desc_high_performance);
			PowerPlanSnapshot snapshot = CreateSnapshot(activePlan: activePlan);
			Assert.IsEmpty(PowerPlanDisplayHelper.GetPlanItemTags(otherPlan, snapshot));
		}

		[TestMethod]
		public void GetPlanTitleUltimatePerformanceReturnsDisplayNameUnmodified()
		{
			PowerPlanInfo plan = new(PowerPlanGuids.UltimatePerformance, "Ultimate Performance", string.Empty);
			Assert.AreEqual("Ultimate Performance", PowerPlanDisplayHelper.GetPlanTitle(plan));
		}

		[TestMethod]
		public void GetPlanTitleBalancedDoesNotAppendPlusSuffix()
		{
			PowerPlanInfo plan = new(PowerPlanGuids.Balanced, "Balanced", string.Empty);
			Assert.AreEqual("Balanced", PowerPlanDisplayHelper.GetPlanTitle(plan));
		}

		private static PowerPlanSnapshot CreateSnapshot(
			bool canReadPlans = true,
			PowerPlanInfo activePlan = default,
			IReadOnlyList<PowerPlanInfo>? availablePlans = null)
		{
			availablePlans ??=
			[
				new PowerPlanInfo(PowerPlanGuids.Balanced, "Balanced", Resources.power_plan_desc_balanced),
				new PowerPlanInfo(PowerPlanGuids.HighPerformance, "High performance", Resources.power_plan_desc_high_performance),
			];

			return new PowerPlanSnapshot(
				activePlan.SchemeGuid == Guid.Empty ? null : activePlan,
				availablePlans,
				canReadPlans,
				canReadPlans);
		}
	}
}
