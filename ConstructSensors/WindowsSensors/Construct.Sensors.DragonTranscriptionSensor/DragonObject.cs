using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Construct.Sensors.DragonTranscriptionSensor
{
    class DragonObject
    {
        public string
            speaker,
            topic,
            targetWav,
            targetTxt;

        public DragonSystemType
            dragonSystemType;

        public enum DragonSystemType
        {
            Transcriber = 0,
            ProfileUpdater = 1
        }

        DragonObject(DragonSystemType systemType, string Speaker, string Topic, string TargetWav, string TargetTxt = null)
        {
            dragonSystemType = systemType;
            speaker = Speaker;
            topic = Topic;
            targetWav = TargetWav;
            targetTxt = TargetTxt;
        }
    }
}
