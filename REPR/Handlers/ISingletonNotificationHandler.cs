﻿namespace REPR.Handlers;

public interface ISingletonNotificationHandler<TRequest> : INotificationHandler<TRequest>
    where TRequest : notnull
{
}
