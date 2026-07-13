// Copyright (c) JRScott812
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace CmdPal.Ext.Power.Classes;

internal readonly record struct PowerPlanSnapshot(
    PowerPlanInfo? ActivePlan,
    IReadOnlyList<PowerPlanInfo> AvailablePlans,
    bool CanReadPlans,
    bool CanSetPlans);
