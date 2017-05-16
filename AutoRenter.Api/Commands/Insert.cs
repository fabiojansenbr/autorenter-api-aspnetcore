﻿using System;
using System.Threading.Tasks;
using AutoRenter.Api.Data;
using AutoRenter.Api.Domain;
using AutoRenter.Api.Commands.Interfaces;

namespace AutoRenter.Api.Commands
{
    public class Insert<T> : IInsertCommand<T>
        where T : class, IEntity
    {
        private readonly AutoRenterContext context;
        public Insert(AutoRenterContext context)
        {
            this.context = context;
        }

        public async Task<Result<Guid>> Execute(T entity)
        {
            var existingEntity = await context.FindAsync<T>(entity.Id);
            if (existingEntity != null)
            {
                return new Result<Guid>(ResultCode.Conflict);
            }

            var insertResult = await context.AddAsync(entity);
            if (insertResult.State == Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                await context.SaveChangesAsync();
                return new Result<Guid>(ResultCode.Success, insertResult.Entity.Id);
            }

            return new Result<Guid>(ResultCode.Failed);
        }
    }
}
