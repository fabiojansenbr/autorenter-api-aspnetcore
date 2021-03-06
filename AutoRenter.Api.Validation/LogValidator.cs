﻿using FluentValidation;
using AutoRenter.Domain.Models;

namespace AutoRenter.Domain.Validation
{
    public class LogValidator : AbstractValidator<LogEntry>, IValidator<LogEntry>
    {
        public LogValidator()
        {
            RuleFor(m => m.Message).NotNull();
            RuleFor(m => m.Level).NotNull();
        }
    }
}
