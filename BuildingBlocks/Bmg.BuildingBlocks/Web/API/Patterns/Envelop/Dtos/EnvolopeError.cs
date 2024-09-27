﻿using Bmg.BuildingBlocks.Domain.Models;

namespace Bmg.API.Dtos
{
    public record EnvolopeError(string Code, string Message, string? InvalidField = default)
    {
        public static implicit operator EnvolopeError(Error error)
        {
            return new EnvolopeError(error.Code, error.Message);
        }

        public static EnvolopeError Create(Error error, string invalidField)
        {
            return new EnvolopeError(error.Code, error.Message, invalidField);
        }
    }
}
