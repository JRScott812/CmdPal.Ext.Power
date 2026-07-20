using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power.Commands
{
	internal static class ConfirmationFeedback
	{
		/// <summary>
		/// Desktop ShowToast can mis-size when successive messages differ in length
		/// (CmdPal host layout race). Prefer an in-page status banner while the
		/// palette stays open; use the desktop toast only when dismissing.
		/// </summary>
		internal static CommandResult Show(string message, bool dismissPalette, MessageState state = MessageState.Success)
		{
			if (dismissPalette)
			{
				return CommandResult.ShowToast(new ToastArgs()
				{
					Message = message,
					Result = CommandResult.Dismiss(),
				});
			}

			new ToastStatusMessage(new StatusMessage()
			{
				Message = message,
				State = state,
			}).Show();

			return CommandResult.KeepOpen();
		}
	}
}