using System;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Enumerations;
using CmdPal.Ext.Power.Properties;

using Microsoft.Windows.System.Power;

using Windows.Win32;
using Windows.Win32.System.Power;

namespace CmdPal.Ext.Power.Helpers;

internal sealed partial class PowerModeService : IDisposable
{
	private bool _subscribed;

	internal event EventHandler? PowerModeChanged;

	internal PowerModeSnapshot GetSnapshot()
	{
		if (!PowerSourceReader.TryGetPowerStatus(out SYSTEM_POWER_STATUS status))
		{
			return CreateSnapshot(
				userMode: UserPowerMode.Unknown,
				effective: null,
				powerSourceKind: Enumerations.PowerSourceKind.Unknown,
				hasBattery: false,
				isOnAcPower: true,
				isCharging: false,
				canReadUserMode: false);
		}

		Enumerations.PowerSourceKind powerSourceKind = PowerSourceReader.GetPowerSourceKind(in status);
		bool hasBattery = PowerSourceReader.HasBattery(in status);
		bool isOnAcPower = PowerSourceReader.IsOnAcPower(in status);
		bool isCharging = PowerSourceReader.IsCharging(in status);
		bool useAcProfile = PowerSourceReader.UseAcPowerProfile(powerSourceKind);
		bool canReadUserMode = TryGetUserPowerMode(useAcProfile, out Guid userGuid);
		UserPowerMode userMode = canReadUserMode ? PowerModeCatalog.FromGuid(userGuid) : UserPowerMode.Unknown;

		EffectivePowerMode? effective = null;
		try
		{
			effective = PowerManager.EffectivePowerMode2;
		}
		catch
		{
			// WinRT API may be unavailable in some environments.
		}

		return CreateSnapshot(
			userMode,
			effective,
			powerSourceKind,
			hasBattery,
			isOnAcPower,
			isCharging,
			canReadUserMode);
	}

	internal bool SupportsPowerModeControl() => GetSnapshot().CanReadUserMode;

	internal bool TrySetUserPowerMode(UserPowerMode mode, out string? errorMessage)
	{
		errorMessage = null;
		if (mode is UserPowerMode.Unknown)
		{
			errorMessage = Resources.power_mode_unknown;
			return false;
		}

		if (!PowerSourceReader.TryGetPowerStatus(out SYSTEM_POWER_STATUS status))
		{
			errorMessage = Resources.power_mode_not_supported;
			return false;
		}

		Enumerations.PowerSourceKind powerSourceKind = PowerSourceReader.GetPowerSourceKind(in status);
		bool useAcProfile = PowerSourceReader.UseAcPowerProfile(powerSourceKind);
		Guid guid = PowerModeCatalog.ToGuid(mode);

		uint result = useAcProfile
			? PInvoke.PowerSetUserConfiguredACPowerMode(in guid)
			: PInvoke.PowerSetUserConfiguredDCPowerMode(in guid);

		if (result == 0)
		{
			return true;
		}

		guid = PowerModeCatalog.ToGuid(mode);
		result = PInvoke.PowerSetActiveOverlayScheme(ref guid);
		if (result == 0)
		{
			return true;
		}

		errorMessage = Resources.power_mode_set_failed;
		return false;
	}

	internal void EnsureSubscribed()
	{
		if (_subscribed)
		{
			return;
		}

		try
		{
			PowerManager.EffectivePowerModeChanged += OnEffectivePowerModeChanged;
			_subscribed = true;
		}
		catch
		{
		}
	}

	internal void Unsubscribe()
	{
		if (!_subscribed)
		{
			return;
		}

		try
		{
			PowerManager.EffectivePowerModeChanged -= OnEffectivePowerModeChanged;
		}
		catch
		{
		}

		_subscribed = false;
	}

	public void Dispose() => Unsubscribe();

	private void OnEffectivePowerModeChanged(object? sender, object e) => PowerModeChanged?.Invoke(this, EventArgs.Empty);

	private static bool TryGetUserPowerMode(bool useAcProfile, out Guid powerModeGuid)
	{
		powerModeGuid = Guid.Empty;
		uint result = useAcProfile
			? PInvoke.PowerGetUserConfiguredACPowerMode(out powerModeGuid)
			: PInvoke.PowerGetUserConfiguredDCPowerMode(out powerModeGuid);

		if (result == 0)
		{
			return true;
		}

		result = PInvoke.PowerGetActualOverlayScheme(out powerModeGuid);
		return result == 0;
	}

	private static PowerModeSnapshot CreateSnapshot(
		UserPowerMode userMode,
		EffectivePowerMode? effective,
		CmdPal.Ext.Power.Enumerations.PowerSourceKind powerSourceKind,
		bool hasBattery,
		bool isOnAcPower,
		bool isCharging,
		bool canReadUserMode) =>
		new(
			userMode,
			effective,
			powerSourceKind,
			hasBattery,
			isOnAcPower,
			isCharging,
			canReadUserMode);
}
