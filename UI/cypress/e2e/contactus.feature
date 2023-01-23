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

  Scenario: page has link to back button
    Given a user has arrived on the contact us page
    Then they will see the back link

  Scenario: page has link to ftp mail 
    Given a user has arrived on the contact us page
    Then they will see link to tutoring mail address
    
  Scenario: page has link to 'Find out about call charges' 
    Given a user has arrived on the contact us page
    Then they will see link to 'https://www.gov.uk/call-charges' with test id 'find-out-call-charges-link'
    And the 'Find out about call charges' link opens a new window with test id 'find-out-call-charges-link'
    
  Scenario: page has telephone number
    Given a user has arrived on the contact us page
    Then they will see the telephone number 'Phone: 0300 373 0891'


 