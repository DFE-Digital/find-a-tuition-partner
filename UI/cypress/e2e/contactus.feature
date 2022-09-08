Feature: Contactus Page Tests
  Scenario: contact us page url is '/contact-us'
    Given a user has arrived on the contact us page
    Then the page URL ends with '/contact-us'

  Scenario: contact us page title is 'Contact us'
    Given a user has arrived on the contact us page
    Then the page's title is 'Contact us'
    
  Scenario: user clicks service name
    Given a user has arrived on the contact us page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: page as link to back button
    Given a user has arrived on the contact us page
    Then they will see the back link

  Scenario: page as link to ftp mail 
    Given a user has arrived on the contact us page
    Then they will see link to tutoring mail address

  Scenario: page should not show contact us link 
    Given a user has arrived on the contact us page
    Then they will not see contact us link

 Scenario: Complaints page is displayed when read our guidance link is selected
    Given a user has arrived on the contact us page
    When the link ‘read our guidance’ is selected
    Then they will be taken to the 'complaints' page
    And the page's title is 'Complaints Page'

  Scenario: page should have a feedbackform link 
    Given a user has arrived on the contact us page
    Then they will see feedback form link
    And the feedback form link opens in a new window

 