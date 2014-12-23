using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Common.BL;
using GS.Ig.Core.Handlers;
using GS.Ig.Core.Interfaces;
using Microsoft.SharePoint;

namespace GS.Ig
{
    public class IgHandlerFactory
    {
        private static readonly IEnumerable<IIgHandler> _igHandlers;

        static IgHandlerFactory()
        {
            _igHandlers = new List<IIgHandler>()
            {
                new IgHandlerIssueP()
            };
        }

        private static IIgHandler TryGetIgHandler(SPListItem igItem)
        {
            IIgHandler owner = _igHandlers.FirstOrDefault(handler => handler.IsItemOwner(igItem));
            return owner != null ? (IIgHandler)Activator.CreateInstance(owner.GetType()) : null;
        }

        public static void TryProcess(SPListItem igItem, StatusEnum newStatus)
        {
            IIgHandler handler = TryGetIgHandler(igItem);
            if (handler == null)
                return;

            handler.Process(igItem, newStatus);
        }
    }
}
