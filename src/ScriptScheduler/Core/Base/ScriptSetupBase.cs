using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.Options;

namespace ScriptScheduler.Core.Base;

public abstract class ScriptSetupBase<T>
where T : ScriptOptionBase
{
    protected T Option;
    protected readonly Serilog.ILogger Logger;
    private readonly IOptionsMonitor<T> _optionsMonitor;
    public ScriptSetupBase(Serilog.ILogger logger
        , IOptionsMonitor<T> optionsMonitor)
    {
        Logger = logger;
        _optionsMonitor = optionsMonitor;
        _optionsMonitor.OnChange(OptionChange);
        Option = _optionsMonitor.CurrentValue;
    }

    private void OptionChange(T obj)
    {
        this.Option = obj;
    }

    public abstract Task InitializeAsync(CancellationToken cancellationToken = new());
}