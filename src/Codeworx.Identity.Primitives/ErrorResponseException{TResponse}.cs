﻿using System;

namespace Codeworx.Identity
{
    public class ErrorResponseException<TResponse> : ErrorResponseException
    {
        public ErrorResponseException(TResponse response)
        {
            Response = response;
        }

        public override object Response { get; }

        public TResponse TypedResponse => (TResponse)Response;

        public override Type ResponseType { get; } = typeof(TResponse);
    }
}
