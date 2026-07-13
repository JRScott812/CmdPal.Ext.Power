// Copyright (c) JRScott812
// Licensed under the MIT License.

using CmdPal.Ext.Power.Enumerations;
using Microsoft.Windows.System.Power;

namespace CmdPal.Ext.Power.Classes;

internal readonly record struct PowerModeSnapshot(
    UserPowerMode UserMode,
    EffectivePowerMode? EffectiveMode,
    Enumerations.PowerSourceKind PowerSourceKind,
    bool HasBattery,
    bool IsOnAcPower,
    bool IsCharging,
    bool CanReadUserMode);
