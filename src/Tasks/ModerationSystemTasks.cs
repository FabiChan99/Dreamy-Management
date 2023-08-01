using DisCatSharp;
using DreamyManagement.Services.DatabaseHandler;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DreamyManagement.Tasks;

public class ModerationSystemTasks
{
    public async Task StartRemovingWarnsPeriodically(DiscordClient discord)
    {
        if (GlobalProperties.DebugMode)
        {
            discord.Logger.LogWarning(
                "Debugmodus aktiviert. Deaktiviere automatische �berpr�fung auf abgelaufene warns.");
            return;
        }

        if (DatabaseService.IsConnected())
        {
            discord.Logger.LogInformation("Starte �berpr�fung auf abgelaufene Warns..");
            while (true)
            {
                await RemoveWarnsOlderThan7Days(discord);
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }

        discord.Logger.LogWarning(
            "Datenbank nicht verbunden. Deaktiviere automatische �berpr�fung auf abgelaufene warns.");
    }

    private int GetWarnExpiringTime()
    {
        int fallback = 7;
        int days_;

        try
        {
            var days = BotConfig.GetConfig()["ModerationConfig"]["WarnExpireDays"];
            short.TryParse(days, out short parsedDays);
            days_ = parsedDays;
        }
        catch (Exception)
        {
            return fallback;
        }

        return days_;
    }

    private async Task RemoveWarnsOlderThan7Days(DiscordClient discord)
    {
        discord.Logger.LogInformation("Pr�fe auf abgelaufene Warns");
        var warnlist = new List<dynamic>();
        int days = GetWarnExpiringTime(); // Default value is 7 Days if no override in condig present
        int expireTime = (int)DateTimeOffset.UtcNow.AddDays(-days).ToUnixTimeSeconds();


        string selectQuery = $"SELECT * FROM warns WHERE datum < '{expireTime}' AND perma = 'False'";

        await using (NpgsqlDataReader warnReader = DatabaseService.ExecuteQuery(selectQuery))
        {
            while (warnReader.Read())
            {
                var warn = new
                {
                    UserId = warnReader.GetInt64(0),
                    PunisherId = warnReader.GetInt64(1),
                    Datum = warnReader.GetInt32(2),
                    Description = warnReader.GetString(3),
                    Perma = warnReader.GetBoolean(4),
                    CaseId = warnReader.GetString(5)
                };
                warnlist.Add(warn);
            }
        }

        foreach (var warn in warnlist)
        {
            Dictionary<string, object> data = new()
            {
                { "userid", (long)warn.UserId },
                { "punisherid", (long)warn.PunisherId },
                { "datum", warn.Datum },
                { "description", "[AUTO] Warn Abgelaufen: " + warn.Description },
                { "caseid", "EXPIRED-" + warn.CaseId }
            };
            await DatabaseService.InsertDataIntoTable("flags", data);
        }

        Dictionary<string, (object value, string comparisonOperator)> whereConditions = new()
        {
            { "datum", (expireTime, "<") },
            { "perma", (false, "=") }
        };

        int rowsDeleted = await DatabaseService.DeleteDataFromTable("warns", whereConditions);

        discord.Logger.LogInformation($"{rowsDeleted} Abgelaufene Verwarnungen in Flags verschoben.");
    }
}