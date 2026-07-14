using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Pages;
using CmdPal.Ext.Power.Properties;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class FallbackPowerItemTests
	{
		[TestMethod]
		public void UpdateQueryMatchingTermOpensListPage()
		{
			PowerListPage listPage = CreateListPage();
			FallbackPowerItem fallback = new(listPage);

			fallback.UpdateQuery("power mode");

			Assert.AreEqual(listPage, fallback.Command);
			Assert.AreEqual(Resources.power_fallback_title, fallback.Title);
			Assert.AreEqual(Resources.power_fallback_subtitle, fallback.Subtitle);
		}

		[TestMethod]
		public void UpdateQueryNonMatchingTermClearsCommand()
		{
			PowerListPage listPage = CreateListPage();
			FallbackPowerItem fallback = new(listPage);

			fallback.UpdateQuery("clipboard");

			Assert.IsNull(fallback.Command);
			Assert.AreEqual(string.Empty, fallback.Title);
			Assert.AreEqual(string.Empty, fallback.Subtitle);
		}

		private static PowerListPage CreateListPage()
		{
			PowerModeService service = new();
			PowerPlanService powerPlanService = new();
			PowerModeDataManager dataManager = new(service, () => { });
			PowerListItemBuilder itemBuilder = new(service, powerPlanService);
			return new PowerListPage(service, powerPlanService, dataManager, itemBuilder);
		}
	}
}
