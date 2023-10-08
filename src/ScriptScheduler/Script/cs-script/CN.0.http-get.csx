/*
# nameing example
## [MODE].[TIME].[ROLE_NAME].[EXTENSION]
## MODE
### CN : CONTINUE
### DL : DAILY (TIME)
### ON : ONCE (delete script file after once execute)
## TIME (4 letters)
### C : 0
### D : 1200
## ROLE_NAME : filename 
## EXTENSION : py, csx
*/

using System;
using ScriptScheduler.Core.CsScript;
using System.Threading.Tasks;
using System.Net.Http;

public class HttpGetScript : CsScriptRunnerBase
{
    private string _result;
    public override async Task OnProducerAsync()
    {    
        using var client = new HttpClient();
        var res = await client.GetAsync("https://google.com");
        _result = await res.Content.ReadAsStringAsync();
    }

    public override Task OnConsumerAsync()
    {
        Console.WriteLine(_result);
        return Task.CompletedTask;
    }
}