using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fileupload_addcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateUserId",
                table: "T_File_Upload",
                newName: "UpdaterUserId");

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "T_File_Upload",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeleteDateTime",
                table: "T_File_Upload",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "T_File_Upload",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "T_File_Upload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDel",
                table: "T_File_Upload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdateDateTime",
                table: "T_File_Upload",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "T_File_Upload");

            migrationBuilder.DropColumn(
                name: "DeleteDateTime",
                table: "T_File_Upload");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "T_File_Upload");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "T_File_Upload");

            migrationBuilder.DropColumn(
                name: "IsDel",
                table: "T_File_Upload");

            migrationBuilder.DropColumn(
                name: "UpdateDateTime",
                table: "T_File_Upload");

            migrationBuilder.RenameColumn(
                name: "UpdaterUserId",
                table: "T_File_Upload",
                newName: "CreateUserId");
        }
    }
}
