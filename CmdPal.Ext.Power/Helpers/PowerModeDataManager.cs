using System;
using System.Threading;

using Timer = System.Timers.Timer;

namespace CmdPal.Ext.Power.Helpers;

internal sealed partial class PowerModeDataManager : IDisposable
{
	private const int OneSecondInMilliseconds = 1000;

	private readonly Timer _updateTimer;
	private readonly Action _updateAction;
	private readonly PowerModeService _powerModeService;
	private readonly EventHandler _powerModeChangedHandler;
	private readonly Lock _activateLock = new();
	private int _activateCount;
	private bool _updateFailureHandled;

	internal PowerModeDataManager(
		PowerModeService powerModeService,
		Action updateAction)
	{
		_powerModeService = powerModeService;
		_updateAction = updateAction;
		_powerModeChangedHandler = (_, _) => SafeUpdate();
		_updateTimer = new Timer(OneSecondInMilliseconds)
		{
			AutoReset = true,
			Enabled = false,
		};
		_updateTimer.Elapsed += (_, _) => SafeUpdate();
		_powerModeService.PowerModeChanged += _powerModeChangedHandler;
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
	}

	private void StartPolling()
	{
		_updateFailureHandled = false;
		_powerModeService.EnsureSubscribed();
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
