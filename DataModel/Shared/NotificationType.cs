using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Shared
{
    public enum NotificationType
    {
        TwentyFourHourReminder,
        TwelveHourReminder,
        ImmediateConfirmation,
        Cancellation,
        Reschedule
    }
}
