using Npgsql;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connStr = "Host=aws-1-ap-northeast-1.pooler.supabase.com;Database=postgres;Username=postgres.phdueonklmkgietkliqy;Password=malakabookbyayamasincihapit123!;SSL Mode=Require;Trust Server Certificate=true";
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        string[] enums = { "user_role", "booth_category", "talkshow_status", "ticket_type", "ticket_status" };
        foreach (var e in enums)
        {
            try
            {
                await using var cmd = new NpgsqlCommand($"CREATE CAST (character varying AS {e}) WITH INOUT AS IMPLICIT;", conn);
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Created cast for {e}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cast for {e} might already exist or failed: {ex.Message}");
            }
        }
        Console.WriteLine("Done!");
    }
}
