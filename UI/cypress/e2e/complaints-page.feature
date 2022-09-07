Feature: Complaints page

  Scenario: Complaints page is displayed
    Given the ‘contact us’ page is displayed 
    When the link ‘read our guidance’ is selected
    Then the page's title is 'Complaints Page'

  Scenario: page has link to ftp mail 
    Given the 'complaints page' is displayed
    Then they will see link to tutoring mail address

  Scenario: The home link is selected
    Given the 'complaints page' is displayed
    When the home link is selected
    Then the page's title is 'Find a tuition partner'

  Scenario: Local council link is selected
    Given the 'complaints page' is displayed
    When the ‘local council’ link has been selected
    Then the 'report child abuse to local council' page is accessible