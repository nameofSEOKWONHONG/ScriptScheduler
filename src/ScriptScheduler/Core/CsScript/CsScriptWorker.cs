using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;

namespace ScriptScheduler.Core.CsScript;

public class CsScriptWorker : ScriptBaseBackgroundService<CsScriptOption>
{
    private readonly CsScriptSetup _csScriptSetup;
    private readonly CsScriptExecutor _csScriptExecutor;
    
    public CsScriptWorker(Serilog.ILogger logger
        , CsScriptSetup csScriptSetup
        , CsScriptExecutor csScriptExecutor
        , IOptionsMonitor<CsScriptOption> optionsMonitor)
    : base(logger, optionsMonitor, csScriptSetup)
    {
        _csScriptSetup = csScriptSetup;
        _csScriptExecutor = csScriptExecutor;
    }

    protected override async Task ExecuteCoreAsync(CancellationToken stoppingToken)
    {
        await _csScriptExecutor.ExecuteAsync(stoppingToken);
    }
}