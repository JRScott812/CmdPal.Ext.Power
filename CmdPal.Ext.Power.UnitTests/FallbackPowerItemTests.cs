// Copyright (c) JRScott812
// Licensed under the MIT License.

using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Pages;
using CmdPal.Ext.Power.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests;

[TestClass]
public sealed class FallbackPowerItemTests
{
    [TestMethod]
    public void UpdateQuery_MatchingTerm_OpensListPage()
    {
        var listPage = CreateListPage();
        var fallback = new FallbackPowerItem(listPage);

        fallback.UpdateQuery("power mode");

        Assert.AreEqual(listPage, fallback.Command);
        Assert.AreEqual(Resources.power_fallback_title, fallback.Title);
        Assert.AreEqual(Resources.power_fallback_subtitle, fallback.Subtitle);
    }

    [TestMethod]
    public void UpdateQuery_NonMatchingTerm_ClearsCommand()
    {
        var listPage = CreateListPage();
        var fallback = new FallbackPowerItem(listPage);

        fallback.UpdateQuery("clipboard");

        Assert.IsNull(fallback.Command);
        Assert.AreEqual(string.Empty, fallback.Title);
        Assert.AreEqual(string.Empty, fallback.Subtitle);
    }

    private static PowerListPage CreateListPage()
    {
        var service = new PowerModeService();
        var powerPlanService = new PowerPlanService();
        var dataManager = new PowerModeDataManager(service, () => { });
        var itemBuilder = new PowerListItemBuilder(service, powerPlanService);
        return new PowerListPage(service, powerPlanService, dataManager, itemBuilder);
    }
}
