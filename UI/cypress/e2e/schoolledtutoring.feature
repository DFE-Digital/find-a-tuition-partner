Feature: Academic Mentors Page Tests
 Scenario: page as header Academic Mentors
    Given a user has arrived on the school led page
    Then they will see the school led header

  Scenario: page as link to funding allocation link
     Given a user has arrived on the school led page
     Then they will see the funding allocation link

  Scenario: page as link to dbs check link
     Given a user has arrived on the school led page
     Then they will see the dbs check link
     And the dbs check link opens in a new window

   Scenario: page as link to home link
     Given a user has arrived on the school led page
     Then they will see the home link

    Scenario: page as link to funding and reporting link
     Given a user has arrived on the school led page
     Then they will see the funding and reporting link

