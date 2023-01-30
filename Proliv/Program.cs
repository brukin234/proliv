using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json.Serialization;
using System.Threading;

namespace Proliv
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Archive();
            upload();
            Console.ReadKey();
        }
        static void Archive()
        {
            string[] names = File.ReadAllLines("Names.txt");
            string[] files = Directory.GetFiles("Files");
            foreach (string name in names)
            {
                var fileStream = new FileStream($"Result\\{name}.zip", FileMode.Create);
                var archive = new ZipArchive(fileStream, ZipArchiveMode.Create);

                foreach (string file in files)
                {
                    string z = file.Remove(0,6);
                    if (file.Contains(".exe") == true)
                    {
                        string ifexe = name + ".exe";
                        archive.CreateEntryFromFile($"Files\\{z}", ifexe);
                    }
                    else
                    {
                        archive.CreateEntryFromFile($"Files\\{z}", z);
                    }
                }
                archive.Dispose();
            }
            Console.WriteLine($"Created {names.Length} zips");
            
        }
        static void upload()
        {
            using (WebClient client = new WebClient())
            {
                string[] names = File.ReadAllLines("Names.txt");
                using (StreamWriter sw = new StreamWriter("result.txt"))
                {
                    foreach (string name in names)
                    {
                        try
                        {
                            mainupload("https://store5.gofile.io/uploadFile", name);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                mainupload("https://store4.gofile.io/uploadFile", name);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    mainupload("https://store3.gofile.io/uploadFile", name);
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        mainupload("https://store2.gofile.io/uploadFile", name);
                                    }
                                    catch (Exception)
                                    {
                                        try
                                        {
                                            mainupload("https://store1.gofile.io/uploadFile", name);
                                        }
                                        catch (Exception)
                                        {

                                            Console.WriteLine("Cant upload file on any gofile server");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        static void mainupload(string storelink,string name)
        {
            using (StreamWriter sw = new StreamWriter("result.txt"))
            { 
                using (WebClient client = new WebClient())
                { 
                    var upload = client.UploadFile("https://store2.gofile.io/uploadFile", $"Result\\{name}.zip");
                    var result = JObject.Parse(Encoding.UTF8.GetString(upload));
                    var link = result["data"]["downloadPage"];
                    Console.WriteLine("[+] " + link.ToString() + " " + name);
                    sw.WriteLine($"{link.ToString()} {name}");
                }
            }
        }
    }
}
