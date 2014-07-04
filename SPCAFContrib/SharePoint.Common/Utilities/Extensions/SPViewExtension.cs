using System;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class SPViewExtension
    {
        public static SPView Update(this SPView view, SPWeb web, Action<SPView> updateAction)
        {
            bool allowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            updateAction(view);
            view.Update();
            web.Update();
            web.AllowUnsafeUpdates = allowUnsafeUpdates;
            return view;
        }
    }
}
