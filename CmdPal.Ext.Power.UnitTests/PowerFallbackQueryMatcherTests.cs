// Copyright (c) JRScott812
// Licensed under the MIT License.

using CmdPal.Ext.Power.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests;

[TestClass]
public sealed class PowerFallbackQueryMatcherTests
{
    [TestMethod]
    public void Matches_PowerModeQuery_ReturnsTrue()
    {
        Assert.IsTrue(PowerFallbackQueryMatcher.Matches("power mode"));
    }

    [TestMethod]
    public void Matches_CatalogLabelQuery_ReturnsTrue()
    {
        Assert.IsTrue(PowerFallbackQueryMatcher.Matches("best performance"));
    }

    [TestMethod]
    public void Matches_SupplementalAlias_ReturnsTrue()
    {
        Assert.IsTrue(PowerFallbackQueryMatcher.Matches("powercfg"));
    }

    [TestMethod]
    public void Matches_UnrelatedQuery_ReturnsFalse()
    {
        Assert.IsFalse(PowerFallbackQueryMatcher.Matches("clipboard"));
    }

    [TestMethod]
    public void Matches_EmptyQuery_ReturnsFalse()
    {
        Assert.IsFalse(PowerFallbackQueryMatcher.Matches(string.Empty));
        Assert.IsFalse(PowerFallbackQueryMatcher.Matches("   "));
    }
}
