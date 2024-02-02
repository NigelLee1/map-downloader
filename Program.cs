using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GetMapTile
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            //Console.WriteLine(Path.DirectorySeparatorChar);
            //return;
            try
            {
                Console.WriteLine(Utils.ServerIP + ";ThreadCount:" + Utils.ThreadCount);
                GoogleMapTileManager.Init();
                GoogleMapTileManager.GetAllImage(12596581, 2535679, 12656140, 2604541);
                //GoogleMapTileManager.GetAllImage(13128817, 3913210, 13266639, 4085639);
                if (!GoogleMapTileManager.hasError)
                    Console.WriteLine("Success!");
                else
                    Console.WriteLine("Failed!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main error:" + ex.ToString());
            }
        }
        // 20 minRow:417385,maxRow:421897  
        // 21 minRow:834771,maxRow:843795
                                                                 // 120.77.175.240 level:20, row:420755 maxRow: 420799
                                                                 // 120.78.74.1 level:20, row:421254 maxRow: 421199
                                                                 // 120.78.92.208 level:20 row:421451 maxRow 421399
                                                                 // 120.78.88.80 level:20 row:421625 maxRow 421599
                                                                 // 39.108.187.204 level:20 row:421741 maxRow 421799

        
        
        

        // finished
        // 39.108.190.162 level:21, row:842694 maxRow:842699  // 5
        // 8.129.213.8 level:21, row:839381 maxRow: 839399    // 18 
        // 47.119.131.192 level:21, row:836779 maxRow: 836799 // 20
        // 39.108.187.204 level:21, row:836568 maxRow: 836599 // 31
        // 39.108.108.19 level:21, row: 835956 maxRow: 835999 // 43 
        // 47.119.131.192 level:21, row:835223 maxRow: 835199 // -24
        // 120.77.47.121 level:21, row:842494 maxRow:842499   // 5 
        // 39.108.187.204 level:21, row:837493 maxRow: 837499 // 6
        // 112.74.190.113 level:21, row:842992 maxRow:842999  // 7 
        // 39.108.191.211 level:21, row:841991 maxRow: 841999 // 8
        // 120.78.88.80 level:21, row:837782 maxRow: 837799   // 17
        // 39.108.190.162 level:21, row:841480 maxRow: 841499 // 19 
        // 39.108.185.40 level:21, row:840502 maxRow: 840599  // 97 //
        // 120.78.92.208 level:21, row:841812 maxRow: 841799  // -13 
        // 8.129.213.8 level:21, row:838590 maxRow: 838599    // 9
        // 47.119.140.58 level:21, row:837989 maxRow: 837999  // 10 
        // 120.78.74.1 level:21, row: 835800 maxRow: 835799   // 30 
        // 120.78.88.45 level:21, row:840897 maxRow: 840899   // 2  
        // 47.119.127.231 level:21, row:841297 maxRow: 841299 // 2  
        // 39.108.181.215 level:21, row:838488 maxRow: 838499 // 11 
        // 39.108.108.19 level:21, row:839016 maxRow: 838999  // 19 
        // 120.78.65.67 level:21, row:843500 maxRow:843499    // 26 
        // 39.108.157.4 level:21, row:835501 maxRow: 835499   // 1
        // 39.108.181.215 level:21, row:836383 maxRow: 836379 // 1
        // 120.78.92.208 level:21, row:840998 maxRow:840999   // 1 
        // 8.129.208.174 level:21, row:839957 maxRow: 839959  // 2 
        // 112.74.190.113 level:21, row:840970 maxRow: 840959 // 2 

        // 39.108.145.101 level:21, row:836182 maxRow: 836189 // 7
        // 39.108.142.136 level:21, row:836943 maxRow: 836949 // 6 
        // 39.108.185.40 level:21, row:839990 maxRow: 839999  // 9 
        // 120.78.88.45 level:21, row:835095 maxRow: 835099   // 4
        // 39.108.157.4 level:21, row:836260 maxRow: 836269   // 9
        // 47.119.131.192 level:21, row:836377 maxRow: 836369 // 10 
        // 120.78.74.1 level:21,    row:836389 maxRow: 836399 // 10 
        // 39.108.231.250 level:21, row::838905 maxRow: 838899 // 11 
        // 47.119.127.231 level:21, row:841388 maxRow: 841399 // 11 
        // 39.108.191.211 level:21, row:841369 maxRow: 841379 // 10 
        // 39.108.108.19 level:21, row:834997 maxRow: 834999  // 2
        // 120.77.175.240 level:21, row: 843786 maxRow:843795 // 9 
        // 39.108.186.2 level:21, row:834976 maxRow: 834979   // 3
        // 112.74.190.113 level:21, row:836181 maxRow: 836189 // 8
        // 39.108.181.215 level:21, row:836195 maxRow: 836199 // 4
        // 120.77.47.121 level:21 row:836256 maxRow: 836259   // 3
        // 8.129.213.8 level:21, row:836294 maxRow: 836299    // 5
        // 39.108.190.162 level:21, row:836984 maxRow: 836989 // 5 
        // 120.78.65.67 level:21,  row:836996 maxRow: 836999  // 3
        // 8.129.208.158 level:21, row:837386 maxRow: 837389  // 3 
        // 120.78.92.208 level:21, row:837394 maxRow: 837399  // 5
        // 39.108.233.43 level:21, row:839594 maxRow: 839599  // 5 
        // 120.78.88.80 level:21, row:840562 maxRow: 840569   // 7  
        // 47.119.140.58 level:21, row:840585 maxRow: 840589  // 4
        // 8.129.208.174 level:21, row:840592 maxRow: 840599  // 7
        // 39.108.187.204 level:21, row:835078 maxRow: 835079 // 8
    }
}
