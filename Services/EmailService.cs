using FluentEmail.Core;

namespace FAKA.Server.Services;

public class EmailService
{
    private readonly IFluentEmail _email;

    public EmailService(IFluentEmail email)
    {
        _email = email;
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var result = await _email
            .To(to)
            .Subject(subject)
            .Body(body)
            .SendAsync();
        Console.WriteLine(result);
    }
    
    public async Task SendVerificationEmailAsync(string to, string code)
    {
        const string subject = "faka 邮箱验证";
        var body = $@"
            <p>您好，</p>
            <p>感谢您注册 faka 账号。</p>
            <p>请点击下面的链接完成邮箱验证：</p>
            <p><a href=""https://faka.com/verify?code={code}"">https://faka.com/verify?code={code}</a></p>
            <p>如果您没有注册过 faka 账号，请忽略此邮件。</p>
            <p>谢谢！</p>
        ";
        await SendEmailAsync(to, subject, body);
    }
}