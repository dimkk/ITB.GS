using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Common.BL;
using Microsoft.SharePoint;

namespace GS.Ig.Interfaces
{
    public interface IIgHandler
    {
        bool IsItemOwner(SPListItem igItem);
        void Process(SPListItem igItem, StatusEnum newStatus);
    }
}
