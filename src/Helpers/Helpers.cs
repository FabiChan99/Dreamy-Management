using System.Text;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;

namespace DreamyManagement.Helpers;

public static class Helpers
{
    public static async Task<bool> CheckForReason(CommandContext ctx, string reason)
    {
        if (reason == null)
        {
            var embedBuilder = new DiscordEmbedBuilder().WithTitle("Fehler: Kein Grund angegeben!")
                .WithDescription("Bitte gebe einen Grund an")
                .WithColor(DiscordColor.Red).WithFooter($"{ctx.User.UsernameWithDiscriminator}", ctx.User.AvatarUrl);
            var msg = new DiscordMessageBuilder().WithEmbed(embedBuilder.Build()).WithReply(ctx.Message.Id);
            await ctx.Channel.SendMessageAsync(msg);


            return true;
        }

        if (reason == "")
        {
            var embedBuilder = new DiscordEmbedBuilder().WithTitle("Fehler: Kein Grund angegeben!")
                .WithDescription("Bitte gebe einen Grund an")
                .WithColor(DiscordColor.Red).WithFooter($"{ctx.User.UsernameWithDiscriminator}", ctx.User.AvatarUrl);
            var msg = new DiscordMessageBuilder().WithEmbed(embedBuilder.Build()).WithReply(ctx.Message.Id);
            await ctx.Channel.SendMessageAsync(msg);
            return true;
        }

        return false;
    }
    
    public static int ParseDate(string dateInput)
    {
        var input = dateInput;
        var minutes = 0;
        int currentNumber;
        var numberBuilder = new StringBuilder();

        input = input.Replace("mo", "#");
        input = input.Replace("min", "m");

        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];
            if (char.IsDigit(ch))
            {
                numberBuilder.Append(ch);
            }
            else
            {
                if (numberBuilder.Length > 0)
                {
                    currentNumber = int.Parse(numberBuilder.ToString());
                    numberBuilder.Clear();

                    minutes += ch switch
                    {
                        'd' => currentNumber * 1440,
                        'h' => currentNumber * 60,
                        'm' => currentNumber,
                        'w' => currentNumber * 10080,
                        '#' => currentNumber * 43200,
                        _ => throw new NotSupportedException("Invalid input")
                    };
                }
            }
        }

        return minutes;
    }

    public static string FormatDate(int minutes)
    {
        var time = TimeSpan.FromMinutes(minutes);
        var days = time.Days;
        var hours = time.Hours;
        var minutesLeft = time.Minutes;

        var formattedTime = new StringBuilder();

        if (days > 0)
        {
            formattedTime.Append(days);
            formattedTime.Append("d ");
        }

        if (hours > 0)
        {
            formattedTime.Append(hours);
            formattedTime.Append("h ");
        }

        if (minutesLeft > 0)
        {
            formattedTime.Append(minutesLeft);
            formattedTime.Append("m");
        }

        return formattedTime.ToString();
    }


    public static IEnumerable<DiscordOverwriteBuilder> MergeOverwrites(DiscordChannel userChannel, List<DiscordOverwriteBuilder> overwrites,
        out IEnumerable<DiscordOverwriteBuilder> targetOverwrites)
    {
        targetOverwrites = userChannel.PermissionOverwrites.Select(x => x.ConvertToBuilder());
        foreach (var overwrite in overwrites)
        {
            targetOverwrites =
                targetOverwrites.Merge(overwrite.Type, overwrite.Target, overwrite.Allowed, overwrite.Denied);
        }

        var newOverwrites = targetOverwrites.ToList();
        return newOverwrites;
    }



    public static string GenerateCaseID()
    {
        var guid = Guid.NewGuid().ToString("N");
        var uniqueID = guid.Substring(0, 8);
        return uniqueID;
    }

    public static async Task<bool> TicketUrlCheck(CommandContext ctx, string reason)
    {
        var TicketUrl = "modtickets.animegamingcafe.de";
        if (reason == null) return false;
        if (reason.ToLower().Contains(TicketUrl.ToLower()))
        {
            Console.WriteLine("Ticket-URL enthalten");
            var embedBuilder = new DiscordEmbedBuilder().WithTitle("Fehler: Ticket-URL enthalten")
                .WithDescription("Bitte schreibe den Grund ohne Ticket-URL").WithColor(DiscordColor.Red);
            var embed = embedBuilder.Build();
            var msg_e = new DiscordMessageBuilder().WithEmbed(embed).WithReply(ctx.Message.Id);
            await ctx.Channel.SendMessageAsync(msg_e);

            return true;
        }

        return false;
    }
}