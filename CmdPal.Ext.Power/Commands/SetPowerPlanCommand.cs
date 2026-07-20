using System;

using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power.Commands
{
	internal sealed partial class SetPowerPlanCommand : InvokableCommand
	{
		private readonly PowerPlanService _service;
		private readonly Guid _schemeGuid;
		private readonly string _displayName;
		private readonly Action _onChanged;
		private readonly bool _dismissOnSuccess;

		internal SetPowerPlanCommand(
			PowerPlanService service,
			Guid schemeGuid,
			string displayName,
			Action onChanged,
			bool dismissOnSuccess = false)
		{
			_service = service;
			_schemeGuid = schemeGuid;
			_displayName = displayName;
			_onChanged = onChanged;
			_dismissOnSuccess = dismissOnSuccess;
			Id = $"com.jrscott812.cmdpal.power.setPlan.{schemeGuid:B}";
			Name = PowerPlanDisplayHelper.GetPlanTitle(schemeGuid, displayName);
			Icon = Icons.PlanGlyph(schemeGuid);
		}

		public override CommandResult Invoke()
		{
			if (!PowerPlanService.TrySetActivePlan(_schemeGuid, out string? error))
			{
				return ConfirmationFeedback.Show(
					error ?? Resources.power_plan_set_failed,
					dismissPalette: false,
					MessageState.Error);
			}

			_onChanged();

			string message = Resources.power_plan_set_toast_prefix + PowerPlanDisplayHelper.GetPlanTitle(_schemeGuid, _displayName);
			return ConfirmationFeedback.Show(message, _dismissOnSuccess);
		}
	}
}
