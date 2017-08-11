using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestRP
{
    public class DictAggregateLevel
    {
        public int count;
        public ConcurrentDictionary<string, DictAggregateLevel> dict;
    }

    public class DictAggregateFirstLevel : DictAggregateLevel
    {
        public DateTime fromTime = DateTime.UtcNow;
    }

    public class AggregateLog
    {

        public AggregateLog()
        {
            this.firstLevel = new DictAggregateFirstLevel();
            this.exceptionSet = new ConcurrentDictionary<Exception, object>();
        }

        public DateTime fromTime = DateTime.UtcNow;
        DictAggregateFirstLevel firstLevel;
        ConcurrentDictionary<Exception, object> exceptionSet;

        public void Log(params object[] args)
        {
            DictAggregateLevel previous = firstLevel;
            Interlocked.MemoryBarrier();
            for (int x = 0; x < args.Length; x++)
            {
                if (previous.dict == null)
                {
                    Interlocked.CompareExchange<ConcurrentDictionary<string, DictAggregateLevel>>(ref previous.dict,
                        new ConcurrentDictionary<string, DictAggregateLevel>(), null);
                }
                string key = args[x].ToString();
                DictAggregateLevel current;
                while (true)
                {
                    if (previous.dict.TryGetValue(key, out current))
                    {
                        break;
                    }
                    current = new DictAggregateLevel();
                    if (previous.dict.TryAdd(key, current))
                    {
                        break;
                    }
                }
                previous = current;
                Interlocked.Increment(ref previous.count);
            }
        }

        public IEnumerable<string> GetLogData()
        {
            List<string> rval = new List<string>();
            var dict = Interlocked.Exchange<DictAggregateFirstLevel>(ref firstLevel, new DictAggregateFirstLevel());
            var exceptions = Interlocked.Exchange<ConcurrentDictionary<Exception, object>>(ref exceptionSet, new ConcurrentDictionary<Exception, object>());
            rval.Add("Events since:" + dict.fromTime.ToString());
            DumpDict("", dict.dict, rval);
            return rval;
        }

        public string GetLogString()
        {
            var logs = GetLogData();
            return string.Join("\r\n", logs);
        }

        private void DumpDict(string prefix, ConcurrentDictionary<string, DictAggregateLevel> dict, List<string> rval)
        {
            if (dict != null)
            {
                foreach (var kv in dict)
                {
                    rval.Add(prefix + kv.Key + " :<->: " + kv.Value.count);
                    if (kv.Value.dict != null)
                    {
                        DumpDict(prefix + "    ", kv.Value.dict, rval);
                    }
                }
            }
        }
        public void AggregateLogException0(Exception e, string msg, params object[] args)
        {
            AggregateLogException3(e, msg, null, null, null, args);
        }

        public void AggregateLogException1(Exception e, string msg, string d1, params object[] args)
        {
            AggregateLogException3(e, msg, d1, null, null, args);
        }

        public void AggregateLogException2(Exception e, string msg, string d1, string d2, params object[] args)
        {
            AggregateLogException3(e, msg, d1, d2, null, args);
        }

        public void AggregateLogException3(Exception e, string msg, string d1, string d2, string d3, params object[] args)
        {
            if (TryMarkExceptionLogged(e))
            {
                int countSynth = (d3 != null ? 3 : (d2 != null ? 2 : (d1 != null ? 1 : 0)));
                string[] synth = new string[3] { d1, d2, d3 };
                var countmatched = countSynth;
                if (countmatched > args.Length) countmatched = args.Length;
                var newArgs = new object[3 + countmatched + countSynth - countmatched + args.Length - countmatched];
                newArgs[2] = Utility.GetExceptionCallstack(e);
                newArgs[1] = Utility.GetExceptionMessage(e);
                newArgs[0] = msg;
                for (int x = 0; x < countmatched; x++)
                {
                    string val;
                    try
                    {
                        val = string.Format(synth[x], args[x]);
                    }
                    catch
                    {
                        val = synth[x] ?? "" + "!" + (args[x] ?? "").ToString();
                    }
                    newArgs[x + 3] = val;
                }
                for (int x = 0; x < countSynth - countmatched; x++)
                {
                    newArgs[x + 3 + countmatched] = synth[x + countmatched];
                }
                for (int x = 0; x < args.Length - countmatched; x++)
                {
                    newArgs[x + 3 + countSynth] = args[x + countmatched];
                }
                Log(newArgs);
            }
        }

        public bool TryMarkExceptionLogged(Exception e)
        {
            return exceptionSet.TryAdd(e, null);
        }
    }
}
