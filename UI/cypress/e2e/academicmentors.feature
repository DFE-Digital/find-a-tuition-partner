Feature: Academic Mentors Page Tests
  Scenario: academic mentors page url is '/academic-mentors'
    Given a user has arrived on the academic mentors page
    Then the page URL ends with '/academic-mentors'

  Scenario: academic mentors page title is 'Employ mentors'
    Given a user has arrived on the academic mentors page
    Then the page's title is 'Employ mentors'

  Scenario: user clicks service name on academic mentors page
    Given a user has arrived on the academic mentors page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page
    
  Scenario: page has header Academic Mentors
    Given a user has arrived on the academic mentors page
    Then they will see the academic mentor header

  Scenario: page has link to book training
     Given a user has arrived on the academic mentors page
     Then they will see the book training link
     And the book training link opens in a new window

  Scenario: page has link to funding allocation link
     Given a user has arrived on the academic mentors page
     Then they will see the funding allocation link

  Scenario: page has link to home link
     Given a user has arrived on the academic mentors page
     Then they will see the home link
 
  Scenario: page has link to dbs check link
     Given a user has arrived on the academic mentors page
     Then they will see the dbs check link
     And the dbs check link opens in a new window

  Scenario: page has link to funding and reporting link
     Given a user has arrived on the academic mentors page
     Then they will see the funding and reporting link

  Scenario: funding link to page
    Given a user has arrived on the academic mentors page
    When they click funding and reporting link
    Then they will see the funding reporting header
    And  they will click the back link
    Then they redirects to academic mentors page
