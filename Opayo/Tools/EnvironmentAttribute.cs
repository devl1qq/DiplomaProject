using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opayo.Tools
{
    internal class EnvironmentAttribute : PXStringListAttribute
    {
        public EnvironmentAttribute() : base(Values.Arr, Labels.Arr) { }

        public class Values
        {

            public static readonly string[] Arr =
            {
                Test,
                Live
            };

            public const string Test = "T";
            public const string Live = "L";
        }
        public class Labels
        {
            public static readonly string[] Arr =
            {
                Test,
                Live
            };

            public const string Test = "TEST";
            public const string Live = "LIVE";
        }
    }
}
