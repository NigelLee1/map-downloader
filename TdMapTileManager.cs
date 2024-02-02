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
    class TdMapTileManager
    {
        // www.google.com.tw
        // https://www.google.co.il/maps/vt/pb=!1m4!1m3!1i13!2i6671!3i3569!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050
        // https://khms3.google.com/kh/v=894?x=426799&y=228420&z=19
        const string StreetBaseDir = "/mntdisk/OfflineMap/tianditu/street";
        const string ImageBaseDir = "/mntdisk/OfflineMap/tianditu/image";
        const string LabelBaseDir = "/mntdisk/OfflineMap/tianditu/label";
        static string[] keys = new string[] { "403a726ef731432b528ff76c5b539bac", "6248f4680ca772280918d00a554cbdf6", 
            "8d3203de2472f8c27a94f2e6414de611", "0ec48c70380e4bfc7c4695543af92514", "d2ed0bda02aec2c1364accc53b9bd64e",
            "61e691f976a73313d51243572ee39c54", "14f53424d2f698e154320faa15727db2", "bade605a46007ee68f5c78623acd410e",
            "e9f4a8ca4492eacc8c24c29affa78aa0",
            "efa4bd31f6892e11769b8e664b98714e", "11891823a3a49aa553270c4e0c6d16b4", "d2aa68466dcd8c87a9031ca22ab530d7",
            "8ea02a13a8458be07871d50c13ab4d58", "4b86482c7f5c7e7eebdb8884f3318b9c", "7f5a00c4c59bb6b60cc9696cbfe7ad66",
            "d30f173fdfbd1638f79a48374108877a", "55cc4658a42dc404e765e4e65e928c97", "3555c89b7d17ba6486d2de2acb1037c0",
            "5b371588da3f1cdddf39e15f988bce73"};
        // xmin 117.994406 xmax 118.599226 ymin 33.798597 ymax 34.171063 宿豫区 火星坐标
        // xmin 117.988834 xmax 118.594017 ymin 33.800031 ymax 34.172547 宿豫区 Gps坐标
        // xmin 13134456 xmax 13201825 ymin 4001982 ymax 4051994 宿豫区 Gps的投影坐标
        // +-5row\col
        static int keyIndex = 0;
        private static List<Lod> lods;
        public static void Init()
        {
            string json = "[{ \"level\":0,\"resolution\":156543.03392800014,\"scale\":591657527.591555},{ \"level\":1,\"resolution\":78271.51696400007,\"scale\":295828763.7957775},{ \"level\":2,\"resolution\":39135.758482000034,\"scale\":147914381.89788875},{ \"level\":3,\"resolution\":19567.879241000017,\"scale\":73957190.94894437},{ \"level\":4,\"resolution\":9783.939620500008,\"scale\":36978595.47447219},{ \"level\":5,\"resolution\":4891.969810250004,\"scale\":18489297.737236094},{ \"level\":6,\"resolution\":2445.984905125002,\"scale\":9244648.868618047},{ \"level\":7,\"resolution\":1222.992452562501,\"scale\":4622324.434309023},{ \"level\":8,\"resolution\":611.4962262812505,\"scale\":2311162.2171545117},{ \"level\":9,\"resolution\":305.74811314062526,\"scale\":1155581.1085772559},{ \"level\":10,\"resolution\":152.87405657031263,\"scale\":577790.5542886279},{ \"level\":11,\"resolution\":76.43702828515632,\"scale\":288895.27714431396},{ \"level\":12,\"resolution\":38.21851414257816,\"scale\":144447.63857215698},{ \"level\":13,\"resolution\":19.10925707128908,\"scale\":72223.81928607849},{ \"level\":14,\"resolution\":9.55462853564454,\"scale\":36111.909643039246},{ \"level\":15,\"resolution\":4.77731426782227,\"scale\":18055.954821519623},{ \"level\":16,\"resolution\":2.388657133911135,\"scale\":9027.977410759811},{ \"level\":17,\"resolution\":1.1943285669555674,\"scale\":4513.988705379906},{ \"level\":18,\"resolution\":0.5971642834777837,\"scale\":2256.994352689953},{ \"level\":19,\"resolution\":0.29858214173889186,\"scale\":1128.4971763449764},{ \"level\":20,\"resolution\":0.14929107086944593,\"scale\":564.2485881724882},{ \"level\":21,\"resolution\":0.07464553543472296,\"scale\":282.1242940862441},{ \"level\":22,\"resolution\":0.03732276771736148,\"scale\":141.06214704312205},{ \"level\":23,\"resolution\":0.01866138385868074,\"scale\":70.53107352156103}]";
            lods = JsonConvert.DeserializeObject<List<Lod>>(json);
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
            client.Headers.Add("user-agent", "GetMapTile 1.0 contact 953579411@qq.com"); 
            byte[] urlContents = client.DownloadData(new Uri(uri));
            System.IO.FileStream fs = new System.IO.FileStream(path,System.IO.FileMode.Create);
            fs.Write(urlContents, 0, urlContents.Length);
            fs.Close();
            Console.WriteLine(DateTime.Now.ToString("G") + " 下载图片:" + path);
        }

        private static bool hasError = false;
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
            for (int i = 3; i <= 18; i++)
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
                //if (i == 19)
                  //  minRow = 208773;
                for (int row = minRow; row <= maxRow; row++)
                {
                    //Stopwatch sw2 = new Stopwatch();
                    //sw2.Restart();
                    for (int col = minCol; col <= maxCol; col++)                    
                    {
                        Task task = Task.Factory.StartNew((Object obj) =>
                        {
                            TaskParam taskParam = obj as TaskParam;
                            GetLabelTile(taskParam);
                        }, new TaskParam() { level = i, row = row, col = col });
                        tasks.Add(task);
                        if (hasError)
                            throw new Exception("error");
                        while (tasks.Count > 10)
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
            Random random = new Random();
            for (int k = 1; k <= 10; k++)
            {
                int server = random.Next(1, 7);
                //https://www.google.co.il/maps/vt/pb=!1m4!1m3!1i13!2i6671!3i3569!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050
                //string uri = "http://www.google.com.tw/maps/vt/pb=!1m4!1m3!1i" + taskParam.level + "!2i" + taskParam.col + "!3i" + taskParam.row + "!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050";
                //string uri = "http://webrd0" + server + ".is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + col + "&y=" + row + "&z=" + i;
                int tmpKeyIndex = keyIndex;
                string uri = "https://t" + server + ".tianditu.gov.cn/vec_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=vec&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILECOL=" + taskParam.col + "&TILEROW=" + taskParam.row + "&TILEMATRIX=" + taskParam.level + "&tk=" + keys[tmpKeyIndex];
                //string uri = "https://t" + server + ".tianditu.gov.cn/vec_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=vec&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&tk=ef6151d9f0386f3b2a2fdf1d58fe9b32&TILECOL=1667&TILEROW=892&TILEMATRIX=11
                try
                {
                    GetImage(uri, taskParam.level, taskParam.row, taskParam.col, StreetBaseDir);
                    break;
                }
                catch (Exception ex)
                {
                    if (k == 10)
                    {
                        if (tmpKeyIndex != keyIndex)
                        {
                            //GetStreetTile(taskParam);
                        }
                        else if (keyIndex < keys.Length - 1)
                        {
                            keyIndex++;
                            Console.WriteLine("keyIndex:" + keyIndex + ";keys.Length:" + keys.Length);
                            GetStreetTile(taskParam);
                        }
                        else
                        {
                            string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString() + ";keyIndex:" + keyIndex + ";keyLength:" + keys.Length;
                            Console.WriteLine(msg);
                            Utils.SendMail(msg);
                            hasError = true;
                            throw ex;
                        }
                    }
                    else
                    {
                        string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                        Console.WriteLine(msg);
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
                    if (k == 60)
                    {
                        string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                        Console.WriteLine(msg);
                        Utils.SendMail(msg);
                        hasError = true;
                        throw ex;
                    }
                    else
                    {
                        string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                        Console.WriteLine(msg);
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private static void GetLabelTile(TaskParam taskParam)
        {
            Random random = new Random();
            for (int k = 1; k <= 10; k++)
            {
                int server = random.Next(1, 7);
                //https://www.google.co.il/maps/vt/pb=!1m4!1m3!1i13!2i6671!3i3569!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050
                //string uri = "http://www.google.com.tw/maps/vt/pb=!1m4!1m3!1i" + taskParam.level + "!2i" + taskParam.col + "!3i" + taskParam.row + "!2m3!1e0!2sm!3i546271748!3m7!2szh-CN!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!4e0!5m1!1e0!23i10203575!23i1381938!23i1381033!23i1368782!23i1368785!23i1375246!23i1385853!23i46990830!23i1375050";
                //string uri = "http://webrd0" + server + ".is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + col + "&y=" + row + "&z=" + i;
                //http://t0.tianditu.gov.cn/cva_w/wmts?tk=您的密钥
                //https://t0.tianditu.gov.cn/cva_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=cva&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILECOL=6670&TILEROW=3559&TILEMATRIX=13&tk=ef6151d9f0386f3b2a2fdf1d58fe9b32
                int tmpKeyIndex = keyIndex;
                //string uri = "https://t" + server + ".tianditu.gov.cn/vec_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=vec&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILECOL=" + taskParam.col + "&TILEROW=" + taskParam.row + "&TILEMATRIX=" + taskParam.level + "&tk=" + keys[tmpKeyIndex];
                string uri = "https://t" + server + ".tianditu.gov.cn/cva_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=cva&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILECOL=" + taskParam.col + "&TILEROW=" + taskParam.row + "&TILEMATRIX=" + taskParam.level + "&tk=" + keys[tmpKeyIndex];
                //string uri = "https://t" + server + ".tianditu.gov.cn/vec_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=vec&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&tk=ef6151d9f0386f3b2a2fdf1d58fe9b32&TILECOL=1667&TILEROW=892&TILEMATRIX=11
                try
                {
                    GetImage(uri, taskParam.level, taskParam.row, taskParam.col, LabelBaseDir);
                    break;
                }
                catch (Exception ex)
                {
                    if (k == 10)
                    {
                        if (tmpKeyIndex != keyIndex)
                        {
                            //GetStreetTile(taskParam);
                        }
                        else if (keyIndex < keys.Length - 1)
                        {
                            keyIndex++;
                            Console.WriteLine("keyIndex:" + keyIndex + ";keys.Length:" + keys.Length);
                            GetLabelTile(taskParam);
                        }
                        else
                        {
                            string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString() + ";keyIndex:" + keyIndex + ";keyLength:" + keys.Length;
                            Console.WriteLine(msg);
                            Utils.SendMail(msg);
                            hasError = true;
                            throw ex;
                        }
                    }
                    else
                    {
                        string msg = DateTime.Now.ToString("G") + " level:" + taskParam.level + "; col:" + taskParam.col + "; row:" + taskParam.row + "; uri:" + uri + ";" + ex.ToString();
                        Console.WriteLine(msg);
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
