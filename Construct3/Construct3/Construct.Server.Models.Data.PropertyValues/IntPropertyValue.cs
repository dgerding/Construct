using System;
using System.Linq;
using System.Runtime.Serialization;


namespace Construct.Server.Models.Data.PropertyValue
{
    [DataContract]
    public class IntPropertyValue
    {
        [DataMember]
        public Guid ItemID
        {
            get;
            set;
        }

        [DataMember]
        public Guid SourceID
        {
            get;
            set;
        }

        [DataMember]
        public Guid PropertyID
        {
            get;
            set;
        }

        [DataMember]
        public long? Interval
        {
            get;
            set;
        }

        [DataMember]
        public DateTime StartTime
        {
            get;
            set;
        }

        [DataMember]
        public Int32 Value
        {
            get;
            set;
        }

        [DataMember]
        public string Latitude
        {
            get;
            set;
        }

        [DataMember]
        public string Longitude
        {
            get;
            set;
        }


        public static bool operator ==(IntPropertyValue left, IntPropertyValue right)
        {

            if ((object)left == null)
            {
                if ((object)right == null)
                {
                    return true;
                }
                return false;
            }
            else if ((object)right == null)
            {
                if ((object)left != null)
                {
                    return false;
                }
            }
            else
            {
                if (left.ItemID == right.ItemID)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool operator !=(IntPropertyValue left, IntPropertyValue right)
        {
            return (left == right) == false;
        }

        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj is IntPropertyValue)
            {
                result = this == (IntPropertyValue)obj;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}