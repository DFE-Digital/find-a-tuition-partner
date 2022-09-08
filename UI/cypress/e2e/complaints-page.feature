Feature: Complaints page

  Scenario: Complaints page is displayed
    Given the ‘contact us’ page is displayed 
    When the link ‘read our guidance’ is selected
    Then they will be taken to the 'complaints' page
    And the page's title is 'Complaints Page'

  Scenario: page has link to tutoring support email address
    Given the 'complaints page' is displayed
    Then page has link to tutoring support email address

  Scenario: The home link is selected
    Given the 'complaints page' is displayed
    When the home link is selected
    Then they will be taken to the 'Find a tuition partner' journey start page
    And the page's title is 'Find a tuition partner'

  Scenario: Local council link is accessible
    Given the 'complaints page' is displayed
    Then the 'report child abuse to local council' page is accessible in a new tab