using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Configuration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SharePoint.Common.Utilities.Extensions;

namespace MOSS.Common.Utilities
{
    public class MailHelper
    {
        SmtpClient smtp;
        string smtpHostAddress;

        public MailHelper()
        {
            smtpHostAddress = DefaultServerAddress;
        }

        public MailHelper(string smtpHostAddress)
        {
            this.smtpHostAddress = smtpHostAddress;
        }

        private void InitializeSmtpClient()
        {
            if (smtp == null)
            {
                smtp = new SmtpClient();
                smtp.Host = smtpHostAddress;
            }
        }

        public void Send(string sender, IEnumerable<string> recipients, string replyTo, Action<MailMessage> setMessageProperties)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    if (!string.IsNullOrEmpty(sender))
                        message.From = new MailAddress(sender);

                    if (recipients != null)
                        foreach(var recipient in recipients)
                            message.To.Add(new MailAddress(recipient));

                    if (!string.IsNullOrEmpty(replyTo))
                        message.ReplyTo = new MailAddress(replyTo);

                    setMessageProperties(message);

                    if (HasNoRecipients(message))
                        throw new SmtpException(SmtpStatusCode.GeneralFailure, "Both MailMessage.To and MailMessage.Bcc cannot be empty.");

                    InitializeSmtpClient();

                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
                throw;
            }
        }

        private bool HasNoRecipients(MailMessage message)
        {
            return (message.To == null || message.To.Count == 0) && (message.Bcc == null || message.Bcc.Count == 0);
        }

        public void Send(string sender, string recipient, string replyTo, Action<MailMessage> setMessageProperties)
        {
            this.Send(sender, new string[] { recipient }, replyTo, setMessageProperties); 
        }

        public void Send(string sender, string recipient, Action<MailMessage> setMessageProperties)
        {
            Send(sender, recipient, DefaultReplyTo, setMessageProperties);
        }

        public void Send(string sender, Action<MailMessage> setMessageProperties)
        {
            Send(DefaultSender, sender, DefaultReplyTo, setMessageProperties);
        }

        public void Send(Action<MailMessage> setMessageProperties)
        {
            Send(null, (List<string>)null, null, setMessageProperties);
        }

        public void SendDefault(Action<MailMessage> setMessageProperties)
        {
            Send(DefaultSender, DefaultRecipient, DefaultReplyTo, setMessageProperties);
        }

        private string DefaultServerAddress
        {
            get { return MailService.Server.Address; }
        }

        private SPWebApplication _webApplication;
        private SPWebApplication WebApplication
        {
            get
            {
                if (_webApplication == null)
                    _webApplication = SPContext.Current.Site.WebApplication;
                return _webApplication;
            }
        }

        private SPOutboundMailServiceInstance MailService
        {
            get { return WebApplication.OutboundMailServiceInstance; }
        }
        private string DefaultSender
        {
            get { return WebApplication.OutboundMailSenderAddress; }
        }
        private string DefaultReplyTo
        {
            get { return WebApplication.OutboundMailReplyToAddress; }
        }

        private string DefaultRecipient
        {
            get
            {
                return WebConfigurationManager.AppSettings["SharePoint.Mail.DefaultTo"] ?? string.Empty;
            }
        }
    }

}
