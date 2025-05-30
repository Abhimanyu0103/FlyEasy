using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLYEASY
{
    internal class SessionInfo
    {

        private static SessionInfo instance;

        public string Username { get; set; }
        public string Email { get; set; }
        public string Dept_Airport { get; set; }
        public string Arr_Airport { get; set; }
        public int no_of_passengers { get; set; }
        public string airline {  get; set; }
        public string flight_no {  get; set; }
        public DateTime bookingdate { get; set; }
        public int total {  get; set; }

        public int price { get; set; }

        private SessionInfo()
        {
            Username = "";
            Email = "";
            no_of_passengers = 1;
            Dept_Airport = "";
            Arr_Airport = "";
            price = 0;
            airline = "";
            flight_no = "";
            bookingdate = new DateTime();
            total = 0;
        }

        public static SessionInfo GetInstance()
        {
            if (instance == null)
                instance = new SessionInfo();
            return instance;
        }
    }
    
}
