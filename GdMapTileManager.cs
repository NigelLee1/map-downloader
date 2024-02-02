using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GetMapTile
{
    static class GdMapTileManager
    {
        /*
         http://webrd02.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=1668&y=893&z=11
https://webst01.is.autonavi.com/appmaptile?style=6&x=213637&y=114234&z=18
https://wprd01.is.autonavi.com/appmaptile?x=213634&y=114233&z=18&lang=zh_cn&size=1&scl=1&style=8
        double resolution = Lods.GetLods().lods[level].resolution;
            var minPoint = extents2D.MinPoint;
            var maxPoint = extents2D.MaxPoint;
            int minCol = (int)((Lods.originX + minPoint.X) / (Tile.width * resolution));
            int maxCol = (int)((Lods.originX + maxPoint.X) / (Tile.width * resolution));
            int maxRow = (int)((Lods.originY - minPoint.Y) / (Tile.height * resolution));
            int minRow = (int)((Lods.originY - maxPoint.Y) / (Tile.height * resolution));
         */
        /*
         * xmin:113.157014,ymin:22.201038,xmax:113.692042,ymax:22.772602  // 中山市 xmin:12596581,ymin:2535679,xmax:12656140,ymax:2604541
         * 没较正坐标 xmin:113.15162,ymin:22.20372,xmax:113.6869,ymax:22.77548 // 中山市 xmin:12595980,ymin:2536001,xmax:12655567,ymax:2604889
         * xmin:117.938172,ymin:33.134801,xmax:119.17625,ymax:34.422235 // 宿迁市 xmin:13128817,ymin:3913210,xmax:13266639,ymax:4085639
         */
        const string StreetBaseDir = "/mntdisk/OfflineMap/street";
        const string ImageBaseDir = "/mntdisk/OfflineMap/image";
        private static List<Lod> lods;
        public static void Init()
        {
            string json = "[{ \"level\":0,\"resolution\":156543.03392800014,\"scale\":591657527.591555},{ \"level\":1,\"resolution\":78271.51696400007,\"scale\":295828763.7957775},{ \"level\":2,\"resolution\":39135.758482000034,\"scale\":147914381.89788875},{ \"level\":3,\"resolution\":19567.879241000017,\"scale\":73957190.94894437},{ \"level\":4,\"resolution\":9783.939620500008,\"scale\":36978595.47447219},{ \"level\":5,\"resolution\":4891.969810250004,\"scale\":18489297.737236094},{ \"level\":6,\"resolution\":2445.984905125002,\"scale\":9244648.868618047},{ \"level\":7,\"resolution\":1222.992452562501,\"scale\":4622324.434309023},{ \"level\":8,\"resolution\":611.4962262812505,\"scale\":2311162.2171545117},{ \"level\":9,\"resolution\":305.74811314062526,\"scale\":1155581.1085772559},{ \"level\":10,\"resolution\":152.87405657031263,\"scale\":577790.5542886279},{ \"level\":11,\"resolution\":76.43702828515632,\"scale\":288895.27714431396},{ \"level\":12,\"resolution\":38.21851414257816,\"scale\":144447.63857215698},{ \"level\":13,\"resolution\":19.10925707128908,\"scale\":72223.81928607849},{ \"level\":14,\"resolution\":9.55462853564454,\"scale\":36111.909643039246},{ \"level\":15,\"resolution\":4.77731426782227,\"scale\":18055.954821519623},{ \"level\":16,\"resolution\":2.388657133911135,\"scale\":9027.977410759811},{ \"level\":17,\"resolution\":1.1943285669555674,\"scale\":4513.988705379906},{ \"level\":18,\"resolution\":0.5971642834777837,\"scale\":2256.994352689953},{ \"level\":19,\"resolution\":0.29858214173889186,\"scale\":1128.4971763449764},{ \"level\":20,\"resolution\":0.14929107086944593,\"scale\":564.2485881724882},{ \"level\":21,\"resolution\":0.07464553543472296,\"scale\":282.1242940862441},{ \"level\":22,\"resolution\":0.03732276771736148,\"scale\":141.06214704312205},{ \"level\":23,\"resolution\":0.01866138385868074,\"scale\":70.53107352156103}]";
            lods = JsonConvert.DeserializeObject<List<Lod>>(json);
        }

        private static async Task GetImage(string uri, int level, int row, int col, string baseDir)
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
            HttpClient client = new HttpClient();
            byte[] urlContents = await client.GetByteArrayAsync(uri);
            System.IO.FileStream fs = new System.IO.FileStream(path,System.IO.FileMode.Create);
            fs.Write(urlContents, 0, urlContents.Length);
            fs.Close();
        }

        public static async Task GetAllImage(double xmin, double ymin, double xmax, double ymax)
        {
            double originX = 20037508.342787;
            double originY = 20037508.342787;
            Random random = new Random();
            Stopwatch sw = new Stopwatch();
            for (int i = 3; i <= 18; i++)
            {
                sw.Restart();
                int minCol = (int)((originX + xmin) / (256 * lods[i].resolution));
                int maxCol = (int)((originX + xmax) / (256 * lods[i].resolution));
                int maxRow = (int)((originY - ymin) / (256 * lods[i].resolution));
                int minRow = (int)((originY - ymax) / (256 * lods[i].resolution));
                //http://webrd02.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=1668&y=893&z=11
                //z 3-18
                for (int col = minCol; col <= maxCol; col++)
                {
                    for (int row = minRow; row <= maxRow; row++)
                    {
                        for (int k = 1; k <= 10; k++)
                        {
                            int server = random.Next(1, 5);
                            string uri = "http://webrd0" + server + ".is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + col + "&y=" + row + "&z=" + i;
                            try
                            {
                                await GetImage(uri, i, row, col, StreetBaseDir);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("uri:" + uri + ";" + ex.ToString());
                            }
                        }
                        for (int k = 1; k <= 10; k++)
                        {
                            int server = random.Next(1, 5);
                            string uri = "https://webst0" + server + ".is.autonavi.com/appmaptile?style=6&x=" + col + "&y=" + row + "&z=" + i;
                            try
                            {
                                await GetImage(uri, i, row, col, ImageBaseDir);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("uri:" + uri + ";" + ex.ToString());
                            }
                        }
                    }
                }
                sw.Stop();
                Console.WriteLine("level:" + i + " finished time used:" + sw.ElapsedMilliseconds/1000 + "s.");
            }
        }
    }
}
