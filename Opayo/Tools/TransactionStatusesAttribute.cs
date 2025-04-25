using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opayo.Tools
{
    public class TransactionStatusesAttribute : PXStringListAttribute
    {
        public TransactionStatusesAttribute() : base(Values.Arr, Labels.Arr) {}

        public class Values
        {
            public static readonly string[] Arr =
            {
                Open,
                Failed,
                Completed,
                Canceled
            };

            public const string Open = "O";
            public const string Failed = "F";
            public const string Completed = "C";
            public const string Canceled = "Q";
        }

        public class Labels
        {
            public static readonly string[] Arr =
            {
                Open,
                Failed,
                Completed,
                Canceled
            };

            public const string Open = "Open";
            public const string Failed = "Failed";
            public const string Completed = "Completed";
            public const string Canceled = "Canceled";
        }

        public class BQL
        {
            public class Open : BqlString.Constant<Open> { public Open() : base(Values.Open) { } }
            public class Failed : BqlString.Constant<Failed> { public Failed() : base(Values.Failed) { } }
            public class Completed : BqlString.Constant<Completed> { public Completed() : base(Values.Completed) { } }
            public class Canceled : BqlString.Constant<Canceled> { public Canceled() : base(Values.Canceled) { } }
        }

    }

}
