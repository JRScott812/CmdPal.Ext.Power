using System.Linq;

using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Pages;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class PowerDockPageTests
	{
		[TestMethod]
		public void ModeDockPageHasScopedId()
		{
			TestContext context = CreateContext();
			PowerDockPage page = CreateDockPage(context, PowerDockScope.Mode);

			Assert.AreEqual("com.jrscott812.cmdpal.power.mode", page.Id);
		}

		[TestMethod]
		public void PlanDockPageHasScopedId()
		{
			TestContext context = CreateContext();
			PowerDockPage page = CreateDockPage(context, PowerDockScope.Plan);

			Assert.AreEqual("com.jrscott812.cmdpal.power.plan", page.Id);
		}

		[TestMethod]
		public void ModeDockPageHasSingleItemWithoutSeparators()
		{
			TestContext context = CreateContext();
			PowerDockPage page = CreateDockPage(context, PowerDockScope.Mode);
			IListItem[] items = page.GetItems();

			Assert.IsTrue(items.Length is 0 or 1);
			Assert.DoesNotContain(item => item is Separator, items);
		}

		[TestMethod]
		public void PlanDockPageHasSingleItemWithoutSeparators()
		{
			TestContext context = CreateContext();
			PowerDockPage page = CreateDockPage(context, PowerDockScope.Plan);
			IListItem[] items = page.GetItems();

			Assert.IsTrue(items.Length is 0 or 1);
			Assert.DoesNotContain(item => item is Separator, items);
		}

		[TestMethod]
		public void ModePickerPageHasExpectedItemCount()
		{
			TestContext context = CreateContext();
			PowerModePickerPage picker = CreateModePicker(context);
			IListItem[] items = picker.GetItems();

			Assert.IsTrue(items.Length is 0 or 3);
		}

		[TestMethod]
		public void ProviderGetDockBandsIncludesComponentTitles()
		{
			using PowerCommandsProvider provider = new();
			ICommandItem[]? bands = provider.GetDockBands();

			Assert.IsNotNull(bands);
			Assert.IsGreaterThanOrEqualTo(1, bands!.Length);

			string[] titles = [.. bands.Select(b => b.Title)];
			if (bands.Length >= 2)
			{
				Assert.Contains(Resources.power_mode_dock_band_title, titles);
				Assert.Contains(Resources.power_plan_dock_band_title, titles);
			}
		}

		private sealed record TestContext(
			PowerModeService PowerModeService,
			PowerPlanService PowerPlanService,
			PowerModeDataManager DataManager,
			PowerListItemBuilder ItemBuilder,
			PowerModePickerPage ModePicker,
			PowerPlanPickerPage PlanPicker);

		private static TestContext CreateContext()
		{
			PowerModeService powerModeService = new();
			PowerPlanService powerPlanService = new();
			PowerListItemBuilder itemBuilder = new(powerModeService, powerPlanService);
			PowerModePickerPage modePicker = null!;
			PowerPlanPickerPage planPicker = null!;
			PowerModeDataManager dataManager = new(
				powerModeService,
				() =>
				{
					modePicker?.HandleLiveStateChanged();
					planPicker?.HandleLiveStateChanged();
				});

			modePicker = new PowerModePickerPage(powerModeService, dataManager, itemBuilder, () => { });
			planPicker = new PowerPlanPickerPage(powerPlanService, dataManager, itemBuilder, () => { });

			return new TestContext(
				powerModeService,
				powerPlanService,
				dataManager,
				itemBuilder,
				modePicker,
				planPicker);
		}

		private static PowerDockPage CreateDockPage(TestContext context, PowerDockScope scope) =>
			new(
				scope,
				context.PowerModeService,
				context.PowerPlanService,
				context.ModePicker,
				context.PlanPicker,
				context.DataManager);

		private static PowerModePickerPage CreateModePicker(TestContext context) => context.ModePicker;
	}
}
