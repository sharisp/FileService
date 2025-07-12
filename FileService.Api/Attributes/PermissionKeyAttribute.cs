namespace FileService.Api.Attributes
{
    public class PermissionKeyAttribute(string key) : Attribute
    {
        public string Key { get; } = key;
    }
}
