using System;
using System.Collections.Generic;
using System.Linq;

using CmdPal.Ext.Power.Classes;
using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power.Pages
{
	internal sealed partial class PowerPlanPickerPage : OnLoadStaticListPage
	{
		public override string Id => "com.jrscott812.cmdpal.power.planPicker";

		private readonly PowerPlanService _powerPlanService;
		private readonly PowerModeDataManager _dataManager;
		private readonly PowerListItemBuilder _itemBuilder;
		private readonly Action _onChanged;

		private IListItem[] _items = [];
		private List<ListItem> _planItems = [];
		private Guid[] _cachedPlanGuids = [];

		internal PowerPlanPickerPage(
			PowerPlanService powerPlanService,
			PowerModeDataManager dataManager,
			PowerListItemBuilder itemBuilder,
			Action onChanged)
		{
			_powerPlanService = powerPlanService;
			_dataManager = dataManager;
			_itemBuilder = itemBuilder;
			_onChanged = onChanged;
			Title = Resources.power_section_power_plan;
			Name = Resources.power_section_power_plan;
			Icon = Icons.PowerPlanBandIcon;

			RebuildItems(force: true);
		}

		public override IListItem[] GetItems()
		{
			RebuildItems(force: false);
			return _items;
		}

		protected override void Loaded()
		{
			_dataManager.PushActivate();
			RefreshPresentation();
		}

		protected override void Unloaded() => _dataManager.PopActivate();

		internal void HandleLiveStateChanged()
		{
			RebuildItems(force: false);
			RefreshPresentation();
		}

		private void RebuildItems(bool force)
		{
			PowerPlanSnapshot snapshot = PowerPlanService.GetSnapshot();
			if (!snapshot.CanReadPlans)
			{
				if (_planItems.Count > 0)
				{
					_planItems = [];
					_cachedPlanGuids = [];
					_items = [];
					RaiseItemsChanged(0);
				}

				return;
			}

			Guid[] planGuids = [.. snapshot.AvailablePlans.Select(plan => plan.SchemeGuid)];
			if (!force && _cachedPlanGuids.SequenceEqual(planGuids))
			{
				RefreshPresentation();
				return;
			}

			bool structureChanged = _cachedPlanGuids.Length > 0 && !_cachedPlanGuids.SequenceEqual(planGuids);
			_cachedPlanGuids = planGuids;
			_planItems = [.. snapshot.AvailablePlans.Select(plan => _itemBuilder.CreatePlanItem(plan, snapshot, _onChanged, dismissOnSuccess: true))];
			_items = [.. _planItems];

			if (structureChanged)
			{
				RaiseItemsChanged(_items.Length);
			}
		}

		private void RefreshPresentation()
		{
			PowerPlanSnapshot snapshot = PowerPlanService.GetSnapshot();
			for (int i = 0; i < _planItems.Count && i < snapshot.AvailablePlans.Count; i++)
			{
				PowerListItemBuilder.RefreshPlanItem(_planItems[i], snapshot.AvailablePlans[i], snapshot);
			}
		}
	}
}
