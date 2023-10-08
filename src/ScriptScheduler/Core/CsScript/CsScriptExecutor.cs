using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CSScriptLib;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;
using ScriptScheduler.Core.PythonScript;

namespace ScriptScheduler.Core.CsScript;

public class CsScriptExecutor : IScriptExecutor
{
    private readonly Serilog.ILogger _logger;
    private readonly IOptionsMonitor<CsScriptOption> _optionsMonitor;
    private CsScriptOption _scriptOption;
    
    public CsScriptExecutor(Serilog.ILogger logger
        , IOptionsMonitor<CsScriptOption> optionsMonitor)
    {
        _logger = logger;
        _scriptOption = optionsMonitor.CurrentValue;
        _optionsMonitor = optionsMonitor;
        _optionsMonitor.OnChange(OptionChange);
    }
    
    private void OptionChange(CsScriptOption obj)
    {
        _scriptOption = obj;
    }
    
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var dirs = Directory.GetDirectories(_scriptOption.ScriptPath).Where(m => m.Contains("cs-script"));
        await Parallel.ForEachAsync(dirs, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 4,
            TaskScheduler = TaskScheduler.Default
        }, async (dir, token) =>
        {
            var files = Directory.GetFiles(dir, "*.csx", SearchOption.AllDirectories);
            foreach (var file in files.Where(m => !m.Contains(".done")))
            {
                await ExecuteCoreAsync(file, cancellationToken);
            }
        });
    }

    private async ValueTask ExecuteCoreAsync(string file, CancellationToken cancellationToken)
    {
        if(cancellationToken.IsCancellationRequested)
            _logger.Information("{File} canceled", file);
        
        _logger.Information("{File} executing", file);
        
        var code = await File.ReadAllTextAsync(file);

        try
        {
            ICsScriptRunner runner = CSScript.Evaluator
                .ReferenceAssembliesFromCode(code)
                .ReferenceAssembly(Assembly.GetExecutingAssembly())
                .ReferenceAssembly(Assembly.GetExecutingAssembly().Location)
                .ReferenceDomainAssemblies()
                .LoadCode<ICsScriptRunner>(code);
            await runner.OnProducerAsync();
            await runner.OnConsumerAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, "{File} Error: {Error}", file, e.Message);
            
            #if DEBUG
            Environment.Exit(-1);
            #endif
        }
        _logger.Information("{File} executed", file);
    }
}