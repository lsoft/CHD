using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common;

namespace CHD.Tests.CompositionRoot.Modules.Fakes.Stat
{
    public sealed class StatRecord
    {
        private readonly List<double> _times;

        public string Key
        {
            get;
            private set;
        }

        public int Count
        {
            get
            {
                return
                    _times.Count;
            }

        }

        public TimeSpan Min
        {
            get
            {
                if (_times.Count < 1)
                {
                    return
                        TimeSpan.Zero;
                }

                var min = _times.Min();

                return
                    TimeSpan.FromSeconds(min);
            }
        }

        public TimeSpan Max
        {
            get
            {
                if (_times.Count < 1)
                {
                    return
                        TimeSpan.Zero;
                }

                var max = _times.Max();

                return
                    TimeSpan.FromSeconds(max);
            }
        }

        public TimeSpan Avg
        {
            get
            {
                if (_times.Count < 1)
                {
                    return
                        TimeSpan.Zero;
                }

                var avg = _times.Average();

                return
                    TimeSpan.FromSeconds(avg);
            }
        }

        public TimeSpan Median
        {
            get
            {
                if (_times.Count < 1)
                {
                    return
                        TimeSpan.Zero;
                }

                
                var s = new List<double>(_times);

                s.Sort();

                double med;

                if ((s.Count % 2) == 1)
                {
                    med = s[s.Count/2];
                }
                else
                {
                    var medleft = s[s.Count/2 - 1];
                    var medright = s[s.Count/2];
                    med = (medleft + medright)/2;
                }

                return
                    TimeSpan.FromSeconds(med);
            }
        }

        public TimeSpan Total
        {
            get
            {
                if (_times.Count < 1)
                {
                    return
                        TimeSpan.Zero;
                }

                var total = _times.Sum();

                return
                    TimeSpan.FromSeconds(total);
            }
        }



        public StatRecord(
            string key
            )
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Key = key;

            _times = new List<double>();
        }

        public void AddRecord(
            double timeInterval
            )
        {
            _times.Add(timeInterval);
        }

        public void Log(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            logger.LogFormattedMessage(
                "{0}:{1}    Count={2}, Min={3}, Max={4}, Avg={5}, Median={6}, Total={7}{1}",
                Key,
                Environment.NewLine,
                Count,
                Min,
                Max,
                Avg,
                Median,
                Total
                );
        }
    }
}