using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Diploma2
{
    public static class ilSerializer
    {
        public static int Serialize(string path, object obj)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = File.OpenWrite(path))
                    bf.Serialize(fs, obj);
            }
            catch (Exception e)
            {
                return 1;
            }
            return 0;
        }
        public static object Deserialize(string path)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = File.OpenRead(path))
                    return bf.Deserialize(fs);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
