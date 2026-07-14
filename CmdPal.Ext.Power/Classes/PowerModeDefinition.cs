using System;

using CmdPal.Ext.Power.Enumerations;

namespace CmdPal.Ext.Power.Classes
{
	internal readonly record struct PowerModeDefinition(
		UserPowerMode Mode,
		Guid Guid,
		string Label,
		string ShortLabel);
}
