
using System.ComponentModel.DataAnnotations.Schema;

namespace FileService.Domain
{
    public class BaseEntity
    {
        public long Id { get; init; }

        public BaseEntity()
        {

            Id = IdGeneratorFactory.NewId();
            CreateDateTime=DateTimeOffset.Now;
        }


        public DateTimeOffset? CreateDateTime { get; set; }
        public long? CreateUserId { get; set; }
    }
}
