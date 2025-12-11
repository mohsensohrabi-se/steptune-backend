using StepTune.Domain.Workouts.Aggregates;

namespace StepTune.Domain.Workouts.Services
{
    public static class CaloriesCalculator
    {
        public static double Estimate(WorkoutSession session, double weightKg)
        {
            double durationMinutes = (session.EndTime!.Value - session.StartTime).TotalMinutes;
            double MET = session.AverageCadence switch
            {
                <90 => 2.5,
                <110 => 3.5,
                <130 => 5.0,
                <150 => 7.0,
                _ => 9.0
            };
            //double weightKg = weightkg; // later get this from UserProfile
            return 0.0175 * MET * weightKg * durationMinutes;
        }
    }
}
