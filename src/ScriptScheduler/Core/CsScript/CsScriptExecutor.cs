using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CSScriptLib;
using Microsoft.Extensions.Options;
using ScriptScheduler.Core.Base;

namespace ScriptScheduler.Core.CsScript;

public class CsScriptExecutor : ScriptExecutorBase<CsScriptOption>
{   
    public CsScriptExecutor(Serilog.ILogger logger, IOptionsMonitor<CsScriptOption> optionsMonitor)
        : base(logger, optionsMonitor)
    {
    }
    
    protected override async Task ExecuteCoreAsync(string file, CancellationToken cancellationToken)
    {
        if(cancellationToken.IsCancellationRequested)
            this.Logger.Information("{File} canceled", file);
        
        this.Logger.Information("{File} executing", file);
        
        var code = await File.ReadAllTextAsync(file, cancellationToken);

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
            this.Logger.Error(e, "{File} Error: {Error}", file, e.Message);
            
            #if DEBUG
            Environment.Exit(-1);
            #endif
        }
        this.Logger.Information("{File} executed", file);
    }
}