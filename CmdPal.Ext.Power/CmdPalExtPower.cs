using System;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CommandPalette.Extensions;

namespace CmdPal.Ext.Power
{
	[Guid("6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE")]
	public sealed partial class CmdPalExtPower(ManualResetEvent extensionDisposedEvent) : IExtension, IDisposable
	{
		private readonly ManualResetEvent _extensionDisposedEvent = extensionDisposedEvent;
		private PowerCommandsProvider? _provider;

		public object? GetProvider(ProviderType providerType) => providerType switch
		{
			ProviderType.Commands => GetCommandsProvider(),
			_ => null,
		};

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
