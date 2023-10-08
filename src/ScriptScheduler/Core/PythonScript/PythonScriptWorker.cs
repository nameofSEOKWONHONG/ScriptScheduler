using Microsoft.Extensions.Options;

namespace ScriptScheduler.Core.PythonScript;

public class PythonScriptWorker : BackgroundService
{
    private readonly Serilog.ILogger _logger;
    private readonly PythonSetup _pythonSetup;
    private PythonExecutor _pythonExecutor;
    private PythonScriptOption _scriptOption;

    public PythonScriptWorker(Serilog.ILogger logger
    , PythonSetup pythonSetup
    , PythonExecutor pythonExecutor
    , IOptionsMonitor<PythonScriptOption> _optionsMonitor)
    {
        _logger = logger;
        _pythonSetup = pythonSetup;
        _pythonExecutor = pythonExecutor;
        _scriptOption = _optionsMonitor.CurrentValue;
        _optionsMonitor.OnChange(OptionChange);
    }

    private void OptionChange(PythonScriptOption obj)
    {
        _scriptOption = obj;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _pythonSetup.InitializeAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Information("Worker running at: {time}", DateTimeOffset.Now);
            await _pythonExecutor.ExecuteAsync(stoppingToken);
            await Task.Delay(1000 * _scriptOption.Interval, stoppingToken);
        }
    }
}