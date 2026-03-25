namespace Infrastructure.Sms;

public interface ISmsSender
{
    Task<bool> SendAsync(string phone, string message);
}