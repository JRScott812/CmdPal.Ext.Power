using System;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Constants;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power.Helpers;

internal static class PowerPlanDisplayHelper
{
	internal static string GetStatusSubtitle(PowerPlanSnapshot snapshot)
	{
		return !snapshot.CanReadPlans
			? Resources.power_plan_status_unavailable
			: snapshot.ActivePlan is { } activePlan ? GetPlanTitle(activePlan) : Resources.power_plan_status_unknown;
	}

	internal static string GetPlanTitle(PowerPlanInfo plan) =>
		GetPlanTitle(plan.SchemeGuid, plan.DisplayName);

	internal static string GetPlanTitle(Guid schemeGuid, string displayName) => displayName;

	internal static string GetPlanShortTitle(PowerPlanInfo plan) =>
		GetPlanShortTitle(plan.SchemeGuid, plan.DisplayName);

	internal static string GetPlanShortTitle(Guid schemeGuid, string displayName)
	{
		return schemeGuid == PowerPlanGuids.PowerSaver
			? Resources.power_plan_power_saver_short
			: schemeGuid == PowerPlanGuids.Balanced
			? Resources.power_plan_balanced_short
			: schemeGuid == PowerPlanGuids.HighPerformance
			? Resources.power_plan_high_performance_short
			: schemeGuid == PowerPlanGuids.UltimatePerformance ? Resources.power_plan_ultimate_performance_short : displayName;
	}

	internal static ITag[] GetPlanItemTags(PowerPlanInfo plan, PowerPlanSnapshot snapshot) => snapshot.ActivePlan?.SchemeGuid == plan.SchemeGuid ? [new Tag(Resources.power_list_current)] : [];
}
