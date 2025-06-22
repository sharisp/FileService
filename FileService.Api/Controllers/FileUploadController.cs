using FileService.Domain.Constants;
using FileService.Domain.Interface;
using FileService.Domain.Service;
using FileService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FileService.Api.Dtos;
using System.IO;
using FluentValidation;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController(FileUploadService domainService, IFileUploadRepository fileUploadRepository, IUnitOfWork unitOfWork, IConfiguration configuration, ApiClientHelper apiClientHelper, IValidator<CheckFileExistsRequestDto> validator) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [RequestSizeLimit(60_000_000)]
        public async Task<ActionResult<ApiResponse<Uri>>> Upload(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file.Length == 0 || file.Length > 50 * 1024 * 1024)
            {
                return Ok(ApiResponse<Uri>.Fail("file size must be bigger than 0 and less than 50MB"));
            }
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var res = await CheckPermission(Convert.ToInt64(userId), cancellationToken);
            if (res == false)
            {
                return Ok(ApiResponse<List<Uri>>.Fail("no permission"));
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
                var urls = new List<Uri>();
                if (upItem.AwsUri != null)
                    urls.Add(upItem.AwsUri);
                if (upItem.AzureUrl != null)
                    urls.Add(upItem.AzureUrl);

                return Ok(ApiResponse<List<Uri>>.Ok(urls));
            }
            return Ok(ApiResponse<List<Uri>>.Fail("upload error"));
        }

        [HttpPost("FileExists")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CheckFileExistsResponseDto>>> CheckFileExists(CheckFileExistsRequestDto dto, CancellationToken cancellationToken = default)
        {
            await ValidationHelper.ValidateModelAsync(dto, validator);
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var res = await CheckPermission(Convert.ToInt64(userId), cancellationToken);
            if (res == false)
            {
                return Ok(ApiResponse<CheckFileExistsResponseDto>.Fail("no permission"));
            }

            var (exists, existFileUpload) = await fileUploadRepository.CheckFileExistsAsync(dto.FileHash, dto.FileSizeBytes);

            return Ok(ApiResponse<CheckFileExistsResponseDto>.Ok(new CheckFileExistsResponseDto(exists, existFileUpload?.AwsUri)));



        }

        private async Task<bool> CheckPermission(long userId, CancellationToken cancellation = default)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : authHeader;
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            var url = configuration["OuterApiUrl:CheckPermissionApiUrl"];
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            var dto = new
            {
                PermissionKey = "FileUpload.Upload",
                UserId = userId,
                SystemName = ConstantValues.SystemName
            };
            apiClientHelper.SetBearerToken(token);
            var res = await apiClientHelper.PostAsync<ApiResponse<string>>(url, dto);
            if (res?.Success == true)
            {
                return true;
            }
            return false;

        }
    }
}
