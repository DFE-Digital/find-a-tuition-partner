 Feature: Contactus Page Tests
   Scenario: contact us page url is '/contact-us'
     Given a user has arrived on the contact us page
    Then the page URL ends with '/contact-us'

   Scenario: contact us page title is 'Contact us'
    GGiven a user has arrived on the contact us page
    Then the page's title is 'Contact us'
    
  Scenario: user clicks service name
  Given a user has arrived on the contact us page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

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

   Scenario: page should have a read our guidance link 
    Given a user has arrived on the contact us page
    Then they will see read our guidance link
    And the read our guidance link opens in a new window

   Scenario: page should have a feedbackform link 
    Given a user has arrived on the contact us page
    Then they will see feedback form link
    And the feedback form link opens in a new window

