// Copyright (c) JRScott812
// Licensed under the MIT License.

using System;

namespace CmdPal.Ext.Power.Classes;

internal readonly record struct PowerPlanInfo(
    Guid SchemeGuid,
    string DisplayName,
    string Description);
