using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System;
using CueSheeter.Models;
using System.Text;
using System.IO;

namespace UnitTest.CueSheeter
{
    [TestClass]
    public class UnitTestCueSheeter
    {
        [TestMethod]
        public void TestFormat()
        {
            var dur = TimeSpan.FromSeconds(505).Add(TimeSpan.FromMilliseconds(235));

            var ms = (long)dur.TotalMilliseconds % 1000;
            var s = ((long)dur.TotalSeconds) % 60;
            var m = (long)dur.TotalMinutes;

            Logger.LogMessage($"{dur:c}");
            Logger.LogMessage($"{m:d2}:{s:d2}:{ms/10:d2}");
        }

        [TestMethod]
        public void TestLoader()
        {
            var cs = CueSheet.LoadFromFile(@"C:\temp\test.cue", 
                Encoding.GetEncoding("ISO-8859-1"));

            cs.Tracks.RemoveAt(0);
            cs.Tracks.RemoveAt(cs.Tracks.Count-1);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            cs.Serialize(sw);

            
            File.WriteAllText(@"C:\temp\test2.cue",
                sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
        }
    }
}
