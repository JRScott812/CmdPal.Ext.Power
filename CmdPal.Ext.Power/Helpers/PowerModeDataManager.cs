using System;
using System.Threading;

using Timer = System.Timers.Timer;

namespace CmdPal.Ext.Power.Helpers
{
	internal sealed partial class PowerModeDataManager : IDisposable
	{
		private const int OneSecondInMilliseconds = 1000;

		private readonly Timer _updateTimer;
		private readonly Action _updateAction;
		private readonly PowerModeService _powerModeService;
		private readonly PowerPlanService _powerPlanService;
		private readonly EventHandler _powerModeChangedHandler;
		private readonly EventHandler _powerPlanChangedHandler;
		private readonly Lock _activateLock = new();
		private int _activateCount;
		private bool _updateFailureHandled;

		internal PowerModeDataManager(
			PowerModeService powerModeService,
			PowerPlanService powerPlanService,
			Action updateAction)
		{
			_powerModeService = powerModeService;
			_powerPlanService = powerPlanService;
			_updateAction = updateAction;
			_powerModeChangedHandler = (_, _) => SafeUpdate();
			_powerPlanChangedHandler = (_, _) => SafeUpdate();
			_updateTimer = new Timer(OneSecondInMilliseconds)
			{
				AutoReset = true,
				Enabled = false,
			};
			_updateTimer.Elapsed += (_, _) => SafeUpdate();
			_powerModeService.PowerModeChanged += _powerModeChangedHandler;
			_powerPlanService.PowerPlanChanged += _powerPlanChangedHandler;
		}

		internal void PushActivate()
		{
			lock (_activateLock)
			{
				if (_activateCount++ == 0)
				{
					StartPolling();
				}
			}
		}

		internal void PopActivate()
		{
			lock (_activateLock)
			{
				_activateCount = Math.Max(0, _activateCount - 1);
				if (_activateCount == 0)
				{
					StopPolling();
				}
			}
		}

		public void Dispose()
		{
			StopPolling();
			_updateTimer.Dispose();
			_powerModeService.PowerModeChanged -= _powerModeChangedHandler;
			_powerPlanService.PowerPlanChanged -= _powerPlanChangedHandler;
		}

		private void StartPolling()
		{
			_updateFailureHandled = false;
			_powerModeService.EnsureSubscribed();
			_powerPlanService.CheckForPlanChange();
			SafeUpdate();
			if (!_updateFailureHandled)
			{
				_updateTimer.Enabled = true;
			}
		}

		private void StopPolling()
		{
			_updateTimer.Enabled = false;
			_powerModeService.Unsubscribe();
		}

		private void SafeUpdate()
		{
			try
			{
				_powerPlanService.CheckForPlanChange();
				_updateAction();
			}
			catch
			{
				if (_updateFailureHandled)
				{
					return;
				}

				_updateFailureHandled = true;
				StopPolling();
			}
		}
	}
}
