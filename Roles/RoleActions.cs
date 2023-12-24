using AmongUs.GameOptions;

namespace TOHE.Roles
{
    public abstract class RoleActions
    {
        public abstract void Init();
        public abstract void Add(byte playerId);
        public virtual void SetKillCooldown(byte playerId) => Main.AllPlayerKillCooldown[playerId] = Options.DefaultKillCooldown;
        public virtual void ApplyGameOptions(IGameOptions opt) { }
        public virtual bool CanUseKillButton(PlayerControl pc) => pc.HasKillButton();
        public virtual bool OnCheckMurder(PlayerControl killer, PlayerControl target) => killer != null && target != null;
        public virtual void OnMurder(PlayerControl killer, PlayerControl target) { }
        public virtual bool OnAnyoneCheckMurder(PlayerControl killer, PlayerControl target) => killer != null && target != null;
        public virtual void OnAnyoneMurder(PlayerControl killer, PlayerControl target) { }
        public virtual void OnReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target, PlayerControl killer) { }
        public virtual void AfterMeetingTasks() { }
        public virtual void OnFixedUpdate(PlayerControl pc) { }
        public virtual void OnCoEnterVent(PlayerPhysics physics, Vent vent) { }
        public virtual void OnEnterVent(PlayerControl pc, Vent vent) { }
        public virtual void OnCoExitVent(PlayerPhysics physics, Vent vent) { }
        public virtual void OnExitVent(PlayerControl pc, Vent vent) { }
        public virtual void OnPet(PlayerControl pc) { }
        public virtual bool KnowRole(PlayerControl seer, PlayerControl target) => false;
        public virtual string GetProgressText(byte playerId, bool comms) => Main.PlayerStates[playerId].GetTaskState().hasTasks ? Utils.GetTaskCount(playerId, comms, Options.CurrentGameMode == CustomGameMode.MoveAndStop) : string.Empty;
        public virtual string GetSuffixText(byte playerId) => string.Empty;
        public virtual string GetHUDText(PlayerControl pc) => GetSuffixText(pc.PlayerId);
    }
}
