using System;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Commands;
using CmdPal.Ext.Power.Enumerations;

using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power.Helpers
{
	internal sealed partial class PowerListItemBuilder
	{
		private readonly PowerModeService _powerModeService;
		private readonly PowerPlanService _powerPlanService;

		internal PowerListItemBuilder(PowerModeService powerModeService, PowerPlanService powerPlanService)
		{
			_powerModeService = powerModeService;
			_powerPlanService = powerPlanService;
		}

		internal ListItem CreateModeItem(
			UserPowerMode mode,
			string title,
			string successToast,
			Action onChanged,
			bool dismissOnSuccess = false)
		{
			PowerModeSnapshot snapshot = PowerModeService.GetSnapshot();
			return new ListItem(new SetPowerModeCommand(_powerModeService, mode, successToast, onChanged, dismissOnSuccess))
			{
				Title = title,
				Subtitle = string.Empty,
				Tags = PowerModeDisplayHelper.GetModeItemTags(mode, snapshot),
				Icon = Icons.Glyph(mode),
			};
		}

		internal static void RefreshModeItem(ListItem item, UserPowerMode mode)
		{
			PowerModeSnapshot snapshot = PowerModeService.GetSnapshot();
			item.Tags = PowerModeDisplayHelper.GetModeItemTags(mode, snapshot);
			item.Icon = Icons.Glyph(mode);
		}

		internal ListItem CreatePlanItem(
			PowerPlanInfo plan,
			PowerPlanSnapshot snapshot,
			Action onChanged,
			bool dismissOnSuccess = false)
		{
			return new ListItem(new SetPowerPlanCommand(_powerPlanService, plan.SchemeGuid, plan.DisplayName, onChanged, dismissOnSuccess))
			{
				Title = PowerPlanDisplayHelper.GetPlanTitle(plan),
				Subtitle = string.Empty,
				Tags = PowerPlanDisplayHelper.GetPlanItemTags(plan, snapshot),
				Icon = Icons.PlanGlyph(plan.SchemeGuid),
			};
		}

		internal static void RefreshPlanItem(ListItem item, PowerPlanInfo plan, PowerPlanSnapshot snapshot)
		{
			item.Tags = PowerPlanDisplayHelper.GetPlanItemTags(plan, snapshot);
			item.Icon = Icons.PlanGlyph(plan.SchemeGuid);
		}
	}
}
