using System;
using System.Collections.Generic;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Constants;
using CmdPal.Ext.Power.Properties;

namespace CmdPal.Ext.Power.Helpers;

internal static class PowerPlanCatalog
{
	private const int UnknownPlanSpeedOrder = 100;

	internal static int GetSpeedOrder(Guid schemeGuid)
	{
		return schemeGuid == PowerPlanGuids.PowerSaver
			? 0
			: schemeGuid == PowerPlanGuids.Balanced
			? 1
			: schemeGuid == PowerPlanGuids.HighPerformance
			? 2
			: schemeGuid == PowerPlanGuids.UltimatePerformance ? 3 : UnknownPlanSpeedOrder;
	}

	internal static int CompareBySpeed(PowerPlanInfo left, PowerPlanInfo right)
	{
		int order = GetSpeedOrder(left.SchemeGuid).CompareTo(GetSpeedOrder(right.SchemeGuid));
		return order != 0 ? order : string.Compare(left.DisplayName, right.DisplayName, StringComparison.OrdinalIgnoreCase);
	}

	internal static void SortBySpeed(List<PowerPlanInfo> plans) => plans.Sort(CompareBySpeed);

	internal static bool TryGetKnownDescription(Guid schemeGuid, out string description)
	{
		if (schemeGuid == PowerPlanGuids.Balanced)
		{
			description = Resources.power_plan_desc_balanced;
			return true;
		}

		if (schemeGuid == PowerPlanGuids.HighPerformance)
		{
			description = Resources.power_plan_desc_high_performance;
			return true;
		}

		if (schemeGuid == PowerPlanGuids.PowerSaver)
		{
			description = Resources.power_plan_desc_power_saver;
			return true;
		}

		if (schemeGuid == PowerPlanGuids.UltimatePerformance)
		{
			description = Resources.power_plan_desc_ultimate_performance;
			return true;
		}

		description = string.Empty;
		return false;
	}
}
