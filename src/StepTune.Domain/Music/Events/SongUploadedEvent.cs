using StepTune.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Music.Events
{
    public sealed record SongUploadedEvent(Guid SongId, Guid UserId):DomainEvent;
    
}
