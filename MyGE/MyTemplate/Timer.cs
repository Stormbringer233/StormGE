using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace MyTemplate
{
    /** Timer utility class that provide common usefull feature for measuring time
     * 
     * M. Le Thiec
     * 15/10/2018
     * 
     * V : 1.00
     * last update : 15/10/18
     * Add possibility of put timer in pause and resume mode. This provide convenience when customer
     * want stop timer but keep current value then restart later
     **/

    public class Timer
    {
        public delegate void ThresoldReachedEventHandler(object sender, EventArgs e); // event call when timer reach a specific time recursively
        public delegate void ProcessFinishEventHandler(object sender, EventArgs e); // call when timer is finish

        public event ThresoldReachedEventHandler ThresholdReached;
        public event ProcessFinishEventHandler ProcessFinish;

        public double CurrentTime { get; set; }
        public double Duration { get; set; }
        public bool EngageStepTime;
        public double CumulativeStepTime { get; private set; }
        public double StepTime
        {
            get
            {
                return stepTime;
            }
            private set
            {
                if (value > 0)
                    stepTime = value;
                else
                    stepTime = 0;
            }
        }

        // private
        bool Ready;
        double totalStepTime;
        double InitTimer;
        double SavedTime;
        bool Run;
        double stepTime;
        ProcessFinishEventHandler EndProcessFunction;
        ThresoldReachedEventHandler EndStepFunction;

        public Timer()
            // simple constructor to only use for timer count whitout maxTime comparison
        {
            CurrentTime = 0;
            Duration = 0;
            InitTimer = 0;
            StepTime = 0;
            Initialize();
        }

        public Timer(double pMaxTime)
        // Full timer constructor for common usage
        {
            Duration = pMaxTime;
            InitTimer = pMaxTime;
            StepTime = 0;
            Initialize();
        }

        public Timer(double pMaxTime, ProcessFinishEventHandler pFunction)
            // Full timer constructor for common usage
        {
            EndProcessFunction = pFunction;
            Duration = pMaxTime;
            InitTimer = pMaxTime;
            StepTime = 0;
            Initialize();
        }

        public Timer(double pMaxTime, double pStepTime)
        {
            Duration = pMaxTime;
            StepTime = pStepTime;
            InitTimer = pMaxTime;
            Initialize();
        }

        public Timer(double pMaxTime, double pStepTime, ProcessFinishEventHandler pFunction)
        {
            EndProcessFunction = pFunction;
            Duration = pMaxTime;
            StepTime = pStepTime;
            InitTimer = pMaxTime;
            Initialize();
        }

        public Timer(double pMaxTime, double pStepTime, ProcessFinishEventHandler pFunction, ThresoldReachedEventHandler pStepFunction)
        {
            EndProcessFunction = pFunction;
            EndStepFunction = pStepFunction;
            Duration = pMaxTime;
            StepTime = pStepTime;
            InitTimer = pMaxTime;
            Initialize();
        }

        private void Initialize()
            // Initialize commons variables
        {
            CurrentTime = 0;
            Ready = false; // specify when maxTime is arrived and timer is ready for new count
            Run = true; // set the timer update
            totalStepTime = 0;
            CumulativeStepTime = 0;
            EngageStepTime = false;
        }

        private void AddMicroTime(double time)
        {
            if (StepTime > 0 && EngageStepTime)
            {
                totalStepTime += time;
                if (totalStepTime >= StepTime)
                {
                    CumulativeStepTime += totalStepTime;
                    totalStepTime = 0;
                    OnThresholdReached(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnThresholdReached(EventArgs e)
        {
            //Console.WriteLine("Timer.OnThresholdReached() - ok");
            EndStepFunction?.Invoke(this, e);
        }

        protected virtual void OnProcessFinish(EventArgs e)
            // inform customer that Timer has finish
        {
            CurrentTime = 0;
            EngageStepTime = false;
            CumulativeStepTime = 0;
            //Console.WriteLine("Timer process is finish");
            EndProcessFunction?.Invoke(this, e);
        }

        public void Pause()
        {
            Run = false;
        }

        public void Resume()
        {
            Run = true;
        }

        public void ResetTimer()
        {
            Duration = InitTimer;
            Flush();
            Run = true;
        }

        public void Push()
            // Save current state of the timer
        {
            SavedTime = Duration;
        }

        public void Pop()
            // Restaure state of timer after a push operation
        {
            if (SavedTime > 0)
            {
                Duration = SavedTime;
                SavedTime = 0;
            }
        }

        public void Increase(double pAmount)
            // increase (or decrease if pAmount < 0) the timer
        {
            Duration += Math.Max(0, pAmount);
        }

        public void ProportionalIncrease(double pAmount)
            // Same as Increase but by a ratio
        {
            Duration *= Math.Max(0, pAmount);
        }

        public void Flush()
            // set the current Timer to 0
        {
            CurrentTime = 0;
            EngageStepTime = false;
            CumulativeStepTime = 0;
            //StepTime = 0;
            OnProcessFinish(EventArgs.Empty);
        }

        public void Flood()
            // force the end of timer compute
        {
            CurrentTime = Duration;
        }

        public void Update(GameTime gameTime)
        {
            if (Run) // update timer only if not in pause
            {
            	double time = gameTime.ElapsedGameTime.TotalSeconds;
                //Console.WriteLine("Begin Update() - CurrentTime = " + CurrentTime);
                if (Duration > 0)
                {
                    //Debug.WriteLineIf((CurrentTime >= Duration), "Time is finish inside Timer");
                    if (CurrentTime >= Duration)
                    {
                        OnProcessFinish(EventArgs.Empty);
                    }
                    else
                    {
                        CurrentTime += time;
                        AddMicroTime(time);
                    }
                }
                else
                    // usefull when using simple timer count
                    CurrentTime += time;
                //Console.WriteLine("\t -> End Update() - CurrentTime = " + CurrentTime);

            }
        }
    }
}
