using System;
using System.IO;
using System.Globalization;

enum InstType {
	LOAD,
	STORE,
	NONE
}

struct MemInst {
	InstType type;
	long addr;
}

class LogParser {
	private StreamReader reader;
	private FileStream stream;
	private bool bit64;

	public LogParser(String log, bool bit64) {
		try {
			stream = new FileStream(log, FileMode.Open, FileAccess.Read);
			reader = new StreamReader(stream, Text.ASCIIEncoding);
			this.bit64 = bit64;
		} catch (System.IO.FileNotFoundException e) {
			reader = null;
		}
	}

	public bool IsOpen() {
		return (reader != null && stream.CanRead);
	}

	public MemInst GetNextInst() {
		String line = reader.ReadLine();
		MemInst ret;
		ret.type = InstType.NONE;
		while (!reader.EndOfStream) {
			if (line.Contains("Read")) {
				ret.type = InstType.LOAD;
				break;
			}
			else if (line.Contains("Write")) {
				ret.type = InstType.STORE;
				break;
			}
		}

		if (!(ret.type is InstType.NONE)) {
			Long result;
			bool parseSuccess = Long.TryParse(line.Substring(line.IndexOf(')') + 3, bit64 ? 16 : 8), NumberStyles.HexNumber, new CultureInfo("en-US"), result);
			if (parseSuccess) {
				ret.addr = result;
				return ret;
			}
		}
		return null;
	}
}