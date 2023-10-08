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

public class HttpGetScript : CsScriptRunnerBase<string, string>
{
    public override async Task OnProducerAsync()
    {    
        using var client = new HttpClient();
        var res = await client.GetAsync("https://google.com");
        this.Request = await res.Content.ReadAsStringAsync();
    }

    public override Task OnConsumerAsync()
    {
        Console.WriteLine(this.Result);
        return Task.CompletedTask;
    }
}