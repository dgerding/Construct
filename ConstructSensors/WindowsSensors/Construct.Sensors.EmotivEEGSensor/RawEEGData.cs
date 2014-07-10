using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    [Serializable]
    public struct RawEEGData
    {
        /// <summary>
        /// field for property LeftPrefrontal
        /// </summary>
        private float leftPrefrontal;
        /// <summary>
        /// field for property LeftFrontalA
        /// </summary>
        private float leftFrontalA;
        /// <summary>
        /// field for property LeftFrontalB
        /// </summary>
        private float leftFrontalB;
        /// <summary>
        /// field for property LeftCenterFrontal
        /// </summary>
        private float leftCenterFrontal;
        /// <summary>
        /// field for property LeftTemporal
        /// </summary>
        private float leftTemporal;
        /// <summary>
        /// field for property LeftParietal
        /// </summary>
        private float leftParietal;
        /// <summary>
        /// field for property LeftOccipital
        /// </summary>
        private float leftOccipital;
        /// <summary>
        /// field for property RightOccipital
        /// </summary>
        private float rightOccipital;
        /// <summary>
        /// field for property RightParietal
        /// </summary>
        private float rightParietal;
        /// <summary>
        /// field for property RightTemporal
        /// </summary>
        private float rightTemporal;
        /// <summary>
        /// field for property RightCenterFrontal
        /// </summary>
        private float rightCenterFrontal;
        /// <summary>
        /// field for property RightFrontalB
        /// </summary>
        private float rightFrontalB;
        /// <summary>
        /// field for property RightFrontalA
        /// </summary>
        private float rightFrontalA;
        /// <summary>
        /// field for property RightPrefrontal
        /// </summary>
        private float rightPrefrontal;
        /// <summary>
        /// field for property Counter
        /// </summary>
        private double counter;
        /// <summary>
        /// field for property Interpolated
        /// </summary>
        private double interpolated;
        /// <summary>
        /// field for property RawCQ
        /// </summary>
        private double rawCQ;
        /// <summary>
        /// field for property GyroX
        /// </summary>
        private float gyroX;
        /// <summary>
        /// field for property GyroY
        /// </summary>
        private float gyroY;
        /// <summary>
        /// field for property TimeStamp
        /// </summary>
        private float timeStamp;
        /// <summary>
        /// field for property EsTimeStamp
        /// </summary>
        private float esTimeStamp;
        /// <summary>
        /// field for property FuncId
        /// </summary>
        private float funcId;
        /// <summary>
        /// field for property FuncValue
        /// </summary>
        private float funcValue;
        /// <summary>
        /// field for property Marker
        /// </summary>
        private float marker;
        /// <summary>
        /// field for property SyncSignal
        /// </summary>
        private float syncSignal;

        /// <summary>
        /// gets and sets the value of field leftPrefrontal
        /// </summary>
        public float LeftPrefrontal
        {
            get
            {
                return leftPrefrontal;
            }
            set
            {
                leftPrefrontal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field leftFrontalA
        /// </summary>
        public float LeftFrontalA
        {
            get
            {
                return leftFrontalA;
            }
            set
            {
                leftFrontalA = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field leftFrontalB
        /// </summary>
        public float LeftFrontalB
        {
            get
            {
                return leftFrontalB;
            }
            set
            {
                leftFrontalB = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field leftCentralFrontl
        /// </summary>
        public float LeftCenterFrontal
        {
            get
            {
                return leftCenterFrontal;
            }
            set
            {
                leftCenterFrontal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field leftTemporal
        /// </summary>
        public float LeftTemporal
        {
            get
            {
                return leftTemporal;
            }
            set
            {
                leftTemporal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field leftParietal
        /// </summary>
        public float LeftParietal
        {
            get
            {
                return leftParietal;
            }
            set
            {
                leftParietal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field leftOccipital
        /// </summary>
        public float LeftOccipital
        {
            get
            {
                return leftOccipital;
            }
            set
            {
                leftOccipital = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightOccipital
        /// </summary>
        public float RightOccipital
        {
            get
            {
                return rightOccipital;
            }
            set
            {
                rightOccipital = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightParietal
        /// </summary>
        public float RightParietal
        {
            get
            {
                return rightParietal;
            }
            set
            {
                rightParietal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightTemporal
        /// </summary>
        public float RightTemporal
        {
            get
            {
                return rightTemporal;
            }
            set
            {
                rightTemporal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightCenterFrontal
        /// </summary>
        public float RightCenterFrontal
        {
            get
            {
                return rightCenterFrontal;
            }
            set
            {
                rightCenterFrontal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightFrontalB
        /// </summary>
        public float RightFrontalB
        {
            get
            {
                return rightFrontalB;
            }
            set
            {
                rightFrontalB = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightFrontalA
        /// </summary>
        public float RightFrontalA
        {
            get
            {
                return rightFrontalA;
            }
            set
            {
                rightFrontalA = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightPrefrontal
        /// </summary>
        public float RightPrefrontal
        {
            get
            {
                return rightPrefrontal;
            }
            set
            {
                rightPrefrontal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field counter
        /// </summary>
        public double Counter
        {
            get
            {
                return counter;
            }
            set
            {
                counter = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field interpolated
        /// </summary>
        public double Interpolated
        {
            get
            {
                return interpolated;
            }
            set
            {
                interpolated = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rawCQ
        /// </summary>
        public double RawCQ
        {
            get
            {
                return rawCQ;
            }
            set
            {
                rawCQ = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field gyroX
        /// </summary>
        public float GyroX
        {
            get
            {
                return gyroX;
            }
            set
            {
                gyroX = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field gyroY
        /// </summary>
        public float GyroY
        {
            get
            {
                return gyroY;
            }
            set
            {
                gyroY = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field timeStamp
        /// </summary>
        public float TimeStamp
        {
            get
            {
                return timeStamp;
            }
            set
            {
                timeStamp = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field funcId
        /// </summary>
        public float FuncId
        {
            get
            {
                return funcId;
            }
            set
            {
                funcId = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field funcValue
        /// </summary>
        public float FuncValue
        {
            get
            {
                return funcValue;
            }
            set
            {
                funcValue = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field marker
        /// </summary>
        public float Marker
        {
            get
            {
                return marker;
            }
            set
            {
                marker = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field syncSignal
        /// </summary>
        public float SyncSignal
        {
            get
            {
                return syncSignal;
            }
            set
            {
                syncSignal = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field esTimeStamp
        /// </summary>
        public float EsTimeStamp
        {
            get
            {
                return esTimeStamp;
            }
            set
            {
                esTimeStamp = value;
            }
        }
    }
}
