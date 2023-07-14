﻿using DisCatSharp.Entities;
using DisCatSharp.Enums;

namespace DreamyManagement;

internal static class DiscordExtension
{
    internal static List<DiscordOverwriteBuilder> ConvertToBuilderWithNewOverwrites(
        this IReadOnlyList<DiscordOverwrite> overwrites, DiscordMember member, Permissions allowed, Permissions denied)
    {
        return overwrites.Where(x => x.Id != member.Id)
            .Select(x => x.Type == OverwriteType.Role
                ? new DiscordOverwriteBuilder(x.GetRoleAsync().Result) { Allowed = x.Allowed, Denied = x.Denied }
                : new DiscordOverwriteBuilder(x.GetMemberAsync().Result) { Allowed = x.Allowed, Denied = x.Denied })
            .Append(new DiscordOverwriteBuilder(member)
            {
                Allowed = (overwrites.FirstOrDefault(x => x.Id == member.Id, null)?.Allowed ?? Permissions.None) |
                          allowed,
                Denied = (overwrites.FirstOrDefault(x => x.Id == member.Id, null)?.Denied ?? Permissions.None) | denied
            }).ToList();
    }

    internal static List<DiscordOverwriteBuilder> ConvertToBuilderWithNewOverwrites(
        this IReadOnlyList<DiscordOverwrite> overwrites, DiscordRole role, Permissions allowed, Permissions denied)
    {
        return overwrites.Where(x => x.Id != role.Id)
            .Select(x => x.Type == OverwriteType.Role
                ? new DiscordOverwriteBuilder(x.GetRoleAsync().Result) { Allowed = x.Allowed, Denied = x.Denied }
                : new DiscordOverwriteBuilder(x.GetMemberAsync().Result) { Allowed = x.Allowed, Denied = x.Denied })
            .Append(new DiscordOverwriteBuilder(role)
            {
                Allowed =
                    (overwrites.FirstOrDefault(x => x.Id == role.Id, null)?.Allowed ?? Permissions.None) | allowed,
                Denied = (overwrites.FirstOrDefault(x => x.Id == role.Id, null)?.Denied ?? Permissions.None) | denied
            }).ToList();
    }

    internal static List<DiscordOverwriteBuilder> ConvertToBuilder(this IReadOnlyList<DiscordOverwrite> overwrites)
    {
        return overwrites.Select(x =>
                x.Type == OverwriteType.Role
                    ? new DiscordOverwriteBuilder(x.GetRoleAsync().Result) { Allowed = x.Allowed, Denied = x.Denied }
                    : new DiscordOverwriteBuilder(x.GetMemberAsync().Result) { Allowed = x.Allowed, Denied = x.Denied })
            .ToList();
    }
}