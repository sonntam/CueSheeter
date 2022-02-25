using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueSheeter.Models
{
    public class Track
    {
        public string Performer { get; set; }
        public string Title { get; set; }
        public int TrackNumber { get; set; }
        public TrackIndex Index { get; set; }

        public override string ToString()
        {
            return $"TRACK {TrackNumber} AUDIO{Environment.NewLine}" +
                $"  PERFORMER \"{Performer}\"{Environment.NewLine}" +
                $"  TITLE \"{Title}\"{Environment.NewLine}" +
                $"  {Index}";
        }

    }
}
