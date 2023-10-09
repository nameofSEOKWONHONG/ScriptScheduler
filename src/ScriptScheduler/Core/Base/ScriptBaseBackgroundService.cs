using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ScriptScheduler.Core.Base;

public abstract class ScriptBaseBackgroundService<T> : BackgroundService
where T : ScriptOptionBase
{
    protected readonly Serilog.ILogger Logger;
    protected T Option;
    
    private readonly IOptionsMonitor<T> _optionsMonitor;
    private readonly ScriptSetupBase<T> _scriptSetupBase;

    protected ScriptBaseBackgroundService(Serilog.ILogger logger, IOptionsMonitor<T> optionsMonitor, ScriptSetupBase<T> scriptSetup)
    {
        this.Logger = logger;
        this._optionsMonitor = optionsMonitor;
        this._optionsMonitor.OnChange(OptionChange);
        this.Option = this._optionsMonitor.CurrentValue;
        this._scriptSetupBase = scriptSetup;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this._scriptSetupBase.InitializeAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            this.Logger.Information("Worker running at: {time}", DateTimeOffset.Now);
            ChangeFileDoneToIng(this.Option.ScriptPath, this.Option.ExecutorPathName);
            await ExecuteCoreAsync(stoppingToken);
            await Task.Delay(1000 * this.Option.Interval, stoppingToken);
        }
    }

    protected abstract Task ExecuteCoreAsync(CancellationToken stoppingToken);
    
    private void OptionChange(T obj)
    {
        this.Option = obj;
    }
    
    private void ChangeFileDoneToIng(string scriptPath, string typePath)
    {
        var now = DateTime.Now;
        //AM 00
        if (now.Hour is >= 0 and < 1) 
        {
            var dirs = Directory.GetDirectories(scriptPath).Where(m => m.Contains(typePath));
            foreach (var dir in dirs)
            {
                var files = Directory.GetFiles(dir, "*.done", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    File.Move(file, file.Replace(".done", string.Empty));
                }
            }
        }
    }
}