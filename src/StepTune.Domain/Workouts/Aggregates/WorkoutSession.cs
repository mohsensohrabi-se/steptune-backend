using StepTune.Domain.Common;
using StepTune.Domain.Workouts.Entities;
using StepTune.Domain.Workouts.Events;
using StepTune.Domain.Workouts.Services;
using StepTune.Domain.Workouts.ValueObjects;
using System.Linq;

namespace StepTune.Domain.Workouts.Aggregates
{
    public sealed class WorkoutSession : AggregateRoot
    {
        private readonly List<WorkoutDataPoint> _dataPoints = new();

        private WorkoutSession() { }

        public Guid UserId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        public IReadOnlyList<WorkoutDataPoint> DataPoints => _dataPoints.AsReadOnly();

        public int TotalSteps { get; private set; }
        /// <summary>
        /// Internal distance stored in meters.
        /// Use Summary.DistanceKm to expose kilometers.
        /// </summary>
        public double TotalDistanceMeters { get; private set; }
        public double TotalCalories { get; private set; }

        /// <summary>
        /// AveragePace stored in seconds per km (double).
        /// </summary>
        public double AveragePaceSeconds { get; private set; }

        /// <summary>
        /// Average cadence in steps per minute.
        /// </summary>
        public double AverageCadence { get; private set; }

        public int TempoMatchScore { get; private set; }

        // -----------------------
        // FACTORY
        // -----------------------
        public static Result<WorkoutSession> Start(Guid userId)
        {
            if (userId == Guid.Empty)
                return Result.Failure<WorkoutSession>("Invalid user id");

            var session = new WorkoutSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartTime = DateTime.UtcNow
            };

            return Result.Success(session);
        }

        // -----------------------
        // ADD DATA POINT
        // -----------------------
        public void AddDataPoint(WorkoutDataPoint point)
        {
            if (point is null) throw new ArgumentNullException(nameof(point));
            if (EndTime is not null) throw new InvalidOperationException("Cannot add datapoints to a finished session.");

            _dataPoints.Add(point);

            TotalSteps += point.Steps;

            // Keep averages incremental for simplicity
            if (point.Cadence != null)
                AverageCadence = CalculateAverageCadence();

            if (point.Pace != null)
                AveragePaceSeconds = CalculateAveragePaceSeconds();
        }

        // -----------------------
        // CALCULATIONS
        // -----------------------
        private double CalculateAverageCadence()
        {
            var cadences = _dataPoints.Where(x => x.Cadence != null).Select(x => x.Cadence!.StepsPerMinute);
            return cadences.Any() ? cadences.Average() : 0.0;
        }

        private double CalculateAveragePaceSeconds()
        {
            var paces = _dataPoints.Where(x => x.Pace != null).Select(x => x.Pace!.Value.TotalSeconds);
            return paces.Any() ? paces.Average() : 0.0;
        }

        /// <summary>
        /// Rough estimate based on step length. Default stride 0.78 m.
        /// You can later replace with GPS-based distance using data points' GeoPoint.
        /// </summary>
        private double EstimateDistanceMeters()
        {
            const double averageStepLengthMeters = 0.78; // 78 cm
            return TotalSteps * averageStepLengthMeters;
        }

        private int CalculateTempoScore()
        {
            // Normalise: if 180 spm => 100 score; adjust to your formula
            if (AverageCadence <= 0) return 0;
            return (int)Math.Clamp((AverageCadence / 180.0) * 100.0, 0, 100);
        }

        // -----------------------
        // CALCULATE SUMMARY
        // -----------------------
        public WorkoutSummary CalculateSummary()
        {
            if (EndTime is null)
                throw new InvalidOperationException("Session must be ended before calculating summary.");

            var duration = EndTime.Value - StartTime;

            // ensure TotalDistanceMeters is up-to-date (EstimateDistance or use GPS)
            if (TotalDistanceMeters <= 0)
                TotalDistanceMeters = EstimateDistanceMeters();

            return WorkoutSummary.Create(
                distanceKm: TotalDistanceMeters / 1000.0,
                steps: TotalSteps,
                duration: duration,
                calories: (int)Math.Round(TotalCalories),
                bpmAlignmentScore: TempoMatchScore,
                matchedSongsCount: 0, // populate from matching logic if available
                date: DateOnly.FromDateTime(EndTime.Value)
            );
        }

        // -----------------------
        // END SESSION
        // -----------------------
        /// <summary>
        /// End the session. Application layer should compute calories (using user's weight)
        /// and pass it here.
        /// Returns the computed WorkoutSummary.
        /// </summary>
        public WorkoutSummary End(double calories)
        {
            if (EndTime is not null)
                throw new InvalidOperationException("Session already ended.");

            EndTime = DateTime.UtcNow;

            // compute derived fields
            TotalDistanceMeters = EstimateDistanceMeters();
            AverageCadence = CalculateAverageCadence();
            AveragePaceSeconds = CalculateAveragePaceSeconds();
            TempoMatchScore = CalculateTempoScore();

            TotalCalories = calories;

            // Build summary, raise event
            var summary = CalculateSummary();

            AddDomainEvent(new WorkoutFinishedEvent(Id, UserId, summary.DistanceKm, summary.Steps, summary.Calories));

            return summary;
        }
    }
}
