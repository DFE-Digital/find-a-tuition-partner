namespace Application.TuitionPartnerShortlistStorage.Interfaces;

public interface ITuitionPartnerShortlistStorage
{
    /// <summary>
    /// Adds a Shortlisted Tuition Partner to an implemented form of storage.
    /// </summary>
    /// <param name="shortlistedTuitionPartnerSeoUrl"></param>
    void AddTuitionPartner(string shortlistedTuitionPartnerSeoUrl);

    /// <summary>
    /// Adds a Shortlisted Tuition Partner to an implemented form of storage.
    /// </summary>
    /// <param name="shortlistedTuitionPartnersSeoUrl"></param>
    void AddTuitionPartners(IEnumerable<string> shortlistedTuitionPartnersSeoUrl);

    /// <summary>
    /// Gets all Shortlisted Tuition Partners present in the implemented storage.
    /// Note: It does not remove the Shortlisted Tuition Partners from the storage.
    /// </summary>
    /// <returns>A list of Shortlisted Tuition Partners</returns>
    IEnumerable<string> GetAllTuitionPartners();

    /// <summary>
    /// Removes a single entry of a Shortlisted Tuition Partner stored in the implemented storage
    /// using the provided seoUrl.
    /// Note: It expects a valid seoUrl and that the entry exists.
    /// </summary>
    /// <param name="shortlistedTuitionPartnerSeoUrl"></param>
    void RemoveTuitionPartner(string shortlistedTuitionPartnerSeoUrl);

    /// <summary>
    /// Removes every Shortlisted Tuition Partner present in the implemented storage.
    /// </summary>
    void RemoveAllTuitionPartners();

    /// <summary>
    /// Check if a tuition partner is shortlisted.
    /// </summary>
    /// <param name="tuitionPartnerSeoUrl"></param>
    /// <returns>Returns true if present or false if not</returns>
    bool IsTuitionPartnerShortlisted(string tuitionPartnerSeoUrl);
}