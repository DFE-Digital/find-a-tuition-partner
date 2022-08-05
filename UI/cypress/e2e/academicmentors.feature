Feature: Academic Mentors Page Tests
 Scenario: page as header Academic Mentors
    Given a user has arrived on the academic mentors page
    Then they will see the academic mentor header

 Scenario: page as link to login link
     Given a user has arrived on the academic mentors page
     Then they will see the login link
     And the login link opens in a new window

 Scenario: page as link to funding allocation link
     Given a user has arrived on the academic mentors page
     Then they will see the funding allocation link

 Scenario: page as link to home link
     Given a user has arrived on the academic mentors page
     Then they will see the home link
 
 Scenario: page as link to dbs check link
     Given a user has arrived on the academic mentors page
     Then they will see the dbs check link
     And the dbs check link opens in a new window

 Scenario: page as link to funding and reporting link
     Given a user has arrived on the academic mentors page
     Then they will see the funding and reporting link
