﻿using DisCatSharp;
using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DreamyManagement.Services;

public class BotControl : BaseCommandModule
{
    [Command("eval")]
    [Description("Evaluates C# code.")]
    [Hidden]
    [RequireOwner]
    public async Task EvalCSAsync(CommandContext ctx, [RemainingText] string code)
    {
        var msg = ctx.Message;

        var cs1 = code.IndexOf("```") + 3;
        cs1 = code.IndexOf('\n', cs1) + 1;
        var cs2 = code.LastIndexOf("```");

        if (cs1 == -1 || cs2 == -1)
            throw new ArgumentException("You need to wrap the code into a code block.");

        var cs = code[cs1..cs2];

        msg = await ctx.RespondAsync(new DiscordEmbedBuilder()
            .WithColor(new DiscordColor("#FF007F"))
            .WithDescription("Evaluating...")
            .Build()).ConfigureAwait(false);

        try
        {
            var globals = new EvalVariables(ctx.Message, ctx.Client, ctx);

            var sopts = ScriptOptions.Default;
            sopts = sopts.WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text",
                "System.Threading.Tasks", "DisCatSharp", "DisCatSharp.Entities", "DisCatSharp.CommandsNext",
                "DisCatSharp.CommandsNext.Attributes", "DisCatSharp.Interactivity", "DisCatSharp.Enums",
                "Microsoft.Extensions.Logging");
            sopts = sopts.WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                .Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

            var script = CSharpScript.Create(cs, sopts, typeof(EvalVariables));
            script.Compile();
            var result = await script.RunAsync(globals).ConfigureAwait(false);

            if (result != null && result.ReturnValue != null &&
                !string.IsNullOrWhiteSpace(result.ReturnValue.ToString()))
                await msg.ModifyAsync(new DiscordEmbedBuilder
                {
                    Title = "Evaluation Result",
                    Description = result.ReturnValue.ToString(),
                    Color = new DiscordColor("#007FFF")
                }.Build()).ConfigureAwait(false);
            else
                await msg.ModifyAsync(new DiscordEmbedBuilder
                {
                    Title = "Evaluation Successful",
                    Description = "No result was returned.",
                    Color = new DiscordColor("#007FFF")
                }.Build()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await msg.ModifyAsync(new DiscordEmbedBuilder
            {
                Title = "Evaluation Failure",
                Description = string.Concat("**", ex.GetType().ToString(), "**: ", ex.Message),
                Color = new DiscordColor("#FF0000")
            }.Build()).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     The eval variables.
    /// </summary>
    private class EvalVariables
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EvalVariables" /> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="client">The client.</param>
        /// <param name="ctx">The command context.</param>
        public EvalVariables(DiscordMessage msg, DiscordClient client, CommandContext ctx)
        {
            Client = client;

            Message = msg;
            Channel = ctx.Channel;
            Guild = ctx.Guild;
            User = ctx.User;
            Member = ctx.Member;
            Context = ctx;
        }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        public DiscordMessage Message { get; set; }

        /// <summary>
        ///     Gets or sets the channel.
        /// </summary>
        public DiscordChannel Channel { get; set; }

        /// <summary>
        ///     Gets or sets the guild.
        /// </summary>
        public DiscordGuild Guild { get; set; }

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        public DiscordUser User { get; set; }

        /// <summary>
        ///     Gets or sets the member.
        /// </summary>
        public DiscordMember Member { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public CommandContext Context { get; set; }

        public DiscordClient Client { get; set; }
    }
}