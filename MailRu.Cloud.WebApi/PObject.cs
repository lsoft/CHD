namespace MailRu.Cloud.WebApi
{
    /// <summary>
    ///     Object to response parsing.
    /// </summary>
    internal enum PObject
    {
        /// <summary>
        ///     Authorization token.
        /// </summary>
        Token = 0,

        /// <summary>
        ///     List of items.
        /// </summary>
        Entry = 1,

        /// <summary>
        ///     Servers info.
        /// </summary>
        Shard = 2,

        /// <summary>
        ///     Full body string.
        /// </summary>
        BodyAsString = 3
    }
}