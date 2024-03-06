﻿namespace TOHE.Roles.Impostor
{
    internal class Godfather : RoleBase
    {
        public static byte GodfatherTarget = byte.MaxValue;
        public static bool On;
        public override bool IsEnable => On;

        public override void Add(byte playerId)
        {
            On = true;
        }

        public override void Init()
        {
            On = false;
        }
    }
}