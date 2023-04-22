using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared.Models
{
    public class Section: IComparable<Section>
    {
        public List<Table> Tables { get; set; }
        public Staff server { get; set; }
        public DateTime? timeAssigned { get; set; }
        public int? queuePosition { get; set; }

        public Section()
        {
            Tables = new List<Table>();
        }

      //  public override bool Equals(object obj)
    //    {
  //          return base.Equals(obj);
//        }
        //A.CompareTo(X) 
        public int CompareTo(Section sec)
        {
            int retValue = 0;

            if (this.queuePosition != null && sec.queuePosition == null)
            {
                retValue = 1;
            }
            else if (this.queuePosition == null && sec.queuePosition != null)
            {
                retValue = -1;
            }
            else if (this.queuePosition != null && sec.queuePosition != null) // If this is in the queue then .........
            {
                if (this.queuePosition < sec.queuePosition)
                {   //means this was in the queue before sec and is thus of higher priority, "greater in value"
                    retValue = 1;
                }
                if (this.queuePosition > sec.queuePosition)
                {
                    retValue = -1;
                }
                if (this.queuePosition == sec.queuePosition)
                {
                    retValue = 0;
                }
            }
            else // else compare when the server started shift
            {
                if(this.server.ShiftStart < sec.server.ShiftStart)
                {
                    retValue = 1;
                }
                if (this.server.ShiftStart > sec.server.ShiftStart)
                {
                    retValue = -1;
                }
                if (this.server.ShiftStart == sec.server.ShiftStart)
                {
                    retValue = 0;
                }
                if (this.server.ShiftStart == null && sec.server.ShiftStart == null)
                {
                    retValue = 0;
                }
                if (this.server.ShiftStart != null && sec.server.ShiftStart == null)
                {
                    retValue = -1;
                }
            }
            return retValue;
        }

    }
}
