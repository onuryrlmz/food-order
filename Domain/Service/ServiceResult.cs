using System.Runtime.Serialization;

namespace Domain.Service;

[DataContract(IsReference = true)]
public class ServiceResult : IDisposable
{
    public ServiceResult()
    {
        Messages = new List<ServiceResultMessage>();
    }

    private static bool KeepRawException { get; set; } = true;

    [DataMember]
    public List<ServiceResultMessage> Messages { get; set; }

    [DataMember]
    public string Token { get; set; }

    [DataMember]
    public bool HasFailed { get; set; }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Fail()
    {
        HasFailed = true;
    }

    public void AddSuccessMessage(string message)
    {
        AddSuccessMessage("", message);
    }

    private void AddSuccessMessage(string code, string message)
    {
        Messages.Add(new ServiceResultMessage { Code = code, Description = message, IsSuccess = true });
    }

    public void AddWarningMessage(string message)
    {
        AddWarningMessage("", message);
    }

    private void AddWarningMessage(string code, string message)
    {
        Messages.Add(new ServiceResultMessage { Code = code, Description = message, IsWarning = true });
        WriteLog();
    }

    public void AddErrorMessage(string message)
    {
        AddErrorMessage("", message);
    }

    private void AddErrorMessage(string code, string message)
    {
        Fail();
        Messages.Add(new ServiceResultMessage { Code = code, Description = message, IsError = true });
        WriteLog();
    }

    private void WriteLog()
    {
    }

    public void Fail(ServiceResult result)
    {
        Messages.AddRange(result.Messages);
        Fail();
        WriteLog();
    }

    public void Fail(List<ServiceResultMessage> Messages)
    {
        this.Messages.AddRange(Messages);
        Fail();
        WriteLog();
    }

    public void Fail(string code, string message)
    {
        Messages.Add(new ServiceResultMessage { Code = code, Description = message, IsError = true });
        Fail();
        WriteLog();
    }

    public void Fail(string message)
    {
        Messages.Add(new ServiceResultMessage { Code = "E1", Description = message, IsError = true });
        Fail();
        WriteLog();
    }

    public void Fail(Exception ex, string code = "E1")
    {
        if (KeepRawException)
        {
            var tmpEx = ex;
            while (tmpEx != null)
            {
                Messages.Add(new ServiceResultMessage { Code = code, Description = ex.ToString(), IsError = true });
                tmpEx = tmpEx.InnerException;
            }
        }
        else
        {
            Messages.Add(new ServiceResultMessage { Code = code, Description = "SystemFailureSeeLogsForDetail", IsError = true });
        }

        Fail();
    }

    public void Fail(Exception ex, bool reportError, string entry, string fileName)
    {
        Fail(ex);
    }

    protected virtual void Dispose(bool disposing)
    {
        Messages = null;
    }
}