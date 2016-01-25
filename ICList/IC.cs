using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICList
{
    class IC
    {
        /// <summary>
        /// IC object, resembling the item and the number of this particular instance
        /// (diferent dates should be storable aswell..)
        /// </summary>
        public string name { get; set; }
        public int number { get; set; }
        public string datecode { get; set; }
        public string pakageType { get; set; }

        /// <summary>
        /// IC object, single uniqe version of a ic.
        /// with datecode diferneces we compare the Name with datcodes, it will apear as 1 collection
        /// </summary>
        /// <param name="Name">Specify the part name (full names go under LongName</param>
        /// <param name="Number">The number of this item we have</param>
        /// <param name="Datecode">The datecode if there is one, can hold strings ATM can only decode nnnn datecodes</param>
        /// <param name="PakageType">Pakagetype, only string, might contain number like in SOIC8, pin numbers might be included.</param>
        public IC(string Name, int Number, string Datecode, string PakageType)
        {
            name = Name;
            number = Number;
            datecode = Datecode;
            pakageType = PakageType;
        }
        //Other properties, methods, events...
        //override of Object.ToString()
        public override string ToString()
        {
            //put your custom code here:
            return name + " " + number + " " + datecode + " " + pakageType;
        }
    }
}
