using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.AppDbContext
{
    public partial class ConfigRoomExaminationGetPagingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE proc [dbo].[ConfigRoomExamination_GetPagingData]
            @PageIndex int,
            @PageSize int,
            @RoomExaminationId int = null,
            @OrderBy nvarchar(20),
            @TotalPage int OUTPUT
            as begin
	            DECLARE @offset INT
                DECLARE @newsize INT
                DECLARE @sql NVARCHAR(MAX)

                IF(@PageIndex=1)
                  BEGIN
                    SET @offset = @PageIndex
                    SET @newsize = @PageSize
                   END
                ELSE 
                  BEGIN
                    SET @offset = (@PageIndex*@PageSize)
                    SET @newsize = @PageSize-1
                  END
	            SET NOCOUNT ON;  
                  SELECT 
	              ROW_NUMBER() OVER  
                  (  
                     ORDER BY cr.Id ASC  
                      ) AS RowNumber  
		              , cr.Id, cr.TotalPatient, cr.RoomExaminationId
		              , cr.Deleted, cr.Active, cr.Created, cr.CreatedBy, cr.Updated, cr.UpdatedBy
		              INTO #Results 
		              FROM ConfigRoomExaminations as cr 
		              LEFT JOIN RoomExaminations As re ON re.Id = cr.RoomExaminationId

		              GROUP BY cr.Id, cr.TotalPatient, cr.RoomExaminationId
		              , cr.Deleted, cr.Active, cr.Created, cr.CreatedBy, cr.Updated, cr.UpdatedBy
		              ;
		
                  SELECT @TotalPage = COUNT(*) 
	              FROM #Results as rs
	              WHERE 
	              @RoomExaminationId is null or RoomExaminationId = @RoomExaminationId
	              And rs.Deleted = 0
      
	  
	              Set @sql = N'SELECT * FROM #Results as rs WHERE rs.Deleted = 0';
	 
	              if (@RoomExaminationId is not null and @RoomExaminationId > 0)
		            begin
			            set @sql += ' and RoomExaminationId = @RoomExaminationId';
		            end
                  set @sql += ' and RowNumber BETWEEN ' + CONVERT(NVARCHAR(12), @offset) + ' AND ' + CONVERT(NVARCHAR(12), (@offset + @newsize));
	              if (@OrderBy is not null and len(@OrderBy) > 0)
		            begin
			            set @sql += ' ORDER BY ' + @OrderBy;
		            end
	              EXECUTE sp_executesql @sql
	              , N'@RoomExaminationId int'
	              , @RoomExaminationId = @RoomExaminationId;
      
                  DROP TABLE #Results 

            end";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
