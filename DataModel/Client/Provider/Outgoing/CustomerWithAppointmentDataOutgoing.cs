﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Client.Provider.Outgoing
{
    public class CustomerWithAppointmentDataOutgoing
    {
        public CustomerWithAppointmentDataOutgoing(OutgoingCustomerProfile clientCustomerProfile, OutgoingAppointment clientAppointment)
        {
            this.Profile = clientCustomerProfile;
            this.Appointment = clientAppointment;
        }

        public OutgoingAppointment Appointment { get; set; }
        public OutgoingCustomerProfile Profile { get; set; }
    }
}
