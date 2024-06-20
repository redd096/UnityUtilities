namespace redd096.Game3D
{
    /// <summary>
    /// InteractAgain - release input and press again to call OnDismiss
    /// ReleaseInteractInput - when release input, call immediatly OnDismiss
    /// Manually - interactor doesn't call OnDismiss by input. Or this object doesn't need OnDismiss or someone must call the function manually
    /// </summary>
    public enum EDismissType
    {
        InteractAgain, ReleaseInteractInput, Manually
    }
}