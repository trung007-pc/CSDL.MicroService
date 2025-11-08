using System.Net;
using System.Net.Mail;
using Volo.Abp.DependencyInjection;

namespace CSDL7.MasterService.Infrastructures;

public class EmailService : ITransientDependency
{
    /// <summary>
    /// Send email to one 
    /// </summary>
    /// <param name="toEmail"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="config"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SendEmailToOneAsync(string toEmail, string message)
    {
        // 2. Tạo đối tượng MailMessage
        using var mail = new MailMessage();
        mail.From = new MailAddress("trungn3loveyou@gmail.com");
        mail.To.Add(toEmail);
        
        mail.Subject = "CSDL Email Service";
        mail.Body = message;
        mail.IsBodyHtml = true;
        mail.Bcc.Add(new MailAddress(toEmail));

        // 8. Cấu hình SMTP Client
        using var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("trungn3loveyou@gmail.com", "hoib kfej uaap xibl"),
            EnableSsl = true,
        };

        // 9. Gửi email (dùng await vì hàm async)
        await smtpClient.SendMailAsync(mail);
    }
}