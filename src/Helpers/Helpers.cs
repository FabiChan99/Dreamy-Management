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