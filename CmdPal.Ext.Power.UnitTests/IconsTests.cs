using System;

using CmdPal.Ext.Power.Constants;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests
{
	[TestClass]
	public sealed class IconsTests
	{
		[TestMethod]
		public void PlanGlyphKnownPlansUsePlanSpecificIcons()
		{
			Assert.AreSame(Icons.PowerPlanSaverIcon, Icons.PlanGlyph(PowerPlanGuids.PowerSaver));
			Assert.AreSame(Icons.PowerPlanBalancedIcon, Icons.PlanGlyph(PowerPlanGuids.Balanced));
			Assert.AreSame(Icons.PowerPlanPerformanceIcon, Icons.PlanGlyph(PowerPlanGuids.HighPerformance));
			Assert.AreSame(Icons.PowerPlanUltimatePerformanceIcon, Icons.PlanGlyph(PowerPlanGuids.UltimatePerformance));
		}

		[TestMethod]
		public void PlanGlyphUnknownPlanUsesPowerPlanBandIcon()
		{
			Guid unknown = Guid.Parse("11111111-1111-1111-1111-111111111111");
			Assert.AreSame(Icons.PowerPlanBandIcon, Icons.PlanGlyph(unknown));
		}
	}
}
