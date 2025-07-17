using Domain.SharedKernel.Interfaces;
using FileService.Api.Attributes;
using FileService.Api.Dtos;
using FileService.Domain.Constants;
using FileService.Domain.Interface;
using FileService.Domain.Service;
using FileService.Infrastructure;
using FileService.Infrastructure.Repository;
using FileService.Infrastructure.StoreClients;
using FluentValidation;
using Infrastructure.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FileService.Api.Controllers
{
    [Authorize]
    //   [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController(FileUploadService domainService, IFileUploadRepository fileUploadRepository, IUnitOfWork unitOfWork, IValidator<CheckFileExistsRequestDto> validator) : ControllerBase
    {
        [HttpPost]
        [PermissionKey("FileUpload.Upload")]
        [RequestSizeLimit(60_000_000)]
        public async Task<ActionResult<ApiResponse<Uri>>> Upload(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file.Length == 0 || file.Length > 50 * 1024 * 1024)
            {
                return Ok(ApiResponse<Uri>.Fail("file size must be bigger than 0 and less than 50MB"));
            }


            string fileName = file.FileName;
            using Stream stream = file.OpenReadStream();
            var (isExist, upItem) = await domainService.UploadFileAsync(stream, fileName, 0, cancellationToken);

            var addFlag = false;
            if (!isExist)
            {
                await fileUploadRepository.AddFileAsync(upItem);
                addFlag = (await unitOfWork.SaveChangesAsync(cancellationToken)) > 0;
            }

            if (addFlag || isExist)
            {

                return Ok(ApiResponse<Uri>.Ok(upItem.GetPublicUri()));
            }
            return Ok(ApiResponse<Uri>.Fail("upload error"));
        }

        [HttpPost("FileExists")]
        [PermissionKey("FileUpload.FileExists")]
        public async Task<ActionResult<ApiResponse<CheckFileExistsResponseDto>>> CheckFileExists(CheckFileExistsRequestDto dto, CancellationToken cancellationToken = default)
        {
            await ValidationHelper.ValidateModelAsync(dto, validator);

            var (exists, existFileUpload) = await fileUploadRepository.CheckFileExistsAsync(dto.FileHash, dto.FileSizeBytes);

            return Ok(ApiResponse<CheckFileExistsResponseDto>.Excute(exists, "", 200, new CheckFileExistsResponseDto(exists, existFileUpload?.GetPublicUri())));

        }



    }
}
