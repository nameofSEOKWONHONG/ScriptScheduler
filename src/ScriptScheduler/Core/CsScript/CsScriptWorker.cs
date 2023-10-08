using Microsoft.Extensions.Options;

namespace ScriptScheduler.Core.CsScript;

public class CsScriptWorker : BackgroundService
{
    private readonly Serilog.ILogger _logger;
    private readonly CsScriptExecutor _csScriptExecutor;
    private readonly IOptionsMonitor<CsScriptOption> _optionsMonitor;
    private CsScriptOption _option;
    
    public CsScriptWorker(Serilog.ILogger logger
    , CsScriptExecutor csScriptExecutor
    , IOptionsMonitor<CsScriptOption> optionsMonitor)
    {
        _logger = logger;
        _csScriptExecutor = csScriptExecutor;
        _optionsMonitor = optionsMonitor;
        _option = _optionsMonitor.CurrentValue;
        _optionsMonitor.OnChange(Listener);
    }

    private void Listener(CsScriptOption obj)
    {
        _option = obj;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Information("Worker running at: {time}", DateTimeOffset.Now);
            await _csScriptExecutor.ExecuteAsync(stoppingToken);
            await Task.Delay(1000 * _option.Interval, stoppingToken);
        }
    }
}