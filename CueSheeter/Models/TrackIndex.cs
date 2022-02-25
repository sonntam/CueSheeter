using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueSheeter.Models
{
    public class TrackIndex
    {
        public int Index { get; set; }
        public TimeSpan StartTime { get; set; }
        public override string ToString()
        {
            var ms = (long)StartTime.TotalMilliseconds % 1000;
            var s = ((long)StartTime.TotalSeconds) % 60;
            var m = (long)StartTime.TotalMinutes;

            return $"INDEX {Index:D2} {m:d2}:{s:d2}:{ms / 10:d2}";
        }
    }
}
