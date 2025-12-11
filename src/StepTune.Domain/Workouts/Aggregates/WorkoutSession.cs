using StepTune.Domain.Common;
using StepTune.Domain.Workouts.Entities;
using StepTune.Domain.Workouts.Events;
using StepTune.Domain.Workouts.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Workouts.Aggregates
{
    public class WorkoutSession : AggregateRoot
    {
        private readonly List<WorkoutDataPoint> _dataPoints = new();
        private WorkoutSession() { }
        public Guid UserId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        public IReadOnlyList<WorkoutDataPoint> DataPoints => _dataPoints;

        public int TotalSteps { get; private set; }
        public double TotalDistance { get; private set; }
        public double TotalCalories { get; private set; }
        public double AveragePace { get; private set; }
        public double AverageCadence { get; private set; }
        public int TempoMatchScore { get; private set; }

        //---------------------------
        // FACTORY
        //--------------------------
        public static Result<WorkoutSession> Start(Guid userId)
        {
            if (userId == Guid.Empty)
                return Result.Failure<WorkoutSession>("Invalide User Id");
            var session = new WorkoutSession()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartTime = DateTime.UtcNow
            };

            return Result.Success(session);
        }

        //-----------------------------
        // ADD DATA POINTS
        //-----------------------------
        public void AddDatapoint(WorkoutDataPoint point)
        {
            _dataPoints.Add(point);
            TotalSteps += point.steps;
            if (point.Cadence != null)
                AverageCadence = CalculateAverageCadence();
            if(point.Pace != null)
                AveragePace = CalculateAveragePace();

        }

        // ------------------------------
        // END
        //-------------------------------
        public void End(double calories)
        {
            if (EndTime is not null)
                return;

            EndTime = DateTime.UtcNow;

            TotalCalories = calories;
            TotalDistance = EstimateDistance();
            TempoMatchScore = CalculateTempoScore();

            AddDomainEvent(new WorkoutFinishedEvent(Id, UserId, TotalDistance, TotalSteps, TotalCalories));
        }
        private double EstimateDistance() => TotalSteps * 0.78;
        private double CalculateAverageCadence() =>
            _dataPoints.Where(x => x.Cadence != null).Average(x => x.Cadence!.StepsPerMinute);
        private double CalculateAveragePace() =>
            _dataPoints.Where(x => x.Pace != null).Average(x => x.Pace!.Value.TotalSeconds);
        private int CalculateTempoScore() =>
            (int)Math.Clamp((AverageCadence / 180) * 100, 0, 100);
    }
}
