using Opayo.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opayo.Models;
using Opayo.Webhooks;
using PX.Api;
using PX.Api.Webhooks.DAC;
using PX.Data;
using PX.Api.Webhooks.Graph;
using PX.Common;
using static PX.Data.BQL.BqlPlaceholder;

namespace Opayo.Tools
{
    public static class OpayoUtils
    {
        public static ParsedName ParseFullName(string[] fullNames)
        {
            var res = new ParsedName() {};

            foreach (var name in fullNames)
            {
                if (string.IsNullOrEmpty(name?.Trim()))
                    continue;

                var splitted = name?.Trim().Split(' ').Where(i => !string.IsNullOrEmpty(i)).ToArray();
                string firstName = splitted?.Length > 0 ? splitted[0] : null;
                string lastName = splitted?.Length > 1 ? splitted[1] : null;

                res.FirstName = firstName;
                res.LastName = lastName ?? firstName; // If last name is null it uses as first name 

                break;
            }

            return res;
        }

        public class ParsedName
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public static string GenerateId()
        {
            var ticks = new DateTime(2023, 1, 1).Ticks;
            var ans = DateTime.Now.Ticks - ticks;
            var uniqueId = ans.ToString("x");
            return uniqueId;
        }

        public static class SegmentedStringFormatter
        {
            public static string Format(Tuple<string, int>[] pairs, string segmentSeparator = "-", string charsToAppend = "")
            {
                string res = string.Empty;

                foreach (var p in pairs)
                {
                    int segmentMaxLen = p.Item2;
                    string value = p.Item1?.Trim() ?? string.Empty;

                    if (value.Length > segmentMaxLen)
                        value = value.Substring(0, segmentMaxLen);

                    int charsCountToAppend = segmentMaxLen - value.Length;

                    if (!string.IsNullOrEmpty(res))
                        res += segmentSeparator;

                    res += AppendCharsBeforeValue(value, charsCountToAppend, charsToAppend);
                }
                return res;
            }

            private static string AppendCharsBeforeValue(string value, int count, string charsToAppend) 
                => string.Concat(Enumerable.Repeat(charsToAppend, count)) + value;
        }
    }
}
