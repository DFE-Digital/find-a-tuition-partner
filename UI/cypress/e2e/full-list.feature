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

  Scenario: full list of quality-assured tuition partners is in alphabetical order by name
    Given a user has arrived on the all quality-assured tuition partners page
    Then the full list of quality-assured tuition partners is in alphabetical order by name

  Scenario: tuition partner summaries only shows name, website, phone number and email address
    Given a user has arrived on the all quality-assured tuition partners page
    Then the user is only shown the name, website, phone number and email address for each tuition partner

  Scenario: tuition partner summaries' name links to tuition partner details page
    Given a user has arrived on the all quality-assured tuition partners page
    Then the name of each tuition partner links to their details page

  Scenario: tuition partner summaries' website link opens their website in a new tab
    Given a user has arrived on the all quality-assured tuition partners page
    Then the website link for each tuition partner opens their website in a new tab

  Scenario: tuition partner summaries' phone number link initiates device's calling options
    Given a user has arrived on the all quality-assured tuition partners page
    Then the phone number link for each tuition partner initiates their device's calling options

  Scenario: tuition partner summaries' email link initiates email client options
    Given a user has arrived on the all quality-assured tuition partners page
    Then the email link for each tuition partner initiates their email client options

  Scenario: tuition partner details page linked from all quality-assured tuition partners page has 'Back to tuition partners' back link
    Given a user has arrived on the all quality-assured tuition partners page
    When they click on the '4th' tuition partner's name
    Then the back link's text is 'Back to tuition partners'

  Scenario: back link returns to all quality-assured tuition partners page
    Given a user has arrived on the all quality-assured tuition partners page
    When they click on the '21st' tuition partner's name
    When they click 'Back'
    Then they will see the 'All quality-assured tuition partners' page