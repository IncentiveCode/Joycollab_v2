using System;

namespace Joycollab.v2
{
    public struct EulerAngles 
    {
        private float roll, pitch, yaw;

        public float Roll 
        {
            get { return roll; }
            set { roll = value; }
        }

        public float Pitch 
        {
            get { return pitch; }
            set { pitch = value; }
        }

        public float Yaw 
        {
            get { return yaw; }
            set { yaw = value; }
        }
    }


    [Serializable]
    public class GestureDetectProperties 
    {
        private float force;
        private float angularForce;
        private float tolerance;
        private float detectTime;

        public GestureDetectProperties() 
        {
            force = angularForce = tolerance = detectTime = 0f;
        }

        public float Force 
        {
            get { return (force == 0f) ? 2.75f : force; }
            set { force = 2f + (4 * value); }
        }

        public float AngularForce 
        {
            get { return (angularForce == 0f) ? 450.0f : angularForce; }
            set { angularForce = 200f + (1000 * value); }
        }

        public float Tolerance 
        {
            get { return (tolerance == 0f) ? 10.0f : tolerance; }
            set { tolerance = (40 * value); }
        }

        public float DetectTime 
        {
            get { return (detectTime == 0f) ? 3.0f : detectTime; }
            set { detectTime = 0.5f + (5 * value); }
        }
    }
}