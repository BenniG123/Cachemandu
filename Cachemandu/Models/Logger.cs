using System.Collections.Generic;

namespace Cachemandu.Models
{
    public class Logger {
	    private long numHits;
	    private long numMisses;
	    private long numToAvg;
	    private long curHits;
        private long curAvgCnt;
        private List<float> history;

	    public Logger(long numToAvg) {
		    numHits = 0;
		    numMisses = 0;
		    this.numToAvg = numToAvg;
		    curHits = 0;
		    history = new List<float>();
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
			    history.Add(curHits * 1.0f / numToAvg);
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

	    public List<float> getHistory() {
		    return history;
	    }
    }

}