using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CueSheeter.Models
{
    public class CueSheet
    {
        public string Performer { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public IList<Track> Tracks { get; set; } = new List<Track>();

        /// <summary>
        /// Recalculates Track numbers and offset times to start at 0:0:0 at track 1
        /// </summary>
        public void Update()
        {
            TimeSpan offs = TimeSpan.Zero;
            for( int i = 0; i < Tracks.Count; i++ )
            {
                if (i == 0) offs = Tracks[i].Index.StartTime;
                Tracks[i].TrackNumber = i + 1;
                Tracks[i].Index.StartTime -= offs;
            }
        }

        protected enum LoaderState
        {
            ROOT,
            TRACK
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();

            Serialize(sb);

            return sb.ToString();
        }

        public void Serialize(StringBuilder sb)
        {
            StringWriter sw = new StringWriter(sb);
            Serialize(sw);
        }

        public void Serialize(TextWriter sw)
        {
            Update();

            sw.WriteLine($"PERFORMER \"{Performer}\"");
            sw.WriteLine($"TITLE \"{Title}\"");
            sw.WriteLine($"FILE \"{FileName}\"");
            foreach (var track in Tracks)
            {
                sw.WriteLine("  " + $"{track}".Replace(Environment.NewLine, Environment.NewLine + "  "));
            }
        }

        public static CueSheet LoadFromFile(string path, Encoding srcEncoding = null)
        {
            if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");

            if (srcEncoding is null) srcEncoding = Encoding.Default;

            CueSheet ret = new CueSheet();

            LoaderState state = LoaderState.ROOT;
            LoaderState nextstate = LoaderState.ROOT;
            Track curTrack = null;

            foreach (var line in File.ReadLines(path, srcEncoding))
            {
                var list = Regex.Matches(line, @"\""(\""\""|[^\""])+\""|[^ ]+",
                    RegexOptions.ExplicitCapture)
                        .Cast<Match>()
                        .Select(m => m.Value)
                        .Select(m => m.StartsWith("\"") ? m.Substring(1, m.Length - 2).Replace("\"\"", "\"") : m)
                        .ToList();

                if (list.Count == 0) continue;

                do
                {
                    state = nextstate;

                    switch (state)
                    {
                        case LoaderState.ROOT:
                            {
                                if (list[0].ToUpper() == "PERFORMER")
                                    ret.Performer = list[1];
                                else if (list[0].ToUpper() == "TITLE")
                                    ret.Title = list[1];
                                else if (list[0].ToUpper() == "FILE")
                                    ret.FileName = list[1];
                                else if (list[0].ToUpper() == "TRACK")
                                    nextstate = LoaderState.TRACK;
                                break;
                            }

                        case LoaderState.TRACK:
                            {
                                if (list[0].ToUpper() == "TRACK")
                                {
                                    curTrack = new Track() { TrackNumber = int.Parse(list[1]) };
                                    ret.Tracks.Add(curTrack);
                                }
                                else if (list[0].ToUpper() == "PERFORMER")
                                    curTrack.Performer = list[1];
                                else if (list[0].ToUpper() == "TITLE")
                                    curTrack.Title = list[1];
                                else if (list[0].ToUpper() == "INDEX")
                                {
                                    // Parse starttime
                                    var startTimeParts = list[2].Split(':').ToList();
                                    var startTime = TimeSpan.FromMinutes(int.Parse(startTimeParts[0]))
                                        .Add(TimeSpan.FromSeconds(int.Parse(startTimeParts[1])))
                                        .Add(TimeSpan.FromMilliseconds(int.Parse(startTimeParts[2]) * 10));

                                    curTrack.Index = new TrackIndex()
                                    {
                                        Index = int.Parse(list[1]),
                                        StartTime = startTime
                                    };
                                }
                                break;
                            }
                    }
                } while (state != nextstate);
            }

            return ret;
        }
    }
}
