using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetMapTile
{
    class GoogleMapTileManager
    {
        // www.google.com.tw
        // https://www.google.co.il/maps/vt/pb=!1m4!1m3!1i13!2i6671!3i3569!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050
        // https://khms3.google.com/kh/v=894?x=426799&y=228420&z=19
        const string StreetBaseDir = @"C:\Projects\map-downloader\googleMap\street";
        const string ImageBaseDir = @"C:\Projects\map-downloader\googleMap\image";
        private static List<Lod> lods;
        public static void Init()
        {
            string json = "[{ \"level\":0,\"resolution\":156543.03392800014,\"scale\":591657527.591555},{ \"level\":1,\"resolution\":78271.51696400007,\"scale\":295828763.7957775},{ \"level\":2,\"resolution\":39135.758482000034,\"scale\":147914381.89788875},{ \"level\":3,\"resolution\":19567.879241000017,\"scale\":73957190.94894437},{ \"level\":4,\"resolution\":9783.939620500008,\"scale\":36978595.47447219},{ \"level\":5,\"resolution\":4891.969810250004,\"scale\":18489297.737236094},{ \"level\":6,\"resolution\":2445.984905125002,\"scale\":9244648.868618047},{ \"level\":7,\"resolution\":1222.992452562501,\"scale\":4622324.434309023},{ \"level\":8,\"resolution\":611.4962262812505,\"scale\":2311162.2171545117},{ \"level\":9,\"resolution\":305.74811314062526,\"scale\":1155581.1085772559},{ \"level\":10,\"resolution\":152.87405657031263,\"scale\":577790.5542886279},{ \"level\":11,\"resolution\":76.43702828515632,\"scale\":288895.27714431396},{ \"level\":12,\"resolution\":38.21851414257816,\"scale\":144447.63857215698},{ \"level\":13,\"resolution\":19.10925707128908,\"scale\":72223.81928607849},{ \"level\":14,\"resolution\":9.55462853564454,\"scale\":36111.909643039246},{ \"level\":15,\"resolution\":4.77731426782227,\"scale\":18055.954821519623},{ \"level\":16,\"resolution\":2.388657133911135,\"scale\":9027.977410759811},{ \"level\":17,\"resolution\":1.1943285669555674,\"scale\":4513.988705379906},{ \"level\":18,\"resolution\":0.5971642834777837,\"scale\":2256.994352689953},{ \"level\":19,\"resolution\":0.29858214173889186,\"scale\":1128.4971763449764},{ \"level\":20,\"resolution\":0.14929107086944593,\"scale\":564.2485881724882},{ \"level\":21,\"resolution\":0.07464553543472296,\"scale\":282.1242940862441},{ \"level\":22,\"resolution\":0.03732276771736148,\"scale\":141.06214704312205},{ \"level\":23,\"resolution\":0.01866138385868074,\"scale\":70.53107352156103}]";
            lods = JsonConvert.DeserializeObject<List<Lod>>(json);
            ServicePointManager.DefaultConnectionLimit = 100;
            //Console.WriteLine("DefaultConnectionLimit:" + ServicePointManager.DefaultConnectionLimit);
        }

        private static void GetImage(string uri, int level, int row, int col, string baseDir)
        {
            // AppDomain.CurrentDomain.BaseDirectory
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            if (!Directory.Exists(baseDir + Path.DirectorySeparatorChar + level))
                Directory.CreateDirectory(baseDir + Path.DirectorySeparatorChar + level);
            if (!Directory.Exists(baseDir + Path.DirectorySeparatorChar + level + Path.DirectorySeparatorChar + row))
                Directory.CreateDirectory(baseDir + Path.DirectorySeparatorChar + level + Path.DirectorySeparatorChar + row);
            string path = baseDir + Path.DirectorySeparatorChar + level + Path.DirectorySeparatorChar + row + Path.DirectorySeparatorChar + col + ".png";
            if (File.Exists(path))
                return;
            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            //byte[] urlContents = await client.GetByteArrayAsync(uri);
            WebClient client = new WebClient();
            //client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"); // Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)
            //[My program name] [version number] contact [contact@adress.com]
            //client.Headers.Add("user-agent", "aaefwerwe 1.0 contact 2343242@163.com");
            client.Headers.Add("user-agent", "aaefwerweq 2.0 contact 3343242@163.com");
            byte[] urlContents = client.DownloadData(new Uri(uri));
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
            fs.Write(urlContents, 0, urlContents.Length);
            fs.Close();
           // Console.WriteLine(DateTime.Now.ToString("G") + " 下载图片:" + path);
        }

        public static bool hasError = false;
        private static object lockObject = new object();

        public static void GetAllImage(double xmin, double ymin, double xmax, double ymax)
        {
            double originX = 20037508.342787;
            double originY = 20037508.342787;
            hasError = false;
            Random random = new Random();
            //Stopwatch sw = new Stopwatch();
            //LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(2);
            //TaskFactory factory = new TaskFactory(lcts);
            List<Task> tasks = new List<Task>();
            for (int i = 21; i <= 21; i++)
            {
                //sw.Restart();
                int minCol = (int)((originX + xmin) / (256 * lods[i].resolution));
                int maxCol = (int)((originX + xmax) / (256 * lods[i].resolution));
                int maxRow = (int)((originY - ymin) / (256 * lods[i].resolution));
                int minRow = (int)((originY - ymax) / (256 * lods[i].resolution));
                //http://webrd02.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=1668&y=893&z=11
                if (i >= 19)
                    Console.WriteLine("minRow:" + minRow + ",maxRow:" + maxRow);
                //z 3-18
                if (i == 21)
                    minRow = 914044;
                    //minRow = 840033;
                for (int row = minRow; row <= maxRow; row++)
                {
                    //Stopwatch sw2 = new Stopwatch();
                    //sw2.Restart();
                    for (int col = minCol; col <= maxCol; col++)                    
                    {
                        Task task = Task.Factory.StartNew((Object obj) =>
                        {
                            TaskParam taskParam = obj as TaskParam;
                            //GetImageTile(taskParam);
                            GetStreetTile(taskParam);
                        }, new TaskParam() { level = i, row = row, col = col });
                        tasks.Add(task);
                        if (hasError)
                            //throw new Exception("error");
                            return;
                        while (tasks.Count > Utils.ThreadCount) 
                        {
                            int taskIndex = Task.WaitAny(tasks.ToArray());
                            tasks.RemoveAt(taskIndex);
                        }
                        /*for (int k = 1; k <= 10; k++)
                        {
                            int server = random.Next(1, 5);
                            // // https://khms3.google.com/kh/v=894?x=426799&y=228420&z=19
                            string uri = "https://khms3.google.com/kh/v=894?x=" + col + "&y=" + row + "&z=" + i;
                            //string uri = "https://webst0" + server + ".is.autonavi.com/appmaptile?style=6&x=" + col + "&y=" + row + "&z=" + i;
                            try
                            {
                                await GetImage(uri, i, row, col, ImageBaseDir);
                                break;
                            }
                            catch (Exception ex)
                            {
                                if (k == 10)
                                    Console.WriteLine(DateTime.Now.ToString("G") + " uri:" + uri + ";" + ex.ToString());
                            }
                        }*/
                    }
                    //sw2.Stop();
                    if (i >= 19)
                    {
                        string msg = DateTime.Now.ToString("G") + " level:" + i + ", row:" + row;
                        Console.WriteLine(msg);
                        //Utils.SendMail(msg);
                    }
                }
                //sw.Stop();
                string msg2 = DateTime.Now.ToString("G") + " level:" + i;
                Console.WriteLine(msg2);
                //Utils.SendMail(msg2);
            }
        }

        private static void GetStreetTile(TaskParam taskParam)
        {
            for (int k = 1; k <= 60; k++)
            {
                //int server = random.Next(1, 5);
                //https://www.google.co.il/maps/vt/pb=!1m4!1m3!1i13!2i6671!3i3569!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050
                string uri = "http://www.google.com/maps/vt/pb=!1m4!1m3!1i" + taskParam.level + "!2i" + taskParam.col + "!3i" + taskParam.row + "!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050";
                //string uri = "http://webrd0" + server + ".is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + col + "&y=" + row + "&z=" + i;
                try
                {
                    GetImage(uri, taskParam.level, taskParam.row, taskParam.col, StreetBaseDir);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("(404)") >= 0)
                        break;
                    else if (k == 60 || ex.Message.IndexOf("(429)") >= 0)
                    {
                        lock (lockObject)
                        {
                            if (!hasError)
                            {
                                hasError = true;
                                string msg = DateTime.Now.ToString("G") + " [" + Utils.ThreadCount + "]" + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                                Console.WriteLine(msg);
                                Utils.SendMail(msg);
                            }
                        }
                        break;
                        //throw ex;
                    }
                    else
                    {
                       // string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                       // Console.WriteLine(msg);
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private static void GetImageTile(TaskParam taskParam)
        {
            for (int k = 1; k <= 60; k++)
            {
                //int server = random.Next(1, 5);
                //https://www.google.co.il/maps/vt/pb=!1m4!1m3!1i13!2i6671!3i3569!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050
                //string uri = "http://www.google.com.tw/maps/vt/pb=!1m4!1m3!1i" + taskParam.level + "!2i" + taskParam.col + "!3i" + taskParam.row + "!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050";
                string uri = "https://khms3.google.com/kh/v=894?x=" + taskParam.col + "&y=" + taskParam.row + "&z=" + taskParam.level;
                //string uri = "http://webrd0" + server + ".is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + col + "&y=" + row + "&z=" + i;
                try
                {
                    GetImage(uri, taskParam.level, taskParam.row, taskParam.col, ImageBaseDir);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("(404)") >= 0)
                        break;
                    else if (k == 60 || ex.Message.IndexOf("(429)") >= 0)
                    {
                        lock (lockObject)
                        {
                            if (!hasError)
                            {
                                hasError = true;
                                string msg = DateTime.Now.ToString("G") + " [" + Utils.ThreadCount + "]" + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                                Console.WriteLine(msg);
                                Utils.SendMail(msg);
                            }
                        }
                        break;
                        //throw ex;
                    }
                    else
                    {
                        //string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                        //Console.WriteLine(msg);
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
