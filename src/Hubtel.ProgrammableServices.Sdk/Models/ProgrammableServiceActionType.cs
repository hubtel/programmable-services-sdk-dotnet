namespace Hubtel.ProgrammableServices.Sdk.Models
{
    public enum ProgrammableServiceActionType
    {
        /// <summary>
        /// For new request
        /// </summary>
        Initiation,
        /// <summary>
        /// For subsequent requests with same SessionId
        /// </summary>
        Response,
        /// <summary>
        /// Terminates a session
        /// </summary>
        Release,
        /// <summary>
        /// Terminates a session and acts as a signal to start checkout process
        /// </summary>
        AddToCart,
        /// <summary>
        /// Received from Hubtel. Can be safely ignored, since a response does not affect user's interaction.
        /// Especially for USSD channel, this is a signal that your service took too long to respond.
        /// A subsequent response from your application for this request is ignored by Hubtel
        /// </summary>
        Timeout,
        /// <summary>
        /// Signals this service about an API request to query for some data from your service
        /// </summary>
        Query,
        /// <summary>
        /// Special case of Initiation. Signals this service that a user intends to skip
        /// some interaction steps since data is already
        /// passed along in the request from Hubtel
        /// </summary>
        Favorite,
        /// <summary>
        /// Invalid type or not yet implemented
        /// </summary>
        Unknown
    }
}