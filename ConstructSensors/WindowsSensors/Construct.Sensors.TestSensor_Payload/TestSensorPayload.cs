using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Sensors.TestSensorPayload
{
    public class TestSensorPayload
    {
        private int theInt;
        public int TheInt { get { return theInt; } set { theInt = value; } }
        public string TheString;
        private bool theBool;
        public bool TheBool { get { return theBool; } set { theBool = value; } }
        public byte[] TheBytes;
        public Guid TheGuid;
        public SubClass TheSubClass;

        public TestSensorPayload()
        {
            theInt = 17;
            TheString = "theString";
            theBool = true;
            TheBytes = new byte[5] { 0, 1, 2, 3, 4 };
            TheGuid = Guid.NewGuid();
            TheSubClass = new SubClass();
        }

        public TestSensorPayload(int intV, string stringV, bool boolV, byte[] bytes, Guid guid, SubClass subClass)
        {
            theInt = intV;
            TheString = stringV;
            theBool = boolV;
            TheBytes = bytes;
            TheGuid = guid;
            TheSubClass = subClass;
        }

        public class SubClass
        {
            public int intOne;
            public double doubleOne;
            public string name;

            public SubClass()
            {
                intOne = 42;
                doubleOne = 6.15;
                name = "someName";
            }

            public SubClass(int intOneV, double doubleOneV, string nameV)
            {
                intOne = intOneV;
                doubleOne = doubleOneV;
                name = nameV;
            }
        }
    }
}
