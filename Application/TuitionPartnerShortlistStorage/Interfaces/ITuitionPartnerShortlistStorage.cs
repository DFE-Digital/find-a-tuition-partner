namespace Application.TuitionPartnerShortlistStorage.Interfaces;

public interface ITuitionPartnerShortlistStorage
{
    /// <summary>
    /// Adds a Shortlisted Tuition Partner to an implemented form of storage.
    /// </summary>
    /// <param name="shortlistedTuitionPartner"></param>
    /// <returns>1 if successfully added and 0 if unsuccessful</returns>
    int AddTuitionPartner(ShortlistedTuitionPartner shortlistedTuitionPartner);

    /// <summary>
    /// Gets a Shortlisted Tuition Partner based on the local authority name passed.
    /// Note: It does not remove the Shortlisted Tuition Partners from the storage.
    /// </summary>
    /// <param name="localAuthorityName"></param>
    /// <returns>A list of Shortlisted Tuition Partners</returns>
    IEnumerable<ShortlistedTuitionPartner> GetTuitionPartnersByLocalAuthorityName(string localAuthorityName);

    /// <summary>
    /// Gets all Shortlisted Tuition Partners present in the implemented storage.
    /// Note: It does not remove the Shortlisted Tuition Partners from the storage.
    /// </summary>
    /// <returns>A list of Shortlisted Tuition Partners</returns>
    IEnumerable<ShortlistedTuitionPartner> GetAllTuitionPartners();

    /// <summary>
    /// Removes a single entry of a Shortlisted Tuition Partner stored in the implemented storage
    /// using the provided seoUrl.
    /// Note: It expects a valid seoUrl and that the entry exists.
    /// </summary>
    /// <param name="seoUrl"></param>
    /// <param name="localAuthorityName"></param>
    /// <returns>1 if successfully removed or 0 if unsuccessful,
    /// or if the seoUrl provided is invalid</returns>
    int RemoveTuitionPartner(string seoUrl, string localAuthorityName);

    /// <summary>
    /// Removes every Shortlisted Tuition Partner present in the implemented storage.
    /// </summary>
    /// <returns>Total number of entries removed if it successfully removes all or 0 if unsuccessful</returns>
    int RemoveAllTuitionPartners();

    /// <summary>
    ///  Removes every Shortlisted Tuition Partner present in the implemented storage of the local authority supplied.
    /// </summary>
    /// <param name="localAuthorityName"></param>
    /// <returns></returns>
    int RemoveAllTuitionPartnersByLocalAuthority(string localAuthorityName);
}