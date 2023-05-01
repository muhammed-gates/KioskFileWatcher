using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskFileWatcher.Model
{
    public class ReservationModel
    {
        public string MembershipNumber { get; set; }
        public string MembershipType { get; set; }
        public string ConfirmationNumber { get; set; }
        public string CRSNumber { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string ExternalRefNumber { get; set; }
        public string RoomNumber { get; set; }
        public string ReservationNameID { get; set; }
        public string ReservationStatus { get; set; }
        public bool ShareFlag { get; set; } = false;
        public string ShareID { get; set; }
        public string AccorRefernce { get; set; }
        public string NameID { get; set; }
    }
}
