using System;
using System.Collections.Generic;
using System.Text;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Properties;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;

namespace CmdPal.Ext.Power.Helpers;

internal sealed partial class PowerPlanService
{
	internal PowerPlanSnapshot GetSnapshot()
	{
		if (!TryEnumeratePlans(out List<PowerPlanInfo>? plans))
		{
			return new PowerPlanSnapshot(
				null,
				Array.Empty<PowerPlanInfo>(),
				CanReadPlans: false,
				CanSetPlans: false);
		}

		PowerPlanInfo? activePlan = null;
		if (TryGetActivePlanGuid(out Guid activeGuid))
		{
			foreach (PowerPlanInfo plan in plans)
			{
				if (plan.SchemeGuid == activeGuid)
				{
					activePlan = plan;
					break;
				}
			}

			if (activePlan is null && TryReadFriendlyName(activeGuid, out string? activeName))
			{
				activePlan = CreatePlanInfo(activeGuid, activeName);
			}
		}

		return new PowerPlanSnapshot(
			activePlan,
			plans,
			CanReadPlans: true,
			CanSetPlans: true);
	}

	internal bool TrySetActivePlan(Guid schemeGuid, out string? errorMessage)
	{
		errorMessage = null;
		if (PInvoke.PowerSetActiveScheme(null, schemeGuid) == WIN32_ERROR.NO_ERROR)
		{
			return true;
		}

		errorMessage = Resources.power_plan_set_failed;
		return false;
	}

	private static bool TryEnumeratePlans(out List<PowerPlanInfo> plans)
	{
		plans = [];
		for (uint index = 0; ; index++)
		{
			if (!TryEnumerateSchemeGuid(index, out Guid schemeGuid))
			{
				break;
			}

			if (!TryReadFriendlyName(schemeGuid, out string? displayName))
			{
				displayName = schemeGuid.ToString("B");
			}

			plans.Add(CreatePlanInfo(schemeGuid, displayName));
		}

		PowerPlanCatalog.SortBySpeed(plans);

		return plans.Count > 0;
	}

	private static bool TryEnumerateSchemeGuid(uint index, out Guid schemeGuid)
	{
		schemeGuid = Guid.Empty;
		uint bufferSize = 0u;
		WIN32_ERROR result = PInvoke.PowerEnumerate(
			null,
			null,
			null,
			POWER_DATA_ACCESSOR.ACCESS_SCHEME,
			index,
			default,
			ref bufferSize);

		if (result == WIN32_ERROR.ERROR_NO_MORE_ITEMS)
		{
			return false;
		}

		if (result is not WIN32_ERROR.NO_ERROR and not WIN32_ERROR.ERROR_MORE_DATA)
		{
			return false;
		}

		if (bufferSize == 0)
		{
			return false;
		}

		byte[] buffer = new byte[bufferSize];
		result = PInvoke.PowerEnumerate(
			null,
			null,
			null,
			POWER_DATA_ACCESSOR.ACCESS_SCHEME,
			index,
			buffer.AsSpan(),
			ref bufferSize);

		if (result != WIN32_ERROR.NO_ERROR)
		{
			return false;
		}

		schemeGuid = new Guid(buffer);
		return true;
	}

	private static bool TryGetActivePlanGuid(out Guid schemeGuid)
	{
		schemeGuid = Guid.Empty;
		unsafe
		{
			if (PInvoke.PowerGetActiveScheme(null, out Guid* activePolicyGuid) != WIN32_ERROR.NO_ERROR)
			{
				return false;
			}

			try
			{
				schemeGuid = *activePolicyGuid;
				return true;
			}
			finally
			{
				_ = PInvoke.LocalFree((HLOCAL)activePolicyGuid);
			}
		}
	}

	private static bool TryReadFriendlyName(Guid schemeGuid, out string friendlyName)
	{
		friendlyName = string.Empty;
		uint bufferSize = 0u;
		WIN32_ERROR result = PInvoke.PowerReadFriendlyName(
			null,
			schemeGuid,
			null,
			null,
			default,
			ref bufferSize);

		if (result is not WIN32_ERROR.NO_ERROR and not WIN32_ERROR.ERROR_MORE_DATA)
		{
			return false;
		}

		if (bufferSize == 0)
		{
			return false;
		}

		byte[] buffer = new byte[bufferSize];
		result = PInvoke.PowerReadFriendlyName(
			null,
			schemeGuid,
			null,
			null,
			buffer.AsSpan(),
			ref bufferSize);

		if (result != WIN32_ERROR.NO_ERROR)
		{
			return false;
		}

		friendlyName = Encoding.Unicode.GetString(buffer.AsSpan(0, (int)bufferSize)).TrimEnd('\0');
		return friendlyName.Length > 0;
	}

	private static PowerPlanInfo CreatePlanInfo(Guid schemeGuid, string displayName)
	{
		string description = PowerPlanCatalog.TryGetKnownDescription(schemeGuid, out string? knownDescription)
			? knownDescription
			: string.Empty;

		return new PowerPlanInfo(schemeGuid, displayName, description);
	}
}
