using StepTune.Domain.Common;
using StepTune.Domain.Workouts.Events;
using StepTune.Domain.Workouts.ValueObjects;
using System.ComponentModel;

namespace StepTune.Domain.Workouts.Entities
{
    public sealed class WorkoutStats:AggregateRoot
    {
        private WorkoutStats() { }

        public Guid UserId { get; private set; }

        //lifetime Stats
        public double TotalDistanceKm {  get; private set; }
        public int TotalSteps { get; private set; }
        public TimeSpan TotalDuration { get; private set; }
        public int TotalCalories {  get; private set; }
        public int TotalSessions { get; private set; }

        // Personal records
        public double LongestRunKm { get; private set; }
        public TimeSpan LongestRunDuration { get; private set; }
        public int MaxStepsInSession { get; private set; }

        //Streaks
        public DateOnly? LastWorkoutDate { get; private set; }
        public int CurrentStreakDays { get; private set; }
        public int LongestStreakDays { get; private set; }

        //Music alignment
        public double AverageBpmMatch { get; private set; }
        public double BestBpmMatch { get; private set; }    
        public int TotalMatchedSongs { get; private set; }

        // Factory
        public static WorkoutStats Create(Guid userId)
        {
            return new WorkoutStats
            {
                UserId = userId,
                Id = Guid.NewGuid(),
            };
        }

        public void ApplyWorkout(WorkoutSummary summary)
        {
            TotalDistanceKm += summary.DistanceKm;
            TotalSteps += summary.Steps;
            TotalDuration += summary.Duration;
            TotalCalories += summary.Calories;
            TotalSessions++;

            UpdatePersonalRecords(summary);
            UpdateStreak(summary.Date);
            UpdateBpmStats(summary.BpmAlignmentScore, summary.MatchedSongsCount);
            AddDomainEvent(new WorkoutStatsUpdatedEvent(UserId, Id));
        }
        private void UpdatePersonalRecords(WorkoutSummary s)
        {
            if(s.DistanceKm > LongestRunKm)
                LongestRunKm = s.DistanceKm;
            if(s.Duration > LongestRunDuration)
                LongestRunDuration = s.Duration;
            if(s.Steps > MaxStepsInSession)
                MaxStepsInSession = s.Steps;
        }

        private void UpdateStreak(DateOnly workoutDay)
        {
            if(LastWorkoutDate is null)
            {
                LastWorkoutDate = workoutDay;
                CurrentStreakDays = 1;
                LongestStreakDays = 1;
                return;
            }

            var previous = LastWorkoutDate.Value;
            var diff = workoutDay.DayNumber - previous.DayNumber;
            if(diff == 1)
            {
                CurrentStreakDays++;
                LongestStreakDays = Math.Max(LongestStreakDays, CurrentStreakDays);
            }
            if(diff > 1)
            {
                CurrentStreakDays = 1;
            }
            LastWorkoutDate = workoutDay;
        }

        private void UpdateBpmStats(double alignmentScore, int matchedSongs)
        {
            if (alignmentScore > BestBpmMatch)
                BestBpmMatch = alignmentScore;

            TotalMatchedSongs += matchedSongs;

            AverageBpmMatch =
                ((AverageBpmMatch * (TotalSessions - 1)) + alignmentScore)
                / TotalSessions;
        }
    }
}
