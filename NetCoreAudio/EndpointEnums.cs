using CoreAudio.Enumerations;

namespace NetCoreAudio
{
    public enum EndpointState : uint
    {
        Active = CoreAudio.Constants.DEVICE_STATE_XXX.DEVICE_STATE_ACTIVE,
        Disabled = CoreAudio.Constants.DEVICE_STATE_XXX.DEVICE_STATE_DISABLED,
        NotPresent = CoreAudio.Constants.DEVICE_STATE_XXX.DEVICE_STATE_NOTPRESENT,
        Unplugged = CoreAudio.Constants.DEVICE_STATE_XXX.DEVICE_STATE_UNPLUGGED,
        Any = CoreAudio.Constants.DEVICE_STATE_XXX.DEVICE_STATEMASK_ALL
    }

    public enum EndpointRole : uint
    {
        Console = ERole.eConsole,
        Multimedia = ERole.eMultimedia,
        Communication = ERole.eCommunications
    }

    public enum EndpointDataFlow
    {
        Render = EDataFlow.eRender,
        Capture = EDataFlow.eCapture,
        All = EDataFlow.eAll
    }
}
