using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Extensions;
using tas.Application.Features.SysFilesFeature.MultiUploadSysFile;
using tas.Application.Features.SysFilesFeature.UploadSysFile;
using tas.Application.Features.SysFilesFeature.UploadSysFiles;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class SysFileRepository : BaseRepository<SysFile>, ISysFileRepository
    {
        private  readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public SysFileRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task<UploadSysFileResponse> UploadSysFile(UploadSysFileRequest request, CancellationToken cancellationToken)
        { 
            var returnData = new UploadSysFileResponse();
            IFormFile file = request.file;
            var fileInfo = file.SaveFile();
            if (fileInfo.Status)
            {
                var newData = new SysFile
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    FileAddress = fileInfo.FileName,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                };

                Context.SysFile.Add(newData);
                await Context.SaveChangesAsync();

                returnData.FileAddress = fileInfo.FileName;
                returnData.Id = newData.Id;
            }
            else {
                throw new BadRequestException(fileInfo.Error);
            }


            return returnData;
        }








        public async Task<List<MultiUploadSysFileResponse>> MultiUploadSysFile(MultiUploadSysFileRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<MultiUploadSysFileResponse>(); ;
            foreach (var item in request.files)
            {
                IFormFile file = item;
                var fileInfo = file.SaveFile();
                if (fileInfo.Status)
                {
                    var newData = new SysFile
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        FileAddress = fileInfo.FileName,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    };

                    Context.SysFile.Add(newData);
                    await Context.SaveChangesAsync();

                    returnData.Add(new MultiUploadSysFileResponse { Id = newData.Id, FileAddress = newData.FileAddress });
                }
                else
                {
                    throw new BadRequestException(fileInfo.Error);
                }
            }

            return returnData;

        }


    }

}
