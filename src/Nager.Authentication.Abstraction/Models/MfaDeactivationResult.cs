namespace Nager.Authentication.Abstraction.Models
{
    /// <summary>
    /// Mfa Deactivation Result
    /// </summary>
    public enum MfaDeactivationResult
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
        /// MFA was not active
        /// </summary>
        NotActive,

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
