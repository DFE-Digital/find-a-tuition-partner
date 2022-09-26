Feature: Show all quality-assured tuition partners
  Scenario: all quality-assured tuition partners page url is '/full-list'
    Given a user has arrived on the all quality-assured tuition partners page
    Then the page URL ends with '/full-list'

  Scenario: all quality-assured tuition partners page title is 'Full List'
    Given a user has arrived on the all quality-assured tuition partners page
    Then the page's title is 'Full List'

  Scenario: user clicks service name on all quality-assured tuition partners page
    Given a user has arrived on the all quality-assured tuition partners page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: link from the service start page
    Given a user has started the 'Find a tuition partner' journey
    When they click the 'All quality-assured tuition partners' link
    Then they will see the 'All quality-assured tuition partners' page

  Scenario: home link returns to service start page
    Given a user has arrived on the all quality-assured tuition partners page
    When they click 'Home'
    Then they will be taken to the 'Find a tuition partner' journey start page