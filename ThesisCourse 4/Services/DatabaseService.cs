using Npgsql;

public static class DatabaseService
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=root";

    public static async Task<bool> ValidateUserAsync(string login, string password)
    {
        const string sql = "SELECT password FROM Users WHERE login = @login";

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("login", login);

        var result = await cmd.ExecuteScalarAsync();

        if (result == null || result.ToString() != password) return false; 
        return true;
    }
}