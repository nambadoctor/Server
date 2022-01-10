using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

namespace MiddleWare.Utils
{
    public class SlotGenerator
    {

        public static List<ProviderClientOutgoing.GeneratedSlot> GetPastAppointmentTimings(List<Mongo.Appointment> appointments)
        {
            List<ProviderClientOutgoing.GeneratedSlot> pastAppointmentTimings = new List<ProviderClientOutgoing.GeneratedSlot>();
            if (appointments != null)
                foreach (var appointment in appointments)
                {
                    if (appointment.Status != Mongo.AppointmentStatus.Cancelled)
                        pastAppointmentTimings.Add(new ProviderClientOutgoing.GeneratedSlot
                        {
                            StartDateTime = appointment.ScheduledAppointmentStartTime.HasValue ? appointment.ScheduledAppointmentStartTime.Value : DateTime.UtcNow.AddDays(100),
                            EndDateTime = appointment.ScheduledAppointmentEndTime.HasValue ? appointment.ScheduledAppointmentEndTime.Value : DateTime.UtcNow.AddDays(100)
                        });
                }
            return pastAppointmentTimings;
        }
        public static List<ProviderClientOutgoing.GeneratedSlot> GenerateAvailableSlotsForDays(List<Mongo.ServiceProviderAvailability> availabilities, int duration, int interval, int weeks, List<Mongo.Appointment> appointments)
        {

            duration = duration == 0 ? 10 : duration; // to make sure it while loop doesnt run infinetly

            List<ProviderClientOutgoing.GeneratedSlot> allSlots = new List<ProviderClientOutgoing.GeneratedSlot>();

            for (int i = 1; i <= weeks; i++)
            {
                if (availabilities != null)
                    allSlots.AddRange(parseAvailabilitiesToSlots(availabilities, duration, interval, i, GetPastAppointmentTimings(appointments)));
            }

            return getOrderedSlots(allSlots);
        }

        public static List<ProviderClientOutgoing.GeneratedSlot> parseAvailabilitiesToSlots(List<Mongo.ServiceProviderAvailability> availabilities, int duration, int interval, int week, List<ProviderClientOutgoing.GeneratedSlot> bookedSlots)
        {
            List<ProviderClientOutgoing.GeneratedSlot> slots = new List<ProviderClientOutgoing.GeneratedSlot>();

            if (week > 1)
                week = (week - 1) * 7;
            else
                week = 0;

            foreach (var availability in availabilities)
            {
                if (!availability.IsDeleted)
                {
                    var currentStartTime = availability.StartTime;

                    while ((currentStartTime.AddMinutes(duration).TimeOfDay) <= availability.EndTime.TimeOfDay)
                    {
                        var currentDate = DateTime.UtcNow;
                        TimeSpan ts = new TimeSpan(currentStartTime.Hour, currentStartTime.Minute, currentStartTime.Second);
                        currentDate = currentDate.Date + ts;

                        var dayOffSet = 0;

                        if ((availability.DayOfWeek - currentDate.DayOfWeek) < 0)
                            dayOffSet = 7 + (availability.DayOfWeek - currentDate.DayOfWeek);
                        else
                            dayOffSet = availability.DayOfWeek - currentDate.DayOfWeek;

                        currentDate = currentDate.AddDays(dayOffSet + week);

                        var tempSlot = new ProviderClientOutgoing.GeneratedSlot();
                        tempSlot.Duration = duration;
                        tempSlot.StartDateTime = currentDate;
                        tempSlot.EndDateTime = currentDate.AddMinutes(duration);
                        tempSlot.PaymentType = availability.PaymentType.ToString();

                        if (availability.OrganisationId != null)
                            tempSlot.OrganisationId = availability.OrganisationId;

                        if (availability.AddressId != null)
                            tempSlot.AddressId = availability.AddressId;

                        if (!IsSlotOverlapping(bookedSlots, tempSlot) && tempSlot.StartDateTime > DateTime.UtcNow)
                        {
                            slots.Add(tempSlot);
                        }

                        currentStartTime = currentStartTime.AddMinutes(duration + interval);
                    }
                }
            }

            return slots;
        }

        private static bool IsSlotOverlapping(List<ProviderClientOutgoing.GeneratedSlot> bookedSlots, ProviderClientOutgoing.GeneratedSlot slotToAdd)
        {
            var preBookedTime = false;
            preBookedTime = IsSlotTimePreBooked(bookedSlots, slotToAdd, preBookedTime);
            return preBookedTime;
        }

        private static bool IsSlotTimePreBooked(List<ProviderClientOutgoing.GeneratedSlot> bookedSlots, ProviderClientOutgoing.GeneratedSlot slotToAdd, bool preBookedTime)
        {
            foreach (var slot in bookedSlots)
            {
                if (slotToAdd.StartDateTime.TimeOfDay == slot.StartDateTime.TimeOfDay &&
                    slotToAdd.StartDateTime.Day == slot.StartDateTime.Day &&
                    slotToAdd.StartDateTime.Month == slot.StartDateTime.Month &&
                    slotToAdd.StartDateTime.Year == slot.StartDateTime.Year)
                {
                    preBookedTime = true;
                    break;
                }
            }

            return preBookedTime;
        }

        private static List<ProviderClientOutgoing.GeneratedSlot> getOrderedSlots(List<ProviderClientOutgoing.GeneratedSlot> slots)
        {
            var allSlots = from slot in slots
                           orderby slot.StartDateTime
                           select slot;
            return allSlots.ToList();
        }
    }
}
