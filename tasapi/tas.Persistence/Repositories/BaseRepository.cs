using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Repositories;
using tas.Domain.Common;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DataContext Context;
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public BaseRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository)
        {
            Context = context;
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
        }

        public void Create(T entity)
        {
            
            entity.Active = 1;
            entity.DateCreated = DateTime.Now;
            entity.UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id;

            Context.Add(entity);
        }

        public async Task CheckDuplicateData(T entity, params Expression<Func<T, Object>>[] properties)
        {

            var query = Context.Set<T>().AsQueryable();
            var fields = string.Empty;

            foreach (var property in properties)
            {
                var propertyName = ((MemberExpression)property.Body).Member.Name;

                if (fields == string.Empty)
                {
                    fields = propertyName;
                }
                else
                {
                    fields = $"{fields} ,{propertyName}";
                }
                var propertyValue = property.Compile()(entity);
                if (entity.Id > 0)
                {
                    query = Context.Set<T>().Where(x => x.Id != entity.Id && Equals(EF.Property<object>(x, propertyName), propertyValue));
                }
                else
                {
                    query = Context.Set<T>().Where(x => Equals(EF.Property<object>(x, propertyName), propertyValue));
                }

            }

            var duplicateEntity = await query.FirstOrDefaultAsync();

            if (duplicateEntity != null)
            {
                throw new BadRequestException("Sorry, duplicate data " + fields);
            }

            await Task.CompletedTask;

            
        }



        public void Update(T entity)
        {
            var existingEntity = Context.Set<T>().FirstOrDefault(x => x.Id == entity.Id);

            var existingEntityStatus = existingEntity?.Active;

            if (existingEntity == null)
            {
                throw new BadRequestException("Record not found");
            }
            else
            {
                    Context.Entry(existingEntity).CurrentValues.SetValues(entity);
                    existingEntity.DateUpdated = DateTime.Now;
                    existingEntity.Active = existingEntityStatus.Value;
                    existingEntity.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    Context.Entry(existingEntity).State = EntityState.Modified;
            }
        }

        public async Task Patch(T entity)
        {
            var existingEntity = await Context.Set<T>().FirstOrDefaultAsync(x => x.Id == entity.Id);
            if (existingEntity == null)
            {
                throw new BadRequestException("Record not found");
            }
            else
            {

                if (existingEntity.GetType().GetProperty("NationalityId")?.GetValue(existingEntity) != entity.GetType().GetProperty("NationalityId")?.GetValue(entity))
                {
                    if (entity.GetType().GetProperty("NRN")?.GetValue(entity) == null)
                    {
                        var propertyInfoNRN = entity.GetType().GetProperty("NRN");
                        if (propertyInfoNRN != null)
                        {
                            var currentValue = existingEntity.GetType().GetProperty("NRN")?.GetValue(existingEntity);
                            propertyInfoNRN.SetValue(entity, currentValue);
                        }

                    }
                    if (entity.GetType().GetProperty("PassportName")?.GetValue(entity) == null)
                    {
                        var propertyInfoPassportName = entity.GetType().GetProperty("PassportName");
                        if (propertyInfoPassportName != null)
                        {
                            var currentValue = existingEntity.GetType().GetProperty("PassportName")?.GetValue(existingEntity);
                            propertyInfoPassportName.SetValue(entity, currentValue);
                        }

                    }
                    if (entity.GetType().GetProperty("PassportExpiry")?.GetValue(entity) == null)
                    {
                        var propertyInfoPassportExpiry = entity.GetType().GetProperty("PassportExpiry");
                        if (propertyInfoPassportExpiry != null)
                        {
                            var currentValue = existingEntity.GetType().GetProperty("PassportExpiry")?.GetValue(existingEntity);
                            propertyInfoPassportExpiry.SetValue(entity, currentValue);
                        }

                    }
                    if (entity.GetType().GetProperty("PassportImage")?.GetValue(entity) == null)
                    {
                        var propertyInfoPassportImage = entity.GetType().GetProperty("PassportImage");
                        if (propertyInfoPassportImage != null)
                        {
                            var currentValue = existingEntity.GetType().GetProperty("PassportImage")?.GetValue(existingEntity);
                            propertyInfoPassportImage.SetValue(entity, currentValue);
                        }

                    }
                    if (entity.GetType().GetProperty("PassportNumber")?.GetValue(entity) == null)
                    {
                        var propertyInfoPassportNumber = entity.GetType().GetProperty("PassportNumber");
                        if (propertyInfoPassportNumber != null)
                        {
                            var currentValue = existingEntity.GetType().GetProperty("PassportNumber")?.GetValue(existingEntity);
                            propertyInfoPassportNumber.SetValue(entity, currentValue);
                        }

                    }
                    if (entity.GetType().GetProperty("Hometown")?.GetValue(entity) == null)
                    {
                        var propertyInfoHometown = entity.GetType().GetProperty("Hometown");
                        if (propertyInfoHometown != null)
                        {
                            var currentValue = existingEntity.GetType().GetProperty("Hometown")?.GetValue(existingEntity);
                            propertyInfoHometown.SetValue(entity, currentValue);
                        }

                    }

                }

                var propertyInfo = entity.GetType().GetProperty("PassportImage");
                if (propertyInfo != null && propertyInfo.GetValue(entity) == null)
                {
                    var currentValue = existingEntity.GetType().GetProperty("PassportImage")?.GetValue(existingEntity);
                    propertyInfo.SetValue(entity, currentValue);

                }

                var propertyInfoRoomId = entity.GetType().GetProperty("RoomId");
                if (propertyInfoRoomId != null && propertyInfoRoomId.GetValue(entity) == null)
                {
                    var currentValue = existingEntity.GetType().GetProperty("RoomId")?.GetValue(existingEntity);
                    propertyInfoRoomId.SetValue(entity, currentValue);

                }





                entity.DateCreated = existingEntity.DateCreated;
                entity.UserIdCreated = existingEntity.UserIdCreated;
                Context.Entry(existingEntity).CurrentValues.SetValues(entity);
                existingEntity.DateUpdated = DateTime.Now;
                existingEntity.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                Context.Entry(existingEntity).State = EntityState.Modified;
            }
        }


        public void Delete(T entity)
        {
            var existingEntity = Context.Set<T>().FirstOrDefault(x => x.Id == entity.Id);
            if (existingEntity == null)
            {
                throw new BadRequestException("Record not found");
            }

            // Toggle the 'Active' state.
            existingEntity.Active = existingEntity.Active == 0 ? 1 : 0;
            if (existingEntity.Active == 0)
            {
                existingEntity.DateDeleted = DateTime.Now;
                existingEntity.UserIdDeleted = _hTTPUserRepository.LogCurrentUser()?.Id;
            }
            else
            {
                existingEntity.DateUpdated = DateTime.Now;
                existingEntity.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
            }
        }

        public void DeleteFroce(T entity)
        {
            var deleteModel = Context.Set<T>().FirstOrDefault(x => x.Id == entity.Id);
            if (deleteModel == null)
            {
                throw new BadRequestException("Record not found");
            }
            else
            {
                Context.Remove(deleteModel);
                Context.Entry(entity).State = EntityState.Detached;
            }

        }




        public async Task<T> Get(int id, CancellationToken cancellationToken)
        {
            try
            {
                var returnData = await  Context.Set<T>()
                                    .AsNoTracking()
                                    .Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
                return returnData;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the record.", ex);
            }
        }

        public Task<List<T>> GetAll(CancellationToken cancellationToken)
        {
            return Context.Set<T>().OrderByDescending(x=> x.DateCreated).ToListAsync(cancellationToken);
        }

        public Task<List<T>> GetAllActiveFilter(int status, CancellationToken cancellationToken)
        {
            if (status == 1 || status == 0)
            {
                return Context.Set<T>().Where(x => x.Active == status).OrderByDescending(x => x.DateCreated).ToListAsync(cancellationToken);
            }
            else {
                return Context.Set<T>().OrderByDescending(x => x.DateCreated).ToListAsync(cancellationToken);
            }

        }

    }
}
