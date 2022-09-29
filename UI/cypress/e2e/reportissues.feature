Feature: Report issues page

Scenario: Report issues page url is '/report-issues'
    Given a user has arrived on the Report issues page
    Then the page URL ends with '/report-issues'

  Scenario: contact us page title is 'Report issues'
    Given a user has arrived on the Report issues page
    Then the page's title is 'Report issues'

  Scenario: Page header is displayed
    Given a user has arrived on the Report issues page
    Then they will see the Report issues header

  Scenario: Page has link to tutoring support email address
    Given a user has arrived on the Report issues page
    Then page has link to tutoring support email address

  Scenario: The home link is selected
    Given a user has arrived on the Report issues page
    When they click 'Home'
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: Local council link is accessible
    Given a user has arrived on the Report issues page
    Then the 'report child abuse to local council' page is accessible in a new tab