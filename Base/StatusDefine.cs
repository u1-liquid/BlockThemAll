namespace BlockThemAll.Base
{
    public enum WorkStatus
    {
        READY,
        PREVIEW,
        STARTING,
        STARTED,
        PAUSED,
        STOPPING,
        STOPPED,
        DONE,
        ERROR
    }

    public enum UserStatus
    {
        NO_APIKEY,
        NO_CREDITIONAL,
        LOGIN_REQUESTED,
        LOGIN_SUCCESS,
        INVALID_CREDITIONAL
    }
}
