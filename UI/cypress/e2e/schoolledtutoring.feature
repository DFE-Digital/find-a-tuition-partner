Feature: School Led Page Tests
 Scenario: page as header school led tutoring
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

   Scenario: funding link to page
     Given a user has arrived on the school led page
     When they click funding and reporting link
     Then they will see the funding reporting header
     And  they will click the back link
     Then they redirects to school led tutoring page

