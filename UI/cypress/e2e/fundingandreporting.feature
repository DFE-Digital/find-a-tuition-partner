Feature: Funding and Reporting Page Tests
 Scenario: page as header funding and reporting
    Given a user has arrived on the funding and reporting page
    Then they will see the funding and report header

 Scenario: page as link to back button
    Given a user has arrived on the funding and reporting page
    Then they will see the back link

 Scenario: academic mentor tutoring rates are hidden
    Given a user has arrived on the funding and reporting page
    Then the academic mentor tutoring rates details are hidden

 Scenario: academic mentor tutoring rates are not hidden
    Given a user has arrived on the funding and reporting page
    When they click academic mentor tutoring rates
    Then the academic mentor tutoring rates are shown

 Scenario: page as example1 shown inset class
    Given a user has arrived on the funding and reporting page
    Then they will see example 1 rates in inset text class

 Scenario: page as example2 shown inset class
    Given a user has arrived on the funding and reporting page
    Then they will see example 2 rates in inset text class


 Scenario: page as example3 shown inset class
    Given a user has arrived on the funding and reporting page
    Then they will see example 3 rates in inset text class