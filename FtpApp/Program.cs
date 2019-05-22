using System;
using System.IO;
using System.Net;

namespace FtpApp
{
    internal static class Program
    {
        private static void Main()
        {
            var ftpPath = "ftp://ftp.example.com/remote/path/file.zip";
            var networkCredentials = new NetworkCredential("username", "password");

            using (var stream = File.Create(@"C:\local\path\file.zip")
                .ReadFromFtp(ftpPath, networkCredentials))
            {
            }

            using (var stream = new MemoryStream())
            {
                stream.ReadFromFtp(ftpPath, networkCredentials);
            }

            Console.WriteLine("Hello World!");
        }

        private static Stream ReadFromFtp(this Stream stream, string ftpPath, ICredentials networkCredentials)
        {
            var request =
                (FtpWebRequest) WebRequest.Create(ftpPath);
            request.Credentials = networkCredentials;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (var ftpStream = request.GetResponse().GetResponseStream())
            {
                if (ftpStream == null) throw new NullReferenceException(nameof(ftpStream));
                var buffer = new byte[10240];
                int read;
                while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, read);
                    Console.WriteLine("Downloaded {0} bytes", stream.Position);
                }
            }

            return stream;
        }
    }
}