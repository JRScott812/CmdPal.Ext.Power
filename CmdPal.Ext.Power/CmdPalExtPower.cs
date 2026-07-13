using System;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CommandPalette.Extensions;

namespace CmdPal.Ext.Power;

[Guid("6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE")]
public sealed partial class CmdPalExtPower : IExtension, IDisposable
{
	private readonly ManualResetEvent _extensionDisposedEvent;

	private readonly PowerCommandsProvider _provider = new();

	public CmdPalExtPower(ManualResetEvent extensionDisposedEvent) => _extensionDisposedEvent = extensionDisposedEvent;

	public object? GetProvider(ProviderType providerType)
	{
		return providerType switch
		{
			ProviderType.Commands => _provider,
			_ => null,
		};
	}

	public void Dispose() => _extensionDisposedEvent.Set();
}
