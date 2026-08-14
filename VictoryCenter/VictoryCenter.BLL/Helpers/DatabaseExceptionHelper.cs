using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace VictoryCenter.BLL.Helpers;

public static class DatabaseExceptionHelper
{
    public static bool IsUniqueConstraintException(this DbUpdateException exception)
    {
        const int uniqueIndexViolationErrorNumber = 2601;
        const int uniqueKeyConstraintViolationErrorNumber = 2627;

        return exception.InnerException is SqlException
        {
            Number: uniqueIndexViolationErrorNumber or uniqueKeyConstraintViolationErrorNumber
        };
    }

    public static bool IsForeignKeyConstraintException(this DbUpdateException exception)
    {
        const int foreignKeyConstraintViolationErrorNumber = 547;

        return exception.InnerException is SqlException
        {
            Number: foreignKeyConstraintViolationErrorNumber
        };
    }
}
