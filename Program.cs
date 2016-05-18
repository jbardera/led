using System;
using System.IO;
using System.Text;

namespace led
{
    class Program
    {
        static System.IO.StreamWriter sw = null;
        static System.Net.Sockets.TcpClient tcpc = null;
        static System.Net.Security.SslStream ssl = null;
        static StreamWriter sslw = null;
        static StreamReader sslr = null;
        static string username, password, pattern;
        static string path, imapresponse;
        static StringBuilder sb = new StringBuilder();
        static void Main(string[] args)
        {
            if (args.Length<3)
            {
                Console.WriteLine("\nRequired parameter(s) missing:\n\nled \"IMAP_login\" \"IMAP_password\" \"pattern\"\n\nQuotation marks are mandatory.");
            } 
            else try
            {
              
                path = Environment.CurrentDirectory + "\\led.log";

                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

                sw = new System.IO.StreamWriter(System.IO.File.Create(path));
             
                tcpc = new System.Net.Sockets.TcpClient("imap.gmail.com", 993);
                ssl = new System.Net.Security.SslStream(tcpc.GetStream());
                ssl.AuthenticateAsClient("imap.gmail.com");

                sslw = new StreamWriter(ssl);
                sslr = new StreamReader(ssl);


                username = args[0];
                password = args[1];
                pattern = args[2];

                Console.Clear();

                sslw.WriteLine("a1 LOGIN " + username + " " + password);
                sslw.Flush();
                ReadResponse("a1", sslr);

                sslw.WriteLine("a2 SELECT INBOX");
                sslw.Flush();
                ReadResponse("a2", sslr);

                sslw.WriteLine("a3 SEARCH UNSEEN HEADER SUBJECT \""+pattern+"\"");
                sslw.Flush();
                imapresponse = ReadSearch("a3", sslr);

                //now processing the messages marked at 'a3'
                if (imapresponse.Length > 9)
                {
                    //we have at least 1 message matching the pattern at 'a3'
                    sw.WriteLine(imapresponse);
                    /* code to process each message matched:
                    imapresponse2 = imapresponse.Substring(9);
                    Console.WriteLine(imapresponse2);
                    string[] smsgs = imapresponse.Split(' ');
                    Console.WriteLine(smsgs);
                    sw.WriteLine(imapresponse);
                    int nummsgs = smsgs.Length;
                    int[] imsgs = new int[nummsgs];
                    for (int i = 0; i < nummsgs; i++) imsgs[i] = Int32.Parse(smsgs[i]);
                    //now we have all messages UID at imsgs[i]
                    //we can fetch everyone of them
                    sslw.WriteLine("a3 FETCH ("+imapresponse2+") (FLAGS BODY[HEADER.FIELDS (SUBJECT)])");
                    */
                }
                else
                {
                    //no messages matching pattern at 'a3'
                    Console.WriteLine("-- MESSAGES MATCHING NOT FOUND --");
                }
                
                sslw.WriteLine("a4 LOGOUT");
                sslw.Flush();
                ReadResponse("a4", sslr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (ssl != null)
                {
                    ssl.Close();
                    ssl.Dispose();
                }
                if (sslr != null)
                {
                    sslr.Close();
                    sslr.Dispose();
                }
                    if (sslw != null)
                {
                    sslw.Close();
                    sslw.Dispose();
                }
                    if (tcpc != null)
                { 
                    tcpc.Close();
                }
            }
        }

        private static string ReadResponse(string tag, StreamReader sr)
        {
            string response;

            while ((response = sr.ReadLine()) != null)
            {
                Console.WriteLine(response);
                if (response.StartsWith(tag, StringComparison.Ordinal))
                {
                    break;
                }
            }
            return response;
        }

        private static string ReadSearch(string tag, StreamReader sr)
        {
            string response;

            response=sr.ReadLine();
            Console.WriteLine(response);
            return response;
        }
    }
}
