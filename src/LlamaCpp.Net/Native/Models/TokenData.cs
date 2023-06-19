using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native.Models
{
    /// <summary>
    ///    LLama token data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct TokenData
    {
        /// <summary>
        ///     token id
        /// </summary>
        public int id;

        /// <summary>
        ///     log-odds of the token
        /// </summary>
        public float logit;

        /// <summary>
        ///     probability of the token
        /// </summary>
        public float p;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="logit"></param>
        /// <param name="p"></param>
        public TokenData(int id, float logit, float p)
        {
            this.id = id;
            this.logit = logit;
            this.p = p;
        }
    }
}
