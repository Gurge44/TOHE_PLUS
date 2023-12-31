﻿using Hazel;
using System.Collections.Generic;
using UnityEngine;

namespace TOHE;

public static class NameNotifyManager
{
    public static Dictionary<byte, (string TEXT, long TIMESTAMP)> Notice = [];
    public static void Reset() => Notice = [];
    public static bool Notifying(this PlayerControl pc) => Notice.ContainsKey(pc.PlayerId);
    public static void Notify(this PlayerControl pc, string text, float time = 5f)
    {
        if (!AmongUsClient.Instance.AmHost || pc == null) return;
        if (!GameStates.IsInTask) return;
        //if (!text.Contains("<color=#")) text = Utils.ColorString(Utils.GetRoleColor(pc.GetCustomRole()), text);
        if (!text.Contains("<color=") && !text.Contains("</color>")) text = Utils.ColorString(Color.white, text);
        if (!text.Contains("<size=")) text = "<size=1.8>" + text + "</size>";
        Notice.Remove(pc.PlayerId);
        Notice.Add(pc.PlayerId, new(text, Utils.GetTimeStamp() + (long)time));
        SendRPC(pc.PlayerId);
        Utils.NotifyRoles(SpecifySeer: pc, SpecifyTarget: pc);
        Logger.Info($"New name notify for {pc.GetNameWithRole().RemoveHtmlTags()}: {text} ({time}s)", "Name Notify");
    }
    public static void OnFixedUpdate(PlayerControl player)
    {
        if (!GameStates.IsInTask)
        {
            if (Notice.Count > 0) Notice = [];
            return;
        }
        if (Notice.ContainsKey(player.PlayerId) && Notice[player.PlayerId].TIMESTAMP < Utils.GetTimeStamp())
        {
            Notice.Remove(player.PlayerId);
            Utils.NotifyRoles(SpecifySeer: player, SpecifyTarget: player);
        }
    }
    public static bool GetNameNotify(PlayerControl player, out string name)
    {
        name = string.Empty;
        if (!Notice.ContainsKey(player.PlayerId)) return false;
        name = Notice[player.PlayerId].TEXT;
        return true;
    }
    private static void SendRPC(byte playerId)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncNameNotify, SendOption.Reliable, -1);
        writer.Write(playerId);
        if (Notice.ContainsKey(playerId))
        {
            writer.Write(true);
            writer.Write(Notice[playerId].TEXT);
            writer.Write(Notice[playerId].TIMESTAMP - Utils.GetTimeStamp());
        }
        else writer.Write(false);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
    public static void ReceiveRPC(MessageReader reader)
    {
        byte PlayerId = reader.ReadByte();
        Notice.Remove(PlayerId);
        long now = Utils.GetTimeStamp();
        if (reader.ReadBoolean())
            Notice.Add(PlayerId, new(reader.ReadString(), now + (long)reader.ReadSingle()));
        Logger.Info($"New name notify for {Main.AllPlayerNames[PlayerId]}: {Notice[PlayerId].TEXT} ({Notice[PlayerId].TIMESTAMP - now}s)", "Name Notify");
    }
}