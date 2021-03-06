﻿using System.Threading.Tasks;
using AutoRenter.Domain.Data;
using AutoRenter.Domain.Models;
using AutoRenter.Domain.Interfaces;

namespace AutoRenter.Domain.Services.Commands
{
    internal class Delete<T> : IDeleteCommand<T>
        where T : class, IEntity
    {
        private readonly AutoRenterContext context;
        public Delete(AutoRenterContext context)
        {
            this.context = context;
        }

        public async Task<ResultCode> Execute(T entity)
        {
            var existingEntity = await context.FindAsync<T>(entity.Id);
            if (existingEntity == null)
            {
                return ResultCode.NotFound;
            }

            var deleteResult = context.Remove(existingEntity);
            if (deleteResult.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
            {
                await context.SaveChangesAsync();
                return ResultCode.Success;
            }
            else
            {
                return ResultCode.Failed;
            }
        }
    }
}
