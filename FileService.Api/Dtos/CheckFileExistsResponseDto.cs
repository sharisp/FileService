namespace FileService.Api.Dtos
{
    /// <summary>
    /// response dto for checking if a file exists
    /// </summary>
    public record CheckFileExistsResponseDto
    {
        public bool IsExist { get; set; }
        public Uri? PublicUri { get; set; }

        public CheckFileExistsResponseDto(bool isExist,Uri? publicUri)
        {
            this.PublicUri = publicUri;
            this.IsExist = isExist;
        }
    }
}
