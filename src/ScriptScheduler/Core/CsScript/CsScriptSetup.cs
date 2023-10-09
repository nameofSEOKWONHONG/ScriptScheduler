using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;

namespace ScriptScheduler.Core.CsScript;

public class CsScriptSetup : ScriptSetupBase<CsScriptOption>
{
    private CsScriptOption _scriptOption;
    private readonly Serilog.ILogger _logger;
    public CsScriptSetup(Serilog.ILogger logger, IOptionsMonitor<CsScriptOption> optionsMonitor)
        : base(logger, optionsMonitor)
    {
        
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = new())
    {
        return Task.CompletedTask;
    }
}