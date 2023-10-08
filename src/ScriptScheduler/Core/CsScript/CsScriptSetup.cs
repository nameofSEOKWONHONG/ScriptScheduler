using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ScriptScheduler.Core.CsScript;

public class CsScriptSetup
{
    private CsScriptOption _scriptOption;
    private readonly Serilog.ILogger _logger;
    public CsScriptSetup(Serilog.ILogger logger
        , IOptionsMonitor<CsScriptOption> options)
    {
        _logger = logger;
        _scriptOption = options.CurrentValue;
    }

    public Task InitializeAsync(CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }
}