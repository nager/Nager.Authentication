namespace Nager.Authentication.Abstraction.Models
{
    /// <summary>
    /// Mfa Activation Result
    /// </summary>
    public enum MfaActivationResult
    {
        /// <summary>
        /// MFA activation was successful
        /// </summary>
        Success,

        /// <summary>
        /// MFA activation failed
        /// </summary>
        Failed,

        /// <summary>
        /// MFA is already activated
        /// </summary>
        AlreadyActive,

        /// <summary>
        /// The provided authentication code is invalid
        /// </summary>
        InvalidCode,

        /// <summary>
        /// The user could not be found
        /// </summary>
        UserNotFound
    }
}
