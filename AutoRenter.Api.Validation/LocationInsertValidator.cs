﻿using AutoRenter.Domain.Models;
using FluentValidation;

namespace AutoRenter.Domain.Validation
{
    public class LocationInsertValidator : AbstractValidator<Location>, IValidator<Location>
    {
        public LocationInsertValidator()
        {
            RuleFor(m => m.SiteId).NotNull();
            RuleFor(m => m.Name).NotNull();
        }
    }
}
