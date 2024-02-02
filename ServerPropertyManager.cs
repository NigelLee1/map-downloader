using System;
using System.Collections.Generic;
using System.Text;

namespace GetMapTile
{
    static class ServerPropertyManager
    {
        public static Dictionary<String, ServerProperties> ServerPropertyDict = new Dictionary<string, ServerProperties>();

        public static void Init()
        {
            ServerPropertyDict.Add("120.77.175.240", new ServerProperties() { TasksCount = 20 });
            ServerPropertyDict.Add("120.78.74.1", new ServerProperties() { TasksCount = 20 });
            ServerPropertyDict.Add("", new ServerProperties());
        }
    }
}
