namespace Application.Common.Interfaces;

public interface ITuitionPartnerCompareListStorageService
{
    /// <summary>
    /// Adds the specified compare listed Tuition Partners to the implemented storage.
    /// </summary>
    /// <param name="compareListTuitionPartnersSeoUrl"></param>
    bool AddTuitionPartners(IEnumerable<string> compareListTuitionPartnersSeoUrl);

    /// <summary>
    /// Gets all compare listed Tuition Partners present in the implemented storage.
    /// Note: It does not remove the compare listed Tuition Partners from the storage.
    /// </summary>
    /// <returns>A list of compare listed Tuition Partners</returns>
    IEnumerable<string> GetAllTuitionPartners();

    /// <summary>
    /// Removes the specified compare listed Tuition Partner stored in the implemented storage
    /// </summary>
    /// <param name="compareListTuitionPartnerSeoUrl"></param>
    bool RemoveTuitionPartner(string compareListTuitionPartnerSeoUrl);

    /// <summary>
    /// Removes every compare listed Tuition Partner present in the implemented storage.
    /// </summary>
    bool RemoveAllTuitionPartners();

    /// <summary>
    /// Check if a tuition partner is compare listed.
    /// </summary>
    /// <param name="tuitionPartnerSeoUrl"></param>
    /// <returns>Returns true if present or false if not</returns>
    bool IsTuitionPartnerCompareListed(string tuitionPartnerSeoUrl);
}