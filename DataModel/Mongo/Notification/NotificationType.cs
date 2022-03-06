using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Mongo.Notification
{
    public enum NotificationType
    {
        Reminder,
        AppointmentStatus,
        NewCustomer,
        Referral,
        PrescriptionUploaded
    }
}
