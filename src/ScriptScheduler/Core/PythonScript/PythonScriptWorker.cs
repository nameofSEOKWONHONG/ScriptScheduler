using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;

namespace ScriptScheduler.Core.PythonScript;

public class PythonScriptWorker : ScriptBaseBackgroundService<PythonScriptOption>
{
    private readonly PythonScriptSetup _pythonScriptSetup;
    private PythonScriptExecutor _pythonScriptExecutor;

    public PythonScriptWorker(Serilog.ILogger logger
    , IOptionsMonitor<PythonScriptOption> optionsMonitor
    , PythonScriptSetup pythonScriptSetup
    , PythonScriptExecutor pythonScriptExecutor) 
    : base(logger, optionsMonitor, pythonScriptSetup)
    {
        _pythonScriptSetup = pythonScriptSetup;
        _pythonScriptExecutor = pythonScriptExecutor;
    }

    protected override async Task ExecuteCoreAsync(CancellationToken stoppingToken)
    {
        await _pythonScriptExecutor.ExecuteAsync(stoppingToken);
    }
}