namespace Dos.Common
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class EmailHelper
    {
        private EmailAccount accountInfo = new EmailAccount();
        private const string tab = "\t";

        public EmailHelper(EmailAccount accountInfo)
        {
            this.accountInfo.Account = accountInfo.Account;
            this.accountInfo.DisplayName = accountInfo.DisplayName;
            this.accountInfo.EnableSsl = accountInfo.EnableSsl;
            this.accountInfo.Host = accountInfo.Host;
            this.accountInfo.Password = accountInfo.Password;
            this.accountInfo.Port = accountInfo.Port;
        }

        public bool Send(string toemails, string subject, string body, params Attachment[] attachments)
        {
            MailMessage message = new MailMessage();
            Encoding displayNameEncoding = Encoding.UTF8;
            message.From = new MailAddress(this.accountInfo.Account, this.accountInfo.DisplayName, displayNameEncoding);
            message.To.Add(toemails);
            message.Subject = subject;
            message.Body = body;
            message.Priority = MailPriority.High;
            message.BodyEncoding = displayNameEncoding;
            if ((attachments != null) && (attachments.Length > 0))
            {
                foreach (Attachment attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }
            SmtpClient client = new SmtpClient {
                Host = this.accountInfo.Host,
                Port = this.accountInfo.Port
            };
            if (this.accountInfo.IsUnnormal)
            {
                client.Credentials = new NetworkCredential("", this.accountInfo.Account + "\t" + this.accountInfo.Password);
            }
            else
            {
                client.Credentials = new NetworkCredential(this.accountInfo.Account, this.accountInfo.Password);
            }
            client.EnableSsl = this.accountInfo.EnableSsl;
            try
            {
                client.Send(message);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void SendAsync(string toemails, string subject, string body, params Attachment[] attachments)
        {
            MailMessage message = new MailMessage();
            Encoding displayNameEncoding = Encoding.UTF8;
            message.From = new MailAddress(this.accountInfo.Account, this.accountInfo.DisplayName, displayNameEncoding);
            message.To.Add(toemails);
            message.Subject = subject;
            message.Body = body;
            message.Priority = MailPriority.High;
            message.BodyEncoding = displayNameEncoding;
            if ((attachments != null) && (attachments.Length > 0))
            {
                foreach (Attachment attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }
            SmtpClient client = new SmtpClient {
                Host = this.accountInfo.Host,
                Port = this.accountInfo.Port
            };
            if (this.accountInfo.IsUnnormal)
            {
                client.Credentials = new NetworkCredential("", this.accountInfo.Account + "\t" + this.accountInfo.Password);
            }
            else
            {
                client.Credentials = new NetworkCredential(this.accountInfo.Account, this.accountInfo.Password);
            }
            client.EnableSsl = this.accountInfo.EnableSsl;
            client.SendAsync(message, null);
        }
    }
}

