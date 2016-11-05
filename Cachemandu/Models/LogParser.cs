using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cachemandu.Models
{
    public class LogParser
    {
        private StreamReader reader;
        private bool bit64;

        public LogParser(StreamReader reader, bool bit64)
        {
            this.bit64 = bit64;
            this.reader = reader;
        }

        public bool CloseIfDone()
        {
            if (reader.EndOfStream)
            {
                reader.Dispose();
                return true;
            }
            return false;
        }

        public MemInst GetNextInst()
        {
            String line = reader.ReadLine();
            MemInst ret = new MemInst();

            ret.type = InstType.NONE;
            while (!reader.EndOfStream)
            {
                if (line.Contains("Read"))
                {
                    ret.type = InstType.LOAD;
                    break;
                }
                else if (line.Contains("Write"))
                {
                    ret.type = InstType.STORE;
                    break;
                }
            }

            if (!(ret.type == InstType.NONE))
            {
                long result;
                bool parseSuccess = long.TryParse(line.Substring(line.IndexOf(')') + 3, bit64 ? 16 : 8), NumberStyles.HexNumber, new CultureInfo("en-US"), out result);
                if (parseSuccess)
                {
                    ret.addr = result;
                    return ret;
                }
            }

            return null;
        }
    }
}