using System;

using CmdPal.Ext.Power.Enumerations;
using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power.Commands
{
	internal sealed partial class SetPowerModeCommand : InvokableCommand
	{
		private readonly PowerModeService _service;
		private readonly UserPowerMode _mode;
		private readonly string _successToast;
		private readonly Action _onChanged;
		private readonly bool _dismissOnSuccess;

		internal SetPowerModeCommand(
			PowerModeService service,
			UserPowerMode mode,
			string successToast,
			Action onChanged,
			bool dismissOnSuccess = false)
		{
			_service = service;
			_mode = mode;
			_successToast = successToast;
			_onChanged = onChanged;
			_dismissOnSuccess = dismissOnSuccess;
			Id = mode switch
			{
				UserPowerMode.BestEfficiency => "com.jrscott812.cmdpal.power.setEfficiency",
				UserPowerMode.Balanced => "com.jrscott812.cmdpal.power.setBalanced",
				UserPowerMode.BestPerformance => "com.jrscott812.cmdpal.power.setPerformance",
				_ => "com.jrscott812.cmdpal.power.setUnknown",
			};
			Name = PowerModeDisplayHelper.GetUserModeLabel(mode);
			Icon = Icons.Glyph(mode);
		}

		public override CommandResult Invoke()
		{
			if (!PowerModeService.TrySetUserPowerMode(_mode, out string? error))
			{
				return ConfirmationFeedback.Show(
					error ?? Resources.power_mode_set_failed,
					dismissPalette: false,
					MessageState.Error);
			}

			_onChanged();

			if (string.IsNullOrWhiteSpace(_successToast))
			{
				return _dismissOnSuccess ? CommandResult.Dismiss() : CommandResult.KeepOpen();
			}

			return ConfirmationFeedback.Show(_successToast, _dismissOnSuccess);
		}
	}
}
