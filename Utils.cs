using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetMapTile
{
    static class Utils
    {
        public static String ServerIP = "42.194.182.118";
        public static int ThreadCount = 20;
        //120.77.175.240 、39.108.186.2、39.108.145.101、120.77.47.121、39.108.181.215、39.108.185.40、120.78.74.1
        //120.78.92.208、120.78.88.80、39.108.187.204、8.129.208.158、8.129.213.8、47.119.127.231、120.78.65.67
        public static void SendMail(string msg)
        {
            //if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour <= 23)
            {
                Console.WriteLine("SendMail begin");
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("su", "953579411@qq.com"));
                message.To.Add(new MailboxAddress("su", "953579411@qq.com"));
                message.Subject = ServerIP + "-" + ThreadCount;

                message.Body = new TextPart("plain")
                {
                    Text = msg
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.qq.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("953579411@qq.com", "tobysacnkrpsbffe");

                    client.Send(message);
                    client.Disconnect(true);

                    //Console.Write("OK");
                }
                Console.WriteLine("SendMail end");
            }
        }
    }
}
