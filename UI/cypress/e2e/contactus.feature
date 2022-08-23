 Feature: Contactus Page Tests
   Scenario: contact us page url is '/Contactus'
     Given a user has arrived on the contact us page
    Then the page URL ends with '/Contactus'

   Scenario: page as link to back button
    Given a user has arrived on the contact us page
    Then they will see the back link

   Scenario: page as header contact us
    Given a user has arrived on the contact us page
    Then they will see the contact us header

   Scenario: page as link to ftp mail 
    Given a user has arrived on the contact us page
    Then they will see link to tutoring mail address

   Scenario: page should not show contact us link 
    Given a user has arrived on the contact us page
    Then they will not see contact us link

