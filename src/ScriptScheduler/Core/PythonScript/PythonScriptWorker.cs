using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;

namespace ScriptScheduler.Core.PythonScript;

public class PythonScriptWorker : ScriptBaseBackgroundService
{
    private readonly Serilog.ILogger _logger;
    private readonly PythonScriptSetup _pythonScriptSetup;
    private PythonScriptExecutor _pythonScriptExecutor;
    private PythonScriptOption _scriptOption;

    public PythonScriptWorker(Serilog.ILogger logger
    , PythonScriptSetup pythonScriptSetup
    , PythonScriptExecutor pythonScriptExecutor
    , IOptionsMonitor<PythonScriptOption> _optionsMonitor)
    {
        _logger = logger;
        _pythonScriptSetup = pythonScriptSetup;
        _pythonScriptExecutor = pythonScriptExecutor;
        _scriptOption = _optionsMonitor.CurrentValue;
        _optionsMonitor.OnChange(OptionChange);
    }

    private void OptionChange(PythonScriptOption obj)
    {
        _scriptOption = obj;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _pythonScriptSetup.InitializeAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            this.ChangeFileDoneToIng(_scriptOption.ScriptPath, "python-script");
            _logger.Information("Worker running at: {time}", DateTimeOffset.Now);
            await _pythonScriptExecutor.ExecuteAsync(stoppingToken);
            await Task.Delay(1000 * _scriptOption.Interval, stoppingToken);
        }
    }
    

}