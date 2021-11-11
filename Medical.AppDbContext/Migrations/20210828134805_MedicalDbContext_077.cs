using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_077 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"CREATE OR ALTER PROC usp_GetAllTodoItemsByStatus(@isCompleted BIT) AS SELECT * FROM Users WHERE Deleted = 0";
            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROC usp_GetAllTodoItemsByStatus";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
