using StepTune.Domain.Workouts.ValueObjects;


namespace StepTune.Domain.Workouts.Services
{
    public static class CadenceCalculator
    {
        public static Cadence Calculate(DateTime t1, int steps1, DateTime t2, int steps2)
        {
            double deltaSeconds = (t2 - t1).TotalSeconds;
            if (deltaSeconds < 0)
                return Cadence.Create(0).Value!;
            int steps = steps2 - steps1;
            var spm = (int)((steps / deltaSeconds)*60) ;
            return Cadence.Create(spm).Value!;
        }
    }
}
