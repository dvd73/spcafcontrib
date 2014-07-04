using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SharePoint.Common.Utilities
{
    public class EventReceiverHelper
    {
    }

    /// <summary>
    /// Disabled item events scope
    /// </summary>
    /// <see cref="http://adrianhenke.wordpress.com/2010/01/29/disable-item-events-firing-during-item-update/"/>
    public class DisabledItemEventsScope : SPItemEventReceiver, IDisposable
    {
        bool oldValue;

        public DisabledItemEventsScope()
        {
            this.oldValue = base.EventFiringEnabled;
            base.EventFiringEnabled = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            base.EventFiringEnabled = oldValue;
        }

        #endregion
    }

    /// <summary>
    /// Disabled web events scope
    /// </summary>
    /// <see cref="http://adrianhenke.wordpress.com/2010/01/29/disable-item-events-firing-during-item-update/"/>
    public class DisabledWebEventsScope : SPWebEventReceiver, IDisposable
    {
        bool oldValue;

        public DisabledWebEventsScope()
        {
            this.oldValue = base.EventFiringEnabled;
            base.EventFiringEnabled = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            base.EventFiringEnabled = oldValue;
        }

        #endregion
    }

}
