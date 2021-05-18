﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Utilities
{
    public class EmailUtilities
    {

        /// <summary>
        /// Put the properties of the email including "to", "cc", "from", "subject" and "email body"  
        /// </summary>
        /// <param name="emailConfig"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public MailMessage ConstructEmailMessage(EmailSendConfigure emailConfig, EmailContent content)
        {
            MailMessage msg = new MailMessage();
            if (emailConfig.TOs != null)
            {
                foreach (string to in emailConfig.TOs)
                {
                    if (!string.IsNullOrEmpty(to))
                    {
                        msg.To.Add(to);
                    }
                }
            }
            //Chuỗi email
            if (!string.IsNullOrEmpty(emailConfig.EmailTo))
            {
                var emailLists = emailConfig.EmailTo.Split(';');
                if (emailLists != null && emailLists.Any())
                {
                    foreach (var email in emailLists)
                    {
                        if (!string.IsNullOrEmpty(email))
                            msg.To.Add(email);
                    }
                }
            }

            if (emailConfig.CCs != null)
            {
                foreach (string cc in emailConfig.CCs)
                {
                    if (!string.IsNullOrEmpty(cc))
                    {
                        msg.CC.Add(cc);
                    }
                }
            }
            if (emailConfig.BCCs != null)

                foreach (string bcc in emailConfig.BCCs)
                {
                    if (!string.IsNullOrEmpty(bcc))
                    {
                        msg.Bcc.Add(bcc);
                    }
                }
            if (string.IsNullOrEmpty(emailConfig.FromEmail))
                emailConfig.FromEmail = emailConfig.From;
            msg.From = new MailAddress(emailConfig.FromEmail,
                                           emailConfig.FromDisplayName,
                                           Encoding.UTF8);
            msg.IsBodyHtml = content.IsHtml;
            msg.Body = content.Content;
            msg.Priority = emailConfig.Priority;
            msg.Subject = emailConfig.Subject;
            msg.BodyEncoding = Encoding.UTF8;
            msg.SubjectEncoding = Encoding.UTF8;

            if (content.Attachments != null && content.Attachments.Count > 0)
            {
                foreach (var attachment in content.Attachments)
                {
                    msg.Attachments.Add(attachment);
                }
            }
            return msg;
        }

        /// <summary>
        /// Send the email using the SMTP server 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="emailConfig"></param>
        public async Task Send(MailMessage message, EmailSendConfigure emailConfig)
        {
            SmtpClient client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                                  emailConfig.ClientCredentialUserName,
                                  emailConfig.ClientCredentialPassword),
                Host = emailConfig.SmtpServer,
                Port = emailConfig.Port,
                EnableSsl = emailConfig.EnableSsl,
            };
            Console.WriteLine("------------------------------------ Email:" + emailConfig.FromEmail);
            Console.WriteLine("------------------------------------ ClientCredentialUserName:" + emailConfig.ClientCredentialUserName);
            Console.WriteLine("------------------------------------ Password:" + emailConfig.ClientCredentialPassword);
            try
            {
                //Add this line to bypass the certificate validation
                //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                //        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                //        System.Security.Cryptography.X509Certificates.X509Chain chain,
                //        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                //{
                //    return true;
                //};
                client.Send(message);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Console.WriteLine("SmtpFailedRecipientsExceptionMessage:" + ex.Message);
                Console.WriteLine("SmtpFailedRecipientsException:" + ex.StackTrace);
                throw new Exception("SmtpFailedRecipientsException:" + ex.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine("ExceptionMAILMessage:" + e.Message);
                Console.WriteLine("ExceptionMAIL:" + e.StackTrace);
                throw new Exception("ExceptionMAIL:" + e.StackTrace);
                //throw e;
            }
            finally
            {
                message.Dispose();
            }
        }

        //public void Send(string subject, string body, string[] Tos)
        //{
        //    Send(subject, body, Tos, null, null);
        //}


        //public void Send(string subject, string body, string[] Tos, string[] CCs)
        //{
        //    Send(subject, body, Tos, CCs, null);
        //}


        //public async void Send(string subject, string body, string[] Tos, string[] CCs, string[] BCCs)
        //{
        //    EmailSendConfigure emailConfig = GetEmailConfig();
        //    EmailContent content = GetEmailContent();
        //    emailConfig.Subject = subject;
        //    emailConfig.TOs = Tos;
        //    emailConfig.CCs = CCs;
        //    emailConfig.BCCs = BCCs;
        //    content.Content = body;
        //    MailMessage msg = ConstructEmailMessage(emailConfig, content);
        //    Send(msg, emailConfig);
        //}

        //public void SendMail(string subject, string Tos, string[] CCs, string[] BCCs, EmailContent emailContent)
        //{
        //    EmailSendConfigure emailConfig = GetEmailConfig();
        //    EmailContent content = GetEmailContent();
        //    emailConfig.Subject = subject;
        //    emailConfig.EmailTo = Tos;
        //    emailConfig.CCs = CCs;
        //    emailConfig.BCCs = BCCs;
        //    content = emailContent;
        //    MailMessage msg = ConstructEmailMessage(emailConfig, content);
        //    Send(msg, emailConfig);
        //}

        //public async void Send(string subject, string[] Tos, string[] CCs, string[] BCCs, EmailContent emailContent)
        //{
        //    EmailSendConfigure emailConfig = GetEmailConfig();
        //    EmailContent content = GetEmailContent();
        //    emailConfig.Subject = subject;
        //    emailConfig.TOs = Tos;
        //    emailConfig.CCs = CCs;
        //    emailConfig.BCCs = BCCs;
        //    content = emailContent;
        //    MailMessage msg = ConstructEmailMessage(emailConfig, content);
        //    Send(msg, emailConfig);
        //}
    }
}