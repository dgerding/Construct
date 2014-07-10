using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace Construct.Sensors.EmotivEEGSensor
{
    [Serializable]
    public struct CognitivData
    {
        /// <summary>
        /// field for property CurrentAction
        /// </summary>
        private string currentAction;
        /// <summary>
        /// field for property Equal
        /// </summary>
        private bool equal;
        /// <summary>
        /// field for property CurrentActionPower
        /// </summary>
        private float currentActionPower;
        /// <summary>
        /// field for property IsOn
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

        /// <summar>y
        /// gets and sets the value of field currentAction
        /// </summary>
        public string CurrentAction
        {
            get
            {
                return currentAction;
            }
            set
            {
                currentAction = value;
            }
        }

        /// <summary>
        /// gets and sets the value of field currentActionPower
        /// </summary>
        public float CurrentActionPower
        {
            get
            {
                return currentActionPower;
            }
            set
            {
                currentActionPower = value;
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