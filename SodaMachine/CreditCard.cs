using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    public class CreditCard
    {
        protected decimal value;
        public string name;

        public CreditCard()
        {
            value = 5m;
            name = "Credit Card";
        }
        public decimal Value
        {
            get
            {
                return value;
            }


        }
    }
}
