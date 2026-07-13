using System;

namespace CmdPal.Ext.Power.Classes;

internal readonly record struct PowerPlanInfo(
    Guid SchemeGuid,
    string DisplayName,
    string Description);
