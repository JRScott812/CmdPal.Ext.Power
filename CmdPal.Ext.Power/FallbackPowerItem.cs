using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Pages;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power;

internal sealed partial class FallbackPowerItem : FallbackCommandItem
{
	private const string FallbackId = "com.jrscott812.cmdpal.power.fallback";

	private readonly PowerListPage _listPage;

	public FallbackPowerItem(PowerListPage listPage)
		: base(listPage, Resources.power_fallback_title, FallbackId)
	{
		_listPage = listPage;
		Clear();
		Icon = Icons.PowerExtensionIcon;
	}

	public override void UpdateQuery(string query)
	{
		if (!PowerFallbackQueryMatcher.Matches(query))
		{
			Clear();
			return;
		}

		Command = _listPage;
		Title = Resources.power_fallback_title;
		Subtitle = Resources.power_fallback_subtitle;
		Icon = Icons.PowerExtensionIcon;
	}

	private void Clear()
	{
		Command = null;
		Title = string.Empty;
		Subtitle = string.Empty;
	}
}
