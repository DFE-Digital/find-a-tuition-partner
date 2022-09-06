Feature: School Led Tutoring Page Tests
  Scenario: school led tutoring page url is '/school-led-tutoring'
    Given a user has arrived on the school led tutoring page
    Then the page URL ends with '/school-led-tutoring'

  Scenario: school led tutoring page title is 'Employ tutors'
    Given a user has arrived on the school led tutoring page
    Then the page's title is 'Employ tutors'

  Scenario: user clicks service name on school led tutoring page
    Given a user has arrived on the school led tutoring page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page
    
  Scenario: page has header school led tutoring
    Given a user has arrived on the school led tutoring page
    Then they will see the school led tutoring header

  Scenario: page has link to book training
    Given a user has arrived on the school led tutoring page
     Then they will see the book training link
     And the book training link opens in a new window

  Scenario: page has link to funding allocation link
    Given a user has arrived on the school led tutoring page
    Then they will see the funding allocation link

  Scenario: page has link to dbs check link
    Given a user has arrived on the school led tutoring page
    Then they will see the dbs check link
    And the dbs check link opens in a new window

  Scenario: page has link to home link
    Given a user has arrived on the school led tutoring page
    Then they will see the home link

  Scenario: page has link to funding and reporting link
    Given a user has arrived on the school led tutoring page
    Then they will see the funding and reporting link

  Scenario: funding link to page
    Given a user has arrived on the school led tutoring page
    When they click funding and reporting link
    Then they will see the funding reporting header
    And  they will click the back link
    Then they redirects to school led tutoring page
