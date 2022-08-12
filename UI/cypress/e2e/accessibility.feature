Feature: Accessibility page shows users how accessible the website and how to report problems
  Scenario: accessibility statement page url is '/accessibilitys'
    Given a user has arrived on the accessibility page
    Then the page URL ends with '/accessibility'

  Scenario: accessibility page title is 'Accessibility statement'
    Given a user has arrived on the accessibility page
    Then the page's title is 'Accessibility statement'

  Scenario: user clicks service name on accessibility page
    Given a user has arrived on the accessibility page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: page as header accessibility statement
    Given a user has arrived on the accessibility page
    Then they will see the accessibility statement header

  Scenario: page as link to legislation priniciples
    Given a user has arrived on the accessibility page
    Then they will see the legislation link

  Scenario: page as header for cannot access part
    Given a user has arrived on the accessibility page
    Then they will see cannot access part header

  Scenario: page as link to mail tutoring service
    Given a user has arrived on the accessibility page
    Then they will see link to tutoring mail address

  Scenario: page as header for reporting accesibility problems
    Given a user has arrived on the accessibility page
    Then they will see reporting accesibility problems header

  Scenario: page as header for enforcement procedure header
    Given a user has arrived on the accessibility page
    Then they will see enforcement procedure header

  Scenario: page as link to equality advisory service
    Given a user has arrived on the accessibility page
    Then they will see link to equality advisory service

  Scenario: page as header improve accesibility
    Given a user has arrived on the accessibility page
    Then they will see header improve accesibility

  Scenario: page as header accessibility technical
    Given a user has arrived on the accessibility page
    Then they will see header accessibility technical header

  Scenario: page as header Compliance status
    Given a user has arrived on the accessibility page
    Then they will see header Compliance status

  Scenario: page as header how tested  webiste
    Given a user has arrived on the accessibility page
    Then they will see header how we tested website
 