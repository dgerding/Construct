using System;
using System.Collections.Generic;
using System.Text;

namespace FaceLab
{
    class HeadInfo
    {
        public float headPositionX,
              headPositionY,
              headPositionZ,
              leftEyePositionX,
              leftEyePositionY,
              leftEyePositionZ,
              rightEyePositionX,
              rightEyePositionY,
              rightEyePositionZ,
              vergePositionX,
              vergePositionY,
              vergePositionZ;

        public HeadInfo(float headX, float headY, float headZ, float leftX, float leftY, float leftZ, float rightX, float rightY, float rightZ, float vergeX, float vergeY, float vergeZ)
        {
            headPositionX = headX;
            headPositionY = headY;
            headPositionZ = headZ;
            leftEyePositionX = leftX;
            leftEyePositionY = leftY;
            leftEyePositionZ = leftZ;
            rightEyePositionX = rightX;
            rightEyePositionY = rightY;
            rightEyePositionZ = rightZ;
            vergePositionX = vergeX;
            vergePositionY = vergeY;
            vergePositionZ = vergeZ;
        }
    }
}