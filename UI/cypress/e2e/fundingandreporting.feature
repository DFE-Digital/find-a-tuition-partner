Feature: Funding and Reporting Page Tests
  Scenario: funding and reporting page url is '/funding-and-reporting'
    Given a user has arrived on the funding and reporting page
    Then the page URL ends with '/funding-and-reporting'

  Scenario: funding and reporting page title is 'Funding'
    Given a user has arrived on the funding and reporting page
    Then the page's title is 'Funding'

  Scenario: user clicks service name on funding and reporting page
    Given a user has arrived on the funding and reporting page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: page as header funding and reporting
    Given a user has arrived on the funding and reporting page
    Then they will see the funding and report header

  Scenario: academic mentor tutoring rates are hidden
    Given a user has arrived on the funding and reporting page
    Then the academic mentor tutoring rates details are hidden

  Scenario: academic mentor tutoring rates are not hidden
    Given a user has arrived on the funding and reporting page
    When they click academic mentor tutoring rates
    Then the academic mentor tutoring rates are shown

  Scenario: page as example1 shown moj class
    Given a user has arrived on the funding and reporting page
    Then they will see example 1 rates in moj ticket panel
