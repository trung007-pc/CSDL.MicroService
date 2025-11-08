namespace CSDL7.MasterService.Infrastructures;

public interface IEmail
{
    Task SendEmailToOneAsync(string toEmail, string message);
}