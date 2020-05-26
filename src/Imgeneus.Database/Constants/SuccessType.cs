namespace Imgeneus.Database.Constants
{
    public enum SuccessType : byte
    {
        /// <summary>
        /// Skill success is based on luc, dex, wis etc.
        /// </summary>
        None,

        /// <summary>
        /// Skill success is based on success value.
        /// </summary>
        SuccessBasedOnValue,
    }
}
