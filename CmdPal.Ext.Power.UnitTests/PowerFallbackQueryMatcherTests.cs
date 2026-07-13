using CmdPal.Ext.Power.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdPal.Ext.Power.UnitTests;

[TestClass]
public sealed class PowerFallbackQueryMatcherTests
{
	[TestMethod]
	public void MatchesPowerModeQueryReturnsTrue() => Assert.IsTrue(PowerFallbackQueryMatcher.Matches("power mode"));

	[TestMethod]
	public void MatchesCatalogLabelQueryReturnsTrue() => Assert.IsTrue(PowerFallbackQueryMatcher.Matches("best performance"));

	[TestMethod]
	public void MatchesSupplementalAliasReturnsTrue() => Assert.IsTrue(PowerFallbackQueryMatcher.Matches("powercfg"));

	[TestMethod]
	public void MatchesUnrelatedQueryReturnsFalse() => Assert.IsFalse(PowerFallbackQueryMatcher.Matches("clipboard"));

	[TestMethod]
	public void MatchesEmptyQueryReturnsFalse()
	{
		Assert.IsFalse(PowerFallbackQueryMatcher.Matches(string.Empty));
		Assert.IsFalse(PowerFallbackQueryMatcher.Matches("   "));
	}
}
