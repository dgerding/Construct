using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Construct.Server.Models.Visualizations
{
    [DataContract]
    public class LabeledItemAdapter
    {
        private Guid labeledItemID;
        [DataMember]
        public Guid LabeledItemID
        {
            get { return labeledItemID; }
            set { labeledItemID = value; }
        }

        private Guid labeledPropertyID;
        [DataMember]
        public Guid LabeledPropertyID
        {
            get { return labeledPropertyID; }
            set { labeledPropertyID = value; }
        }


        private Guid labeledSourceID;
        [DataMember]
        public Guid LabeledSourceID
        {
            get { return labeledSourceID; }
            set { labeledSourceID = value; }
        }


        private Guid taxonomyLabelID;
        [DataMember]
        public Guid TaxonomyLabelID
        {
            get { return taxonomyLabelID; }
            set { taxonomyLabelID = value; }
        }


        private Guid sessionID;
        [DataMember]
        public Guid SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }


        private DateTime labeledStartTime;
        [DataMember]
        public DateTime LabeledStartTime
        {
            get { return labeledStartTime; }
            set { labeledStartTime = value; }
        }


        private int labeledInterval;
        [DataMember]
        public int LabeledInterval
        {
            get { return labeledInterval; }
            set { labeledInterval = value; }
        }

    }
}
