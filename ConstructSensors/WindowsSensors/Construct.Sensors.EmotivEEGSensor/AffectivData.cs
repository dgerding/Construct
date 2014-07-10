using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    [Serializable]
    public struct AffectivData
    {
        /// <summary>
        /// field for property Equal
        /// </summary>
        private bool equal;
        /// <summary>
        /// field for property EngagementBordomLevel
        /// </summary>
        private float engagementBordomLevel;
        /// <summary>
        /// field for property LongTermExcitement
        /// </summary>
        private float longTermExcitement;
        /// <summary>
        /// field for property ShortTermExcitement
        /// </summary>
        private float shortTermExcitement;
        /// <summary>
        /// field for property FrustrationScore
        /// </summary>
        private float frustrationScore;
        /// <summary>
        /// field for property MeditationScore
        /// </summary>
        private float meditationScore;
        /// <summary>
        /// field for property isOn
        /// </summary>
        private bool isOn;

        /// <summary>
        /// gets and sets the value of field equal
        /// </summary>
        public bool Equal
        {
            get
            {
                return equal;
            }
            set
            {
                equal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field engagementBordomLevel
        /// </summary>
        public float EngagementBordomLevel
        {
            get
            {
                return engagementBordomLevel;
            }
            set
            {
                engagementBordomLevel = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field longTermExcitement
        /// </summary>
        public float LongTermExcitement
        {
            get
            {
                return longTermExcitement;
            }
            set
            {
                longTermExcitement = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field shortTermExcitement
        /// </summary>
        public float ShortTermExcitement
        {
            get
            {
                return shortTermExcitement;
            }
            set
            {
                shortTermExcitement = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field frustrationScore
        /// </summary>
        public float FrustrationScore
        {
            get
            {
                return frustrationScore;
            }
            set
            {
                frustrationScore = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field meditationScore
        /// </summary>
        public float MeditationScore
        {
            get
            {
                return meditationScore;
            }
            set
            {
                meditationScore = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field isOn
        /// </summary>
        public bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                isOn = value;
            }
        }
    }
}
