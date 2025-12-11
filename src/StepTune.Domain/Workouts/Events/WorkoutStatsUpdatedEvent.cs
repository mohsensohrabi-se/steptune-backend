using StepTune.Domain.Common;


namespace StepTune.Domain.Workouts.Events
{
    public sealed record WorkoutStatsUpdatedEvent(Guid UserId, Guid StatsId):DomainEvent;

}
