using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GS.Model.Definitions
{
    public class Constants
    {
        public static readonly string SystemName = "ГС";

        public static string FormName(string value)
        {
            return SystemName + "." + value;
        }
    }
}
