using System;

namespace EncryptedDiary
{
    /// <summary>
    /// Interface for diary forms that can load specific dates
    /// </summary>
    public interface IDiaryForm
    {
        /// <summary>
        /// Loads diary entry for a specific date
        /// </summary>
        /// <param name="date">The date to load</param>
        void LoadSpecificDate(DateTime date);
    }
}
