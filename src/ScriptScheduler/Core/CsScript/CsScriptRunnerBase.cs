namespace ScriptScheduler.Core.CsScript;

public abstract class CsScriptRunnerBase : ICsScriptRunner
{
    public abstract Task OnProducerAsync();

    public abstract Task OnConsumerAsync();
}

public abstract class CsScriptRunnerBase<TRequest, TResult> : ICsScriptRunner<TRequest, TResult>
{
    public TRequest Request { get; set; }
    public TResult Result { get; set; }

    
    protected CsScriptRunnerBase()
    {
        
    }
    
    public abstract Task OnProducerAsync();

    public abstract Task OnConsumerAsync();
}