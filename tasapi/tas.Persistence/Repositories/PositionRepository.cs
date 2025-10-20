using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Application.Features.PositionFeature.AllPosition;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public partial class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly BulkImportExcelService _bulkImportExcelService;
        public PositionRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, BulkImportExcelService bulkImportExcelService) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;   
            _bulkImportExcelService = bulkImportExcelService;
        }

        public Task<Position> GetbyId(int id, CancellationToken cancellationToken)
        {
            return Context.Position.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


        public async Task<GetAllPositionResponse> GetAllData(GetAllPositionRequest request, CancellationToken cancellationToken)
        {
            int pageIndex = request.pageIndex > 0 ? request.pageIndex - 1 : 0;
            int pageSize = request.pageSize;



            IQueryable<Position> query = Context.Position.AsNoTracking().OrderByDescending(x=> x.DateUpdated);

            if (request.Active.HasValue)
            {
                query = query.Where(e => e.Active == request.Active.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                query = query.Where(e => e.Code.ToLower().Contains(request.Code.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                query = query.Where(e => e.Description.ToLower().Contains(request.Description.ToLower()));
            }

            var totalcount = await query.CountAsync(cancellationToken);

            //var response = await query
            //    .Skip(pageIndex * pageSize)
            //    .Take(pageSize)
            //    .Select(async e => new GetAllPositionResult
            //    {
            //        Id = e.Id,
            //        Code = e.Code,
            //        Description = e.Description,
            //        Active = e.Active,
            //        EmployeeCount =Context.Employee.Count(x => x.PositionId == e.Id && x.Active == 1),
            //        DateCreated = e.DateCreated,
            //        DateUpdated = e.DateUpdated
            //    })
            //    .ToListAsync(cancellationToken);

            var response = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(e => new GetAllPositionResult
                {
                    Id = e.Id,
                    Code = e.Code,
                    Description = e.Description,
                    Active = e.Active,
                    // Use a subquery to count active employees for each position
                    EmployeeCount = Context.Employee.AsNoTracking().Count(x => x.PositionId == e.Id && x.Active == 1),
                    DateCreated = e.DateCreated,
                    DateUpdated = e.DateUpdated
                })
                .ToListAsync(cancellationToken);



            var returnData = new GetAllPositionResponse
            {
                data = response,
                pageSize = request.pageSize,
                currentPage = request.pageIndex,
                totalcount = totalcount
            };

            return returnData;
        }


        public async Task<List<AllPositionResponse>> AllData(AllPositionRequest request, CancellationToken cancellationToken)
        {

            var positionQuery = Context.Position.AsNoTracking().AsQueryable();
            if (request.status.HasValue)
            {
                positionQuery = positionQuery.Where(x => x.Active == request.status);  
            }


            var query = from p in positionQuery
                        join ee in (from e in Context.Employee.AsNoTracking()
                                    group e by e.PositionId into grouped
                                    select new
                                    {
                                        PositionId = grouped.Key,
                                        Count = grouped.Count()
                                    }
                                      )

                                      on p.Id equals ee.PositionId into ppGrouped
                        from ee in ppGrouped.DefaultIfEmpty()
                        select new AllPositionResponse
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Active = p.Active,
                            DateCreated = p.DateCreated,
                            DateUpdated = p.DateUpdated,
                            Description = p.Description,
                            EmployeeCount = ee.Count
                        };

            var result = await query.OrderByDescending(x=> x.DateUpdated).ToListAsync(cancellationToken);

            return result;
        }


    }
}
