---
status: accepted
date: 2023-08-09
deciders: NTP Dev Team, DfE Lead Developers
---

# Where to Store anonymised Enquiry Data After Migrating to Azure from GOV.UK PaaS

**Context and Problem Statement**

On GOV.UK PaaS, we exported anonymised enquiry data using a bash script, which is compatible with MacOS, Linux, or WSL. This script utilized conduit to connect in readonly mode to the production database from a local machine and executed SQL queries to export the required data to CSV for further analysis. With the transition to Azure and the limitations posed by Notify email's 2 MB attachment size, a need arises for a secure, reliable storage solution for these CSV files, especially since they might contain sensitive production data.

**Decision Drivers**

- The Enquiries CSV files contain anonymised production data derived from SQL exports.
- Ensuring high data security and integrity is paramount.
- The storage solution should provide fine-grained access control to deter broad permissions.
- The requirement for a scalable and reliable storage solution.

**Considered Options**

1. Sending the Enquiries CSV files via Notify email.
2. Use Azure Storage Account with Azure AD-signed SAS tokens.
3. Use OneDrive.

**Decision Outcome**

Chosen option: "Use Azure Storage Account with Azure AD-signed SAS tokens." This approach is favored due to its robust security mechanisms, scalability, and the ability to use shared access signatures (SAS) signed with Azure AD credentials. This eliminates the need for granting users 'READ_ALL' permissions.

**Pros and Cons of the Options**

1. **Sending the Enquiries CSV files via Notify email**

    - Good, as it provides a straightforward solution without extra infrastructure.
    - Bad, given the 2 MB attachment limitation.
    - Bad, due to the inherent risks of email loss and its unsuitability as a storage medium.

2. **Use Azure Storage Account with Azure AD-signed SAS tokens**

   - Good, for its scalability and capability to handle large data sets.
   - Good, due to its robust security features like encryption at rest, advanced access control, and the utilization of SAS tokens signed with Azure AD. This negates the need for 'READ_ALL' permissions.
   - Good, ensuring data integrity with redundancy support.
   - Bad, considering potential costs related to storage and data transfer.
   - Bad, as it necessitates users to become familiar with Azure's platform.

3. **Use OneDrive**

    - Good, as it's user-friendly and widely recognized.
    - Good, due to its inherent security and access control.
    - Bad, because the service credential would need 'READ_ALL' permissions and could access all files in OneDrive

