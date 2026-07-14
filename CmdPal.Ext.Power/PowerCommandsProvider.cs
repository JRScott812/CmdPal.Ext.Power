using System;
using System.Collections.Generic;

using CmdPal.Ext.Power.Helpers;
using CmdPal.Ext.Power.Pages;
using CmdPal.Ext.Power.Properties;

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Power
{
	public sealed partial class PowerCommandsProvider : CommandProvider, IDisposable
	{
		private readonly PowerModeService _powerModeService = new();
		private readonly PowerPlanService _powerPlanService = new();
		private readonly PowerListItemBuilder _itemBuilder;
		private PowerModeDataManager? _dataManager;
		private CommandItem? _command;
		private PowerListPage? _listPage;
		private PowerModePickerPage? _modePickerPage;
		private PowerPlanPickerPage? _planPickerPage;
		private PowerDockPage? _modeDockPage;
		private PowerDockPage? _planDockPage;
		private FallbackPowerItem? _fallback;

		public PowerCommandsProvider()
		{
			DisplayName = Resources.power_display_name;
			Id = "com.jrscott812.cmdpal.power";
			Icon = Icons.PowerExtensionIcon;

			_itemBuilder = new PowerListItemBuilder(_powerModeService, _powerPlanService);
		}

		public override ICommandItem[] TopLevelCommands() => [TopLevelCommand()];

		public override IFallbackCommandItem[] FallbackCommands() => [FallbackCommand()];

		public override ICommandItem[]? GetDockBands()
		{
			List<ICommandItem> bands = [];

			if (PowerModeService.SupportsPowerModeControl())
			{
				bands.Add(new CommandItem(GetModeDockPage())
				{
					Title = Resources.power_mode_dock_band_title,
					Icon = Icons.PowerModeBandIcon,
				});
			}

			if (PowerPlanService.GetSnapshot().CanReadPlans)
			{
				bands.Add(new CommandItem(GetPlanDockPage())
				{
					Title = Resources.power_plan_dock_band_title,
					Icon = Icons.PowerPlanBandIcon,
				});
			}

			return bands.Count > 0 ? bands.ToArray() : null;
		}

		public override void Dispose()
		{
			_dataManager?.Dispose();
			_powerModeService.Dispose();
			GC.SuppressFinalize(this);
			base.Dispose();
		}

		private CommandItem TopLevelCommand()
		{
			if (_command is not null)
			{
				return _command;
			}

			CommandItem command = new(GetListPage())
			{
				Title = Resources.power_page_title,
				Icon = Icons.PowerExtensionIcon,
			};

			_command = command;
			return command;
		}

		private FallbackPowerItem FallbackCommand()
		{
			if (_fallback is not null)
			{
				return _fallback;
			}

			FallbackPowerItem fallback = new(GetListPage());
			_fallback = fallback;
			return fallback;
		}

		private PowerListPage GetListPage()
		{
			if (_listPage is not null)
			{
				return _listPage;
			}

			PowerListPage listPage = new(_powerModeService, _powerPlanService, GetDataManager(), _itemBuilder);
			_listPage = listPage;
			return listPage;
		}

		private PowerModePickerPage GetModePickerPage()
		{
			if (_modePickerPage is not null)
			{
				return _modePickerPage;
			}

			PowerModePickerPage modePickerPage = new(_powerModeService, GetDataManager(), _itemBuilder, HandleLiveStateChanged);
			_modePickerPage = modePickerPage;
			return modePickerPage;
		}

		private PowerPlanPickerPage GetPlanPickerPage()
		{
			if (_planPickerPage is not null)
			{
				return _planPickerPage;
			}

			PowerPlanPickerPage planPickerPage = new(_powerPlanService, GetDataManager(), _itemBuilder, HandleLiveStateChanged);
			_planPickerPage = planPickerPage;
			return planPickerPage;
		}

		private PowerDockPage GetModeDockPage()
		{
			if (_modeDockPage is not null)
			{
				return _modeDockPage;
			}

			PowerDockPage modeDockPage = new(
				PowerDockScope.Mode,
				_powerModeService,
				_powerPlanService,
				GetModePickerPage(),
				GetPlanPickerPage(),
				GetDataManager());
			_modeDockPage = modeDockPage;
			return modeDockPage;
		}

		private PowerDockPage GetPlanDockPage()
		{
			if (_planDockPage is not null)
			{
				return _planDockPage;
			}

			PowerDockPage planDockPage = new(
				PowerDockScope.Plan,
				_powerModeService,
				_powerPlanService,
				GetModePickerPage(),
				GetPlanPickerPage(),
				GetDataManager());
			_planDockPage = planDockPage;
			return planDockPage;
		}

		private PowerModeDataManager GetDataManager()
		{
			if (_dataManager is not null)
			{
				return _dataManager;
			}

			PowerModeDataManager dataManager = new(_powerModeService, HandleLiveStateChanged);
			_dataManager = dataManager;
			return dataManager;
		}

		private void HandleLiveStateChanged()
		{
			_listPage?.HandleLiveStateChanged();
			_modePickerPage?.HandleLiveStateChanged();
			_planPickerPage?.HandleLiveStateChanged();
			_modeDockPage?.HandleLiveStateChanged();
			_planDockPage?.HandleLiveStateChanged();
		}
	}
}
