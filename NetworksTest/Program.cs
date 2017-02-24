using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Diploma2
{
    class Program
    {
        static void Main(string[] args)
        {
            Networks.Network nw = new SFNetwork(100000, 3);

            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream fs = File.OpenWrite("network.sfnw"))
                bf.Serialize(fs, nw);

            using (FileStream fs = File.OpenRead("network"))
                nw = (Networks.Network)bf.Deserialize(fs);
        }
    }
}
