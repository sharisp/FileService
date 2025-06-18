using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class urlemputy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackupUrl",
                table: "T_File_Upload");

            migrationBuilder.DropColumn(
                name: "PubUri",
                table: "T_File_Upload");

            migrationBuilder.AddColumn<string>(
                name: "AwsUri",
                table: "T_File_Upload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AzureUrl",
                table: "T_File_Upload",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwsUri",
                table: "T_File_Upload");

            migrationBuilder.DropColumn(
                name: "AzureUrl",
                table: "T_File_Upload");

            migrationBuilder.AddColumn<string>(
                name: "BackupUrl",
                table: "T_File_Upload",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PubUri",
                table: "T_File_Upload",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
