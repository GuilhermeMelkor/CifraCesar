using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace Cesar
{
    class Program
    {
        public static string json = "";
        static void Main(string[] args)
        {
            WebRequest request = WebRequest.Create("https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token=6d3eabc2ba5a08d4dd15564028dcb89f8813179a");
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                object responseFromServer = reader.ReadToEnd();
                var post = JsonConvert.DeserializeObject<Mensagem>(responseFromServer.ToString());
                Console.WriteLine($"Numero_casas:{post.Numero_casas}\nToken:{post.Token}\ncifrado:{post.cifrado}\ndecifrado:{post.decifrado}\nresumo_criptografico:{post.resumo_criptografico}");
                Decifrar(post);
            }

            gravarArquivoJson(json);
            PostImage();

            response.Close();
            Console.ReadLine();

        }

        private static void gravarArquivoJson(string json)
        {
            StreamWriter arqJson = new StreamWriter("answer.json");
            arqJson.WriteLine(json);
            arqJson.Close();
        }

        public static void PostImage()
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            byte[] imagebytearraystring = JsonToArray(@"C:\Users\guilh\source\repos\Cesar\Cesar\bin\Debug\answer.json");
            form.Add(new ByteArrayContent(imagebytearraystring), "answer", "answer.json");
            HttpResponseMessage response = httpClient.PostAsync("https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=6d3eabc2ba5a08d4dd15564028dcb89f8813179a", form).Result;

            httpClient.Dispose();
            string sd = response.Content.ReadAsStringAsync().Result;
        }

        private static byte[] JsonToArray(string fullFilePath)
        {
            FileStream fs = File.OpenRead(fullFilePath);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return bytes;
        }



        private static void Decifrar(Mensagem post)
        {
            post.decifrado = null;
            var msg = post.cifrado;
            //var msg = "j pomz cfmjfwf jo tubujtujdt uibu j epdupsfe nztfmg. xjotupo t. divsdijmm?";
            int num = 0;
            string text = "";
            string[] data = {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};

            for ( int i = 0; i < msg.Length; i++)
            {
                if (msg[i].ToString() == ".")
                {
                    text += msg[i].ToString();
                }
                else if (msg[i].ToString()==" ")
                {
                    text += msg[i].ToString();
                }
                else
                {
                    int t = Array.IndexOf(data, msg[i].ToString());

                    int v = t - 12;
                    if (v > data.Count())
                    {
                        v = v - data.Count();
                    }
                    if (v < 0)
                    {
                        v = data.Count() + v;
                    }
                    text += data[v].ToString();
                }
                
                
            }
            //    for (int i = 0; i < msg.Length; i++)
            //{
            //    var toInt = Convert.ToInt32(msg[i]);


            //    if (toInt == 46 || toInt == 32 || toInt == 63)
            //    {
            //        num = toInt;

            //    }                
            //    else
            //    {
            //        num = toInt - post.Numero_casas;
            //    }
            //    if (num >= 33 && num <= 96)
            //    {
            //        num = toInt;
            //    }
            //    if (num < 32)
            //    {
            //        num += 32;
            //    }

                post.decifrado += text;//Convert.ToChar(num);

            //}


            var hash = new SHA1Managed().ComputeHash(Encoding.ASCII.GetBytes(post.decifrado));
            post.resumo_criptografico = string.Concat(hash.Select(b => b.ToString("x2")));

            //byte[] bytes = Encoding.ASCII.GetBytes(post.decifrado);
            //SHA1 sha1 = new SHA1CryptoServiceProvider();
            //var result = sha1.ComputeHash(bytes);
            //string converted = Encoding.ASCII.GetString(result, 0, result.Length);
            //post.resumo_criptografico = converted;

            Console.WriteLine($"\n");
            Console.WriteLine($"Numero_casas:{post.Numero_casas}\nToken:{post.Token}\ncifrado:{post.cifrado}\ndecifrado:{post.decifrado}\nresumo_criptografico:{post.resumo_criptografico}");


            json = JsonConvert.SerializeObject(post);


        }
    }
}
