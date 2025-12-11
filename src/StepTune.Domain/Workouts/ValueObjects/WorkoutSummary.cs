using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Workouts.ValueObjects
{
    public sealed class WorkoutSummary
    {
        public double DistanceKm { get; }
        public int Steps { get; }
        public TimeSpan Duration { get; }
        public int Calories { get; }
        public double BpmAlignmentScore { get; }
        public int MatchedSongsCount { get; }
        public DateOnly Date { get; }

        private WorkoutSummary(
            double distanceKm,
            int steps,
            TimeSpan duration,
            int calories,
            double bpmAlignmentScore,
            int matchedSongsCount,
            DateOnly date
            ) 
        { 
            DistanceKm = distanceKm;
            Steps = steps;
            Duration = duration;
            Calories = calories;
            BpmAlignmentScore = bpmAlignmentScore;
            MatchedSongsCount = matchedSongsCount;
            Date = date;
        }

        public static WorkoutSummary Create(
            double distanceKm,
            int steps,
            TimeSpan duration,
            int calories,
            double bpmAlignmentScore,
            int matchedSongsCount,
            DateOnly date)
        {
            return new WorkoutSummary(distanceKm, steps, duration, calories, bpmAlignmentScore, matchedSongsCount, date);
        }
    }
}
