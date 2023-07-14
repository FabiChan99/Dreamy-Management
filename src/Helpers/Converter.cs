﻿namespace DreamyManagement.Helpers;

public class Converter
{
    public static void SeperateIdsAndReason(string ids_and_reason, out List<ulong> ids, out string reason)
    {
        ids = new List<ulong>();
        reason = "";
        var parts = ids_and_reason.Split(' ');
        var isReasonStarted = false;

        foreach (var part in parts)
            if (!isReasonStarted)
            {
                if (part.StartsWith("<@") && part.EndsWith(">"))
                {
                    var idString = part.Substring(2, part.Length - 3);
                    if (ulong.TryParse(idString, out var id))
                        ids.Add(id);
                    else
                        break;
                }
                else if (ulong.TryParse(part, out var id))
                {
                    ids.Add(id);
                }
                else
                {
                    isReasonStarted = true;
                    reason += part + " ";
                }
            }
            else
            {
                reason += part + " ";
            }
    }


    public static List<ulong> ExtractUserIDsFromString(string users)
    {
        List<ulong> ids = new();
        var parts = users.Split(' ');
        foreach (var part in parts)
        {
            if (part.StartsWith("<@") && part.EndsWith(">"))
            {
                var idString = part.Substring(2, part.Length - 3);
                if (ulong.TryParse(idString, out var id))
                    ids.Add(id);
                else
                    break;
            }
            else if (ulong.TryParse(part, out var id))
            {
                ids.Add(id);
            }
        }

        ids = ids.ToHashSet().ToList();
        return ids;
    }

    public static DateTime ConvertUnixTimestamp(long unixTimestamp)
    {
        DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        DateTime timestamp = unixEpoch.AddSeconds(unixTimestamp).ToLocalTime();
        return timestamp;
    }

    public static string GetDateFromTimestamp(long timestamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        var formattedDate = dateTimeOffset.ToString("dd.MM.yyyy - HH:mm:ss");
        return formattedDate;
    }
}