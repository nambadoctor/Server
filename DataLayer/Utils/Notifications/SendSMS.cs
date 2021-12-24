using System;
using System.Collections.Specialized;
using System.Net;

namespace DataLayer.Notifications
{
    public class TextLocalSMS
    {
        public static string SendAppointmentReminderSMS(string phoneNumber, string time, string user, string link)
        {
            String message = Uri.EscapeDataString($"Upcoming appointment. \nYour appointment with {user.Trim().Replace(" ", "")} is in {time.Trim().Replace(" ", "")}. Please be ready for the call.\n-Namba Doctor");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                    {
                    {"num_parts","1"},
                    {"apikey" , "ZWNhYThlY2I0ZTk5MzVkOTMxZTkxNDQ1MzhhM2I2NTI="},
                    {"numbers" , $"{phoneNumber}"},
                    {"message" , message},
                    {"sender" , "NMBADR"},
                    {"test", "false" },
                    });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

        public static string SendAdminVerificationSMS(string customerPhoneNumber, string drName, string custName, string phoneNumber)
        {
            String message = Uri.EscapeDataString($"New Unverified appointment.\nAppointment with {drName.Trim()} booked by {custName.Trim()}.\nPatient Ph: {customerPhoneNumber}.\n-Namba Doctor");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                    {
                    {"num_parts","1"},
                    {"apikey" , "ZWNhYThlY2I0ZTk5MzVkOTMxZTkxNDQ1MzhhM2I2NTI="},
                    {"numbers" , $"{phoneNumber}"},
                    {"message" , message},
                    {"sender" , "NmbaDr "},
                    {"test", "false" },
                    });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

        public static string SendAppointmentStatusSMS(string phoneNumber, string time, string user, string status)
        {
            String message = Uri.EscapeDataString($"Appointment {status}.\nYour appointment on {time}(IST) with {user} is {status}.\n-Namba Doctor");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                    {
                    {"num_parts","1"},
                    {"apikey" , "ZWNhYThlY2I0ZTk5MzVkOTMxZTkxNDQ1MzhhM2I2NTI="},
                    {"numbers" , $"{phoneNumber}"},
                    {"message" , message},
                    {"sender" , "NmbaDr"},
                    {"test", "false" },
                    });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

        public static string SendNewCustomerRegistrationSMS(string phoneNumber)
        {
            String message = Uri.EscapeDataString($"Successfully registered on Namba Doctor!\nTo view your appointments, download the app at https://nambadoctor.page.link/app\n-Namba Doctor");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                    {
                    {"num_parts","1"},
                    {"apikey" , "ZWNhYThlY2I0ZTk5MzVkOTMxZTkxNDQ1MzhhM2I2NTI="},
                    {"numbers" , $"{phoneNumber}"},
                    {"message" , message},
                    {"sender" , "NmbaDr"},
                    {"test", "false" },
                    });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

        public static string SendPrescriptionSMS(string phoneNumber, string user)
        {
            String message = Uri.EscapeDataString($"Prescription added\nCheck out your prescription sent by {user}.\n-Namba Doctor ");
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                    {
                    {"num_parts","1"},
                    {"apikey" , "ZWNhYThlY2I0ZTk5MzVkOTMxZTkxNDQ1MzhhM2I2NTI="},
                    {"numbers" , $"{phoneNumber}"},
                    {"message" , message},
                    {"sender" , "NmbaDr"},
                    {"test", "false" },
                    });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

    }
}
