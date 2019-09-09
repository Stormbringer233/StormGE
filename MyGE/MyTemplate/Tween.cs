using Microsoft.Xna.Framework;
using MyTemplate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public class Tween
        /// Simple tween class to automatize tween functions
        /// 
        /// M. Le Thiec
        /// creation : 24/02/2019
        /// 
        /// V : 0.00
        /// 

    {
        // Declare Delegate signature Corresponding to one of static method from EaseFunc
        public delegate double BasicTween(double pCurrentTime, double pInitialPosition, double pDistance, double pDuration);

        public delegate void TweenFinishEventHandler(object sender, EventArgs e);

        public double NextValue { get; private set; }
        public double DeltaValue { get; private set; }
        public bool InTween { get; private set; }
        public BasicTween BasicMethod;

        Timer TweenTimer;
        double Distance;
        double Initialposition;
        double oldValue;
        double Amplitude;
        double Frequency;
        TweenFinishEventHandler Function;

        public Tween(BasicTween pMethod, double pDuration, TweenFinishEventHandler pFunction, double pAmplitude = 0, double pFrequency = 0)
            // pMethod must be a static method from EaseFunc class
        {
            BasicMethod = pMethod;
            TweenTimer = new Timer(pDuration, OnTimerFinish);
            Distance = 0;
            Initialposition = 0;
            InTween = false;
            Function = pFunction;
            Amplitude = pAmplitude;
            Frequency = pFrequency;
        }

        public void Initialize(double pInit, double pDist)
        {
            NextValue = 0;
            DeltaValue = 0;
            oldValue = pInit;
            Initialposition = pInit;
            Distance = pDist;
            InTween = true;
            //Debug.WriteLine("Tween initialize at " + pInit + ", " + pDist + "\t| destination = " + (pInit + pDist));
        }

        private double ComputeTween()
            // Call the static method register in Method from constructor
        {
            return BasicMethod(TweenTimer.CurrentTime, Initialposition, Distance, TweenTimer.Duration);
        }

        private void OnTimerFinish(object sender, EventArgs e)
        {
            NextValue = Initialposition + Distance;
            DeltaValue = NextValue - oldValue;
            InTween = false;
            Function?.Invoke(this, EventArgs.Empty);
        }

        public void Update(GameTime gameTime)
        {
            TweenTimer.Update(gameTime);
            if (InTween)
            {
                NextValue = ComputeTween();
                DeltaValue = NextValue - oldValue;
                oldValue = NextValue;
            }
            //Debug.WriteLine("NextValue = " + NextValue + "\tOldValue = " + oldValue + "\tDeltaValue = " + DeltaValue);

        }
    }
}
