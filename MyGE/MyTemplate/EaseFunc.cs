using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    /// <summary>
    /// Simple static class for tween behaviors
    /// 
    /// From : https://github.com/EmmanuelOga/easing/blob/master/lib/easing.lua
    /// 
    /// M. Le Thiec
    /// 22/11/2018
    /// 
    /// </summary>

    public static class EaseFunc
    {
        // Base parameters (t, b, c, d)
        public static double Linear(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            return (Distance * (CurrentTime / Duration) + InitialPosition);
        }

        public static double InQuad(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime /= Duration;
            return (Distance * CurrentTime * CurrentTime) + InitialPosition;
        }

        public static double OutQuad(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime /= Duration;
            return -Distance * CurrentTime * (CurrentTime - 2) + InitialPosition;
        }

        public static double InOutQuad(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime = CurrentTime / Duration * 2;
            if (CurrentTime < 1f)
                return Distance / 2 * CurrentTime * CurrentTime + InitialPosition;
            else
                return -Distance / 2 * ((CurrentTime - 1) * (CurrentTime - 3) - 1) + InitialPosition;
        }

        public static double OutInQuad(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime < Duration / 2)
            {
                return OutQuad(CurrentTime * 2, InitialPosition, Distance / 2, Duration);
            }
            else
            {
                return InQuad((CurrentTime * 2) - Duration, InitialPosition + (Distance / 2), Distance / 2, Duration);
            }
        }

        public static double InCubic(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime /= Duration;
            return Distance * CurrentTime * CurrentTime * CurrentTime + InitialPosition;
        }

        public static double OutCubic(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime = CurrentTime / Duration - 1;
            return Distance * (CurrentTime * CurrentTime * CurrentTime + 1) + InitialPosition;
        }

        public static double InOutCubic(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime = CurrentTime / Duration * 2;
            if (CurrentTime < 1)
            {
                return Distance / 2 * CurrentTime * CurrentTime * CurrentTime + InitialPosition;
            }
            else
            {
                CurrentTime -= 2;
                return Distance / 2 * (CurrentTime * CurrentTime * CurrentTime + 2) + InitialPosition;
            }
        }

        public static double OutInCubic(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime < Duration / 2)
            {
                return OutCubic(CurrentTime * 2, InitialPosition, Distance / 2, Duration);
            }
            else
            {
                return InCubic((CurrentTime * 2) - Duration, InitialPosition + Distance / 2, Distance / 2, Duration);
            }
        }

        public static double InQuart(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime /= Duration;
            return Distance * CurrentTime * CurrentTime * CurrentTime * CurrentTime + InitialPosition;
        }

        public static double OutQuart(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime = CurrentTime / Duration - 1;
            return -Distance * (CurrentTime * CurrentTime * CurrentTime * CurrentTime - 1) + InitialPosition;
        }

        public static double InOutQuart(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            CurrentTime = CurrentTime / Duration * 2;
            if (CurrentTime < 1)
            {
                return Distance / 2 * CurrentTime * CurrentTime * CurrentTime * CurrentTime + InitialPosition;
            }
            else
            {
                CurrentTime -= 2;
                return -Distance / 2 * (CurrentTime * CurrentTime * CurrentTime * CurrentTime - 2) + InitialPosition;
            }
        }

        public static double OutInQuart(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime < Duration / 2)
            {
                return OutQuart(CurrentTime * 2, InitialPosition, Distance / 2, Duration);
            }
            else
            {
                return InQuart((CurrentTime * 2) - Duration, InitialPosition + Distance / 2, Distance / 2, Duration);
            }
        }

        public static double InSine(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            return -Distance * Math.Cos(CurrentTime / Duration * (Math.PI / 2)) + Distance + InitialPosition;
        }

        public static double OutSine(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            return Distance * Math.Sin(CurrentTime / Duration * (Math.PI / 2)) + InitialPosition;
        }

        public static double InOutSine(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            return -Distance / 2 * (Math.Cos(Math.PI * CurrentTime / Duration) - 1) + InitialPosition;
        }

        public static double OutInSine(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime < Duration / 2)
            {
                return OutSine(CurrentTime * 2, InitialPosition, Distance / 2, Duration);
            }
            else
            {
                return InSine((CurrentTime * 2) - Duration, InitialPosition + Distance / 2, Distance / 2, Duration);
            }
        }

        public static double InExpo(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime == 0)
            {
                return InitialPosition;
            }
            else
            {
                return Distance * Math.Pow(2, (10 * (CurrentTime / Duration - 1))) + InitialPosition - Distance * 0.001f;
            }
        }

        public static double OutExpo(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime == Duration)
            {
                return InitialPosition + Distance;
            }
            else
            {
                return Distance * 1.001f * (-Math.Pow(2, -10 * CurrentTime / Duration) + 1) + InitialPosition;
            }
        }

        public static double InElastic(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime == 0) return InitialPosition;
            if ((CurrentTime /= Duration) == 1) return InitialPosition + Distance;

            double Period = Duration * 0.3f;
            double Amplitude = Distance;
            double s = Period / 4;
            double postfix = Amplitude * Math.Pow(2, 10 * (CurrentTime -= 1));
            return -(postfix * Math.Sin((CurrentTime * Duration - s) * (2 * Math.PI) / Period)) + InitialPosition;
        }

        public static double OutElastic(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime == 0) return InitialPosition;
            if ((CurrentTime /= Duration) == 1) return InitialPosition + Distance;
            double Period = Duration * .3f;
            double Amplitude = Distance;
            double s = Period / 4;
            return Amplitude * Math.Pow(2, -10 * CurrentTime) * Math.Sin((CurrentTime * Duration - s) * (2 * Math.PI) / Period) + Distance + InitialPosition;
        }

        public static double InOutElastic(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime == 0) return InitialPosition;
            if ((CurrentTime /= Duration / 2) == 2) return InitialPosition + Distance;
            double Period = Duration * (0.3f * 1.5f);
            double Amplitude = Distance;
            double s = Period / 4;

            if (CurrentTime < 1)
            {
                double postFixLocal = Amplitude * Math.Pow(2, 10 * (CurrentTime -= 1)); // postIncrement is evil
                return -.5f * (postFixLocal * Math.Sin((CurrentTime * Duration - s) * (2 * Math.PI) / Period)) + InitialPosition;
            }
            double postFix = Amplitude * Math.Pow(2, -10 * (CurrentTime -= 1)); // postIncrement is evil
            return postFix * Math.Sin((CurrentTime * Duration - s) * (2 * Math.PI) / Period) * .5f + Distance + InitialPosition;
        }

        public static double OutBounce(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            double p = 2.75;
            CurrentTime /= Duration;
            if (CurrentTime < (1 / p))
            {
                return Distance * (7.5625 * CurrentTime * CurrentTime) + InitialPosition;
            }
            else if (CurrentTime < (2 / p))
            {
                CurrentTime -= 1.5 / p;
                return Distance * (7.5625 * CurrentTime * CurrentTime + .75) + InitialPosition;
            }
            else if (CurrentTime < (2.5 / p))
            {
                CurrentTime -= 2.25 / p;
                return Distance * (7.5625 * CurrentTime * CurrentTime + .9375) + InitialPosition;
            }
            else
            {
                CurrentTime -= 2.625 / p;
                return Distance * (7.5625 * CurrentTime * CurrentTime + .984375) + InitialPosition;
            }
        }

        public static double InBounce(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            return Distance - OutBounce(Duration - CurrentTime, 0, Distance, Duration) + InitialPosition;
        }

        public static double InOutBounce(double CurrentTime, double InitialPosition, double Distance, double Duration)
        {
            if (CurrentTime < Duration / 2)
                return InBounce(CurrentTime * 2, 0, Distance, Duration) * 0.5 + InitialPosition;
            else
                return OutBounce(CurrentTime * 2 - Duration, 0, Distance, Duration) * 0.5 + Distance * 0.5 + InitialPosition;
        }

        // ----------------------- Specific method behavior with amplitude and frequency value --------------------------------------------------

        public static double OutSinGelatine(double CurrentTime, double InitialPosition, double Distance, double Duration, double Amplitude, double Freq)
            // InitialPosition = base scale in case of scale variations
            // distance = attenuation. The more is Distance, the more the attenuation is strength good value arround 2 - 3
            // Amplitude : the amplitude movment. The more is Amplitude, the more the effect is visible
            // Freq : the frequence of the movment.
        {
            return Amplitude * Math.Exp(-Distance * CurrentTime) * Math.Sin(2 * Math.PI * CurrentTime * Freq) + InitialPosition;
        }

        public static double OutCosGelatine(double CurrentTime, double InitialPosition, double Distance, double Duration, double Amplitude, double Freq)
        // InitialPosition = base scale in case of scale variations
        // distance = attenuation. The more is Distance, the more the attenuation is strength good value arround 2 - 3
        // Amplitude : the amplitude movment. The more is Amplitude, the more the effect is visible
        // Freq : the frequence of the movment.
        {
            return Amplitude * Math.Exp(-Distance * CurrentTime) * Math.Cos(2 * Math.PI * CurrentTime * Freq) + InitialPosition;
        }

        public static double InGelatine(double CurrentTime, double InitialPosition, double Distance, double Duration, double Amplitude, double Freq)
        {
            return 1 / OutSinGelatine(CurrentTime, InitialPosition, Distance, Duration, Amplitude, Freq);
        }
    }
}
