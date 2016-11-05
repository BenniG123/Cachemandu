using Collections;

class Logger {
	private long numHits;
	private long numMisses;
	private long numToAvg;
	private long curHits;
	private ArrayList history;

	public Logger(long numToAvg) {
		numHits = 0;
		numMisses = 0;
		this.numToAvg = numToAvg;
		curHits = 0;
		history = new ArrayList();
	}

	public void add(bool hit) {
		if (hit) {
			curHits++;
			numHits++;
		}
		else {
			numMisses++;
		}
		curAvgCnt++;
		if (curAvgCnt == numToAvg) {
			history.Add(numHits * 1.0 / numToAvg);
			curAvgCnt = 0;
			curHits = 0;
		}
	}

	public long getHits() {
		return numHits;
	}
	
	public long getMisses() {
		return numMisses;
	}

	public ArrayList getHistory() {
		return history;
	}
}