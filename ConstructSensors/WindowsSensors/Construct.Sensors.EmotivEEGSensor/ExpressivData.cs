using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    [Serializable]
    public struct ExpressivData
    {
        /// <summary>
        /// field for property Equal
        /// </summary>
        private bool equal;
        /// <summary>
        /// field for property ClenchExtent
        /// </summary>
        private float clenchExtent;
        /// <summary>
        /// field for property EyebrowExtent
        /// </summary>
        private float eyebrowExtent;
        /// <summary>
        /// field for property EyelidStateLeftEye
        /// </summary>
        private float eyelidStateLeftEye;
        /// <summary>
        /// field for property EyelidStateRightEye
        /// </summary>
        private float eyelidStateRightEye;
        /// <summary>
        /// field for property EyeLocationX
        /// </summary>
        private float eyeLocationX;
        /// <summary>
        /// field for property EyeLocationY
        /// </summary>
        private float eyeLocationY;
        /// <summary>
        /// field for property LowerFaceAction
        /// </summary>
        private string lowerFaceAction;
        /// <summary>
        /// field for property LowerFaceActionPower
        /// </summary>
        private float lowerFaceActionPower;
        /// <summary>
        /// field for property SmileExtent
        /// </summary>
        private float smileExtent;
        /// <summary>
        /// field for property UpperFaceAction
        /// </summary>
        private string upperFaceAction;
        /// <summary>
        /// field for property UpperFaceActionPower
        /// </summary>
        private float upperFaceActionPower;
        /// <summary>
        /// field for property IsOn
        /// </summary>
        private bool isOn;
        /// <summary>
        /// field for property Blink
        /// </summary>
        private bool blink;
        /// <summary>
        /// field for property EyeOpen
        /// </summary>
        private bool eyeOpen;
        /// <summary>
        /// field for property LookDown
        /// </summary>
        private bool lookDown;
        /// <summary>
        /// field for property LookUp
        /// </summary>
        private bool lookUp;
        /// <summary>
        /// field for property LookLeft
        /// </summary>
        private bool lookLeft;
        /// <summary>
        /// field for property LookRight
        /// </summary>
        private bool lookRight;
        /// <summary>
        /// field for property LeftWink
        /// </summary>
        private bool leftWink;
        /// <summary>
        /// field for property RightWink
        /// </summary>
        private bool rightWink;

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
        /// gets and sets the value of field clenchExtent
        /// </summary>
        public float ClenchExtent
        {
            get
            {
                return clenchExtent;
            }
            set
            {
                clenchExtent = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field eyebrowExtent
        /// </summary>
        public float EyebrowExtent
        {
            get
            {
                return eyebrowExtent;
            }
            set
            {
                eyebrowExtent = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field eyelidStateLeftEye
        /// </summary>
        public float EyelidStateLeftEye
        {
            get
            {
                return eyelidStateLeftEye;
            }
            set
            {
                eyelidStateLeftEye = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field eyelidStateRightEye
        /// </summary>
        public float EyelidStateRightEye
        {
            get
            {
                return eyelidStateRightEye;
            }
            set
            {
                eyelidStateRightEye = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field eyeLocationX
        /// </summary>
        public float EyeLocationX
        {
            get
            {
                return eyeLocationX;
            }
            set
            {
                eyeLocationX = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field eyeLocationY
        /// </summary>
        public float EyeLocationY
        {
            get
            {
                return eyeLocationY;
            }
            set
            {
                eyeLocationY = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field lowerFaceAction
        /// </summary>
        public string LowerFaceAction
        {
            get
            {
                return lowerFaceAction;
            }
            set
            {
                lowerFaceAction = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field lowerFaceActionPower
        /// </summary>
        public float LowerFaceActionPower
        {
            get
            {
                return lowerFaceActionPower;
            }
            set
            {
                lowerFaceActionPower = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field smileExtent
        /// </summary>
        public float SmileExtent
        {
            get
            {
                return smileExtent;
            }
            set
            {
                smileExtent = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field upperFaceAction
        /// </summary>
        public string UpperFaceAction
        {
            get
            {
                return upperFaceAction;
            }
            set
            {
                upperFaceAction = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field upperFaceActionPower
        /// </summary>
        public float UpperFaceActionPower
        {
            get
            {
                return upperFaceActionPower;
            }
            set
            {
                upperFaceActionPower = value;
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

        /// <summary>
        /// gets and sets the value of field blink
        /// </summary>
        public bool Blink
        {
            get
            {
                return blink;
            }
            set
            {
                blink = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field eyeOpen
        /// </summary>
        public bool EyeOpen
        {
            get
            {
                return eyeOpen;
            }
            set
            {
                eyeOpen = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field lookDown
        /// </summary>
        public bool LookDown
        {
            get
            {
                return lookDown;
            }
            set
            {
                lookDown = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field lookUp
        /// </summary>
        public bool LookUp
        {
            get
            {
                return lookUp;
            }
            set
            {
                lookUp = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field lookLeft
        /// </summary>
        public bool LookLeft
        {
            get
            {
                return lookLeft;
            }
            set
            {
                lookLeft = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field lookRight
        /// </summary>
        public bool LookRight
        {
            get
            {
                return lookRight;
            }
            set
            {
                lookRight = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field leftWink
        /// </summary>
        public bool LeftWink
        {
            get
            {
                return leftWink;
            }
            set
            {
                leftWink = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field rightWink
        /// </summary>
        public bool RightWink
        {
            get
            {
                return rightWink;
            }
            set
            {
                rightWink = value;
            }
        }
    }
}
