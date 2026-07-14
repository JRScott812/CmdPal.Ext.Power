using System;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CommandPalette.Extensions;

namespace CmdPal.Ext.Power
{
	[Guid("6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE")]
	public sealed partial class CmdPalExtPower : IExtension, IDisposable
	{
		private readonly ManualResetEvent _extensionDisposedEvent;
		private PowerCommandsProvider? _provider;

		public CmdPalExtPower(ManualResetEvent extensionDisposedEvent) => _extensionDisposedEvent = extensionDisposedEvent;

		public object? GetProvider(ProviderType providerType)
		{
			return providerType switch
			{
				ProviderType.Commands => GetCommandsProvider(),
				_ => null,
			};
		}

		public void Dispose() => _extensionDisposedEvent.Set();

		private PowerCommandsProvider? GetCommandsProvider()
		{
			if (_provider is not null)
			{
				return _provider;
			}

			try
			{
				_provider ??= new PowerCommandsProvider();
				return _provider;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				return null;
			}
		}
	}
}
