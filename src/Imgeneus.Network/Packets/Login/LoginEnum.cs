namespace Imgeneus.Network.Packets.Login
{
    public enum SelectServer : sbyte
    {
        ServerSaturated = -4,
        VersionDoesntMatch = -3,
        CannotConnect = -2,
        TryAgainLater = -1,
        Success = 0
    }

    public enum AuthenticationResult : byte
    {
        SUCCESS,
        ACCOUNT_DONT_EXIST,
        CANT_CONNECT,
        INVALID_PASSWORD,
        CANT_CONNECT_WITH_THIS_ACCOUNT_GAME,
        CANT_CONNECT_WITH_THIS_ACCOUNT_GAME_PAGE,
        ACCOUNT_IN_DELETE_PROCESS_1,
        ACCOUNT_IN_DELETE_PROCESS_2,
        ACCOUNT_IN_DELETE_PROCESS_3,
        ACCOUNT_CANT_ACCESS_BY_USER,
        BANNED_ACCOUNT,
        CUENTA_RESTRINGIDA_1,
        CUENTA_RESTRINGIDA_2,
        GTMSG_10105,
        GTMSG_10104,
        DONT_PAY_ACCOUNT
    }
}
