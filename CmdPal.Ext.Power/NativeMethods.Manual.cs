using System;
using System.Runtime.InteropServices;

namespace Windows.Win32;

/// <summary>
/// Power Profile (powrprof) overlay / power-mode APIs not always available in CsWin32 metadata.
/// </summary>
internal static partial class PInvoke
{
	[LibraryImport("powrprof.dll")]
	internal static partial uint PowerGetActualOverlayScheme(out Guid actualOverlayGuid);

	[LibraryImport("powrprof.dll")]
	internal static partial uint PowerGetEffectiveOverlayScheme(out Guid effectiveOverlayGuid);

	[LibraryImport("powrprof.dll")]
	internal static partial uint PowerSetActiveOverlayScheme(ref Guid overlaySchemeGuid);

	[LibraryImport("powrprof.dll")]
	internal static partial uint PowerGetUserConfiguredACPowerMode(out Guid powerModeGuid);

	[LibraryImport("powrprof.dll")]
	internal static partial uint PowerGetUserConfiguredDCPowerMode(out Guid powerModeGuid);

	[LibraryImport("powrprof.dll")]
	internal static partial uint PowerSetUserConfiguredACPowerMode(in Guid powerModeGuid);

	[LibraryImport("powrprof.dll")]
	internal static partial uint PowerSetUserConfiguredDCPowerMode(in Guid powerModeGuid);
}
